using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.GameResource;
using _Project.Scripts.Gameplay.GameResource.Config;

namespace _Project.Scripts.Gameplay.Controllers
{
    public class ResourceSpawnerController
    {
        public readonly List<ResourceModel> AllResources = new();
        
        private readonly List<ResourceSpawnGroupComponent> _spawnPoints;
        private readonly Transform _poolParent;
        private readonly int _poolSize;

        private ObjectPool<ResourceModel> _resourcePool;
        private Dictionary<ParticleSystem, ObjectPool<ParticleSystem>> _effectPools;
        private float _spawnInterval;
        
        public ResourceSpawnerController(List<ResourceSpawnGroupComponent> spawnPoints, Transform poolParent, int poolSize)
        {
            _spawnPoints = spawnPoints;
            _poolParent = poolParent;
            _poolSize = poolSize;
        }

        public void InitializePools()
        {
            ResourceSpawnGroup anyGroup = _spawnPoints[0].GroupData;
            _resourcePool = new ObjectPool<ResourceModel>(anyGroup.ResourceDefinitions[0].Prefab, _poolSize, _poolParent);
            _effectPools = new Dictionary<ParticleSystem, ObjectPool<ParticleSystem>>();

            foreach (ResourceSpawnGroupComponent point in _spawnPoints)
            {
                foreach (ResourceDefinition definition in point.GroupData.ResourceDefinitions)
                {
                    if (definition.SpawnEffectPrefab != null && !_effectPools.ContainsKey(definition.SpawnEffectPrefab))
                        _effectPools[definition.SpawnEffectPrefab] = new ObjectPool<ParticleSystem>(definition.SpawnEffectPrefab, _poolSize, _poolParent);
                }
            }
        }

        public void SpawnAllGroups(MonoBehaviour runner)
        {
            runner.StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            int max = _spawnPoints[0].GroupData.MaxSpawnCount;

            while (true)
            {
                int activeCount = AllResources.Count(resource => resource.gameObject.activeInHierarchy);

                if (activeCount < max)
                {
                    ResourceSpawnGroupComponent point = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                    ResourceSpawnGroup data = point.GroupData;
                    ResourceDefinition definition= data.ResourceDefinitions[Random.Range(0, data.ResourceDefinitions.Count)];
                    ResourceModel resourceModel = _resourcePool.Get();
                    Vector3 raw = point.transform.position + new Vector3(Random.Range(-point.GroupData.SpawnSize.x / 2, point.GroupData.SpawnSize.x / 2),
                        0f, Random.Range(-point.GroupData.SpawnSize.z / 2, point.GroupData.SpawnSize.z / 2));
                    Vector3 planar = new Vector3(raw.x, 0f, raw.z);

                    if (NavMesh.SamplePosition(planar, out var hit, Mathf.Max(point.GroupData.SpawnSize.x, point.GroupData.SpawnSize.z) / 2,
                            NavMesh.AllAreas))
                        resourceModel.transform.position = hit.position;
                    else
                        resourceModel.transform.position = planar;

                    ParticleSystem effect = null;
                    ObjectPool<ParticleSystem> effectPool = null;

                    if (definition.SpawnEffectPrefab != null)
                    {
                        effectPool = _effectPools[definition.SpawnEffectPrefab];
                        effect = effectPool.Get();

                        effect.transform.SetParent(resourceModel.transform, false);
                        effect.transform.localPosition = Vector3.zero;
                        effect.Play();
                    }

                    resourceModel.Initialize(definition.ResourceType, definition.Amount, effect, particle =>
                    {
                        if (effectPool != null)
                        {
                            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                            particle.transform.SetParent(_poolParent, false);
                            effectPool.Release(particle);
                        }
                    });

                    AllResources.Add(resourceModel);
                }

                yield return new WaitForSeconds(_spawnInterval);
            }
        }
        
        public void SetSpawnInterval(float interval)
        {
            _spawnInterval = Mathf.Max(0.01f, interval);
        }
    }
}