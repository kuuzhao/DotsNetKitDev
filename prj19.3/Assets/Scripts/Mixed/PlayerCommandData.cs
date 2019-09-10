﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public struct PlayerCommandData : ICommandData<PlayerCommandData>
{
    private uint tick;
    public float horizontal;
    public float vertical;

    public uint Tick => tick;

    public void Serialize(DataStreamWriter writer)
    {
        writer.Write((int)(horizontal * 1000f));
        writer.Write((int)(vertical * 1000f));
    }

    public void Deserialize(uint inputTick, DataStreamReader reader, ref DataStreamReader.Context ctx)
    {
        tick = inputTick;
        horizontal = reader.ReadInt(ref ctx) * 0.001f;
        vertical = reader.ReadInt(ref ctx) * 0.001f;
    }
}