using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// Лёгкая замена стандартного ContentSizeFitter без рекурсий и лагов.
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class LeanContentSizeFitter : UIBehaviour, ILayoutSelfController
{
    public enum FitMode { Unconstrained, MinSize, PreferredSize }

    [SerializeField] private FitMode _horizontal = FitMode.Unconstrained;
    [SerializeField] private FitMode _vertical = FitMode.PreferredSize;

    [Tooltip("Следить за изменениями графики/текста и помечать макет грязным.")]
    [SerializeField] private bool _autoWatch = true;

    RectTransform _rt;
    DrivenRectTransformTracker _tracker;

    Graphic[] _graphics; // Image, RawImage, Text (legacy) и т.п.

    RectTransform RT => _rt ? _rt : (_rt = GetComponent<RectTransform>());

    public FitMode horizontal { get => _horizontal; set { if (_horizontal != value) { _horizontal = value; SetDirty(); } } }
    public FitMode vertical { get => _vertical; set { if (_vertical != value) { _vertical = value; SetDirty(); } } }

    protected override void OnEnable()
    {
        base.OnEnable();
        _tracker.Clear();
        Hook(true);
        SetDirty();
    }

    protected override void OnDisable()
    {
        _tracker.Clear();
        Hook(false);
        LayoutRebuilder.MarkLayoutForRebuild(RT);
        base.OnDisable();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (isActiveAndEnabled) SetDirty();
    }
#endif

    protected override void OnTransformParentChanged() => SetDirty();
    protected override void OnRectTransformDimensionsChange() => SetDirty();
    protected override void OnDidApplyAnimationProperties() => SetDirty();
    protected override void OnCanvasGroupChanged() => SetDirty();

    void ILayoutController.SetLayoutHorizontal() => Apply(RectTransform.Axis.Horizontal, _horizontal);
    void ILayoutController.SetLayoutVertical() => Apply(RectTransform.Axis.Vertical, _vertical);

    void Apply(RectTransform.Axis axis, FitMode mode)
    {
        if (!IsActive() || mode == FitMode.Unconstrained) return;

        _tracker.Add(this, RT, axis == RectTransform.Axis.Horizontal
            ? DrivenTransformProperties.SizeDeltaX
            : DrivenTransformProperties.SizeDeltaY);

        float size = mode == FitMode.MinSize
            ? LayoutUtility.GetMinSize(RT, (int)axis)
            : LayoutUtility.GetPreferredSize(RT, (int)axis);

        RT.SetSizeWithCurrentAnchors(axis, size);
    }

    public void SetDirty()
    {
        if (!IsActive()) return;
        LayoutRebuilder.MarkLayoutForRebuild(RT);
    }

    [ContextMenu("Rebuild Now")]
    public void RebuildNow()
    {
        if (!IsActive()) return;
        LayoutRebuilder.ForceRebuildLayoutImmediate(RT);
    }

    void Hook(bool on)
    {
        if (!_autoWatch) return;

        // Graphic callbacks (UI Text/Image и т.д.)
        if (_graphics != null)
            foreach (var g in _graphics) if (g) g.UnregisterDirtyLayoutCallback(OnGraphicDirty);

        if (on)
        {
            _graphics = GetComponentsInChildren<Graphic>(true);
            foreach (var g in _graphics) if (g) g.RegisterDirtyLayoutCallback(OnGraphicDirty);

            // TMP глобальное событие
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTmpGlobalTextChanged);
        }
        else
        {
            _graphics = null;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTmpGlobalTextChanged);
        }
    }

    void OnGraphicDirty() => SetDirty();

    // Вызывается для ЛЮБОГО TMP-текста в проекте — проверяем, наш ли он.
    void OnTmpGlobalTextChanged(Object changedObj)
    {
        if (!IsActive() || changedObj == null) return;

        var tmp = changedObj as TMP_Text;
        if (tmp == null) return;

        // Наш ли это текст? (сам компонент или его потомок)
        var t = tmp.transform;
        if (t == RT || t.IsChildOf(RT))
            SetDirty();
    }
}
