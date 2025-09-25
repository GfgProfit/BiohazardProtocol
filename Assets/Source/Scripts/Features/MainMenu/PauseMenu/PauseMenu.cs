using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GlobalAudioFader _audioFader;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _pausePanelButtonHolder;

    [Space]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private WeaponManager _weaponManager;
    [SerializeField] private WeaponSway _weaponSway;
    [SerializeField] private InteractDetector _interactDetector;
    [SerializeField] private AimDetector _aimDetector;

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
        _pausePanel.SetActive(opened);
        _pausePanelButtonHolder.SetActive(true);
        _confirmPanel.SetActive(false);
        _optionsPanel.SetActive(false);

        _playerController.enabled = !_isOpened;
        _weaponManager.SwitchWeapon.GetCurrentWeapon().enabled = !_isOpened;
        _weaponSway.enabled = !_isOpened;
        _interactDetector.enabled = !_isOpened;
        _aimDetector.enabled = !_isOpened;

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
}