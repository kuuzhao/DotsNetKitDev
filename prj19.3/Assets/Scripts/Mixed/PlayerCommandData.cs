using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public struct PlayerCommandData : ICommandData<PlayerCommandData>
{
    private uint tick;
    public byte up;
    public byte left;

    public uint Tick => tick;

    public void Serialize(DataStreamWriter writer)
    {
        writer.Write(up);
        writer.Write(left);
    }

    public void Deserialize(uint inputTick, DataStreamReader reader, ref DataStreamReader.Context ctx)
    {
        tick = inputTick;
        up = reader.ReadByte(ref ctx);
        up = reader.ReadByte(ref ctx);
    }
}
