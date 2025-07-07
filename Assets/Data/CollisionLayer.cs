using UnityEngine;

public enum CollisionLayer
{
    Default = 1 << 0,
    Wall = 1 << 6,
    Enemy = 1 << 7,
}

public class LayerMaskHelper
{
    public static uint GetLayersMask(CollisionLayer l1, CollisionLayer l2)
    {
        return (uint)l1 | (uint)l2;
    }
}
