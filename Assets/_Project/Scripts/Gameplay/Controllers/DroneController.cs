using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;
using _Project.Scripts.Gameplay.Drone;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.GameResource;

namespace _Project.Scripts.Gameplay.Controllers
{
    public class DroneController
    {
        private readonly DroneModel _model;
        private readonly DroneView _view;
        private readonly DroneConfig _config;
        private readonly List<ResourceModel> _resources;

        private Vector3 _currentHoldPosition;
        private bool _isHolding;

        public DroneController(DroneModel model, DroneView view, DroneConfig config, List<ResourceModel> resources)
        {
            _model = model;
            _view = view;
            _resources = resources;
            _config = config;

            _view.Arrived += OnArrived;
            _view.GatherComplete += OnGatherComplete;
            _view.UnloadComplete += OnUnloadComplete;

            _view.Initialize(_config);
        }
        
        public DroneView View => _view;
        public DroneModel Model => _model;

        public void UpdateController()
        {
            _view.Tick();

            if (_model.CurrentDroneState == DroneState.Idle)
            {
                ResourceModel resource = _resources.FirstOrDefault(r => r.gameObject.activeInHierarchy && !r.IsTaken);

                if (resource != null)
                {
                    _isHolding = false;
                    
                    TryAssign(resource);
                }
                else if (!_isHolding)
                {
                    _currentHoldPosition = _model.HomeBase.GetHoldPosition();
                    _view.MoveTo(_currentHoldPosition);
                    
                    _isHolding = true;
                }
            }
        }

        public void MoveToUnloadPoint(Vector3 unloadPosition)
        {
            _isHolding = false;
            
            _model.HomeBase.ReleaseHoldPosition(_currentHoldPosition);
            _model.ReturnHome();
            _view.MoveTo(unloadPosition);
        }

        private void TryAssign(ResourceModel resource)
        {
            if (!NavMesh.SamplePosition(resource.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                return;

            resource.IsTaken = true;
            
            _model.SetTarget(resource);
            _view.MoveTo(hit.position);
        }

        private void OnArrived()
        {
            switch (_model.CurrentDroneState)
            {
                case DroneState.ToResource:
                    _model.StartGathering();
                    _view.Gather(_config.GatherDuration);
                    break;

                case DroneState.ToBase:
                    _model.StartUnloading();
                    _view.Unload(_config.UnloadDuration);
                    break;
            }
        }

        private void OnGatherComplete()
        {
            _model.TargetResource.Collect();
            _model.WaitUnload();
            _model.HomeBase.EnqueueForUnload(this);

            _currentHoldPosition = _model.HomeBase.GetHoldPosition();

            _view.MoveTo(_currentHoldPosition);
        }

        private void OnUnloadComplete()
        {
            _model.HomeBase.Receive();
            _model.HomeBase.NotifyFinishedUnloading(this);
            _model.FinishUnloading();
        }
    }
}