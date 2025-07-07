using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct InputComponent : IComponentData
{
    public float2 Movement;
    public float2 MousePos;
    public bool Shoot;
}
