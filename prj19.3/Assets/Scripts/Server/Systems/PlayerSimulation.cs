﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class PlayerManager
{
    public static Entity CreatePlayer(Entity networkConnectionEnt, int networkId)
    {
        var world = ClientServerSystemManager.serverWorld;
        var em = world.EntityManager;

        Entity ent = ReplicatedPrefabMgr.CreateEntity("assets_prefabs_cube", world, "CubePlayer");
        em.AddComponent(ent, typeof(RepCubeComponentData));
        em.AddBuffer<PlayerCommandData>(ent);
        em.AddComponent(ent, typeof(GhostComponent));

        var ctc = em.GetComponentData<CommandTargetComponent>(networkConnectionEnt);
        ctc.targetEntity = ent;
        em.SetComponentData(networkConnectionEnt, ctc);

        var cubeData = new RepCubeComponentData {
            networkId = networkId,
            position = new Unity.Mathematics.float3(
                Random.Range(-5.0f, 5.0f), 0f, Random.Range(-5.0f, 5.0f))
        };
        em.SetComponentData(ent, cubeData);

        return ent;
    }
}

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
[UpdateAfter(typeof(PlayerCommandReceiveSystem))]
[UpdateBefore(typeof(DotsNetKit193GhostSendSystem))]
public class PlayerMovement : ComponentSystem
{
    ServerSimulationSystemGroup m_ServerSimulationSystemGroup;
    EntityQuery m_Players;
    protected override void OnCreate()
    {
        m_ServerSimulationSystemGroup = World.GetOrCreateSystem<ServerSimulationSystemGroup>();

        m_Players = GetEntityQuery(
            ComponentType.ReadWrite<RepCubeComponentData>(),
            ComponentType.ReadOnly<PlayerCommandData>(),
            ComponentType.ReadWrite<Transform>());
    }

    protected override void OnUpdate()
    {
        var entities = m_Players.ToEntityArray(Allocator.TempJob);
        var cubes = m_Players.ToComponentDataArray<RepCubeComponentData>(Allocator.TempJob);
        var transforms = m_Players.ToComponentArray<Transform>();

        for (int i = 0; i < entities.Length; ++i)
        {
            var ent = entities[i];
            var cube = cubes[i];
            var tr = transforms[i];

            var cmdBuf = EntityManager.GetBuffer<PlayerCommandData>(ent);
            PlayerCommandData cmd;
            cmdBuf.GetDataAtTick(m_ServerSimulationSystemGroup.ServerTick, out cmd);

            if (cmd.horizontal > 0.000001f)
                cube.position.x += cmd.horizontal;
            cube.position.z += cmd.vertical;
            PostUpdateCommands.SetComponent(ent, cube);

            tr.position = cube.position;
        }

        entities.Dispose();
        cubes.Dispose();
    }
}