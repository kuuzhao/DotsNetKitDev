using Unity.Entities;
using Unity.DotsNetKit.Transport;

namespace Unity.DotsNetKit.NetCode
{
    public struct NetworkIdComponent : IComponentData
    {
        public int Value;
    }

    internal struct RpcSetNetworkId : IRpcCommand
    {
        public int nid;
        public void Execute(Entity connection, EntityCommandBuffer.Concurrent commandBuffer, int jobIndex)
        {
            commandBuffer.AddComponent(jobIndex, connection, new NetworkIdComponent { Value = nid });
        }

        public void Execute(Entity connection, EntityCommandBuffer commandBuffer)
        {
            commandBuffer.AddComponent(connection, new NetworkIdComponent { Value = nid });
        }

        public void Serialize(DataStreamWriter writer)
        {
            writer.Write(nid);
        }

        public void Deserialize(DataStreamReader reader, ref DataStreamReader.Context ctx)
        {
            nid = reader.ReadInt(ref ctx);
        }
    }
}