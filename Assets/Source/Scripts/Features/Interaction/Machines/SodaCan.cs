using UnityEngine;

public class SodaCan : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;

    public void ChangeMaterial(Material sodaCanMaterial)
    {
        _meshRenderer.sharedMaterial = sodaCanMaterial;
    }
}