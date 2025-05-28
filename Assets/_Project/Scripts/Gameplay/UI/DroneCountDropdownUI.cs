using System;
using System.Linq;
using TMPro;
using _Project.Scripts.Gameplay.Base;
using _Project.Scripts.Gameplay.Drone.Spawner;

namespace _Project.Scripts.Gameplay.UI
{
    public class DroneCountDropdownUI : IDisposable
    {
        private const int MinDrones = 1;
        private const int MaxDrones = 5;

        private readonly TMP_Dropdown _dropdown;
        private readonly BaseComponent _baseComponent;
        private readonly DroneSpawner _droneSpawner;

        public DroneCountDropdownUI(TMP_Dropdown dropdown, BaseComponent baseComponent, DroneSpawner droneSpawner)
        {
            _dropdown = dropdown;
            _baseComponent = baseComponent;
            _droneSpawner = droneSpawner;
            
            _dropdown.ClearOptions();
            var options = Enumerable.Range(MinDrones, MaxDrones).Select(i => new TMP_Dropdown.OptionData(i.ToString())).ToList();
            _dropdown.AddOptions(options);
            
            _dropdown.value = _baseComponent.InitialDroneCount - MinDrones;
            
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int index)
        {
            int count = index + MinDrones;
            
            _droneSpawner.SetDronesPerBase(_baseComponent, count);
        }

        public void Dispose()
        {
            _dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}