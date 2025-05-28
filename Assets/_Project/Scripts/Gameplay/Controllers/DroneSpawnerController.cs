using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Drone;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.GameResource;

namespace _Project.Scripts.Gameplay.Controllers
{
    public class DroneSpawnerController
    {
        private readonly DroneView _prefab;
        private readonly DroneConfig _config;
        private readonly Transform _parent;
        
        private readonly List<ResourceModel> _allResources;
        private readonly List<DroneController> _drones = new();

        public DroneSpawnerController(DroneView prefab, DroneConfig config, List<BaseComponent> bases,
            List<ResourceModel> resources, Transform parent)
        {
            _prefab = prefab;
            _config = config;
            _allResources = resources;
            _parent = parent;

            foreach (var home in bases)
                SpawnForBase(home, home.InitialDroneCount);
        }

        public void UpdateAll()
        {
            foreach (DroneController drone in _drones)
                drone.UpdateController();
        }
        
        public void SetGlobalSpeed(float navMeshSpeed)
        {
            foreach (DroneController drone in _drones)
                drone.View.Agent.speed = navMeshSpeed;
        }
        
        public void SetDronesPerBase(BaseComponent home, int targetCount)
        {
            List<DroneController> existing = _drones.Where(controller => controller.Model.HomeBase == home).ToList();
            int delta = targetCount - existing.Count;
            
            if (delta > 0)
            {
                SpawnForBase(home, delta);
            }
            else if (delta < 0)
            {
                for (int i = 0; i < -delta; i++)
                {
                    DroneController removeDrone = existing[i];
                    
                    if (removeDrone.Model.TargetResource != null)
                        removeDrone.Model.TargetResource.IsTaken = false;
                    
                    Object.Destroy(removeDrone.View.gameObject);
                    _drones.Remove(removeDrone);
                }
            }
        }
        
        private void SpawnForBase(BaseComponent home, int count)
        {
            float sampleRadius = 20f;
            
            Material mat = Resources.Load<Material>(home.DroneMaterialName);
            
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPosition = home.DroneSpawnPoint.position;
                
                if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
                    spawnPosition = hit.position;
                
                var view = Object.Instantiate(_prefab, spawnPosition, Quaternion.identity, _parent);
                view.SetMaterial(mat);
                
                DroneModel model = new DroneModel(home);
                DroneController controller  = new DroneController(model, view, _config, _allResources);
                _drones.Add(controller);
            }
        }
    }
}