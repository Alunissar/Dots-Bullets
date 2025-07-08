using Unity.Entities;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public GameObject BulletPrefab;
    public int BulletCount = 10;
    [Range(0f, 180f)]
    public float BulletSpread = 5f;

    public GameObject spriteFrd;
    public GameObject spriteBck;

    class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(playerEntity, new PlayerComponent
            {
                MoveSpeed = authoring.MoveSpeed,
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                BulletCount = authoring.BulletCount,
                BulletSpread = authoring.BulletSpread,
                spriteFrd = GetEntity(authoring.spriteFrd, TransformUsageFlags.Renderable),
                spriteBck = GetEntity(authoring.spriteBck, TransformUsageFlags.Renderable),
            });


        }
    }
}
