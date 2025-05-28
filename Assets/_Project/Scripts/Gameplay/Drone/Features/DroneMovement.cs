using System;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Gameplay.Drone.Features
{
    public class DroneMovement
    {
        private readonly NavMeshAgent _agent;
        
        private Vector3 _target;

        public event Action Arrived;

        public DroneMovement(NavMeshAgent agent)
        {
            _agent = agent;
            _agent.autoBraking = true;
        }

        public void MoveTo(Vector3 worldPosition)
        {
            _target = worldPosition;
            
            EnsureOnNavMesh();
            
            _agent.isStopped = false;
            
            _agent.SetDestination(_target);
        }

        public void Tick()
        {
            if (_agent.pathPending)
                return;

            if (!_agent.isStopped && _agent.remainingDistance <= _agent.stoppingDistance)
                StopMoving();
        }

        private void StopMoving()
        {
            if (_agent.isStopped)
                return;
            
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            
            Arrived?.Invoke();
        }

        private void EnsureOnNavMesh()
        {
            if (_agent.isOnNavMesh)
                return;
            
            if (NavMesh.SamplePosition(_agent.transform.position, out NavMeshHit hit, _agent.height + _agent.radius, NavMesh.AllAreas))
            {
                _agent.Warp(hit.position);
            }
        }
    }
}