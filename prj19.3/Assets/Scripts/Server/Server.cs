﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;

public class Server
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

        ClientServerSystemManager.InitServerSystems();

        Unity.Networking.Transport.NetworkEndPoint ep = Unity.Networking.Transport.NetworkEndPoint.AnyIpv4;
        ep.Port = 12345;
        World serverWorld = ClientServerSystemManager.serverWorld;
        var nsrs = serverWorld.GetExistingSystem<NetworkStreamReceiveSystem>();
        nsrs.Listen(ep);

        Console.WriteLine(string.Format("Server is listening on port ({0})", ep.Port));

        string levelName = "Level1";
        SceneManager.LoadScene(levelName);
        Console.WriteLine(string.Format("Load level ({0})", levelName));
    }
}
