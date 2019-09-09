using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using UnityEngine.SceneManagement;

public struct LevelLoadingTag : IComponentData { }

[DisableAutoCreation]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateBefore(typeof(DotsNetKit193GhostSendSystem))]
public class LoadRemoteLevelSystem : ComponentSystem
{
    EntityQuery m_NetworkConnection;
    
    protected override void OnCreateManager()
    {
        m_NetworkConnection = GetEntityQuery(
            ComponentType.ReadOnly<NetworkIdComponent>(),
            ComponentType.Exclude<LevelLoadingTag>(),
            ComponentType.Exclude<NetworkStreamInGame>());
    }

    protected override void OnUpdate()
    {
        if (m_NetworkConnection.IsEmptyIgnoreFilter)
            return;

        var entities = m_NetworkConnection.ToEntityArray(Allocator.TempJob);
        var networkIds = m_NetworkConnection.ToComponentDataArray<NetworkIdComponent>(Allocator.TempJob);

        for (int i = 0; i < entities.Length; ++i)
        {
            var ent = entities[i];
            var networkId = networkIds[i];

            // Load level RPC
            var rpcLoadLevelQueue = ClientServerSystemManager.serverWorld.GetOrCreateSystem<DotsNetKit193RpcSystem>().GetRpcQueue<RpcLoadLevel>();
            var rpcBuf = EntityManager.GetBuffer<OutgoingRpcDataStreamBufferComponent>(ent);
            rpcLoadLevelQueue.Schedule(rpcBuf, new RpcLoadLevel { levelName = SceneManager.GetActiveScene().name });

            PostUpdateCommands.AddComponent(ent, new LevelLoadingTag());
        }

        entities.Dispose();
        networkIds.Dispose();
    }
}

// TODO: LZ:
//      we may want to only enter game, after the level is loaded.
//      would be nice to have an async response of a RPC
public class EnterGameSystem : ComponentSystem
{
    EntityQuery m_NetworkConnection;
    protected override void OnCreateManager()
    {
        m_NetworkConnection = GetEntityQuery(
            ComponentType.ReadOnly<NetworkIdComponent>(),
            ComponentType.ReadWrite<LevelLoadingTag>(),
            ComponentType.Exclude<NetworkStreamInGame>());
    }

    protected override void OnUpdate()
    {
        if (m_NetworkConnection.IsEmptyIgnoreFilter)
            return;

        var entities = m_NetworkConnection.ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < entities.Length; ++i)
        {
            var ent = entities[i];
            PostUpdateCommands.RemoveComponent<LevelLoadingTag>(ent);
            PostUpdateCommands.AddComponent(ent, new NetworkStreamInGame());
        }
        entities.Dispose();
    }
}