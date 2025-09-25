using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DevScreen = UnityEngine.Device.Screen;

[DisallowMultipleComponent]
public class GraphicsOptionsSelectors : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;

    [Header("Screen selectors")]
    [SerializeField] private HorizontalSelector selDisplay;
    [SerializeField] private HorizontalSelector selScreenMode;
    [SerializeField] private HorizontalSelector selResolution;
    [SerializeField] private HorizontalSelector selRefresh;

    [Header("URP / Quality selectors")]
    [SerializeField] private HorizontalSelector selRenderScale;
    [SerializeField] private HorizontalSelector selMSAA;
    [SerializeField] private HorizontalSelector selHDR;
    [SerializeField] private HorizontalSelector selDepth;
    [SerializeField] private HorizontalSelector selOpaque;
    [SerializeField] private HorizontalSelector selShadowRes;
    [SerializeField] private HorizontalSelector selShadowDist;
    [SerializeField] private HorizontalSelector selShadowCasc;
    [SerializeField] private HorizontalSelector selTexQuality;
    [SerializeField] private HorizontalSelector selAniso;
    [SerializeField] private HorizontalSelector selLodBias;
    [SerializeField] private HorizontalSelector selSoftParticles;
    [SerializeField] private HorizontalSelector selPostFx;
    [SerializeField] private HorizontalSelector selSSAO;

    [Header("Discrete options")]
    [SerializeField] private List<float> renderScaleSteps = new() { 0.7f, 0.8f, 0.9f, 1.0f, 1.1f, 1.2f };
    [SerializeField] private List<int> msaaSteps = new() { 0, 2, 4, 8 };
    [SerializeField] private List<string> onOff = new() { "Off", "On" };
    [SerializeField] private List<int> shadowResSteps = new() { 512, 1024, 2048, 4096 };
    [SerializeField] private List<float> shadowDistSteps = new() { 20, 40, 60, 80, 100, 120, 150, 200 };
    [SerializeField] private List<int> shadowCascSteps = new() { 0, 2, 4 };
    [SerializeField] private List<string> texQTexts = new() { "Ultra", "High", "Medium", "Low" };
    [SerializeField] private List<string> anisoTexts = new() { "Disable", "Enable", "Force" };
    [SerializeField] private List<float> lodBiasSteps = new() { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f };
    [SerializeField] private List<string> screenModeTexts = new() { "Windowed", "Borderless", "Fullscreen" };

    private readonly List<DisplayInfo> _displayInfos = new();
    private readonly List<string> _displayTexts = new();
    private int _displayCount = 1;
    private readonly List<(int w, int h)> _resList = new();
    private readonly Dictionary<(int w, int h), List<int>> _hzByRes = new();
    private readonly List<string> _resolutionTexts = new();
    private readonly List<string> _refreshTexts = new();
    private readonly Dictionary<HorizontalSelector, int> _lastIndex = new();
    private GfxData _data = new();
    private const string FileName = "graphics_options.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, FileName);
    private UniversalRenderPipelineAsset _urp;

    private void Awake()
    {
        _urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

        if (_urp == null)
        {
            Debug.LogError("URP Asset не найден.");
            enabled = false;
            return;
        }

        LoadFromJson();

        BuildDisplays();
        MoveToDisplay(_data.display);
        RebuildModesFromCurrentDisplay();

        if (_data.width <= 0 || _data.height <= 0)
        {
            if (_resList.Count > 0)
            {
                var last = _resList[^1];
                var hzList = _hzByRes[last];
                _data.width = last.w;
                _data.height = last.h;
                _data.refreshHz = hzList.Count > 0 ? hzList[^1] : 60;
            }
            else
            {
                _data.width = Screen.width;
                _data.height = Screen.height;
                _data.refreshHz = 60;
            }
        }

        ApplyAll(_data);
        BuildAndSyncSelectors();
        SnapshotSelectorIndices();
    }

    private void Update()
    {
        Watch(selRenderScale, i => SetRenderScale(renderScaleSteps[i]));
        Watch(selMSAA, i => SetMsaa(msaaSteps[i]));
        Watch(selHDR, i => SetHdr(i > 0));
        Watch(selDepth, i => SetDepthTexture(i > 0));
        Watch(selOpaque, i => SetOpaqueTexture(i > 0));
        Watch(selShadowRes, i => SetShadowResolution(shadowResSteps[i]));
        Watch(selShadowDist, i => SetShadowDistance(shadowDistSteps[i]));
        Watch(selShadowCasc, i => SetShadowCascades(shadowCascSteps[i]));
        Watch(selTexQuality, i => SetTextureQuality(i));
        Watch(selAniso, i => SetAnisotropicFiltering(i));
        Watch(selLodBias, i => SetLodBias(lodBiasSteps[i]));
        Watch(selSoftParticles, i => SetSoftParticles(i > 0));
        Watch(selPostFx, i => SetPostProcessing(i > 0));
        Watch(selSSAO, i => SetSSAO(i > 0));
        Watch(selScreenMode, i => SetScreenMode(i));
        Watch(selDisplay, OnDisplayChanged);
        Watch(selResolution, OnResolutionChanged);
        Watch(selRefresh, OnRefreshChanged);
    }

    private void BuildDisplays()
    {
        _displayInfos.Clear();
        _displayTexts.Clear();

        DevScreen.GetDisplayLayout(_displayInfos);
        _displayCount = Mathf.Max(1, _displayInfos.Count);

        if (_displayInfos.Count == 0)
        {
            _displayInfos.Add(default);
        }

        for (int i = 0; i < _displayInfos.Count; i++)
        {
            DisplayInfo di = _displayInfos[i];
            string label = string.IsNullOrEmpty(di.name) ? $"Display {i + 1}" : $"{di.name}";
            _displayTexts.Add(label);
        }

        _data.display = Mathf.Clamp(_data.display, 0, _displayCount - 1);
    }

    private void MoveToDisplay(int displayIndex)
    {
        _data.display = Mathf.Clamp(displayIndex, 0, _displayCount - 1);

        if (_displayInfos.Count > _data.display)
        {
            try
            {
                var di = _displayInfos[_data.display];
                DevScreen.MoveMainWindowTo(di, Vector2Int.zero);
            }
            catch
            {
            }
        }
    }

    private void RebuildModesFromCurrentDisplay()
    {
        _resList.Clear();
        _hzByRes.Clear();
        _resolutionTexts.Clear();
        _refreshTexts.Clear();

        Resolution[] modes = Screen.resolutions;

        if (modes == null || modes.Length == 0)
        {
            var w = Screen.width; var h = Screen.height;
            _resList.Add((w, h));
            _hzByRes[(w, h)] = new List<int> { 60 };
            _resolutionTexts.Add($"{w}x{h}");
            _refreshTexts.Add("60");
            return;
        }

        IOrderedEnumerable<IGrouping<(int width, int height), Resolution>> grouped = modes.GroupBy(m => (m.width, m.height)).OrderBy(g => g.Key.width * g.Key.height);

        foreach (IGrouping<(int width, int height), Resolution> g in grouped)
        {
            (int width, int height) key = (g.Key.width, g.Key.height);
            _resList.Add(key);
            List<int> hzList = g.Select(m => Mathf.RoundToInt((float)m.refreshRateRatio.value)).Distinct().OrderBy(v => v).ToList();
            _hzByRes[key] = hzList;
        }

        foreach ((int w, int h) r in _resList)
        {
            _resolutionTexts.Add($"{r.w}x{r.h}");
        }
    }

    private void BuildAndSyncSelectors()
    {
        Setup(selDisplay, new List<string>(_displayTexts), Mathf.Clamp(_data.display, 0, _displayCount - 1));
        Setup(selScreenMode, new List<string>(screenModeTexts), Mathf.Clamp(_data.screenMode, 0, screenModeTexts.Count - 1));

        int resIndex = FindClosestResIndex(_resList, (_data.width, _data.height));
        Setup(selResolution, new List<string>(_resolutionTexts), resIndex);

        (int w, int h) resKey = _resList[Mathf.Clamp(resIndex, 0, _resList.Count - 1)];
        _refreshTexts.Clear();
        foreach (var hz in _hzByRes[resKey]) _refreshTexts.Add($"{hz}");
        int rrIndex = FindClosestHzIndex(_hzByRes[resKey], _data.refreshHz);
        Setup(selRefresh, new List<string>(_refreshTexts), rrIndex);

        Setup(selRenderScale, renderScaleSteps.ConvertAll(v => v.ToString("0.00")), FindClosestIndex(renderScaleSteps, _data.renderScale));
        Setup(selMSAA, msaaSteps.ConvertAll(v => v == 0 ? "Off" : $"{v}x"), IndexOrZero(msaaSteps.IndexOf(_data.msaa)));
        Setup(selHDR, new List<string>(onOff), _data.hdr ? 1 : 0);
        Setup(selDepth, new List<string>(onOff), _data.depthTex ? 1 : 0);
        Setup(selOpaque, new List<string>(onOff), _data.opaqueTex ? 1 : 0);

        Setup(selShadowRes, shadowResSteps.ConvertAll(v => v.ToString()), IndexOrZero(shadowResSteps.IndexOf(_data.shadowRes)));
        Setup(selShadowDist, shadowDistSteps.ConvertAll(v => v.ToString("0")), FindClosestIndex(shadowDistSteps, _data.shadowDist));
        Setup(selShadowCasc, shadowCascSteps.ConvertAll(v => v == 0 ? "Off" : v.ToString()), IndexOrZero(shadowCascSteps.IndexOf(_data.shadowCasc)));

        Setup(selTexQuality, new List<string>(texQTexts), Mathf.Clamp(_data.texQ, 0, texQTexts.Count - 1));
        Setup(selAniso, new List<string>(anisoTexts), Mathf.Clamp(_data.aniso, 0, anisoTexts.Count - 1));
        Setup(selLodBias, lodBiasSteps.ConvertAll(v => v.ToString("0.00")), FindClosestIndex(lodBiasSteps, _data.lodbias));
        Setup(selSoftParticles, new List<string>(onOff), _data.softParticles ? 1 : 0);

        Setup(selPostFx, new List<string>(onOff), _data.postFx ? 1 : 0);
        Setup(selSSAO, new List<string>(onOff), _data.ssao ? 1 : 0);
    }

    private void Setup(HorizontalSelector sel, List<string> contents, int index)
    {
        if (!sel)
        {
            return;
        }

        if (contents == null || contents.Count == 0)
        {
            contents = new List<string> { "Ч" };
        }

        sel.Index = Mathf.Clamp(index, 0, contents.Count - 1);
        sel.Setup(contents);
    }

    private void SnapshotSelectorIndices()
    {
        _lastIndex.Clear();

        foreach (HorizontalSelector s in EnumerateSelectors())
        {
            if (s)
            {
                _lastIndex[s] = s.Index;
            }
        }
    }

    private IEnumerable<HorizontalSelector> EnumerateSelectors()
    {
        yield return selDisplay;
        yield return selScreenMode;
        yield return selResolution;
        yield return selRefresh;
        yield return selRenderScale;
        yield return selMSAA;
        yield return selHDR;
        yield return selDepth;
        yield return selOpaque;
        yield return selShadowRes;
        yield return selShadowDist;
        yield return selShadowCasc;
        yield return selTexQuality;
        yield return selAniso;
        yield return selLodBias;
        yield return selSoftParticles;
        yield return selPostFx;
        yield return selSSAO;
    }

    private void Watch(HorizontalSelector sel, Action<int> onChanged)
    {
        if (!sel)
        {
            return;
        }

        int prev = _lastIndex.TryGetValue(sel, out var p) ? p : -1;
        int now = sel.Index;

        if (now != prev)
        {
            _lastIndex[sel] = now;
            onChanged?.Invoke(now);
            SaveToJson();
        }
    }

    private void OnDisplayChanged(int idx)
    {
        _data.display = Mathf.Clamp(idx, 0, _displayCount - 1);
        MoveToDisplay(_data.display);

        RebuildModesFromCurrentDisplay();

        selResolution.Setup(new List<string>(_resolutionTexts));
        int resIndex = FindClosestResIndex(_resList, (_data.width, _data.height));
        selResolution.Index = resIndex;

        (int w, int h) key = _resList[resIndex];
        _refreshTexts.Clear();

        foreach (var hz in _hzByRes[key])
        {
            _refreshTexts.Add($"{hz}");
        }

        selRefresh.Setup(new List<string>(_refreshTexts));
        selRefresh.Index = FindClosestHzIndex(_hzByRes[key], _data.refreshHz);

        SetResolution(key.w, key.h, ParseHz(selRefresh));
    }

    private void OnResolutionChanged(int idx)
    {
        idx = Mathf.Clamp(idx, 0, Math.Max(0, _resList.Count - 1));
        (int w, int h) key = _resList[idx];

        _refreshTexts.Clear();

        foreach (var hz in _hzByRes[key])
        {
            _refreshTexts.Add($"{hz}");
        }

        selRefresh.Setup(new List<string>(_refreshTexts));
        selRefresh.Index = FindClosestHzIndex(_hzByRes[key], _data.refreshHz);

        SetResolution(key.w, key.h, ParseHz(selRefresh));
    }

    private void OnRefreshChanged(int idx)
    {
        int rIndex = Mathf.Clamp(selResolution?.Index ?? 0, 0, Math.Max(0, _resList.Count - 1));
        (int w, int h) = _resList[rIndex];
        SetResolution(w, h, ParseHz(selRefresh));
    }

    private void ApplyAll(GfxData d)
    {
        SetScreenMode(d.screenMode, false);
        MoveToDisplay(d.display);
        SetResolution(d.width, d.height, d.refreshHz, false);
        SetRenderScale(d.renderScale, false);
        SetMsaa(d.msaa, false);
        SetHdr(d.hdr, false);
        SetDepthTexture(d.depthTex, false);
        SetOpaqueTexture(d.opaqueTex, false);
        SetShadowResolution(d.shadowRes, false);
        SetShadowDistance(d.shadowDist, false);
        SetShadowCascades(d.shadowCasc, false);
        SetTextureQuality(d.texQ, false);
        SetAnisotropicFiltering(d.aniso, false);
        SetLodBias(d.lodbias, false);
        SetSoftParticles(d.softParticles, false);
        SetPostProcessing(d.postFx, false);
        SetSSAO(d.ssao, false);
        SaveToJson();
    }

    public void SetRenderScale(float v, bool save = true)
    {
        v = Mathf.Clamp(v, 0.5f, 2.0f);
        _urp.renderScale = v;
        _data.renderScale = v;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetMsaa(int samples, bool save = true)
    {
        int msaa = (samples == 0) ? 0 : Mathf.ClosestPowerOfTwo(Mathf.Clamp(samples, 2, 8));
        _urp.msaaSampleCount = msaa;
        _data.msaa = msaa;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetHdr(bool enabled, bool save = true)
    {
        _urp.supportsHDR = enabled;
        _data.hdr = enabled;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetDepthTexture(bool enabled, bool save = true)
    {
        _urp.supportsCameraDepthTexture = enabled;
        _data.depthTex = enabled;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetOpaqueTexture(bool enabled, bool save = true)
    {
        _urp.supportsCameraOpaqueTexture = enabled;
        _data.opaqueTex = enabled;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetShadowResolution(int res, bool save = true)
    {
        res = Mathf.ClosestPowerOfTwo(Mathf.Clamp(res, 256, 8192));
        _urp.mainLightShadowmapResolution = res;
        _data.shadowRes = res;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetShadowDistance(float dist, bool save = true)
    {
        dist = Mathf.Clamp(dist, 10f, 200f);
        _urp.shadowDistance = dist;
        _data.shadowDist = dist;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetShadowCascades(int cascades, bool save = true)
    {
        cascades = Mathf.Clamp(cascades, 0, 4);
        _urp.shadowCascadeCount = (cascades == 3) ? 4 : cascades;
        _data.shadowCasc = _urp.shadowCascadeCount;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetTextureQuality(int q, bool save = true)
    {
        q = Mathf.Clamp(q, 0, 3);
        QualitySettings.globalTextureMipmapLimit = q;
        _data.texQ = q;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetAnisotropicFiltering(int mode, bool save = true)
    {
        mode = Mathf.Clamp(mode, 0, 2);
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)mode;
        _data.aniso = mode;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetLodBias(float bias, bool save = true)
    {
        bias = Mathf.Clamp(bias, 0.3f, 2.0f);
        QualitySettings.lodBias = bias;
        _data.lodbias = bias;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetSoftParticles(bool enabled, bool save = true)
    {
        QualitySettings.softParticles = enabled;
        _data.softParticles = enabled;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetPostProcessing(bool enabled, bool save = true)
    {
        if (_globalVolume)
        {
            _globalVolume.enabled = enabled;
        }

        _data.postFx = enabled;

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetSSAO(bool enabled, bool save = true)
    {
        UniversalRendererData rd = GetRendererData();

        if (rd != null)
        {
            foreach (var f in rd.rendererFeatures)
            {
                if (f == null)
                {
                    continue;
                }

                string tn = f.GetType().Name.ToLowerInvariant();

                string nm = f.name?.ToLowerInvariant() ?? "";

                if (tn.Contains("ambientocclusion") || (nm.Contains("ambient") && nm.Contains("occlusion")))
                {
                    f.SetActive(enabled);
                }
            }

            rd.SetDirty();
        }
        _data.ssao = enabled;

        if (save)
        {
            SaveToJson();
        }
    }

    private UniversalRendererData GetRendererData()
    {
        PropertyInfo prop = typeof(UniversalRenderPipelineAsset).GetProperty("scriptableRendererData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        
        if (prop != null)
        {
            return prop.GetValue(_urp) as UniversalRendererData;
        }

        FieldInfo field = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (field != null)
        {
            if (field.GetValue(_urp) is ScriptableRendererData[] arr && arr.Length > 0)
            {
                return arr[0] as UniversalRendererData;
            }
        }

        return null;
    }

    public void SetScreenMode(int modeIndex, bool save = true)
    {
        _data.screenMode = Mathf.Clamp(modeIndex, 0, 2);

        Screen.fullScreenMode = _data.screenMode switch
        {
            0 => FullScreenMode.Windowed,
            1 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.ExclusiveFullScreen
        };

        if (save)
        {
            SaveToJson();
        }
    }

    public void SetResolution(int w, int h, int hz, bool save = true)
    {
        w = Mathf.Max(320, w);
        h = Mathf.Max(240, h);
        hz = hz <= 0 ? 60 : hz;

        if (_hzByRes.TryGetValue((w, h), out var list) && list.Count > 0)
        {
            hz = list.OrderBy(v => Math.Abs(v - hz)).First();
        }

        RefreshRate rr = new RefreshRate { numerator = (uint)hz, denominator = 1u };
        Screen.SetResolution(w, h, Screen.fullScreenMode, rr);

        _data.width = w; _data.height = h; _data.refreshHz = hz;
        if (save)
        {
            SaveToJson();
        }
    }

    private void SaveToJson()
    {
        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(_data, true));
        }
        catch (Exception e)
        {
            Debug.LogError($"[Gfx] Save error: {e}");
        }
    }

    private void LoadFromJson()
    {
        try
        {
            _data = File.Exists(FilePath) ? (JsonUtility.FromJson<GfxData>(File.ReadAllText(FilePath)) ?? new GfxData()) : new GfxData();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Gfx] Load error: {e}");
            _data = new GfxData();
        }
    }

    private static int IndexOrZero(int idx) => Mathf.Max(0, idx);

    private static int FindClosestIndex(List<float> list, float value)
    {
        if (list == null || list.Count == 0)
        {
            return 0;
        }

        int best = 0; float bestDiff = Mathf.Abs(list[0] - value);

        for (int i = 1; i < list.Count; i++)
        {
            float d = Mathf.Abs(list[i] - value);
            if (d < bestDiff)
            {
                best = i;
                bestDiff = d;
            }
        }

        return best;
    }

    private static int FindClosestResIndex(List<(int w, int h)> list, (int w, int h) cur)
    {
        if (list == null || list.Count == 0)
        {
            return 0;
        }

        int best = 0; long bestScore = long.MaxValue;
        
        for (int i = 0; i < list.Count; i++)
        {
            long score = Math.Abs(list[i].w - cur.w) * 1000L + Math.Abs(list[i].h - cur.h) * 1000L;

            if (score < bestScore)
            {
                bestScore = score;
                best = i;
            }
        }

        return best;
    }

    private static int FindClosestHzIndex(List<int> list, int hz)
    {
        if (list == null || list.Count == 0)
        {
            return 0;
        }

        int best = 0; int bestDiff = int.MaxValue;

        for (int i = 0; i < list.Count; i++)
        {
            int d = Math.Abs(list[i] - hz);

            if (d < bestDiff)
            {
                best = i;
                bestDiff = d;
            }
        }

        return best;
    }

    private static int ParseHz(HorizontalSelector sel)
    {
        if (!sel)
        {
            return 60;
        }

        return int.TryParse(sel.GetContentByIndex(), out int hz) ? Mathf.Max(1, hz) : 60;
    }
}