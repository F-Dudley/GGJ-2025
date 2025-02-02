using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
internal struct Bubble
{
    public float3 pos;
    public float3 target;

    public float decayTime;

    public bool isDecayed => decayTime <= 0.0f;

    public bool hasReachedDestination(float minDist)
    {
        return math.distance(pos, target) <= minDist;
    }
}

[BurstCompile]
internal struct BubblePreFireJob : IJobParallelFor
{
    // Fire Location Vars
    [ReadOnly] public float3 fPosition;
    [ReadOnly] public float3 fOriginForward;
    [ReadOnly] public float3 fOriginUp;
    [ReadOnly] public float3 fOriginRight;


    // Raycast Params
    [ReadOnly] public float2 fireSpread;
    [ReadOnly] public float fireDistance;
    [ReadOnly] public QueryParameters parameters;

    [ReadOnly] public uint baseSeed;

    public NativeArray<RaycastCommand> commands;

    public void Execute(int index)
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(baseSeed + (uint)index);

        float3 pitchOffset = fOriginUp * random.NextFloat(
            math.radians(-fireSpread.y),
            math.radians(fireSpread.y)
        );
        float3 yawOffset = fOriginRight * random.NextFloat(
            math.radians(-fireSpread.x),
            math.radians(fireSpread.x)
        );

        commands[index] = new RaycastCommand(
            fPosition,
            math.normalize(fOriginForward + yawOffset + pitchOffset),
            parameters,
            fireDistance
        );
    }
}

[BurstCompile]
internal struct BubbleFunctionJob : IJobParallelFor
{
    public NativeArray<Bubble> BubblesList;

    [ReadOnly] public float moveAmount;
    [ReadOnly] public float deltaTime;

    [ReadOnly] public float3 bubbleScale;

    [WriteOnly] public NativeList<Matrix4x4>.ParallelWriter BubbleMatrices;

    public void Execute(int index)
    {
        Bubble bubble = BubblesList[index];

        if (bubble.isDecayed)
            return;

        bubble.pos = math.lerp(bubble.pos, bubble.target, moveAmount * deltaTime);
        bubble.decayTime -= deltaTime;

        if (!bubble.isDecayed)
            BubbleMatrices.AddNoResize(Matrix4x4.TRS(bubble.pos, quaternion.identity, bubbleScale));

        BubblesList[index] = bubble;
    }
}


internal struct BubbleFilterJob : IJobParallelFor
{
    public NativeList<Bubble>.ParallelWriter FilteredBubbles;
    [ReadOnly] public NativeArray<Bubble> BubblesList;

    public void Execute(int index)
    {
        Bubble bubble = BubblesList[index];

        if (bubble.isDecayed)
            return;

        FilteredBubbles.AddNoResize(bubble);
    }
}