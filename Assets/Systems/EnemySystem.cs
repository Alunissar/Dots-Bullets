
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemySystem : ISystem
{
    private EntityManager entityManager;
    private Entity playerEntity;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;

        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        EnemyMoveJob moveJob = new EnemyMoveJob { deltaTime = SystemAPI.Time.DeltaTime, playerTransform = playerTransform };

        moveJob.ScheduleParallel();


    }
}

[BurstCompile]
public partial struct EnemyMoveJob : IJobEntity
{
    public float deltaTime;
    public LocalTransform playerTransform;
    public void Execute(ref LocalTransform enemyTransform, in EnemyComponent enemyComponent)
    {
        float3 playerDir = math.normalize(playerTransform.Position - enemyTransform.Position);

        enemyTransform.Position += playerDir * enemyComponent.Speed * deltaTime;
        enemyTransform.Rotation = quaternion.LookRotation(playerDir, new float3(0, 1, 0));
    }
}