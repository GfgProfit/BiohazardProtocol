using Unity.AI.Navigation;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class EnableNavMeshInInit : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _navMeshSurface;

    private void Awake()
    {
        _navMeshSurface.enabled = true;
        Destroy(gameObject);
    }
}