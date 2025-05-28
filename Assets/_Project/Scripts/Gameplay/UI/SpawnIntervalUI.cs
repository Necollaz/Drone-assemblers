using System;
using UnityEngine.UI;
using TMPro;
using _Project.Scripts.Gameplay.GameResource.Spawner;

namespace _Project.Scripts.Gameplay.UI
{
    public class SpawnIntervalUI : IDisposable
    {
        private readonly TMP_InputField _inputField;
        private readonly Button _applyButton;
        private readonly ResourceSpawner _resourceSpawner;

        public SpawnIntervalUI(TMP_InputField inputField, Button applyButton, ResourceSpawner resourceSpawner)
        {
            _inputField = inputField;
            _applyButton = applyButton;
            _resourceSpawner = resourceSpawner;

            _applyButton.onClick.AddListener(OnApplyClicked);
        }

        private void OnApplyClicked()
        {
            if (float.TryParse(_inputField.text, out var value) && value > 0f)
                _resourceSpawner.SetSpawnInterval(value);
        }

        public void Dispose()
        {
            _applyButton.onClick.RemoveListener(OnApplyClicked);
        }
    }
}