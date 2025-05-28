using UnityEngine;

namespace _Project.Scripts.Gameplay.GameResource.Common
{
    public interface IResourceProvider
    {
        public ResourceModel AcquireNearest(Vector3 position);
        
        public void Release(ResourceModel resource);
    }
}