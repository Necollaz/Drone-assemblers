using System;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Gameplay.Drone.Features
{
    public class DroneMovement
    {
        private readonly NavMeshAgent _agent;
        
        public DroneMovement(NavMeshAgent agent)
        {
            _agent = agent;
        }

        public event Action Arrived;
        
        public void Moving(Vector3 worldPosition)
        {
            _agent.isStopped = false;
            
            _agent.SetDestination(worldPosition);
        }

        public void StopMoving()
        {
            if (_agent.isStopped)
                return;
            
            _agent.isStopped = true;
            
            Arrived?.Invoke();
        }
        
        public void Tick()
        {
            if (_agent.pathPending)
                return;
            
            if (!_agent.isStopped && _agent.remainingDistance <= _agent.stoppingDistance)
                StopMoving();
        }
    }
}