using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup _self;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private TMP_Text _sceneNameText;
    [SerializeField] private TMP_Text _currentLoading;
    [SerializeField] private Image _progressFillImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Canvas _rootCanvas;

    [Header("Pseudo Lines")]
    [TextArea]
    [SerializeField]
    private string[] _pseudoLines =
    {
        "Loading meshes…",
        "Baking lighting…",
        "Synchronizing navigation…",
        "Compiling shaders…",
        "Loading audio banks…",
        "Warming up object pools…",
        "Optimizing splines…",
        "Fueling zombies with coffee…",
    };

    [Header("Tuning")]
    [SerializeField] private Vector2 _fakeTimeRangeSeconds = new(5f, 15f);
    [SerializeField, Range(0.2f, 3f)] private float _uiSmooth = 1.0f;
    [SerializeField, Range(0.1f, 3f)] private float _lineChangeMin = 0.6f;
    [SerializeField, Range(0.2f, 5f)] private float _lineChangeMax = 1.6f;
    [SerializeField] private bool _autoActivateOnReady = true;

    private float _shownProgress;
    private AsyncOperation _op;
    private float _fakeDuration;
    private float _elapsed;

    private void Awake()
    {
        if (_rootCanvas == null)
        {
            _rootCanvas = GetComponentInChildren<Canvas>(true);
        }

        DontDestroyOnLoad(gameObject);

        _self.DOFade(1.0f, 0.3f).SetEase(Ease.OutCubic);
    }

    public void Begin(MapItem map)
    {
        if (_sceneNameText != null)
        {
            _sceneNameText.text = string.IsNullOrEmpty(map.Name) ? map.SceneName : map.Name;
        }

        if (_backgroundImage != null && map.MapBackgrounds != null && map.MapBackgrounds.Length > 0)
        {
            Sprite pick = map.MapBackgrounds[Random.Range(0, map.MapBackgrounds.Length)];
            _backgroundImage.sprite = pick;
            _backgroundImage.preserveAspect = true;
        }

        float min = Mathf.Min(_fakeTimeRangeSeconds.x, _fakeTimeRangeSeconds.y);
        float max = Mathf.Max(_fakeTimeRangeSeconds.x, _fakeTimeRangeSeconds.y);

        _fakeDuration = Random.Range(min, max);
        _elapsed = 0f;
        _shownProgress = 0f;

        StartCoroutine(LoadRoutine(map.SceneName));
        StartCoroutine(PseudoLinesRoutine());
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        _op = SceneManager.LoadSceneAsync(sceneName);

        if (_op == null)
        {
            Debug.LogError("[LoadingScreen] SceneManager.LoadSceneAsync returned null.");
            yield break;
        }

        _op.allowSceneActivation = false;

        while (true)
        {
            _elapsed += Time.unscaledDeltaTime;

            float fakeCap = (_fakeDuration <= 0f) ? 1f : Mathf.Clamp01(_elapsed / _fakeDuration);
            float target = Mathf.Lerp(0f, 0.99f, fakeCap);

            UpdateUiTowards(target);

            if (_op.progress >= 0.9f && _elapsed >= _fakeDuration)
            {
                break;
            }

            yield return null;
        }

        while (_shownProgress < 1f - 0.0001f)
        {
            UpdateUiTowards(1f);
            yield return null;
        }

        if (_autoActivateOnReady)
        {
            _op.allowSceneActivation = true;
        }

        while (!_op.isDone)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    private void UpdateUiTowards(float target01)
    {
        _shownProgress = Mathf.MoveTowards(_shownProgress, target01, _uiSmooth * Time.unscaledDeltaTime);

        if (_progressFillImage != null)
        {
            _progressFillImage.fillAmount = _shownProgress;
        }

        if (_progressText != null)
        {
            _progressText.text = Mathf.RoundToInt(_shownProgress * 100f) + "%";
        }
    }

    private IEnumerator PseudoLinesRoutine()
    {
        if (_currentLoading == null || _pseudoLines == null || _pseudoLines.Length == 0)
        {
            yield break;
        }

        while (_op == null || !_op.isDone)
        {
            _currentLoading.text = _pseudoLines[Random.Range(0, _pseudoLines.Length)];
            yield return new WaitForSecondsRealtime(Random.Range(_lineChangeMin, _lineChangeMax));
        }
    }
}