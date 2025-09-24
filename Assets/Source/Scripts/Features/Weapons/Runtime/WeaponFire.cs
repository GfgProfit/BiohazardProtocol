using System.Collections;
using UnityEngine;

public sealed class WeaponFire
{
    private readonly IWeaponContext _context;
    private readonly WeaponEffects _effects;
    private readonly WeaponConfig _config;
    private readonly Transform _barrelPoint;

    private float _lastShotT;

    public WeaponFire(IWeaponContext context, WeaponEffects effects, WeaponConfig config, Transform barrelPoint)
    {
        _context = context;
        _effects = effects;
        _config = config;
        _barrelPoint = barrelPoint;
    }

    public bool CanShootNow => Time.time - _lastShotT > 1f / (_config.FireRate / 60f);

    public bool TryShoot(bool aimHeld, MonoBehaviour host)
    {
        if (_config.CurrentAmmo <= 0)
        {
            return false;
        }

        if (!CanShootNow)
        {
            return false;
        }

        _lastShotT = Time.time;

        if (_config.FireMode == FireMode.Shotgun)
        {
            for (int i = 0; i < _config.ShotgunPellets; i++)
            {
                FireOnePellet(aimHeld, host);
            }
        }
        else
        {
            FireOnePellet(aimHeld, host);
        }

        _context.Recoil.Shoot(_config.RecoilPattern.x, _config.RecoilPattern.y, _config.RecoilPattern.z);

        _config.CurrentAmmo--;
        _effects.Muzzle(_config.EnableMuzzle, _config.MuzzlePrefabs, _config.MuzzleScaleFactor, _config.MuzzleDestroyTime);
        _effects.PlayOneFrom(_config.ShootSounds);
        _context.Crosshair.Shoot();

        return true;
    }

    private void FireOnePellet(bool aimHeld, MonoBehaviour host)
    {
        Vector3 camOrigin = _context.ShootOrigin.position;
        Vector3 dir = aimHeld
            ? _context.Forward
            : ApplySpread(_barrelPoint.forward, _config.ApplySpread, _config.SpreadVariance);

        if (Physics.Raycast(camOrigin, dir, out var hit, int.MaxValue, _config.HitScanMask))
        {
            host.StartCoroutine(SpawnTrail(_barrelPoint.position, hit.point)); // из дула в точку попадания
            TrySpawnImpact(hit);
        }
        else
        {
            Vector3 missPoint = camOrigin + dir * 100f;
            host.StartCoroutine(SpawnTrail(_barrelPoint.position, missPoint)); // из дула в “промах”
        }

        _effects.PlayShoot(
            new AnimationNames(_config.ShootName, _config.AimShootName, _config.ReloadBoolName, _config.ReloadFullBoolName, _config.HideName, _config.AimBoolName),
            aimHeld);
    }

    private IEnumerator SpawnTrail(Vector3 from, Vector3 to)
    {
        TrailRenderer trail = _effects.CreateTrail(
            _config.BulletTrail, _config.WidthCurve, _config.Duration,
            _config.MinVertexDistance, _config.TrailColor, _config.Material);

        // Критично: отвязываем от камеры/оружия и фиксируем стартовую точку в МИРЕ
        var t = trail.transform;
        t.SetParent(null, true);
        t.position = from;

        // Если используешь пул — обязательно чистим, иначе появятся “хвосты” из прошлой жизни
        trail.Clear();
        trail.emitting = true;

        yield return WeaponEffects.AnimateTrail(trail, from, to, _config.BulletSpeed);

        trail.emitting = false;
    }

    private Vector3 ApplySpread(Vector3 forward, bool apply, Vector3 variance)
    {
        if (!apply)
        {
            return forward;
        }

        float x = Random.Range(-variance.x, variance.x);
        float y = Random.Range(-variance.y, variance.y);
        float z = Random.Range(-variance.z, variance.z);

        Vector3 dir = forward + new Vector3(x, y, z);
        
        return dir.normalized;
    }

    private void TrySpawnImpact(RaycastHit hit)
    {
        Collider collider = hit.collider;

        if (collider.TryGetComponent(out Impact impact))
        {
            GameObject go = Object.Instantiate(impact.ImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));

            go.transform.parent = collider.transform;

            go.GetComponent<ImpactSound>()?.PlaySound();

            Object.Destroy(go, 2f);
        }

        if (collider.TryGetComponent(out ZombieHitScan hitscan))
        {
            if (!hitscan.enabled)
            {
                return;
            }

            float damage = _config.Damage * hitscan.DamageMultiplier;

            collider.GetComponentInParent<Zombie>()?.Damage((int)damage);
        }
    }
}
