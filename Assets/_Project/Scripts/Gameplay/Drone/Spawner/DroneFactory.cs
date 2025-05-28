using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.Drone.Controllers;
using _Project.Scripts.Gameplay.GameResource.Common;

namespace _Project.Scripts.Gameplay.Drone.Spawner
{
    public class DroneFactory
    {
        private readonly DroneView _prefab;
        private readonly DroneConfig _config;
        private readonly Transform _parent;
        private readonly Dictionary<string, Material> _matCache = new();

        public DroneFactory(DroneView prefab, DroneConfig config, Transform parent)
        {
            _prefab = prefab;
            _config = config;
            _parent = parent;
        }

        public DroneController Create(BaseComponent home, IResourceProvider resourceProvider)
        {
            float sampleRadius = 20f;
            String matName = home.DroneMaterialName;
            
            if (!_matCache.ContainsKey(matName))
                _matCache[matName] = Resources.Load<Material>(matName);

            Vector3 spawn = home.DroneSpawnPoint.position;
            
            if (NavMesh.SamplePosition(spawn, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
                spawn = hit.position;

            DroneView view = Object.Instantiate(_prefab, spawn, Quaternion.identity, _parent);
            
            view.SetMaterial(_matCache[matName]);

            DroneModel model = new DroneModel(home);
            
            return new DroneController(model, view, _config, resourceProvider);
        }
    }
}