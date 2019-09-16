using Unity.Entities;
using Unity.DotsNetKit.Transport;

public interface IRpcCommand
{
    void Execute(Entity connection, EntityCommandBuffer.Concurrent commandBuffer, int jobIndex);
    void Execute(Entity connection, EntityCommandBuffer commandBuffer);
    void Serialize(DataStreamWriter writer);
    void Deserialize(DataStreamReader reader, ref DataStreamReader.Context ctx);
}
