using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.Controllers;

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
        
        private readonly int _maxUnloaders = 1; 
        
        private Queue<DroneController> _unloadQueue = new();
        private ObjectPool<ParticleSystem> _receivePool;
        
        private int _collectedCount = 0;
        private int _currentUnloaders = 0;
        private bool[] _holdPointOccupied;
        
        public Transform DroneSpawnPoint => _droneSpawnPoint;
        public ParticleSystem ReceiveEffectPrefab => _receiveEffectPrefab;
        public string DroneMaterialName => _droneMaterialName;
        public int InitialDroneCount => _initialDroneCount;

        public event Action<int> ResourceReceived;
        
        public void InitializeHoldPoints()
        {
            _holdPointOccupied = new bool[_holdPoints.Length];
        }
        
        public void EnqueueForUnload(DroneController drone)
        {
            _unloadQueue.Enqueue(drone);
            
            TryProcessQueue();
        }
        
        public void NotifyFinishedUnloading(DroneController drone)
        {
            _currentUnloaders--;
            
            TryProcessQueue();
        }
        
        public Vector3 GetHoldPosition()
        {
            for (int i = 0; i < _holdPoints.Length; i++)
            {
                if (!_holdPointOccupied[i])
                {
                    _holdPointOccupied[i] = true;
                    
                    return _holdPoints[i].position;
                }
            }
            
            return transform.position;
        }

        public void ReleaseHoldPosition(Vector3 pos)
        {
            for (int i = 0; i < _holdPoints.Length; i++)
            {
                if (_holdPoints[i].position == pos)
                {
                    _holdPointOccupied[i] = false;
                    return;
                }
            }
        }
        
        public void SetReceivePool(ObjectPool<ParticleSystem> pool) => _receivePool = pool;

        public void Receive()
        {
            if (_receiveEffectPrefab == null || _receivePool == null)
                return;

            ParticleSystem effect = _receivePool.Get();
            effect.transform.position = _resourceUnloadPoint.position;
            effect.transform.rotation = Quaternion.identity;
            effect.Play();

            StartCoroutine(ReleaseEffectRoutine(effect));
            
            _collectedCount++;
            
            ResourceReceived?.Invoke(_collectedCount);
        }

        private IEnumerator ReleaseEffectRoutine(ParticleSystem effect)
        {
            yield return new WaitUntil(() => !effect.IsAlive(true));

            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _receivePool.Release(effect);
        }
        
        private void TryProcessQueue()
        {
            if (_currentUnloaders >= _maxUnloaders || _unloadQueue.Count == 0)
                return;

            DroneController next = _unloadQueue.Dequeue();
            _currentUnloaders++;

            next.MoveToUnloadPoint(_resourceUnloadPoint.position);
        }
    }
}