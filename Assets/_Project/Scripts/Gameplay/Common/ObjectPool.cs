using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Common
{
    public class ObjectPool<T>
        where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Stack<T> _poolStack = new Stack<T>();

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T obj = GameObject.Instantiate(prefab, parent);
                
                obj.gameObject.SetActive(false);
                _poolStack.Push(obj);
            }
        }
        
        public T Get()
        {
            T prefabObject;
            
            if (_poolStack.Count > 0)
            {
                prefabObject = _poolStack.Pop();
                
                prefabObject.gameObject.SetActive(true);
            }
            else
            {
                prefabObject = GameObject.Instantiate(_prefab, _parent);
            }
            
            return prefabObject;
        }
        
        public void Release(T prefabObject)
        {
            prefabObject.gameObject.SetActive(false);
            _poolStack.Push(prefabObject);
        }
    }
}