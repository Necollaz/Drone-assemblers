using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.Drone.Config;

namespace _Project.Scripts.Gameplay.Drone
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(LineRenderer))]
    public class DroneView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _pathRenderer;

        private Renderer _renderer;
        private Coroutine _gatherRoutine;
        private Coroutine _unloadRoutine;

        public NavMeshAgent Agent { get; private set; }
        public event Action GatherComplete;
        public event Action UnloadComplete;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            _pathRenderer = GetComponent<LineRenderer>();
            _renderer = GetComponentInChildren<Renderer>();
        }

        private void Update()
        {
            _pathRenderer.enabled = DebugSettings.ShowDronePaths;

            if (DebugSettings.ShowDronePaths && Agent.hasPath)
            {
                Vector3[] corners = Agent.path.corners;

                _pathRenderer.positionCount = corners.Length;
                _pathRenderer.SetPositions(corners);
            }
        }

        public void Initialize(DroneConfig config)
        {
            Agent.speed = config.Speed;
            Agent.acceleration = config.Acceleration;
            Agent.angularSpeed = config.AngularSpeed;
            Agent.stoppingDistance = config.StoppingDistance;
            Agent.autoBraking = true;
            Agent.updatePosition = true;
            Agent.updateRotation = true;
            Agent.updateUpAxis = true;
            
            _pathRenderer.positionCount = 0;
            _pathRenderer.enabled = DebugSettings.ShowDronePaths;
        }

        public void RenderPath(Vector3[] corners)
        {
            _pathRenderer.positionCount = corners.Length;
            _pathRenderer.SetPositions(corners);
        }

        public void Gather(float duration)
        {
            if (_gatherRoutine != null)
                StopCoroutine(_gatherRoutine);

            _gatherRoutine = StartCoroutine(Delayed(duration, () => { GatherComplete?.Invoke(); }));
        }

        public void Unload(float duration)
        {
            if (_unloadRoutine != null)
                StopCoroutine(_unloadRoutine);

            _unloadRoutine = StartCoroutine(Delayed(duration, () => { UnloadComplete?.Invoke(); }));
        }

        public void SetMaterial(Material material)
        {
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