using UnityEngine;
using UnityEngine.AI;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.Drone.Features;
using _Project.Scripts.Gameplay.GameResource;
using _Project.Scripts.Gameplay.GameResource.Common;

namespace _Project.Scripts.Gameplay.Drone.Controllers
{
    public class DroneController
    {
        private readonly DroneModel _model;
        private readonly DroneView _view;
        private readonly DroneConfig _config;
        private readonly IResourceProvider _resourcePool;
        private readonly DroneMovement _movement;

        private Vector3 _currentHoldPosition;
        private bool _isHolding;

        public DroneController(DroneModel model, DroneView view, DroneConfig config, IResourceProvider resourcePool)
        {
            _model = model;
            _view = view;
            _config = config;
            _resourcePool = resourcePool;
            _movement = new DroneMovement(_view.Agent);

            _movement.Arrived += OnArrived;
            _view.GatherComplete += OnGatherComplete;
            _view.UnloadComplete += OnUnloadComplete;

            _view.Initialize(_config);
        }

        public DroneView View => _view;
        public DroneModel Model => _model;
        public Vector3 HoldPosition => _currentHoldPosition;
        
        public void UpdateController()
        {
            _movement.Tick();

            if (_model.CurrentState != DroneState.Idle)
                return;

            ResourceModel resource = _resourcePool.AcquireNearest(_view.transform.position);
            if (resource != null)
            {
                _isHolding = false;
                TryAssign(resource);
            }
            else if (!_isHolding)
            {
                _currentHoldPosition = _model.HomeBase.GetHoldPoint();
                FlyTo(_currentHoldPosition);
                _isHolding = true;
            }
        }

        private void FlyTo(Vector3 target)
        {
            _movement.MoveTo(target);
            
            var path = new NavMeshPath();
            
            _view.Agent.CalculatePath(target, path);
            _view.RenderPath(path.corners);
        }

        public void MoveToUnloadPoint(Vector3 unloadPosition)
        {
            _isHolding = false;
            _model.HomeBase.ReleaseHoldPoint(_currentHoldPosition);
            _model.SetState(DroneState.ToBase);
            
            FlyTo(unloadPosition);
        }

        private void TryAssign(ResourceModel resource)
        {
            const float maxDistance = 1f;
            if (!NavMesh.SamplePosition(resource.transform.position, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
            {
                _resourcePool.Release(resource);
                
                return;
            }

            _model.SetTarget(resource);
            
            FlyTo(hit.position);
        }

        private void OnArrived()
        {
            switch (_model.CurrentState)
            {
                case DroneState.ToResource:
                    _model.SetState(DroneState.Gathering);
                    _view.Gather(_config.GatherDuration);
                    break;

                case DroneState.ToBase:
                    _model.SetState(DroneState.Unloading);
                    _view.Unload(_config.UnloadDuration);
                    break;
            }
        }

        private void OnGatherComplete()
        {
            _model.TargetResource.Collect();
            _model.SetState(DroneState.WaitingUnload);
            _model.HomeBase.EnqueueForUnload(this);

            if (_model.CurrentState == DroneState.WaitingUnload)
            {
                _currentHoldPosition = _model.HomeBase.GetHoldPoint();
                FlyTo(_currentHoldPosition);
            }
        }

        private void OnUnloadComplete()
        {
            _model.HomeBase.ReceiveResource();
            _model.HomeBase.NotifyFinishedUnloading(this);
            _model.SetState(DroneState.Idle);
        }
    }
}