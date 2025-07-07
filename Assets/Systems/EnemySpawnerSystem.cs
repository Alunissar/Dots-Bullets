using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;

partial struct EnemySpawnerSystem : ISystem
{
    private EntityManager entityManager;

    private Entity enemySpawnerEntity;
    private EnemySpawnerComponent enemySpawnerComponent;
    private Entity playerEntity;

    private Unity.Mathematics.Random random;
    
    public void OnCreate(ref SystemState state)
    {
        random = Unity.Mathematics.Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;
        enemySpawnerEntity = SystemAPI.GetSingletonEntity<EnemySpawnerComponent>();
        enemySpawnerComponent = entityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnerEntity);

        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

        SpawnEnemies(ref state);
    }

    [BurstCompile]
    private void SpawnEnemies(ref SystemState state)
    {
        //timer
        enemySpawnerComponent.TimeToNextSpawn -= SystemAPI.Time.DeltaTime;
        
        if (enemySpawnerComponent.TimeToNextSpawn < 0)
        {


            while (enemySpawnerComponent.TimeToNextSpawn < 0)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
                Entity enemyEntity = entityManager.Instantiate(enemySpawnerComponent.EnemyPrefab);

                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
                LocalTransform enemyTransform = entityManager.GetComponentData<LocalTransform>(enemyEntity);

                enemyTransform.Position.xz = playerTransform.Position.xz + random.NextFloat2Direction() * enemySpawnerComponent.SpawnRadius;

                enemyTransform.Rotation = quaternion.LookRotation(enemyTransform.Position - playerTransform.Position, new float3(0, 1, 0));

                ECB.SetComponent(enemyEntity, enemyTransform);

                ECB.AddComponent(enemyEntity, new EnemyComponent { CurrentHealth = 20f, Speed = 3f });

                ECB.Playback(entityManager);
                ECB.Dispose();

                enemySpawnerComponent.TimeToNextSpawn += 1 / enemySpawnerComponent.EnemiesPerSecond;
                enemySpawnerComponent.EnemiesPerSecond = math.min(enemySpawnerComponent.EnemiesPerSecond + enemySpawnerComponent.EnemiesIncrement, enemySpawnerComponent.MaxEnemies);
            }
        }
        
        entityManager.SetComponentData(enemySpawnerEntity, enemySpawnerComponent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
