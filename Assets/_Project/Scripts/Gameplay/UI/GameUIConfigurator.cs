using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Common;
using _Project.Scripts.Gameplay.Controllers;

namespace _Project.Scripts.Gameplay.UI
{
    public class GameUIConfigurator
    {
        private const int MinDrones = 1;
        private const int MaxDrones = 5;
        private const int SpeedSliderMin = 1;
        private const int SpeedSliderMax = 10;
        private const float NavMeshMinSpeed = 10f;
        private const float NavMeshMaxSpeed = 100f;
        private const float SpeedSliderMidDivisor = 2f;

        private readonly DroneSpawnerController _droneSpawner;
        private readonly ResourceSpawnerController _resourceSpawner;

        private readonly List<TMP_Dropdown> _baseCountDropdowns;
        private readonly List<BaseComponent> _bases;
        private readonly Slider _speedSlider;
        private readonly TextMeshProUGUI _speedValueLabel;
        private readonly TMP_InputField _spawnIntervalInput;
        private readonly Toggle _showPathsToggle;
        private readonly Button _applySpawnIntervalButton;

        public GameUIConfigurator(List<TMP_Dropdown> baseCountDropdowns, List<BaseComponent> bases, Slider speedSlider, TextMeshProUGUI speedValueLabel, TMP_InputField spawnIntervalInput,
            Button applySpawnIntervalButton, Toggle showPathsToggle, DroneSpawnerController droneSpawner, ResourceSpawnerController resourceSpawner)
        {
            _baseCountDropdowns = baseCountDropdowns;
            _bases = bases;
            _speedSlider = speedSlider;
            _speedValueLabel = speedValueLabel;
            _spawnIntervalInput = spawnIntervalInput;
            _applySpawnIntervalButton = applySpawnIntervalButton;
            _showPathsToggle = showPathsToggle;
            _droneSpawner = droneSpawner;
            _resourceSpawner = resourceSpawner;

            Setup();
        }

        private void Setup()
        {
            for (int i = 0; i < _bases.Count; i++)
            {
                int index = i;
                TMP_Dropdown dropdown = _baseCountDropdowns[i];
                
                dropdown.ClearOptions();
                
                var options = Enumerable.Range(MinDrones, MaxDrones).Select(v => new TMP_Dropdown.OptionData(v.ToString())).ToList();
                
                dropdown.AddOptions(options);
                dropdown.value = _bases[i].InitialDroneCount - MinDrones;
                dropdown.onValueChanged.AddListener(selectedIndex => _droneSpawner.SetDronesPerBase(_bases[index], selectedIndex  + MinDrones));
            }
            
            _speedSlider.minValue = SpeedSliderMin;
            _speedSlider.maxValue = SpeedSliderMax;
            _speedSlider.onValueChanged.AddListener(rawValue =>
            {
                int sliderValue = Mathf.RoundToInt(rawValue );
                
                _speedValueLabel.text = sliderValue.ToString();
                
                float navMeshSpeed = Mathf.Lerp(NavMeshMinSpeed, NavMeshMaxSpeed, (sliderValue - SpeedSliderMin) / (float)(SpeedSliderMax - SpeedSliderMin));
                
                _droneSpawner.SetGlobalSpeed(navMeshSpeed);
            });
            
            float initialSliderValue = (SpeedSliderMin + SpeedSliderMax) / SpeedSliderMidDivisor;
            
            _speedSlider.SetValueWithoutNotify(initialSliderValue );
            _speedValueLabel.text = Mathf.RoundToInt(initialSliderValue ).ToString();
            _droneSpawner.SetGlobalSpeed(Mathf.Lerp(NavMeshMinSpeed, NavMeshMaxSpeed, (initialSliderValue  - SpeedSliderMin)/(SpeedSliderMax - SpeedSliderMin)));
            
            _applySpawnIntervalButton.onClick.AddListener(() =>
            {
                if (float.TryParse(_spawnIntervalInput.text, out float x) && x > 0f)
                    _resourceSpawner.SetSpawnInterval(x);
            });
            
            _showPathsToggle.isOn = DebugSettings.ShowDronePaths;
            
            _showPathsToggle.onValueChanged.AddListener(isOn => { DebugSettings.ShowDronePaths = isOn; });
        }
    }
}