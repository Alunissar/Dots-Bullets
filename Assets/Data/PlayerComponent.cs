using Unity.Entities;
using UnityEngine;

public struct PlayerComponent : IComponentData
{
    public float MoveSpeed;
    public Entity BulletPrefab;
    public int BulletCount;
    public float BulletSpread;
    public Entity spriteFrd;
    public Entity spriteBck;
}
