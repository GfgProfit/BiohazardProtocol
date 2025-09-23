using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private Image _weaponIconImage;

    [Space]
    [SerializeField] private WeaponBehaviour _startedWeapon;

    public bool CanSwitch { get; set; } = true;

    private WeaponBehaviour _selectedWeapon;
    private WeaponBehaviour _primaryWeapon;
    private WeaponBehaviour _secondaryWeapon;

    private void Start()
    {
        SwapWeapon(_startedWeapon);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_primaryWeapon != null)
            {
                StartCoroutine(_selectedWeapon.SwitchWeapon(_primaryWeapon));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_secondaryWeapon != null)
            {
                StartCoroutine(_selectedWeapon.SwitchWeapon(_secondaryWeapon));
            }
        }
    }

    public void Select(WeaponBehaviour selectableWeapon)
    {
        if (CanSwitch)
        {
            if (selectableWeapon != null)
            {
                if (selectableWeapon == _primaryWeapon)
                {
                    selectableWeapon.gameObject.SetActive(true);

                    if (_secondaryWeapon != null)
                    {
                        _secondaryWeapon.gameObject.SetActive(false);
                    }
                }
                else if (selectableWeapon == _secondaryWeapon)
                {
                    selectableWeapon.gameObject.SetActive(true);
                    _primaryWeapon.gameObject.SetActive(false);
                }

                _selectedWeapon = selectableWeapon;
                _weaponIconImage.sprite = _selectedWeapon.GetWeaponIcon();
            }
        }
    }

    public void SwapWeapon(WeaponBehaviour newWeapon)
    {
        if (_primaryWeapon != null)
        {
            if (_secondaryWeapon == null)
            {
                _secondaryWeapon = newWeapon;
                Select(_secondaryWeapon);
            }
            else
            {
                var oldWeapon = _selectedWeapon;
                _selectedWeapon = newWeapon;

                if (oldWeapon == _primaryWeapon)
                {
                    _primaryWeapon = _selectedWeapon;
                }
                else if (oldWeapon == _secondaryWeapon)
                {
                    _secondaryWeapon = _selectedWeapon;
                }

                _weaponIconImage.sprite = _selectedWeapon.GetWeaponIcon();

                Destroy(oldWeapon.gameObject);
            }
        }
        else
        {
            _primaryWeapon = newWeapon;
            Select(_primaryWeapon);
        }
    }

    public WeaponBehaviour GetCurrentWeapon() => _selectedWeapon;
}