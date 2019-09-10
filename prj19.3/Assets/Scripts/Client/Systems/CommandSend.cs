using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
public class PlayerCommandSendSystem : CommandSendSystem<PlayerCommandData>
{
}

[DisableAutoCreation]
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
[UpdateAfter(typeof(GhostReceiveSystemGroup))]
[UpdateBefore(typeof(PlayerCommandSendSystem))]
public class InputSystem : ComponentSystem
{
    EntityQuery cmdTargetGroup;

    protected override void OnCreate()
    {
        cmdTargetGroup = GetEntityQuery(
            ComponentType.ReadWrite<CommandTargetComponent>(),
            ComponentType.Exclude<NetworkStreamDisconnected>());
    }

    protected override void OnUpdate()
    {
        var entities = cmdTargetGroup.ToEntityArray(Allocator.TempJob);
        if (entities.Length == 1)
        {
            var ent = entities[0];

            if (!EntityManager.HasComponent<PlayerCommandData>(ent))
            {
                PostUpdateCommands.AddBuffer<PlayerCommandData>(ent);

                var ctc = EntityManager.GetComponentData<CommandTargetComponent>(ent);
                ctc.targetEntity = ent;
                PostUpdateCommands.SetComponent(ent, ctc);
            }
            else
            {
                var cmdBuf = EntityManager.GetBuffer<PlayerCommandData>(ent);

                PlayerCommandData cmdData = default;
                cmdData.tick = NetworkTimeSystem.predictTargetTick;
                cmdData.horizontal = Input.GetAxisRaw("Horizontal");
                cmdData.vertical = Input.GetAxisRaw("Vertical");

                cmdBuf.AddCommandData(cmdData);
            }
        }
        entities.Dispose();
    }
}