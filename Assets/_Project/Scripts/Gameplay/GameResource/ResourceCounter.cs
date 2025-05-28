using UnityEngine;
using TMPro;
using _Project.Scripts.Gameplay.Base;

namespace _Project.Scripts.Gameplay.GameResource
{
    
    [RequireComponent(typeof(BaseComponent))]
    public class ResourceCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;

        private BaseComponent _baseComponent;

        private void Awake()
        {
            _baseComponent = GetComponent<BaseComponent>();
            _baseComponent.ResourceReceived += OnResourceReceived;
        }

        private void OnDestroy()
        {
            _baseComponent.ResourceReceived -= OnResourceReceived;
        }

        private void OnResourceReceived(int total)
        {
            if (_label != null)
                _label.text = total.ToString();
        }
    }
}