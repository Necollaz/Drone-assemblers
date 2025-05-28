using UnityEngine;

namespace _Project.Scripts.Gameplay.Base.Features
{
    public class HoldPointAllocator
    {
        private readonly Transform[] _points;
        private readonly bool[] _occupied;

        public HoldPointAllocator(Transform[] points)
        {
            _points = points;
            _occupied = new bool[points.Length];
        }

        public Vector3 Reserve()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                if (!_occupied[i])
                {
                    _occupied[i] = true;

                    return _points[i].position;
                }
            }

            return Vector3.zero;
        }

        public void Release(Vector3 pos)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                if (_points[i].position == pos)
                {
                    _occupied[i] = false;

                    return;
                }
            }
        }
    }
}