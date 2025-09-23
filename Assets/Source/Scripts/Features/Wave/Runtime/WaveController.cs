using System.Collections;
using UnityEngine;

#pragma warning disable IDE0044

public sealed class WaveController : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    [Space]
    [SerializeField] private WaveStartAnimations _newWaveAnimations;
    [SerializeField] private WaveClearedAnimations _clearedWaveAnimations;
    [SerializeField] private WaveCountdownTimerView _countdownTimerView;

    [Space]
    [SerializeField] private float _spawnCooldown = 0.5f;

    [Inject] private WaveService _waveService;
    [Inject] private IPool<Zombie> _pool;

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
            SpawnOne();
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

    private void SpawnOne()
    {
        Zombie zombie = _pool.Get();
        Transform randomPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        
        zombie.SetSpawnPoint(randomPoint);
        zombie.Died += OnZombieDeath;

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
        if (_spawnPoints != null && _spawnPoints.Length > 0)
        {
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_spawnPoints[i].position, 1.0f);

                Gizmos.color = Color.white;
                Gizmos.DrawSphere(_spawnPoints[i].position, 0.1f);

                Gizmos.color = Color.blue;
                Vector3 start = _spawnPoints[i].position;
                Vector3 end = start + _spawnPoints[i].forward * 2.0f;
                Gizmos.DrawLine(start, end);

                Gizmos.DrawWireSphere(end, 0.1f);

            }
        }
    }
}