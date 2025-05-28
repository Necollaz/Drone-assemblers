using UnityEngine;

namespace _Project.Scripts.Gameplay.GameResource.Config
{
    [CreateAssetMenu(fileName = "ResourceDefinition", menuName = "Game/ResourceDefinition")]
    public class ResourceDefinition : ScriptableObject
    {
        [SerializeField] private ResourceModel _prefab;
        [SerializeField] private ParticleSystem _spawnEffectPrefab;
        [SerializeField] private string _resourceName;
        [SerializeField] private int _minAmount = 1;
        [SerializeField] private int _maxAmount = 5;
        
        public ResourceModel Prefab => _prefab;
        public ParticleSystem SpawnEffectPrefab => _spawnEffectPrefab;
        public string ResourceType => _resourceName;
        public int Amount => Random.Range(_minAmount, _maxAmount);
    }
}