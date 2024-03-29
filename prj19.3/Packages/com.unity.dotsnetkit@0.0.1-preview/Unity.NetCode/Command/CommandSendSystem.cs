using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.DotsNetKit.Transport;
using Unity.DotsNetKit.Transport.LowLevel.Unsafe;

namespace Unity.DotsNetKit.NetCode
{
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    // dependency just for acking
    [UpdateAfter(typeof(GhostReceiveSystemGroup))]
    public class CommandSendSystem<TCommandData> : JobComponentSystem
    where TCommandData : struct, ICommandData<TCommandData>
    {
        [ExcludeComponent(typeof(NetworkStreamDisconnected))]
        struct CommandSendJob : IJobForEachWithEntity<CommandTargetComponent>
        {
            public UdpNetworkDriver.Concurrent driver;
            public NetworkPipeline unreliablePipeline;
            public ComponentDataFromEntity<NetworkStreamConnection> connectionFromEntity;
            public ComponentDataFromEntity<NetworkSnapshotAckComponent> ackSnapshot;
            public BufferFromEntity<TCommandData> inputFromEntity;
            public uint localTime;
            public uint inputTargetTick;
            public unsafe void Execute(Entity entity, int index, [ReadOnly] ref CommandTargetComponent state)
            {
                DataStreamWriter writer = new DataStreamWriter(128, Allocator.Temp);
                var ack = ackSnapshot[entity];
                writer.Write((byte)NetworkStreamProtocol.Command);
                writer.Write(ack.LastReceivedSnapshotByLocal);
                writer.Write(ack.ReceivedSnapshotByLocalMask);
                writer.Write(localTime);

                // writer.Write(ack.LastReceivedRemoteTime - (localTime - ack.LastReceiveTimestamp));
                // TODO: LZ:
                //      to be confirmed
                //      we should send "t0 + (T1 - T0)", but not "t0 - (T1 - T0)"
                //
                // because:
                //      RTT should equals to : (t1 - t0) - (T1 - T0) = t1 - [t0 + (T1 - T0)]
                //      t0: A send time         // ack.LastReceivedRemoteTime
                //      T0: B receive time      // ack.LastReceiveTimestamp
                //      T1: B send time         // localTime
                //      t1: A receive time
                writer.Write(ack.LastReceivedRemoteTime + (localTime - ack.LastReceiveTimestamp));

                if (state.targetEntity != Entity.Null && inputFromEntity.Exists(state.targetEntity))
                {
                    var input = inputFromEntity[state.targetEntity];
                    TCommandData inputData;
                    if (input.GetDataAtTick(inputTargetTick, out inputData) && inputData.Tick == inputTargetTick)
                    {
                        writer.Write(inputTargetTick);
                        inputData.Serialize(writer);
                    }
                }

                driver.Send(unreliablePipeline, connectionFromEntity[entity].Value, writer);
            }
        }

        private NetworkStreamReceiveSystem m_ReceiveSystem;
        private NetworkTimeSystem m_TimeSystem;
        protected override void OnCreate()
        {
            m_ReceiveSystem = World.GetOrCreateSystem<NetworkStreamReceiveSystem>();
            m_TimeSystem = World.GetOrCreateSystem<NetworkTimeSystem>();
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var sendJob = new CommandSendJob
            {
                driver = m_ReceiveSystem.ConcurrentDriver,
                unreliablePipeline = m_ReceiveSystem.UnreliablePipeline,
                connectionFromEntity = GetComponentDataFromEntity<NetworkStreamConnection>(),
                ackSnapshot = GetComponentDataFromEntity<NetworkSnapshotAckComponent>(),
                inputFromEntity = GetBufferFromEntity<TCommandData>(),
                localTime = NetworkTimeSystem.TimestampMS,
                inputTargetTick = m_TimeSystem.predictTargetTick
            };

            return sendJob.ScheduleSingle(this, inputDeps);
        }
    }
}
