using UnityEngine;

public class ZombieAttackBarricadeState : IZombieState
{
    private readonly Zombie _self;
    private readonly BarricadeController _barricade;
    private readonly float _cooldown;
    private readonly string _animTrigger;

    private float _timer;
    private bool _requestedClimb; // <Ч фиксер

    public ZombieAttackBarricadeState(Zombie self, BarricadeController barricade, float cooldown = 1.1f, string animTrigger = "Attack")
    {
        _self = self;
        _barricade = barricade;
        _cooldown = cooldown;
        _animTrigger = animTrigger;
    }

    public void Enter(Zombie zombie)
    {
        _timer = 0f;
        _requestedClimb = false; // сброс при входе
        if (_barricade != null)
            _self.StopAndFace(_barricade.transform.position);
    }

    public void Exit(Zombie zombie) { }

    public void Tick(Zombie zombie)
    {
        if (_barricade == null) return;

        // если открыто Ч запросим перелаз один раз, дальше стоим/ждЄм очереди
        if (_barricade.IsOpen)
        {
            if (!_requestedClimb)
            {
                _requestedClimb = true;
                _barricade.RequestClimb(_self);
            }
            return;
        }

        // иначе Ч бьЄм доски
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            // _self.Animator?.SetTrigger(_animTrigger);
            _barricade.TryHitOnePlank();
            _timer = _cooldown;

            if (_barricade.IsOpen && !_requestedClimb)
            {
                _requestedClimb = true;
                _barricade.RequestClimb(_self);
            }
        }
    }
}
