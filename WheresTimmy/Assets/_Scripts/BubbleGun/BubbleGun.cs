using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BubbleGun : MonoBehaviour

{
    [Header("Shoot Settings")]
    [SerializeField][Range(0.1f, 10)] private float fireCooldown;
    [SerializeField] private float maxFireDistance = 20.0f;

    [SerializeField] private Transform firePosition;

    [Header("Raycast Settings")]
    [SerializeField] private Vector2 spreadAmount;
    [SerializeField] private int rayAmount = 20;

    private float lastFireTime = 0.0f;

    [Header("Bubble Settings")]
    [SerializeField] private float decayTime;
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

        _nativeBubbles = new NativeList<Bubble>();
        _nativeBubbleMatrices = new NativeList<Matrix4x4>();

        // Init Jobs

        bubbleFunctionJob = new BubbleFunctionJob
        {
            BubblesList = _nativeBubbles,
            BubbleMatrices = _nativeBubbleMatrices
        };

        bubbleFilterJob = new BubbleFilterJob
        {

        };

        renderParams = new RenderParams(bubbleMat);
    }

    private void OnDestroy()
    {
        _nativeBubbles.Dispose();
        _nativeBubbleMatrices.Dispose();
    }

    public void FireGun()
    {
        if (lastFireTime > Time.time)
            return;

        Ray ray;
        RaycastHit rhit;

        for (int i = 0; i < rayAmount; i++)
        {
            Vector3 shootSpread = new(UnityEngine.Random.Range(-spreadAmount.x, spreadAmount.x), UnityEngine.Random.Range(-spreadAmount.y, spreadAmount.y));

            ray = new Ray(firePosition.position, (firePosition.forward + shootSpread).normalized);

            if (Physics.Raycast(ray, out rhit, maxFireDistance))
            {
                Bubble newBubble = new Bubble();
                newBubble.pos = firePosition.position;
                newBubble.target = rhit.point;

                _nativeBubbles.Add(newBubble);
            }
        }
    }

    private void Update()
    {
        /*
        JobHandle jobHandle = bubbleFunctionJob.Schedule(_nativeBubbles.Length, 50);

        jobHandle.Complete();

        Graphics.RenderMeshInstanced(renderParams, bubbleMesh, 0, _nativeBubbleMatrices.AsArray());

        _nativeBubbleMatrices.Dispose();
        */
    }
}
