using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Client
{
    // TODO: LZ:
    //      simplify the setup here
    public static void Start()
    {
        Application.targetFrameRate = 60;

        Debug.Log("ClientServerSystemManager.CollectAllSystems()");
        ClientServerSystemManager.CollectAllSystems();

        Debug.Log("ReplicatedPrefabMgr.Initialize()");
        ReplicatedPrefabMgr.Initialize();

        ClientServerSystemManager.InitClientSystems();

        Unity.Networking.Transport.NetworkEndPoint ep = Unity.Networking.Transport.NetworkEndPoint.Parse("127.0.0.1",
            12345);
        World clientWorld = ClientServerSystemManager.clientWorld;
        Entity ent = clientWorld.GetExistingSystem<NetworkStreamReceiveSystem>().Connect(ep);

        Console.WriteLine("Connecting to server ...");
    }
}
