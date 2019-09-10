using Unity.Entities;
using Unity.Collections;
using UnityEngine;

[DisableAutoCreation]
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ServerConnection : ComponentSystem
{
    public static int sNetworkId = -1;

    EntityQuery connectionQuery;
    protected override void OnCreateManager()
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
            Debug.Log(string.Format("Connection to server established. Assigned NetworkId({0}).", sNetworkId));

            EntityManager.AddComponentData(connectionEntity, new NetworkStreamInGame());
        }

        connectionEntities.Dispose();
        networkIdComps.Dispose();
    }
}
