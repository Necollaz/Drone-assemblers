using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Gameplay.GameResource.Common
{
    public class ResourcePool : IResourceProvider
    {
        private readonly List<ResourceModel> _resources;

        public ResourcePool(List<ResourceModel> allResources)
        {
            _resources = allResources;
        }

        public ResourceModel AcquireNearest(Vector3 position)
        {
            ResourceModel model = null;
            float minValue = float.MaxValue;

            foreach (ResourceModel resource in _resources)
            {
                if (!resource.gameObject.activeInHierarchy || resource.IsTaken)
                    continue;

                float magnitude = (resource.transform.position - position).sqrMagnitude;
                
                if (magnitude < minValue)
                {
                    minValue = magnitude;
                    model = resource;
                }
            }

            if (model != null)
                model.IsTaken = true;

            return model;
        }

        public void Release(ResourceModel resource)
        {
            if (resource != null)
                resource.IsTaken = false;
        }
    }
}