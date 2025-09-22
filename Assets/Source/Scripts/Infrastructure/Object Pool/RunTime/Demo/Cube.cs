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
        // ������ ���� ��� ������ ������, ����� ������ ��� ��� ��� �� ������
        _color = new Color(Random.value, Random.value, Random.value);
        _renderer.material.color = _color;
    }

    public void OnDespawned()
    {
        // ����� �������� �������� ����������, �� ��� ������ ���
        Debug.Log($"Cube {name} despawned");
    }
}
