using System;
using System.Collections;
using UnityEngine;
using _Project.Scripts.Gameplay.Common;

namespace _Project.Scripts.Gameplay.Base.Features
{
    public class ResourceReceiver
    {
        private readonly ParticleSystem _prefab;
        private readonly ObjectPool<ParticleSystem> _pool;
        private readonly MonoBehaviour _runner;
        
        private int _count = 0;

        public ResourceReceiver(ParticleSystem prefab, ObjectPool<ParticleSystem> pool, MonoBehaviour runner)
        {
            _prefab = prefab;
            _pool   = pool;
            _runner = runner;
        }
        
        public event Action<int> Received;

        public void Receive(Vector3 position)
        {
            ParticleSystem effect = _pool.Get();
            effect.transform.position = position;
            effect.Play();
            
            _runner.StartCoroutine(ReleaseWhenDone(effect));

            _count++;
            
            Received?.Invoke(_count);
        }

        private IEnumerator ReleaseWhenDone(ParticleSystem effect)
        {
            yield return new WaitUntil(() => !effect.IsAlive(true));
            
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _pool.Release(effect);
        }
    }
}