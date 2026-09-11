using System.Collections.Generic;
using UnityEngine;

public class EquipWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GunScriptableObjectScript> listSOGun;
    [SerializeField] private Transform gunContainerTransform;
    [SerializeField] private AssetsInputSystems input;
    [SerializeField] private TopDown3DController controller;

    private int _currentGunIndex = 0;
    private Gun _currentGun;
    private Transform _currentGunInstance;

    private void Start()
    {
        ShowWeapon();
    }

    private void Update()
    {
        ChangeWeapon();
    }

    private void ChangeWeapon()
    {
        if (input.switchWeaponTrigger)
        {
            if (++_currentGunIndex >= listSOGun.Count)
            {
                _currentGunIndex = 0;
            }

            ShowWeapon();

            input.SetSwitchWeaponTrigger(false);
        }
    }

    private void ShowWeapon()
    {
        if (_currentGunInstance != null)
        {
            Destroy(_currentGunInstance.gameObject);
        }

        _currentGun = new Gun(listSOGun[_currentGunIndex]);
        controller.SetGun(_currentGun);
        _currentGunInstance = Instantiate(_currentGun.prefabTransform, gunContainerTransform);
    }

    public Gun GetCurrentGun()
    {
        return _currentGun;
    }

    public Transform GetGunContainerTransform()
    {
        return gunContainerTransform;
    }

    public Transform GetCurrentGunInstance()
    {
        return _currentGunInstance;
    }
}
