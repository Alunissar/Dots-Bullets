using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public partial class InputSystem : SystemBase
{
    private InputSystem_Actions actionMap;

    protected override void OnCreate()
    {
        if (!SystemAPI.TryGetSingleton(out InputComponent input))
        {
            EntityManager.CreateEntity(typeof(InputComponent));
        }

        actionMap = new InputSystem_Actions();
        actionMap.Enable();
    }
    protected override void OnUpdate()
    {
        Vector2 moveVec = actionMap.Player.Move.ReadValue<Vector2>();
        Vector2 mousePos = actionMap.Player.MousePos.ReadValue<Vector2>();
        bool shoot = actionMap.Player.Shoot.IsPressed();

        SystemAPI.SetSingleton(new InputComponent
        {
            Movement = moveVec,
            MousePos = mousePos,
            Shoot = shoot
        });
    }
    
}
