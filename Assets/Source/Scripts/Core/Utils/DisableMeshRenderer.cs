using UnityEngine;

public class DisableMeshRenderer : MonoBehaviour
{
    private void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        Destroy(meshRenderer);
    }
}