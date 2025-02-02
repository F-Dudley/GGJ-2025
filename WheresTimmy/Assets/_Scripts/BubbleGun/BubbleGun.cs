using DG.Tweening;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleGun : MonoBehaviour, IInteractable

{
    [Header("Shoot Settings")]
    [SerializeField] private float fireCooldown;
    [SerializeField] private float maxFireDistance = 20.0f;

    [SerializeField] private Transform firePosition;

    [Header("Raycast Settings")]
    [SerializeField] private Vector2 spreadAmount;
    [SerializeField] private int rayAmount = 20;
    [SerializeField] private LayerMask hittableLayers;

    private float lastFireTime = 0.0f;

    [Header("Bubble Settings")]
    [SerializeField] private float bubbleMovement = 0.5f;
    [SerializeField] private float bubbleSize = 0.01f;

    [SerializeField] private float minDecayTime = 1.0f;
    [SerializeField] private float maxDecayTime = 4.0f;
    [SerializeField] private Mesh bubbleMesh;
    [SerializeField] private Material bubbleMat;

    [Header("Interaction Settings")]
    [SerializeField] private Collider gunCollider;

    [Header("Sound")]
    [SerializeField] private AudioSource motorNoise;
    [SerializeField] private AudioSource bubbleGlub;

    [Header("Jobs")]
    private NativeList<Bubble> _nativeBubbles;
    private NativeList<Matrix4x4> _nativeBubbleMatrices;

    private BubblePreFireJob preFireJob;
    private BubbleFunctionJob bubbleFunctionJob;
    private BubbleFilterJob bubbleFilterJob;
    private RenderParams renderParams;

    private void Start()
    {
        gunCollider = GetComponent<Collider>();
        lastFireTime = Time.time;

        _nativeBubbles = new NativeList<Bubble>(20000, Allocator.Persistent);

        // Init Jobs
        preFireJob = new BubblePreFireJob();
        bubbleFunctionJob = new BubbleFunctionJob();
        bubbleFilterJob = new BubbleFilterJob();

        renderParams = new RenderParams(bubbleMat);
    }

    private void OnDestroy()
    {
        if (_nativeBubbles.IsCreated)
            _nativeBubbles.Dispose();

        if (_nativeBubbleMatrices.IsCreated)
            _nativeBubbleMatrices.Dispose();
    }

    public void FireGun()
    {
        if (lastFireTime + fireCooldown > Time.time)
            return;

        { // Positional Vars
            preFireJob.fPosition = firePosition.position;
            preFireJob.fOriginForward = firePosition.forward;
            preFireJob.fOriginRight = firePosition.right;
        }

        { // Raycast Vars
            QueryParameters qParams = QueryParameters.Default;
            qParams.hitTriggers = QueryTriggerInteraction.Ignore;
            qParams.layerMask = hittableLayers;

            preFireJob.fireSpread = spreadAmount;
            preFireJob.fireDistance = maxFireDistance;
            preFireJob.parameters = qParams;

            preFireJob.baseSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue);
        }

        NativeArray<RaycastCommand> raycastCommands = new NativeArray<RaycastCommand>(rayAmount, Allocator.TempJob);
        preFireJob.commands = raycastCommands;

        JobHandle preFireHandle = preFireJob.Schedule(raycastCommands.Length, 100);

        NativeArray<RaycastHit> hits = new NativeArray<RaycastHit>(rayAmount, Allocator.TempJob);

        JobHandle raycastJob = RaycastCommand.ScheduleBatch(raycastCommands, hits, 25, preFireHandle);
        raycastJob.Complete();

        raycastCommands.Dispose();

        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;

            Bubble newBubble = new Bubble();
            newBubble.pos = firePosition.position;
            newBubble.target = hit.point;
            newBubble.decayTime = UnityEngine.Random.Range(minDecayTime, maxDecayTime);

            _nativeBubbles.Add(newBubble);
        }

        hits.Dispose();
        lastFireTime = Time.time;
    }

    private void Update()
    {
        if (_nativeBubbles.IsEmpty)
            return;

        //
        // Apply Movement and Decay to Initialized Bubbles
        //

        _nativeBubbleMatrices = new NativeList<Matrix4x4>(_nativeBubbles.Length, Allocator.TempJob);

        bubbleFunctionJob.BubblesList = _nativeBubbles.AsDeferredJobArray();
        bubbleFunctionJob.BubbleMatrices = _nativeBubbleMatrices.AsParallelWriter();
        bubbleFunctionJob.moveAmount = bubbleMovement;
        bubbleFunctionJob.bubbleScale = new float3(bubbleSize, bubbleSize, bubbleSize);
        bubbleFunctionJob.deltaTime = Time.deltaTime;

        var bubbleFuncHandle = bubbleFunctionJob.Schedule(_nativeBubbles.Length, 100);
        bubbleFuncHandle.Complete();


        if (_nativeBubbleMatrices.IsCreated)
            Graphics.RenderMeshInstanced(renderParams, bubbleMesh, 0, _nativeBubbleMatrices.AsArray());

        _nativeBubbleMatrices.Dispose();


        //
        // Apply Bubble Filtering on Decayed Bubbles
        //

        NativeList<Bubble> FilteredBubbles = new NativeList<Bubble>(_nativeBubbles.Capacity, Allocator.TempJob);

        bubbleFilterJob.FilteredBubbles = FilteredBubbles.AsParallelWriter();
        bubbleFilterJob.BubblesList = _nativeBubbles.AsDeferredJobArray();

        var bubbleFilterHandle = bubbleFilterJob.Schedule(_nativeBubbles.Length, 64, bubbleFuncHandle);
        bubbleFilterHandle.Complete();

        _nativeBubbles.Dispose();
        _nativeBubbles = FilteredBubbles;

        Debug.Log("Filtered Amount: " + FilteredBubbles.Length);
    }

    public void Interact(PlayerInteraction player)
    {
        gameObject.layer = LayerMask.GetMask("Default");
        gunCollider.enabled = false;

        gameObject.transform.SetParent(player.GetBubbleHeldLocation());

        gameObject.transform.DOLocalRotate(Vector3.zero, 0.2f);
        gameObject.transform.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() =>
        {
            Debug.Log("Gun Obtained");
            player.SetBubbleGun(this);
        });
    }

    public void PlayAudio()
    {
        if (!motorNoise.isPlaying)
            motorNoise.Play();

        if (!bubbleGlub.isPlaying)
            bubbleGlub.Play();
    }

    public void StopAudio()
    {
        motorNoise.Stop();

        bubbleGlub.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(firePosition.position, firePosition.forward * maxFireDistance);
    }
}
