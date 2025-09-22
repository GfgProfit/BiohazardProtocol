using UnityEngine;

public sealed class TestSceneInstaller : SceneInstaller
{
    [Header("Pool Settings")]
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private Transform _poolRoot;
    [SerializeField] private int _preload = 5;

    protected override void Install(IContainer container)
    {
        // биндим пул кубиков
        container.BindComponentPool(_cubePrefab, _preload, maxSize: 50, _poolRoot);
    }
}
