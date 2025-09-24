using System.Collections;
using UnityEngine;

public class ZombieAttackBarricadeState : IZombieState
{
    private readonly Zombie _self;
    private readonly BarricadeController _barricade;
    private readonly float _cooldown;
    private float _timer;
    private bool _requestedClimb;
    private const float BoolPulse = 0.05f;

    public ZombieAttackBarricadeState( Zombie self, BarricadeController barricade, float cooldown = 1.1f)
    {
        _self = self;
        _barricade = barricade;
        _cooldown = Mathf.Max(0.05f, cooldown);
    }

    public void Enter(Zombie zombie)
    {
        _timer = 0f;
        _requestedClimb = false;

        if (_barricade != null)
        {
            _self.StopAndFace(_barricade.transform.position);
        }

        _self.EnableUpperLayer(true);
        _self.BindAttackHit(OnAttackHit);
        _self.BindAttackFinished(OnAttackFinished);
    }

    public void Exit(Zombie zombie)
    {
        _self.SetUpperAttackBool(false);
        _self.EnableUpperLayer(false);
        _self.UnbindAttackHit();
        _self.UnbindAttackFinished();
    }

    public void Tick(Zombie zombie)
    {
        if (_barricade == null)
        {
            return;
        }

        if (_barricade.IsOpen)
        {
            if (!_requestedClimb)
            {
                _requestedClimb = true;
                _barricade.RequestClimb(_self);
            }

            return;
        }

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _self.SetUpperAttackBool(true);
            _self.StartCoroutine(ResetBoolSoon());

            _timer = _cooldown;
        }
    }

    private IEnumerator ResetBoolSoon()
    {
        yield return new WaitForSeconds(BoolPulse);

        _self.SetUpperAttackBool(false);
    }

    private void OnAttackHit()
    {
        if (_barricade == null)
        {
            return;
        }

        _barricade.TryHitOnePlank();

        if (_barricade.IsOpen && !_requestedClimb)
        {
            _requestedClimb = true;
            _barricade.RequestClimb(_self);
        }
    }

    private void OnAttackFinished()
    {
        _self.SetUpperAttackBool(false);
    }
}