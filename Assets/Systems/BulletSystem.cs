using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Jobs;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> entities = entityManager.GetAllEntities();

        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.TempJob);

        BulletMoveJob moveJob = new BulletMoveJob { deltaTime = SystemAPI.Time.DeltaTime, ECB = ECB.AsParallelWriter() };
        moveJob.ScheduleParallel();
        state.Dependency.Complete();
        ECB.Playback(entityManager);
        ECB.Dispose();

        foreach (Entity entity in entities)
        {
            if (entityManager.HasComponent<BulletComponent>(entity))
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);

                //physics
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                float3 point1 = new float3(bulletTransform.Position - bulletTransform.Forward() * 0.15f);
                float3 point2 = new float3(bulletTransform.Position + bulletTransform.Forward() * 0.15f);

                physicsWorld.CapsuleCastAll(point1, point2, bulletComponent.Size / 2, float3.zero, 1f, ref hits, new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = LayerMaskHelper.GetLayersMask(CollisionLayer.Wall, CollisionLayer.Enemy),
                });

                for (int i = 0; i < hits.Length; i++)
                {
                    Entity hitEntity = hits[i].Entity;

                    if (entityManager.HasComponent<EnemyComponent>(hitEntity))
                    {
                        EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(hitEntity);
                        enemyComponent.CurrentHealth -= bulletComponent.Damage;
                        entityManager.SetComponentData(hitEntity, enemyComponent);

                        if (enemyComponent.CurrentHealth < 0) { entityManager.DestroyEntity(hitEntity); }
                    }

                    entityManager.DestroyEntity(entity);
                }
                hits.Dispose();
            }
        }
    }
}

[WithAll]
public partial struct BulletMoveJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ECB;
    public void Execute([EntityIndexInQuery] int index, ref LocalTransform bulletTransform, ref BulletLifetimeComponent lifetimeComponent, in BulletComponent bulletComponent, Entity entity)
    {

        bulletTransform.Position += bulletTransform.Forward() * bulletComponent.Speed * deltaTime;

        lifetimeComponent.RemainingLifeTime -= deltaTime;
        if (lifetimeComponent.RemainingLifeTime <= 0) { ECB.DestroyEntity(index, entity); }
    }
}
