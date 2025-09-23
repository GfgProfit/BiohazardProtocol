using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable IDE0044

public sealed class WaveController : MonoBehaviour
{
    [SerializeField] private WaveSpawnData[] _waveSpawnDatas;

    [Space]
    [SerializeField] private WaveStartAnimations _newWaveAnimations;
    [SerializeField] private WaveClearedAnimations _clearedWaveAnimations;
    [SerializeField] private WaveCountdownTimerView _countdownTimerView;

    [Space]
    [SerializeField] private float _spawnCooldown = 0.5f;

    [Inject] private WaveService _waveService;
    [Inject] private IPool<Zombie> _pool;
    [Inject] private IZoneService _zones;

    private int _spawnedThisWave;
    private int _aliveZombies;
    private int _maxZombiesInWave;
    private Coroutine _spawnRoutine;

    private void Awake()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        _waveService.NextWave();
        _spawnedThisWave = 0;
        _aliveZombies = 0;

        _maxZombiesInWave = Mathf.Min(50, 8 + _waveService.WaveIndex * 2);

        _spawnRoutine = StartCoroutine(SpawnZombies());
        StartCoroutine(PlayInOutNewWaveAnimation());
    }

    private IEnumerator SpawnZombies()
    {
        while (_spawnedThisWave < _maxZombiesInWave)
        {
            List<WaveSpawnData> allowed = GetAllowedSpawns();

            if (allowed.Count == 0)
            {
                bool resumed = false;
                void OnOpened(string _) => resumed = true;

                _zones.ZoneOpened += OnOpened;

                while (!resumed && GetAllowedSpawns().Count == 0)
                {
                    yield return null;
                }

                _zones.ZoneOpened -= OnOpened;

                allowed = GetAllowedSpawns();
                if (allowed.Count == 0)
                {
                    yield return null;
                }
            }

            SpawnOne(allowed);

            yield return new WaitForSeconds(_spawnCooldown);
        }

        while (_aliveZombies > 0)
        {
            yield return null;
        }

        yield return PlayInOutWaveClearedAnimation();

        _countdownTimerView.StartInAnimation();

        float countdown = 15f;

        while (countdown > 0f)
        {
            countdown -= Time.deltaTime;
            _countdownTimerView.UpdateText(countdown);

            yield return null;
        }

        _countdownTimerView.StartOutAnimation();
        StartNextWave();
    }

    private List<WaveSpawnData> GetAllowedSpawns()
    {
        return _waveSpawnDatas.Where(sd => _zones.IsOpen(sd.RequiredZoneKey)).ToList();
    }

    private void SpawnOne(List<WaveSpawnData> allowed)
    {
        var data = allowed[Random.Range(0, allowed.Count)];

        Zombie zombie = _pool.Get();
        zombie.SetSpawnPoint(data.SpawnPoint);

        zombie.Died += OnZombieDeath;
        zombie.SetTarget(data.PointToBarricade);
        zombie.SetDestinationToTarget();

        _spawnedThisWave++;
        _aliveZombies++;
    }

    private void OnZombieDeath(Zombie zombie)
    {
        zombie.Died -= OnZombieDeath;
        _aliveZombies--;
    }

    private IEnumerator PlayInOutNewWaveAnimation()
    {
        _newWaveAnimations.StartInAnimation();

        yield return new WaitForSeconds(3.0f);

        _newWaveAnimations.StartOutAnimation();
    }

    private IEnumerator PlayInOutWaveClearedAnimation()
    {
        _clearedWaveAnimations.StartInAnimation();

        yield return new WaitForSeconds(3.0f);

        _clearedWaveAnimations.StartOutAnimation();
    }

    private void OnDrawGizmos()
    {
        if (_waveSpawnDatas == null || _waveSpawnDatas.Length == 0)
        {
            return;
        }

        for (int i = 0; i < _waveSpawnDatas.Length; i++)
        {
            WaveSpawnData spawnData = _waveSpawnDatas[i];
            bool canSpawn = Application.isPlaying && _zones != null ? _zones.IsOpen(spawnData.RequiredZoneKey) : string.IsNullOrEmpty(spawnData.RequiredZoneKey);

            Color main = canSpawn ? Color.red : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Color dir = canSpawn ? Color.blue : new Color(0.5f, 0.5f, 0.5f, 0.35f);

            Gizmos.color = main;
            Gizmos.DrawWireSphere(spawnData.SpawnPoint.position, 1.0f);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(spawnData.SpawnPoint.position, 0.1f);

            Gizmos.color = dir;
            Vector3 start = spawnData.SpawnPoint.position;
            Vector3 end = start + spawnData.SpawnPoint.forward * 2.0f;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(end, 0.1f);
        }

        for (int i = 0; i < _waveSpawnDatas.Length; i++)
        {
            var d = _waveSpawnDatas[i];
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(d.PointToBarricade.position, 1.0f);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(d.PointToBarricade.position, 0.1f);

            Gizmos.color = Color.yellow;
            Vector3 start = d.PointToBarricade.position;
            Vector3 end = start + d.PointToBarricade.forward * 2.0f;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(end, 0.1f);
        }
    }
}
