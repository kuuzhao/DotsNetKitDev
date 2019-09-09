using Unity.Mathematics;
using Unity.Networking.Transport;

public struct RepCubeSnapshotData : ISnapshotData<RepCubeSnapshotData>
{
    public uint tick;
    int RepCubeComponentDatanetworkId;
    int RepCubeComponentDatapositionX;
    int RepCubeComponentDatapositionY;
    int RepCubeComponentDatapositionZ;


    public uint Tick => tick;
    public int GetRepCubeComponentDatanetworkId()
    {
        return RepCubeComponentDatanetworkId;
    }
    public void SetRepCubeComponentDatanetworkId(int val)
    {
        RepCubeComponentDatanetworkId = val;
    }
    public float3 GetRepCubeComponentDataposition()
    {
        return new float3(RepCubeComponentDatapositionX, RepCubeComponentDatapositionY, RepCubeComponentDatapositionZ) * 0.01f;
    }
    public void SetRepCubeComponentDataposition(float3 val)
    {
        RepCubeComponentDatapositionX = (int)(val.x * 100);
        RepCubeComponentDatapositionY = (int)(val.y * 100);
        RepCubeComponentDatapositionZ = (int)(val.z * 100);
    }


    public void PredictDelta(uint tick, ref RepCubeSnapshotData baseline1, ref RepCubeSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        RepCubeComponentDatanetworkId = predictor.PredictInt(RepCubeComponentDatanetworkId, baseline1.RepCubeComponentDatanetworkId, baseline2.RepCubeComponentDatanetworkId);
        RepCubeComponentDatapositionX = predictor.PredictInt(RepCubeComponentDatapositionX, baseline1.RepCubeComponentDatapositionX, baseline2.RepCubeComponentDatapositionX);
        RepCubeComponentDatapositionY = predictor.PredictInt(RepCubeComponentDatapositionY, baseline1.RepCubeComponentDatapositionY, baseline2.RepCubeComponentDatapositionY);
        RepCubeComponentDatapositionZ = predictor.PredictInt(RepCubeComponentDatapositionZ, baseline1.RepCubeComponentDatapositionZ, baseline2.RepCubeComponentDatapositionZ);

    }

    public void Serialize(ref RepCubeSnapshotData baseline, DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        writer.WritePackedIntDelta(RepCubeComponentDatanetworkId, baseline.RepCubeComponentDatanetworkId, compressionModel);
        writer.WritePackedIntDelta(RepCubeComponentDatapositionX, baseline.RepCubeComponentDatapositionX, compressionModel);
        writer.WritePackedIntDelta(RepCubeComponentDatapositionY, baseline.RepCubeComponentDatapositionY, compressionModel);
        writer.WritePackedIntDelta(RepCubeComponentDatapositionZ, baseline.RepCubeComponentDatapositionZ, compressionModel);

    }

    public void Deserialize(uint tick, ref RepCubeSnapshotData baseline, DataStreamReader reader, ref DataStreamReader.Context ctx,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        RepCubeComponentDatanetworkId = reader.ReadPackedIntDelta(ref ctx, baseline.RepCubeComponentDatanetworkId, compressionModel);
        RepCubeComponentDatapositionX = reader.ReadPackedIntDelta(ref ctx, baseline.RepCubeComponentDatapositionX, compressionModel);
        RepCubeComponentDatapositionY = reader.ReadPackedIntDelta(ref ctx, baseline.RepCubeComponentDatapositionY, compressionModel);
        RepCubeComponentDatapositionZ = reader.ReadPackedIntDelta(ref ctx, baseline.RepCubeComponentDatapositionZ, compressionModel);

    }
    public void Interpolate(ref RepCubeSnapshotData target, float factor)
    {
        SetRepCubeComponentDataposition(math.lerp(GetRepCubeComponentDataposition(), target.GetRepCubeComponentDataposition(), factor));

    }
}