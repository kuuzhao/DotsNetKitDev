using Unity.Entities;
using Unity.Collections;
using Unity.DotsNetKit.NetCode;
using UnityEngine;

[DisableAutoCreation]
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ServerConnection : ComponentSystem
{
    public static int sNetworkId = -1;

    EntityQuery connectionQuery;
    protected override void OnCreate()
    {
        connectionQuery = GetEntityQuery(
            typeof(NetworkIdComponent),
            ComponentType.Exclude<NetworkStreamInGame>()
        );
    }

    protected override void OnUpdate()
    {
        var connectionEntities = connectionQuery.ToEntityArray(Allocator.TempJob);
        var networkIdComps = connectionQuery.ToComponentDataArray<NetworkIdComponent>(Allocator.TempJob);

        if (connectionEntities.Length == 1)
        {
            var connectionEntity = connectionEntities[0];
            var networkIdComp = networkIdComps[0];

            sNetworkId = networkIdComp.Value;
            Console.WriteLine(string.Format("NetworkId({0}) Assigned .", sNetworkId));

            EntityManager.AddComponentData(connectionEntity, new NetworkStreamInGame());
        }

        connectionEntities.Dispose();
        networkIdComps.Dispose();
    }
}
