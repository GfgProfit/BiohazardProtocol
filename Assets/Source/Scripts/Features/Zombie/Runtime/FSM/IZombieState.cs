public interface IZombieState
{
    void Enter(Zombie zombie);
    void Tick(Zombie zombie);
    void Exit(Zombie zombie);
}