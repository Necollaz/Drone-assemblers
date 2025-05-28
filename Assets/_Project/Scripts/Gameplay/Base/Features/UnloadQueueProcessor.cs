using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using _Project.Scripts.Gameplay.Drone;
using _Project.Scripts.Gameplay.Drone.Controllers;

namespace _Project.Scripts.Gameplay.Base.Features
{
    public class UnloadQueueProcessor
    {
        private readonly Vector3 _unloadPoint;
        private readonly int _maxConcurrent;
     
        private Queue<DroneController> _queue = new();
        private int _active = 0;

        public UnloadQueueProcessor(int maxConcurrent, Vector3 unloadPoint, Action<DroneController, Vector3> moveToUnload)
        {
            _maxConcurrent  = maxConcurrent;
            _unloadPoint   = unloadPoint;
            _moveToUnload  = moveToUnload;
        }
        
        private readonly Action<DroneController, Vector3> _moveToUnload;

        public void Enqueue(DroneController drone)
        {
            _queue.Enqueue(drone);
            
            TryProcessNext();
        }

        public void NotifyFinished(DroneController drone)
        {
            _active = Math.Max(0, _active - 1);
            
            TryProcessNext();
        }
        
        public void Remove(DroneController drone)
        {
            Queue<DroneController> newQueue = new Queue<DroneController>();
            
            while (_queue.Count > 0)
            {
                DroneController droneController = _queue.Dequeue();
                
                if (droneController != drone)
                    newQueue.Enqueue(droneController);
            }
            
            _queue = newQueue;
        }

        private void TryProcessNext()
        {
            if (_active >= _maxConcurrent)
                return;

            while (_queue.Count > 0)
            {
                DroneController nestDrone = _queue.Dequeue();
                DroneView view = nestDrone.View;
                NavMeshAgent agent = view?.Agent;
                
                if (view == null || agent == null)
                    continue;

                if (!agent.isOnNavMesh)
                {
                    if (NavMesh.SamplePosition(agent.transform.position, out var hit, agent.height + agent.radius, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                    }
                    else
                    {
                        continue;
                    }
                }

                _active++;
                
                _moveToUnload(nestDrone, _unloadPoint);
                
                break;
            }
        }
    }
}