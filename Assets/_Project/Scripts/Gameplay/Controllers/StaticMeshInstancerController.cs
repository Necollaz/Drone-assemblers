using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Scripts.Gameplay.Controllers
{
    public class StaticMeshInstancerController
    {
        private const int MaxBatchSize = 150;

        private readonly Camera _camera;
        private readonly Mesh _mesh;
        private readonly Material _material;
        private readonly Matrix4x4[] _matrices;

        public StaticMeshInstancerController(Camera camera, Mesh mesh, Material material, Transform instancesRoot)
        {
            _camera = camera;
            _mesh = mesh;
            _material = material;
            int count = instancesRoot.childCount;
            _matrices = new Matrix4x4[count];
            
            for (int i = 0; i < count; i++)
            {
                Transform child = instancesRoot.GetChild(i);
                _matrices[i] = child.localToWorldMatrix;
                
                if (child.TryGetComponent(out MeshRenderer meshRenderer))
                    meshRenderer.enabled = false;
            }

            _material.enableInstancing = true;
        }

        public void Enable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        public void Disable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext renderContext, Camera camera)
        {
            if (camera != _camera)
                return;

            for (int i = 0; i < _matrices.Length; i += MaxBatchSize)
            {
                int batch = Mathf.Min(MaxBatchSize, _matrices.Length - i);
                Matrix4x4[] slice = new Matrix4x4[batch];
                
                Array.Copy(_matrices, i, slice, 0, batch);
                Graphics.DrawMeshInstanced(_mesh, 0, _material, slice);
            }
        }
    }
}