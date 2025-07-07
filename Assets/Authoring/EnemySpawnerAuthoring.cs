using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float enemiesPerSecond = 50;
    public float enemiesIncrement = 2;
    public int maxEnemies = 200;
    public float spawnRadius = 40f;

    public float timeToFirstSpawn;
    
    class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity spawnerEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(spawnerEntity, new EnemySpawnerComponent
            {
                EnemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.None),
                EnemiesPerSecond = authoring.enemiesPerSecond,
                EnemiesIncrement = authoring.enemiesIncrement,
                MaxEnemies = authoring.maxEnemies,
                SpawnRadius = authoring.spawnRadius,
                TimeToNextSpawn = authoring.timeToFirstSpawn,
            });
        }
    }
}

