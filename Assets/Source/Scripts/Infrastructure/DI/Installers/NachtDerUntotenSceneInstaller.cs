public sealed class NachtDerUntotenSceneInstaller : SceneInstaller
{
    protected override void Install(IContainer container)
    {
        container.Bind<IMoney>(_ => new MoneyManager(1000000)).AsSingle();
    }
}