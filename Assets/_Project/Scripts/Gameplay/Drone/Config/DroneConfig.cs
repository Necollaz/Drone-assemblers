using UnityEngine;

namespace _Project.Scripts.Gameplay.Drone.Config
{
    [CreateAssetMenu(fileName = "DroneConfig", menuName = "Game/DroneConfig")]
    public class DroneConfig : ScriptableObject
    {
        [SerializeField] private float _speed = 40f;
        [SerializeField] private float _acceleration = 8f;
        [SerializeField] private float _angularSpeed = 120f;
        [SerializeField] private float _stoppingDistance = 0.1f;
        [SerializeField] private float _gatherDuration = 2f;
        [SerializeField] private float _unloadDuration = 1f;
        
        public float Speed => _speed;
        public float Acceleration => _acceleration;
        public float AngularSpeed => _angularSpeed;
        public float StoppingDistance => _stoppingDistance;
        public float GatherDuration => _gatherDuration;
        public float UnloadDuration => _unloadDuration;
    }
}