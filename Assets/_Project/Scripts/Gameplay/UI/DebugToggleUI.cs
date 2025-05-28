using System;
using UnityEngine.UI;
using _Project.Scripts.Gameplay.Common;

namespace _Project.Scripts.Gameplay.UI
{
    public class DebugToggleUI : IDisposable
    {
        private readonly Toggle _toggle;

        public DebugToggleUI(Toggle toggle)
        {
            _toggle = toggle;
            _toggle.isOn = DebugSettings.ShowDronePaths;
            
            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool on)
        {
            DebugSettings.ShowDronePaths = on;
        }

        public void Dispose()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }
}