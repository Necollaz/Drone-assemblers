using System;
using UnityEngine;
using _Project.Scripts.Gameplay.Base.Features;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.Drone.Controllers;

namespace _Project.Scripts.Gameplay.Base
{
    public class BaseComponent : MonoBehaviour
    {
        [Header("Unload & Hold Points")]
        [SerializeField] private Transform _droneSpawnPoint;
        [SerializeField] private Transform _resourceUnloadPoint;
        [SerializeField] private Transform[] _holdPoints;

        [Header("Visuals")]
        [SerializeField] private string _droneMaterialName;

        [Header("Unload FX")]
        [SerializeField] private ParticleSystem _receiveEffectPrefab;
        [SerializeField, Range(1, 5)] private int _initialDroneCount = 3;

        private HoldPointAllocator _holdAllocator;
        private UnloadQueueProcessor _unloadProcessor;
        private ResourceReceiver _receiver;

        public Transform DroneSpawnPoint => _droneSpawnPoint;
        public ParticleSystem ReceiveEffectPrefab => _receiveEffectPrefab;
        public string DroneMaterialName => _droneMaterialName;
        public int InitialDroneCount => _initialDroneCount;

        public event Action<int> ResourceReceived;

        public void Initialize(ObjectPool<ParticleSystem> effectObjectPool)
        {
            _holdAllocator = new HoldPointAllocator(_holdPoints);
            _unloadProcessor = new UnloadQueueProcessor(1, _resourceUnloadPoint.position, MoveDroneToUnload);
            _receiver = new ResourceReceiver(_receiveEffectPrefab, effectObjectPool, this);
            
            _receiver.Received += cnt => ResourceReceived?.Invoke(cnt);
        }
        
        public Vector3 GetHoldPoint() => _holdAllocator.Reserve();
        
        public void ReleaseHoldPoint(Vector3 position) => _holdAllocator.Release(position);

        public void EnqueueForUnload(DroneController drone) => _unloadProcessor.Enqueue(drone);

        public void NotifyFinishedUnloading(DroneController drone) => _unloadProcessor.NotifyFinished(drone);
        
        public void RemoveFromUnloadQueue(DroneController drone) => _unloadProcessor.Remove(drone);

        public void ReceiveResource() => _receiver.Receive(_resourceUnloadPoint.position);

        private void MoveDroneToUnload(DroneController drone, Vector3 point) => drone.MoveToUnloadPoint(point);
    }
}