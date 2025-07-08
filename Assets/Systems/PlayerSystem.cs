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

    private Vector2 faceDirection;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

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

        faceDirection = math.normalize((Vector2)inputComponent.MousePos - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position));
        
        playerTransform.Rotation = Quaternion.LookRotation(new Vector3(faceDirection.x,0), Vector3.forward);
        
        LocalTransform sprtFdTfm = entityManager.GetComponentData<LocalTransform>(playerComponent.spriteFrd);
        LocalTransform sprtBkTfm = entityManager.GetComponentData<LocalTransform>(playerComponent.spriteBck);

        sprtFdTfm.Scale = faceDirection.y>0? 0f : 1f;
        sprtBkTfm.Scale = faceDirection.y<=0? 0f : 1f;

        entityManager.SetComponentData(playerComponent.spriteFrd, sprtFdTfm);
        entityManager.SetComponentData(playerComponent.spriteBck, sprtBkTfm);
    
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
                
                float angle = math.degrees(math.atan2(faceDirection.x, faceDirection.y));

                float offset = (math.frac((float)SystemAPI.Time.ElapsedTime * 3) - .5f) * (playerComponent.BulletSpread * (i - playerComponent.BulletCount / 2));
                bulletTransform.Rotation = Quaternion.AngleAxis(angle + offset, new float3(0,1,0));

                bulletTransform.Position.xz = playerTransform.Position.xz + ((float2)faceDirection * 1f);

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
