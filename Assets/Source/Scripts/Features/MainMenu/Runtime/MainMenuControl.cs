using DG.Tweening;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField] private CanvasGroup _confirmExitPanel;
    [SerializeField] private CanvasGroup _mainMenuPanel;
    [SerializeField] private CanvasGroup _choseMapPanel;
    [SerializeField] private MapManager _mapManager;

    public void OnConfirmPanelIn()
    {
        _confirmExitPanel.gameObject.SetActive(true);
        _confirmExitPanel.DOFade(1, 0.3f).SetEase(Ease.OutCubic);

        _mainMenuPanel.DOFade(0, 0.3f)
            .OnComplete(() =>
            {
                _mainMenuPanel.gameObject.SetActive(false);
            }).SetEase(Ease.OutCubic);
    }

    public void OnConfirmPanelOut()
    {
        _mainMenuPanel.gameObject.SetActive(true);
        _mainMenuPanel.DOFade(1, 0.3f).SetEase(Ease.OutCubic);

        _confirmExitPanel.DOFade(0, 0.3f)
            .OnComplete(() =>
            {
                _confirmExitPanel.gameObject.SetActive(false);
            }).SetEase(Ease.InCubic);
    }

    public void OnChoseMapIn()
    {
        _choseMapPanel.gameObject.SetActive(true);
        _choseMapPanel.DOFade(1, 0.3f).SetEase(Ease.OutCubic);

        _mainMenuPanel.DOFade(0, 0.3f)
            .OnComplete(() =>
            {
                _mainMenuPanel.gameObject.SetActive(false);
            }).SetEase(Ease.OutCubic);
    }

    public void OnChoseMapOut()
    {
        _mainMenuPanel.gameObject.SetActive(true);
        _mainMenuPanel.DOFade(1, 0.3f).SetEase(Ease.OutCubic);

        _choseMapPanel.DOFade(0, 0.3f)
            .OnComplete(() =>
            {
                _choseMapPanel.gameObject.SetActive(false);
            }).SetEase(Ease.InCubic);

        _mapManager.ClearSelection();
    }

    public void Exit()
    {
        Application.Quit();
    }
}