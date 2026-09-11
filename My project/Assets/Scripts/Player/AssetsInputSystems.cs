using UnityEngine;
using UnityEngine.InputSystem;

public class AssetsInputSystems : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private bool analogMovement;

    public Vector2 moveInput {get; private set;}
    public bool attackTrigger {get; private set;}
    public bool switchWeaponTrigger {get; private set;}
    public bool bombTrigger {get; private set;}

    public bool IsAnalogMovement()
    {
        return analogMovement;
    }

    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    public void OnAttack(InputValue inputValue)
    {
        attackTrigger = inputValue.isPressed;
    }

    public void OnSwitchWeapon(InputValue inputValue)
    {
        switchWeaponTrigger = inputValue.isPressed;
    }

    public void SetSwitchWeaponTrigger(bool value)
    {
        switchWeaponTrigger = value;
    }

    public void OnBomb(InputValue inputValue)
    {
        bombTrigger = inputValue.isPressed;
    }
}
