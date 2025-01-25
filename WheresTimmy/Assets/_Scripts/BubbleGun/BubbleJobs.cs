using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

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
internal struct BubbleFunctionJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Bubble> BubblesList;

    [ReadOnly] public float moveAmount;
    [ReadOnly] public float deltaTime;

    [ReadOnly] public float3 bubbleScale;

    [WriteOnly] public NativeList<Matrix4x4>.ParallelWriter BubbleMatrices;

    public void Execute(int index)
    {
        if (BubblesList[index].isDecayed)
            return;

        Bubble bubble = BubblesList[index];

        bubble.pos = math.lerp(bubble.pos, bubble.target, moveAmount * deltaTime);
        bubble.decayTime -= deltaTime;

        if (!bubble.isDecayed)
            BubbleMatrices.AddNoResize(Matrix4x4.TRS(bubble.pos, quaternion.identity, bubbleScale));

        BubblesList[index] = bubble;
    }
}


internal struct BubbleFilterJob : IJobParallelFor
{
    public NativeList<Bubble> BubblesList { get; internal set; }

    public void Execute(int index)
    {

    }
}