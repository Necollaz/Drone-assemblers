using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.Drone.Controllers;
using _Project.Scripts.Gameplay.GameResource;
using _Project.Scripts.Gameplay.GameResource.Common;

namespace _Project.Scripts.Gameplay.Drone.Spawner
{
    public class DroneSpawner
    {
        private readonly DroneFactory _factory;
        private readonly DroneManager _manager = new();
        private readonly IResourceProvider _resourcePool;
        private readonly List<BaseComponent> _bases;
        
        private float _currentSpeed;
        
        public DroneSpawner(DroneView prefab, DroneConfig config, List<BaseComponent> bases, IResourceProvider resourcePool, Transform parent)
        {
            _factory = new DroneFactory(prefab, config, parent);
            _resourcePool = resourcePool;
            _bases = bases;
            _currentSpeed = config.Speed;
            
            foreach (BaseComponent home in _bases)
            {
                for (int i = 0; i < home.InitialDroneCount; i++)
                    AddNewDrone(home);
            }
        }
        
        public void UpdateAll() => _manager.UpdateAll();

        public void SetGlobalSpeed(float speed)
        {
            _currentSpeed = speed;
            
            foreach (var drone in _manager.GetAll())
            {
                NavMeshAgent agent = drone?.View?.Agent;
                
                if (agent != null)
                    agent.speed = speed;
            }
        }

        public void SetDronesPerBase(BaseComponent home, int target)
        {
            int current = _manager.CountForHome(home);

            if (target > current)
            {
                for (int i = 0; i < target - current; i++)
                {
                    DroneController newDrone = _factory.Create(home, _resourcePool);
                    
                    NavMeshAgent agent = newDrone.View.Agent;
                    
                    if (agent != null)
                        agent.speed = _currentSpeed;
                    
                    _manager.Add(newDrone);
                }
            }
            else if (target < current)
            {
                int toRemove = current - target;
                
                List<DroneController> candidates = _manager.GetAll().Where(drone => drone.Model.HomeBase == home).Take(toRemove).ToList();

                foreach (var drone in candidates)
                {
                    if (drone.Model.CurrentState == DroneState.WaitingUnload)
                        home.RemoveFromUnloadQueue(drone);
                    
                    if (drone.Model.CurrentState == DroneState.ToBase || drone.Model.CurrentState == DroneState.Unloading)
                        home.NotifyFinishedUnloading(drone);
                    
                    home.ReleaseHoldPoint(drone.HoldPosition);
                    
                    ResourceModel resource = drone.Model.TargetResource;
                    
                    if (resource != null)
                        _resourcePool.Release(resource);
                    
                    Object.Destroy(drone.View.gameObject);
                    _manager.Remove(drone);
                }
            }
        }
        
        private void AddNewDrone(BaseComponent home)
        {
            DroneController drone = _factory.Create(home, _resourcePool);
            
            if (drone.View.Agent != null)
                drone.View.Agent.speed = _currentSpeed;
            
            _manager.Add(drone);
        }
    }
}