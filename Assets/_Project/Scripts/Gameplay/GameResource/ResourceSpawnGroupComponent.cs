using UnityEngine;
using _Project.Scripts.Gameplay.GameResource.Config;

namespace _Project.Scripts.Gameplay.GameResource
{
    public class ResourceSpawnGroupComponent : MonoBehaviour
    {
        [SerializeField] private ResourceSpawnGroup _groupData;

        public ResourceSpawnGroup GroupData => _groupData;
        
        private void OnDrawGizmos()
        {
            if (_groupData == null)
                return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _groupData.SpawnSize);
        }
    }
}