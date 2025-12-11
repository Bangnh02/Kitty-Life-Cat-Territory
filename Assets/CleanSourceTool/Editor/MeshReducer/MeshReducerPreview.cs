using Sirenix.OdinInspector;
using UnityEngine;

namespace CleanSourceTool.MeshAPI
{
    /*[RequireComponent(typeof(MeshFilter))]
    public class MeshReducerPreview : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private Mesh _sharedMesh;
        
        [InfoBox("@Vertices()")]
        [SerializeField, Range(0f, 1f)] 
        private float _reductionFactor = 1f;

        private string Vertices()
        {
            return $"Vertices: {_meshFilter.sharedMesh.vertices.Length}";
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();

            if (_sharedMesh == null)
                _sharedMesh = _meshFilter.sharedMesh;

            if (_reductionFactor < 1f)
                _meshFilter.sharedMesh = MeshReducer.ReduceMesh(_sharedMesh, _reductionFactor);
            else
                _meshFilter.sharedMesh = _sharedMesh;
        }
#endif
    }*/
}