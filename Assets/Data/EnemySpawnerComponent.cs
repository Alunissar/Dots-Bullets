using Unity.Entities;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity EnemyPrefab;

    public float EnemiesPerSecond;
    public float EnemiesIncrement;
    public int MaxEnemies;
    public float SpawnRadius;

    public float TimeToNextSpawn;
    
}
