using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleGun : MonoBehaviour

{
    [Header("Shoot Settings")]
    [SerializeField] private float fireCooldown;
    [SerializeField] private float maxFireDistance = 20.0f;

    [SerializeField] private Transform firePosition;

    [Header("Raycast Settings")]
    [SerializeField] private Vector2 spreadAmount;
    [SerializeField] private int rayAmount = 20;

    private float lastFireTime = 0.0f;

    [Header("Bubble Settings")]
    [SerializeField] private float bubbleMovement = 0.5f;
    [SerializeField] private float bubbleSize = 0.01f;
    [SerializeField] private float decayTime = 4.0f;
    [SerializeField] private Mesh bubbleMesh;
    [SerializeField] private Material bubbleMat;
    private NativeList<Bubble> _nativeBubbles;
    private NativeList<Matrix4x4> _nativeBubbleMatrices;

    private BubbleFunctionJob bubbleFunctionJob;
    private BubbleFilterJob bubbleFilterJob;
    private RenderParams renderParams;

    private void Start()
    {
        lastFireTime = Time.time;

        _nativeBubbles = new NativeList<Bubble>(Allocator.Persistent);
        // Init Jobs
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

        Ray ray;
        RaycastHit rhit;

        for (int i = 0; i < rayAmount; i++)
        {
            float randomYaw = UnityEngine.Random.Range(-spreadAmount.x, spreadAmount.x);
            float randomPitch = UnityEngine.Random.Range(-spreadAmount.y, spreadAmount.y);

            ray = new Ray(firePosition.position, (Quaternion.Euler(randomPitch, randomYaw, 0) * firePosition.forward).normalized);

            if (Physics.Raycast(ray, out rhit, maxFireDistance))
            {
                Bubble newBubble = new Bubble();
                newBubble.pos = firePosition.position;
                newBubble.target = rhit.point;
                newBubble.decayTime = decayTime;

                _nativeBubbles.Add(newBubble);
            }
        }

        lastFireTime = Time.time;
    }

    private void Update()
    {
        if (_nativeBubbles.IsEmpty)
            return;

        _nativeBubbleMatrices = new NativeList<Matrix4x4>(_nativeBubbles.Capacity, Allocator.TempJob);

        bubbleFunctionJob.BubblesList = _nativeBubbles.AsDeferredJobArray();
        bubbleFunctionJob.BubbleMatrices = _nativeBubbleMatrices.AsParallelWriter();
        bubbleFunctionJob.moveAmount = bubbleMovement;
        bubbleFunctionJob.bubbleScale = new float3(bubbleSize, bubbleSize, bubbleSize);
        bubbleFunctionJob.deltaTime = Time.deltaTime;

        var bubbleFuncHandle = bubbleFunctionJob.Schedule(_nativeBubbles.Length, 50);
        bubbleFuncHandle.Complete();

        NativeList<Bubble> FilteredBubbles = new NativeList<Bubble>(_nativeBubbles.Length, Allocator.TempJob);

        var bubbleFilterHandle = bubbleFilterJob.Schedule(_nativeBubbles.Length, 50, bubbleFuncHandle);
        bubbleFilterHandle.Complete();

        if (_nativeBubbleMatrices.IsCreated)
            Graphics.RenderMeshInstanced(renderParams, bubbleMesh, 0, _nativeBubbleMatrices.AsArray());

        _nativeBubbleMatrices.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(firePosition.position, firePosition.forward * maxFireDistance);
    }
}
