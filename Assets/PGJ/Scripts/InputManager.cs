using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SingletonBehaviour<InputManager>
{
    public bool isUICheck { get; private set; }

    [Header("Character Input Values")]
    internal Vector2 move;
    internal Vector2 look;
    internal bool space_Input;
    internal bool lShift_Input;
    internal bool aim;
    internal bool r_Input;
    internal bool weapon0_Choice_Input;
    internal bool weapon1_Choice_Input;
    internal bool weapon2_Choice_Input;
    internal bool weapon3_Choice_Input;
    internal bool mouse0_Input;
    internal bool mouse1_Input;

    [Header("Movement Settings")]
    internal bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    bool pause = false;

    protected override void Init()
    {
        base.Init();

        EventManager.Instance.OnPauseAction += SetPause;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventManager.Instance.OnPauseAction -= SetPause;
    }

    void SetPause(bool _isPuase)
    {
        pause = _isPuase;
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        if (pause)
        {
            return;
        }

        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (pause)
        {
            return;
        }

        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        if (pause)
        {
            return;
        }

        SpaceInput(value.isPressed);
    }

    public void OnDash(InputValue value)
    {
        if (pause)
        {
            return;
        }

        LShiftInput(value.isPressed);
    }

    public void OnAim(InputValue value)
    {
        if (pause)
        {
            return;
        }

        AimInput(value.isPressed);
    }

    public void OnReload(InputValue value)
    {
        if (pause)
        {
            return;
        }

        R_Input(value.isPressed);
    }

    public void OnWeaponChoice0(InputValue value)
    {
        if (pause)
        {
            return;
        }

        Weapon0_Choice_Input(value.isPressed);
    }

    public void OnWeaponChoice1(InputValue value)
    {
        if (pause)
        {
            return;
        }

        Weapon1_Choice_Input(value.isPressed);
    }

    public void OnWeaponChoice2(InputValue value)
    {
        if (pause)
        {
            return;
        }

        Weapon2_Choice_Input(value.isPressed);
    }

    public void OnWeaponChoice3(InputValue value)
    {
        if (pause)
        {
            return;
        }

        Weapon3_Choice_Input(value.isPressed);
    }

    public void OnMouse0(InputValue value)
    {
        if (pause)
        {
            return;
        }

        Mouse0_Input(value.isPressed);
    }

    public void OnMouse1(InputValue value)
    {
        if (pause)
        {
            return;
        }

        Mouse1_Input(value.isPressed);
    }
#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void SpaceInput(bool spaceState)
    {
        space_Input = spaceState;
    }

    public void AimInput(bool newAimState)
    {
        aim = newAimState;
    }

    public void R_Input(bool rInputState)
    {
        r_Input = rInputState;
    }

    public void Weapon0_Choice_Input(bool weaponChoiceState)
    {
        weapon0_Choice_Input = weaponChoiceState;
    }

    public void Weapon1_Choice_Input(bool weaponChoiceState)
    {
        weapon1_Choice_Input = weaponChoiceState;
    }

    public void Weapon2_Choice_Input(bool weaponChoiceState)
    {
        weapon2_Choice_Input = weaponChoiceState;
    }

    public void Weapon3_Choice_Input(bool weaponChoiceState)
    {
        weapon3_Choice_Input = weaponChoiceState;
    }

    public void Mouse0_Input(bool mouse0InputState)
    {
        mouse0_Input = mouse0InputState;
    }

    public void Mouse1_Input(bool mouse1InputState)
    {
        mouse1_Input = mouse1InputState;
    }

    public void LShiftInput(bool lShiftState)
    {
        lShift_Input = lShiftState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (pause)
        {
            return;
        }

        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}