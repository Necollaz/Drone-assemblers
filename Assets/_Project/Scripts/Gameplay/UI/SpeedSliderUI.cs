using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Project.Scripts.Gameplay.Controllers;
using _Project.Scripts.Gameplay.Drone;
using _Project.Scripts.Gameplay.Drone.Spawner;

namespace _Project.Scripts.Gameplay.UI
{
    public class SpeedSliderUI : IDisposable
    {
        private const int SliderMin = 1;
        private const int SliderMax = 10;
        private const float NavMin = 10f;
        private const float NavMax = 200f;
        private const float SpeedSliderMidDivisor = 2f;

        private readonly Slider _sliderSpeed;
        private readonly TextMeshProUGUI _valueLabel;
        private readonly DroneSpawner _droneSpawner;

        public SpeedSliderUI(Slider sliderSpeed, TextMeshProUGUI valueLabel, DroneSpawner droneSpawner)
        {
            _sliderSpeed = sliderSpeed;
            _valueLabel = valueLabel;
            _droneSpawner = droneSpawner;

            _sliderSpeed.minValue = SliderMin;
            _sliderSpeed.maxValue = SliderMax;
            _sliderSpeed.onValueChanged.AddListener(OnSliderChanged);
            
            float initialSliderValue = (SliderMin + SliderMax) / SpeedSliderMidDivisor;
            
            _sliderSpeed.SetValueWithoutNotify(initialSliderValue);
            OnSliderChanged(initialSliderValue);
        }

        private void OnSliderChanged(float rawValue)
        {
            int intVal = Mathf.RoundToInt(rawValue);
            
            _valueLabel.text = intVal.ToString();
            
            float sliderValue = (rawValue - SliderMin) / (SliderMax - SliderMin);
            float navSpeed = Mathf.Lerp(NavMin, NavMax, sliderValue);
            
            _droneSpawner.SetGlobalSpeed(navSpeed);
        }

        public void Dispose()
        {
            _sliderSpeed.onValueChanged.RemoveListener(OnSliderChanged);
        }
    }
}