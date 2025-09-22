using UnityEngine;

public sealed class NachtDerUntotenSceneInstaller : SceneInstaller
{
    [Header("Compass")]
    [SerializeField] private Compass _compass;

    [Header("Zombie Pool Settings")]
    [SerializeField] private Zombie _zombiePrefab;
    [SerializeField] private Transform _poolRoot;
    [SerializeField] private int _preload = 50;

    protected override void Install(IContainer container)
    {
        container.BindInstance(_compass).AsSingle();

        container.Bind<IMoney>(_ => new MoneyManager(1000000)).AsSingle();

        container.Bind(_ => new WaveService()).AsSingle();

        container.Bind(c => new HealthModel(c.Resolve<WaveService>().CurrentBaseHP)).AsTransient();

        container.BindComponentPool(_zombiePrefab, _preload, maxSize: 50, _poolRoot);
    }
}