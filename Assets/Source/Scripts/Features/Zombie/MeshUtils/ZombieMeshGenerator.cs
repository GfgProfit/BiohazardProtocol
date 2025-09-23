using UnityEngine;

public class ZombieMeshGenerator : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] _meshes;

    [Space]
    [SerializeField] private Material[] _materials;

    private void Start()
    {
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        int meshRandomIndex = Random.Range(0, _meshes.Length);
        int materialRandomIndex = Random.Range(0, _materials.Length);

        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i].gameObject.SetActive(false);
        }

        _meshes[meshRandomIndex].gameObject.SetActive(true);
        _meshes[meshRandomIndex].sharedMaterial = _materials[materialRandomIndex];
    }
}