using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class Main : MonoBehaviour
{
    bool showButton = true;
    void Awake()
    {
        Application.targetFrameRate = 60;

        Debug.Log("ClientServerSystemManager.CollectAllSystems()");
        ClientServerSystemManager.CollectAllSystems();

        Debug.Log("ReplicatedPrefabMgr.Initialize()");
        ReplicatedPrefabMgr.Initialize();

    }
    void Start()
    {
        Screen.SetResolution(640, 480, false);
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (!showButton)
            return;

        if (GUI.Button(new Rect(100, 100, 100, 50), "Start sever"))
        {
            ClientServerSystemManager.InitServerSystems();

            Unity.Networking.Transport.NetworkEndPoint ep = Unity.Networking.Transport.NetworkEndPoint.AnyIpv4;
            ep.Port = 12345;
            World serverWorld = ClientServerSystemManager.serverWorld;
            var nsrs = serverWorld.GetExistingSystem<NetworkStreamReceiveSystem>();
            nsrs.Listen(ep);

            Debug.Log("Server started!");
            showButton = false;
        }
        if (GUI.Button(new Rect(100, 200, 100, 50), "Start client"))
        {
            ClientServerSystemManager.InitClientSystems();

            Unity.Networking.Transport.NetworkEndPoint ep = Unity.Networking.Transport.NetworkEndPoint.Parse("127.0.0.1",
                12345);
            World clientWorld = ClientServerSystemManager.clientWorld;
            Entity ent = clientWorld.GetExistingSystem<NetworkStreamReceiveSystem>().Connect(ep);

            Debug.Log("Client initialized");
            showButton = false;
        }
    }
}
