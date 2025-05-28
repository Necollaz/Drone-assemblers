using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.Controllers;
using _Project.Scripts.Gameplay.Drone;
using _Project.Scripts.Gameplay.Drone.Config;
using _Project.Scripts.Gameplay.Drone.Spawner;
using _Project.Scripts.Gameplay.GameResource;
using _Project.Scripts.Gameplay.GameResource.Common;
using _Project.Scripts.Gameplay.GameResource.Spawner;
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

        [Header("Pool settings")]
        [SerializeField, Min(1)] private int _poolSize = 20;
        
        [Header("UI References")]
        [SerializeField] private List<TMP_Dropdown> _baseCountDropdowns;
        [SerializeField] private Slider _speedSlider;
        [SerializeField] private TextMeshProUGUI _speedValueLabel;
        [SerializeField] private TMP_InputField _spawnIntervalInput;
        [SerializeField] private Button _applySpawnIntervalBtn;
        [SerializeField] private Toggle _showPathsToggle;
        
        private readonly List<IDisposable> _uiHandlers = new();
        
        private ResourceSpawner _resourceSpawner;
        private ResourcePool _resourcePool;
        private StaticMeshInstancerController _meshInstancer;
        private DroneSpawner _droneSpawner;
        
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
            _resourceSpawner = new ResourceSpawner(_spawnPoints, _poolParent, _poolSize);
            _resourceSpawner.InitializePools();
            _resourceSpawner.SpawnAllGroups(this);
            
            _resourcePool = new ResourcePool(_resourceSpawner.AllResources);
            
            _meshInstancer = new StaticMeshInstancerController(_mainCamera, _sharedMesh, _sharedMaterial, _instancesRoot);
            _meshInstancer.Enable();
            
            var basePools = new Dictionary<ParticleSystem, ObjectPool<ParticleSystem>>();
            
            foreach (BaseComponent baseComponent in _bases)
            {
                ParticleSystem effectPrefab = baseComponent.ReceiveEffectPrefab;
                
                if (effectPrefab != null && !basePools.ContainsKey(effectPrefab))
                    basePools[effectPrefab] = new ObjectPool<ParticleSystem>(effectPrefab, _poolSize, _poolParent);
            }
            
            foreach (BaseComponent baseComponent in _bases)
                baseComponent.Initialize(basePools[baseComponent.ReceiveEffectPrefab]);
            
            _droneSpawner = new DroneSpawner(_dronePrefab, _droneConfig, _bases, _resourcePool, transform);
            
            for (int i = 0; i < _bases.Count && i < _baseCountDropdowns.Count; i++)
            {
                _uiHandlers.Add(new DroneCountDropdownUI(_baseCountDropdowns[i], _bases[i], _droneSpawner));
            }
            
            _uiHandlers.Add(new SpeedSliderUI(_speedSlider, _speedValueLabel, _droneSpawner));
            _uiHandlers.Add(new SpawnIntervalUI(_spawnIntervalInput, _applySpawnIntervalBtn, _resourceSpawner));
            _uiHandlers.Add(new DebugToggleUI(_showPathsToggle));
        }
    }
}