using System;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

partial struct PlayerSystem : ISystem
{
    private EntityManager entityManager;

    private Entity playerEntity;
    private Entity inputEntity;

    private PlayerComponent playerComponent;
    private InputComponent inputComponent;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;

        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        playerComponent = entityManager.GetComponentData<PlayerComponent>(playerEntity);
        inputComponent = entityManager.GetComponentData<InputComponent>(inputEntity);

        Move(ref state);
        Shoot(ref state);
    }

    private void Move(ref SystemState state)
    {
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        playerTransform.Position.xz += inputComponent.Movement * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime;

        Vector2 dir = (Vector2)inputComponent.MousePos - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);
        float angle = math.degrees(math.atan2(dir.x, dir.y));

        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.up);

        entityManager.SetComponentData(playerEntity, playerTransform);
    }
    [BurstCompile]
    private void Shoot(ref SystemState state)
    {
        if (inputComponent.Shoot)
        {
            for (int i = 0; i < playerComponent.BulletCount; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);

                Entity bulletEntity = entityManager.Instantiate(playerComponent.BulletPrefab);

                ECB.AddComponent(bulletEntity, new BulletComponent { Speed = 25f, Size = 0.2f, Damage = 1f });
                ECB.AddComponent(bulletEntity, new BulletLifetimeComponent { RemainingLifeTime = 2f });

                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(bulletEntity);

                float offset = (math.frac((float)SystemAPI.Time.ElapsedTime * 3) - .5f) * (playerComponent.BulletSpread * (i - playerComponent.BulletCount / 2));
                bulletTransform.Rotation = playerTransform.Rotation * Quaternion.AngleAxis(offset, playerTransform.Up());

                bulletTransform.Position = playerTransform.Position + (playerTransform.Forward() * 1f);

                entityManager.SetComponentData(bulletEntity, bulletTransform);

                ECB.Playback(entityManager);
                ECB.Dispose();
            }
        }
    }


    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
