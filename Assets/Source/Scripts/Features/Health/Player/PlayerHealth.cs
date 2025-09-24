using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private float _regenDelay = 5f;
    [SerializeField] private float _regenRate = 1f;

    public HealthModel Health { get; private set; }
    public bool JuggernogActive { get; set; }

    private Coroutine _regenCoroutine;
    private Coroutine _delayCoroutine;
    private float RegenRate => JuggernogActive ? _regenRate * 2 : _regenRate;

    private void Awake()
    {
        Health = new HealthModel(_maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TEST();
        }
    }

    public void TakeDamage(int amount)
    {
        Health.Damage(amount);

        if (_regenCoroutine != null)
        {
            StopCoroutine(_regenCoroutine);
            _regenCoroutine = null;
        }
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
            _delayCoroutine = null;
        }

        if (!Health.IsDead)
        {
            _delayCoroutine = StartCoroutine(StartRegenDelay());
        }
    }

    private IEnumerator StartRegenDelay()
    {
        yield return new WaitForSeconds(_regenDelay);

        _regenCoroutine = StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        while (Health.Current < Health.Max)
        {
            Health.Heal(1);
            yield return new WaitForSeconds(1f / RegenRate);
        }

        _regenCoroutine = null;
    }

    public void HealImmediate(int amount)
    {
        Health.Heal(amount);
    }

    public void ResetHealth(int newMax)
    {
        Health.Reset(newMax);
    }

    [ContextMenu("TEST DAMAGE")]
    public void TEST()
    {
        TakeDamage(45);
    }
}