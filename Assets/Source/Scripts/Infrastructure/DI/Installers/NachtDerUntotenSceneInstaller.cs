using UnityEngine;

public sealed class NachtDerUntotenSceneInstaller : SceneInstaller
{
    [Header("Compass")]
    [SerializeField] private Compass _compass;

    [Header("Weapon")]
    [SerializeField] private WeaponManager _weaponManager;

    [Header("Zombie Pool Settings")]
    [SerializeField] private Zombie _zombiePrefab;
    [SerializeField] private Transform _poolRoot;
    [SerializeField] private int _preload = 50;

    [Header("Started Money")]
    [SerializeField] private int _moneyOnStart = 1000;

    protected override void Install(IContainer container)
    {
        container.BindInstance(_compass).AsSingle();
        container.BindInstance(_weaponManager).AsSingle();
        container.Bind<IZoneService>(_ => new ZoneService()).AsSingle();
        container.Bind<IMoney>(_ => new MoneyManager(_moneyOnStart)).AsSingle();
        container.Bind(_ => new WaveService()).AsSingle();
        container.Bind(c => new HealthModel(c.Resolve<WaveService>().CurrentBaseHP)).AsTransient();
        container.BindComponentPool(_zombiePrefab, _preload, maxSize: 50, _poolRoot);
    }
}