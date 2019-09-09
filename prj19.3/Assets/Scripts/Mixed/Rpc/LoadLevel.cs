using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine.SceneManagement;

public struct RpcLoadLevel : IRpcCommand
{
    public string levelName;

    public void Execute(Entity connection, EntityCommandBuffer.Concurrent commandBuffer, int jobIndex)
    {
        UnityEngine.Debug.Log(string.Format("LZ: load level ({0})", levelName));

        SceneManager.LoadScene(levelName);
    }

    public void Execute(Entity connection, EntityCommandBuffer commandBuffer)
    {
        UnityEngine.Debug.Log(string.Format("LZ: load level ({0})", levelName));

        SceneManager.LoadScene(levelName);
    }

    public void Serialize(DataStreamWriter writer)
    {
        writer.WriteUnicodeString(levelName);
    }

    public void Deserialize(DataStreamReader reader, ref DataStreamReader.Context ctx)
    {
        levelName = reader.ReadUnicodeString(ref ctx);
    }
}
