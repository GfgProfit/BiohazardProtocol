using UnityEngine;

public sealed class Cube : MonoBehaviour, IPoolable
{
    [SerializeField] private Renderer _renderer;
    private Color _color;

    private void Awake()
    {
        if (!_renderer) _renderer = GetComponent<Renderer>();
    }

    public void OnSpawned()
    {
        // Меняем цвет при каждом спавне, чтобы видеть что это тот же объект
        _color = new Color(Random.value, Random.value, Random.value);
        _renderer.material.color = _color;
    }

    public void OnDespawned()
    {
        // Можно добавить анимацию выключения, но тут просто лог
        Debug.Log($"Cube {name} despawned");
    }
}
