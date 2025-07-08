
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

    private quaternion rot1, rot2;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        rot1 = quaternion.LookRotation(new float3(1, 0, 0), new float3(0, 0, 1));
        rot2 = quaternion.LookRotation(new float3(-1, 0, 0), new float3(0, 0, 1));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;

        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        EnemyMoveJob moveJob = new EnemyMoveJob { deltaTime = SystemAPI.Time.DeltaTime, playerTransform = playerTransform, rot1 = rot1, rot2 = rot2 };

        moveJob.ScheduleParallel();


    }
}

[BurstCompile]
public partial struct EnemyMoveJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public LocalTransform playerTransform;
    [ReadOnly] public quaternion rot1;
    [ReadOnly] public quaternion rot2;
    public void Execute(ref LocalTransform enemyTransform, in EnemyComponent enemyComponent)
    {
        float3 playerDir = math.normalize(playerTransform.Position - enemyTransform.Position);

        enemyTransform.Position.xz += playerDir.xz * enemyComponent.Speed * deltaTime;
        enemyTransform.Rotation = playerDir.x > 0 ? rot1 : rot2;
    }
}