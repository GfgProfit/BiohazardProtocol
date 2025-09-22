using UnityEngine;

public class ZombieMeshGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] _meshes;

    private void Awake()
    {
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        int randomIndex = Random.Range(0, _meshes.Length);

        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i].SetActive(false);
        }

        _meshes[randomIndex].SetActive(true);
    }
}