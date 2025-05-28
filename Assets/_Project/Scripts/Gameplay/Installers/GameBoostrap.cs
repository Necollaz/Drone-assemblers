using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.Controllers;
using _Project.Scripts.Gameplay.Drone;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.GameResource;
using _Project.Scripts.Gameplay.UI;

namespace _Project.Scripts.Gameplay.Installers
{
    public class GameBoostrap : MonoBehaviour
    {
        [Header("Resource Spawner")]
        [SerializeField] private List<ResourceSpawnGroupComponent> _spawnPoints;
        [SerializeField] private Transform _poolParent;

        [Header("Mesh Instancer")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Mesh _sharedMesh;
        [SerializeField] private Material _sharedMaterial;
        [SerializeField] private Transform _instancesRoot;

        [Header("Bases & Drones")]
        [SerializeField] private DroneConfig _droneConfig;
        [SerializeField] private DroneView _dronePrefab;
        [SerializeField] private List<BaseComponent> _bases;
        [SerializeField, Min(1)] private int _droneCount = 3;

        [Header("Pool settings")]
        [SerializeField, Min(1)] private int _poolSize = 20;
        
        [Header("UI References")]
        [SerializeField] private List<TMP_Dropdown> _baseCountDropdowns;
        [SerializeField] private Slider _speedSlider;
        [SerializeField] private TextMeshProUGUI _speedValueLabel;
        [SerializeField] private TMP_InputField _spawnIntervalInput;
        [SerializeField] private Button _applySpawnIntervalBtn;
        [SerializeField] private Toggle _showPathsToggle;
        
        private ResourceSpawnerController _resourceSpawner;
        private StaticMeshInstancerController _meshInstancer;
        private DroneSpawnerController _droneSpawner;
        private GameUIConfigurator _gameUIConfigurator;

        private void Awake()
        {
            InitializeComponents();
        }

        private void Update()
        {
            _droneSpawner.UpdateAll();
        }
        
        private void OnDisable()
        {
            _meshInstancer.Disable();
        }
        
        private void InitializeComponents()
        {
            _resourceSpawner = new ResourceSpawnerController(_spawnPoints, _poolParent, _poolSize);
            _resourceSpawner.InitializePools();
            _resourceSpawner.SpawnAllGroups(this);
            
            _meshInstancer = new StaticMeshInstancerController(_mainCamera, _sharedMesh, _sharedMaterial, _instancesRoot);
            _meshInstancer.Enable();
            
            var basePools = new Dictionary<ParticleSystem, ObjectPool<ParticleSystem>>();
            
            foreach (BaseComponent baseComponent in _bases)
            {
                baseComponent.InitializeHoldPoints();
                
                ParticleSystem effectPrefab = baseComponent.ReceiveEffectPrefab;
                
                if (effectPrefab != null && !basePools.ContainsKey(effectPrefab))
                    basePools[effectPrefab] = new ObjectPool<ParticleSystem>(effectPrefab, _poolSize, _poolParent);
            }
            
            foreach (BaseComponent baseComponent in _bases)
                baseComponent.SetReceivePool(basePools[baseComponent.ReceiveEffectPrefab]);
            
            _droneSpawner = new DroneSpawnerController(_dronePrefab, _droneConfig, _bases, _resourceSpawner.AllResources, transform);
            _gameUIConfigurator = new GameUIConfigurator(_baseCountDropdowns, _bases, _speedSlider, _speedValueLabel, _spawnIntervalInput, _applySpawnIntervalBtn, _showPathsToggle, _droneSpawner, _resourceSpawner);
        }
    }
}