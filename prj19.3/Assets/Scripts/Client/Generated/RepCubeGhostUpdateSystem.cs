using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateInGroup(typeof(GhostUpdateSystemGroup))]
public class RepCubeGhostUpdateSystem : JobComponentSystem
{
    // TODO: LZ:
    //    turn it off, because of the error : Loading a managed string literal is not supported by burst
    // [BurstCompile]
    [RequireComponentTag(typeof(RepCubeSnapshotData))]
    [ExcludeComponent(typeof(PredictedEntityComponent))]
    struct UpdateInterpolatedJob : IJobForEachWithEntity<RepCubeComponentData>
    {
        [NativeDisableParallelForRestriction] public BufferFromEntity<RepCubeSnapshotData> snapshotFromEntity;
        public uint targetTick;
        public void Execute(Entity entity, int index,
            ref RepCubeComponentData ghostRepCubeComponentData)
        {
            var snapshot = snapshotFromEntity[entity];
            RepCubeSnapshotData snapshotData;
            snapshot.GetDataAtTick(targetTick, out snapshotData);

            ghostRepCubeComponentData.networkId = snapshotData.GetRepCubeComponentDatanetworkId();
            ghostRepCubeComponentData.position = snapshotData.GetRepCubeComponentDataposition();

        }
    }
    // TODO: LZ:
    //    turn it off, because of the error : Loading a managed string literal is not supported by burst
    // [BurstCompile]
    [RequireComponentTag(typeof(RepCubeSnapshotData), typeof(PredictedEntityComponent))]
    struct UpdatePredictedJob : IJobForEachWithEntity<RepCubeComponentData>
    {
        [NativeDisableParallelForRestriction] public BufferFromEntity<RepCubeSnapshotData> snapshotFromEntity;
        public uint targetTick;
        public void Execute(Entity entity, int index,
            ref RepCubeComponentData ghostRepCubeComponentData)
        {
            var snapshot = snapshotFromEntity[entity];
            RepCubeSnapshotData snapshotData;
            snapshot.GetDataAtTick(targetTick, out snapshotData);

            ghostRepCubeComponentData.networkId = snapshotData.GetRepCubeComponentDatanetworkId();
            ghostRepCubeComponentData.position = snapshotData.GetRepCubeComponentDataposition();

        }
    }
    // TODO: LZ:
    //      we may not have predicted job
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var updateInterpolatedJob = new UpdateInterpolatedJob
        {
            snapshotFromEntity = GetBufferFromEntity<RepCubeSnapshotData>(),
            targetTick = NetworkTimeSystem.interpolateTargetTick
        };
        var updatePredictedJob = new UpdatePredictedJob
        {
            snapshotFromEntity = GetBufferFromEntity<RepCubeSnapshotData>(),
            targetTick = NetworkTimeSystem.predictTargetTick
        };
        inputDeps = updateInterpolatedJob.Schedule(this, inputDeps);
        return updatePredictedJob.Schedule(this, inputDeps);
    }
}