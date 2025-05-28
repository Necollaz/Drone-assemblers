using System;
using UnityEngine;

namespace _Project.Scripts.Gameplay.GameResource
{
    public class ResourceModel : MonoBehaviour
    {
        private ParticleSystem _effectInstance;

        public string ResourceType { get; private set; }
        public int Amount { get; private set; }
        public bool IsTaken { get; set; }

        private Action<ParticleSystem> _releaseEffect;

        public void Initialize(string resourceType, int amount, ParticleSystem effectInstance, Action<ParticleSystem> releaseEffectCallback)
        {
            ResourceType = resourceType;
            Amount = amount;
            IsTaken = false;

            gameObject.SetActive(true);

            _effectInstance = effectInstance;
            _releaseEffect = releaseEffectCallback;
        }

        public void Collect()
        {
            if (_effectInstance != null && _releaseEffect != null)
            {
                _releaseEffect(_effectInstance);

                _effectInstance = null;
                _releaseEffect = null;
            }

            IsTaken = false;
            gameObject.SetActive(false);
        }
    }
}