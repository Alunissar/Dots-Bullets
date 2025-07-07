using Unity.Entities;
using UnityEngine;

class ForceLocalTransform : MonoBehaviour
{

    class ForceLocalTransformBaker : Baker<ForceLocalTransform>
    {
        public override void Bake(ForceLocalTransform authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        }
    }
}