using DG.Tweening;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup _confirmPanel;
    [SerializeField] private GlobalAudioFader _audioFader;

    [Space]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private WeaponSway _weaponSway;

    private bool _isOpened = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isOpened = !_isOpened;
            Open(_isOpened);
        }
    }

    private void Open(bool opened)
    {
        OnConfirmPanelOut();

        _canvasGroup.DOFade(opened ? 1.0f : 0.0f, 0.5f).SetEase(Ease.OutBack);

        _playerController.enabled = !_isOpened;
        _weaponManager.SwitchWeapon.GetCurrentWeapon().enabled = !_isOpened;
        _weaponSway.enabled = !_isOpened;

        Cursor.visible = opened;
        Cursor.lockState = opened ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void UnPause()
    {
        _isOpened = false;

        Open(_isOpened);
    }

    public void MuteAudio()
    {
        _audioFader.FadeToMuted(0.5f);
    }

    public void OnConfirmPanelIn()
    {
        _confirmPanel.gameObject.SetActive(true);
        _confirmPanel.DOFade(1.0f, 0.5f).SetEase(Ease.OutBack);
    }

    public void OnConfirmPanelOut()
    {
        _confirmPanel.DOFade(0.0f, 0.5f)
            .OnComplete(() =>
            {
                _confirmPanel.gameObject.SetActive(false);
            }).SetEase(Ease.OutBack);
    }
}