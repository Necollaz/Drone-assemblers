using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Gameplay.GameResource.Config
{
    [CreateAssetMenu(fileName = "ResourceSpawnGroup", menuName = "Game/ResourceSpawnGroup")]
    public class ResourceSpawnGroup : ScriptableObject
    {
        [SerializeField] private List<ResourceDefinition> _resourceDefinitions;
        [SerializeField] private Vector3 _spawnSize;
        [SerializeField] private int _maxSpawnCountResources;
        [SerializeField] private float _spawnInterval = 0.5f;
        
        public IReadOnlyList<ResourceDefinition> ResourceDefinitions => _resourceDefinitions;
        public Vector3 SpawnSize  => _spawnSize;
        public int MaxSpawnCount => _maxSpawnCountResources;
    }
}