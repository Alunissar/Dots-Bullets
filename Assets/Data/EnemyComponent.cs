using Unity.Entities;

public struct EnemyComponent : IComponentData
{
    public float CurrentHealth;
    public float Speed;
}
