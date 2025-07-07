using Unity.Entities;

public struct PlayerComponent : IComponentData
{
    public float MoveSpeed;
    public Entity BulletPrefab;
    public int BulletCount;
    public float BulletSpread;
}
