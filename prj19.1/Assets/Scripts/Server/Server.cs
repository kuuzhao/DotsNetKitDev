using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;

public class Server
{
    public static void StartServer()
    {
        ClientServerSystemManager.InitServerSystems();

        Unity.Networking.Transport.NetworkEndPoint ep = Unity.Networking.Transport.NetworkEndPoint.AnyIpv4;
        ep.Port = 12345;
        World serverWorld = ClientServerSystemManager.serverWorld;
        var nsrs = serverWorld.GetExistingSystem<NetworkStreamReceiveSystem>();
        nsrs.Listen(ep);

        Debug.Log("Server started!");

        SceneManager.LoadScene("Level1");

        var em = serverWorld.EntityManager;
        Entity ent = ReplicatedPrefabMgr.CreateEntity("assets_prefabs_cube", serverWorld, "CubePlayer");
        em.AddComponent(ent, typeof(RepCubeComponentData));
        em.AddBuffer<PlayerCommandData>(ent);
        em.AddComponent(ent, typeof(GhostComponent));
    }
}
