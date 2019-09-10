using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

[DisableAutoCreation]
[UpdateInGroup(typeof(ClientPresentationSystemGroup))]
public class PlayerPresentation : ComponentSystem
{
    EntityQuery createCubeGoQuery;
    EntityQuery updateCubeGoQuery;

    protected override void OnCreate()
    {
        createCubeGoQuery = GetEntityQuery(
            ComponentType.ReadOnly<RepCubeComponentData>(),
            ComponentType.Exclude<RepCubeGoCreated>());
        updateCubeGoQuery = GetEntityQuery(ComponentType.ReadOnly<RepCubeGoCreated>());
    }

    protected override void OnUpdate()
    {
        var cubeEntities = createCubeGoQuery.ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < cubeEntities.Length; ++i)
        {
            var cubeEnt = cubeEntities[i];
            ReplicatedPrefabMgr.LoadPrefabIntoEntity("assets_prefabs_cube", World, cubeEnt);
            EntityManager.AddComponentData(cubeEnt, default(RepCubeGoCreated));
        }
        cubeEntities.Dispose();

        var goEntities = updateCubeGoQuery.ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < goEntities.Length; ++i)
        {
            var goEnt = goEntities[i];

            var transform = EntityManager.GetComponentObject<Transform>(goEnt);
            var cubeData = EntityManager.GetComponentData<RepCubeComponentData>(goEnt);
            transform.position = cubeData.position;
        }
        goEntities.Dispose();
    }
}

public struct RepCubeGoCreated : IComponentData { }
