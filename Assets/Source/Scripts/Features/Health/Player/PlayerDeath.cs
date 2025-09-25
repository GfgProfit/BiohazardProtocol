using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Transform _headHolder;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _endGameClip;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private CanvasGroup _hudCanvasGroup;
    [SerializeField] private WaveStartAnimations _waveStartAnimation;
    [SerializeField] private GameInfo _gameInfo;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private StatusStat _statusStat;
    [SerializeField] private CrosshairController _crosshairController;
    [SerializeField] private InteractDetector _interactDetector;
    [SerializeField] private AimDetector _aimDetector;
    [SerializeField] private PlayerHealthBarView _playerHealthBarView;
    [SerializeField] private PlayerHealthView _playerHealthView;
    [SerializeField] private PlayerHurtSound _playerHurtSound;
    [SerializeField] private GameObject _playerHeartBeat;
    [SerializeField] private CanvasGroup _fade;
    [SerializeField] private GlobalAudioFader _globalAudioFader;
    [SerializeField] private CanvasGroup _buttonToMainMenu;
    [SerializeField] private PauseMenu _pauseMenu;

    [Space]
    [SerializeField] private MapItem _mapItem;

    [Space]
    [SerializeField] private Transform _cameraHolder, _point1, _point2, _point3, _point4, _point5, _point6;
    
    [Space]
    [SerializeField] private float _duration = 30;

    private Sequence _sequence;
    private Coroutine _waitFramesRoutine;

    private void OnEnable()
    {
        _playerHealth.OnDead += Death;
    }

    private void OnDisable()
    {
        _playerHealth.OnDead -= Death;
    }

    private void Death()
    {
        _cameraTransform.SetParent(_headHolder);
        _animator.enabled = true;

        Destroy(_playerController);
        Destroy(_statusStat);
        Destroy(_crosshairController);
        Destroy(_interactDetector);
        _aimDetector.UnFocus();
        Destroy(_aimDetector);
        Destroy(_playerHealthBarView);
        Destroy(_playerHealthView);
        Destroy(_playerHurtSound);
        Destroy(_playerHeartBeat);
        Destroy(_pauseMenu.gameObject);

        StartCoroutine(_weaponManager.SwitchWeapon.GetCurrentWeapon().HideWeapon());
        _audioSource.PlayOneShot(_endGameClip);

        _hudCanvasGroup.DOFade(0, 1);

        StartCoroutine(GameOverText());
    }

    private IEnumerator GameOverText()
    {
        _waveStartAnimation.StartInAnimation();

        yield return new WaitForSeconds(2);

        Destroy(_playerHealth);

        _waveStartAnimation.StartOutAnimation();

        yield return new WaitForSeconds(1);

        _gameInfo.ShowPanel();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(SwitchCameraCoroutine());
    }

    private IEnumerator SwitchCameraCoroutine()
    {
        yield return new WaitForSeconds(2);

        ShowFade();
    }

    public void GoToMainMenu()
    {
        SceneLoader.Load(_mapItem);
    }

    private void ShowFade()
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_fade.DOFade(1, 0.5f).SetEase(Ease.OutCubic));
        _sequence.AppendCallback(() =>
        {
            if (_waitFramesRoutine != null)
            {
                StopCoroutine(_waitFramesRoutine);
            }

            _cameraTransform.SetParent(_cameraHolder);
            _cameraTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            Vector3[] waypoints = { _point1.position, _point2.position, _point3.position, _point4.position, _point5.position, _point6.position };

            _cameraTransform.DOPath(waypoints, _duration, PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.InOutSine).SetLookAt(0.1f)
                .OnComplete(() =>
                {
                    _fade.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
                    _globalAudioFader.FadeToMuted(0.5f);
                    _buttonToMainMenu.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
                });

            _waitFramesRoutine = StartCoroutine(WaitFrames(5));
        });
        _sequence.Append(_fade.DOFade(0, 0.5f).SetEase(Ease.OutCubic));
    }

    private IEnumerator WaitFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
    }
}