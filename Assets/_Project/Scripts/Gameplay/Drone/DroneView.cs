using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.Drone.Features;

namespace _Project.Scripts.Gameplay.Drone
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(LineRenderer))]
    public class DroneView : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;

        private DroneMovement _movement;
        private Renderer _renderer;
        private LineRenderer _pathRenderer;
        private Vector3[] _lastPathCorners = new Vector3[0];
        
        public NavMeshAgent Agent => _agent;
        
        public event Action Arrived;
        public event Action GatherComplete;
        public event Action UnloadComplete;

        public void Initialize(DroneConfig config)
        {
            _agent = GetComponent<NavMeshAgent>();
            _pathRenderer = GetComponent<LineRenderer>();

            _agent.speed = config.Speed;
            _agent.acceleration = config.Acceleration;
            _agent.angularSpeed = config.AngularSpeed;
            _agent.stoppingDistance = config.StoppingDistance;
            _agent.updatePosition = true;
            _agent.updateRotation = true;
            _agent.updateUpAxis = true;


            _movement = new DroneMovement(_agent);
            _movement.Arrived += () => Arrived?.Invoke();
            
            _pathRenderer.positionCount = 0;
            _pathRenderer.enabled = DebugSettings.ShowDronePaths;
        }

        public void MoveTo(Vector3 worldPosition)
        {
            NavMeshPath path = new NavMeshPath();
            
            _agent.CalculatePath(worldPosition, path);
            _lastPathCorners = path.corners;
            
            _pathRenderer.positionCount = _lastPathCorners.Length;
            _pathRenderer.SetPositions(_lastPathCorners);

            _movement.Moving(worldPosition);
        }

        public void Tick()
        {
            _movement.Tick();
            
            _pathRenderer.enabled = DebugSettings.ShowDronePaths;
        }

        public void Gather(float duration)
        {
            _movement.StopMoving();

            StartCoroutine(Delayed(duration, () => GatherComplete?.Invoke()));
        }

        public void Unload(float duration)
        {
            _movement.StopMoving();

            StartCoroutine(Delayed(duration, () => UnloadComplete?.Invoke()));
        }
        
        public void SetMaterial(Material material)
        {
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            if (_renderer != null && material != null)
                _renderer.material = material;
        }

        private IEnumerator Delayed(float time, Action callback)
        {
            yield return new WaitForSeconds(time);

            callback();
        }
    }
}