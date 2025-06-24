using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeed;
    public float dashSpeed;
    public float RotationSmoothTime;
    public float SpeedChangeRate;
    public float dashCoolTime;
    public float dashDurationTime;
    public int dashMaxStackNum;
    public float dashStackCoolTime;

    int currentDashStack;
    float dashStackCoolTimer;

    [Space(10)]
    [Header("Weapon")]
    public WeaponBase[] weaponArray;
    public int startWeaponNum;

    float weaponSwapTime = 1;
    float weaponSwapTimer = 0;

    float verticalVelocity;
    float terminalVelocity = 53.0f;
    float targetRotation = 0.0f;
    float rotationVelocity;

    [Space(10)]
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;
    public bool LockCameraPosition = false;

    [Space(10)]
    [Header("Cinemachine")]
    public GameObject cinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;

    const float threshold = 0.01f;

    float speed;
    float dashTimer = 0;
    bool isDash = false;

    Vector3 targetDirection;
    Vector3 inputDirection;

    // timeout deltatime
    float jumpTimeoutDelta;
    float fallTimeoutDelta;

    // cinemachine
    float cinemachineTargetYaw;
    float cinemachineTargetPitch;

    [Header("Player Grounded")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("Player Body")]
    public GameObject chestObj;

    private GameObject mainCamera;
    CharacterController controller;
    InputManager input;

    WeaponBase currentWeapon;
    Coroutine coWeaponChange = null;

    bool playerDie = false;
    int remainRevival = 1;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
    }

    void Start()
    {
        input = InputManager.Instance;

        Init();
    }

    void Update()
    {
        weaponSwapTimer -= Time.deltaTime;

        DashCoolTimeProc();

        JumpAndGravity();
        GroundedCheck();
        Move();

        WeaponChangeInputCheck();
        ReloadCheck();
        ShootCheck();
    }

    void LateUpdate()
    {
        CameraRotation();
    }

    void WeaponChangeInputCheck()
    {
        if (input.weapon0_Choice_Input)
        {
            input.weapon0_Choice_Input = false;

            coWeaponChange = StartCoroutine(ChangeWeapon(0));
        }
        else if (input.weapon1_Choice_Input)
        {
            input.weapon1_Choice_Input = false;

            coWeaponChange = StartCoroutine(ChangeWeapon(1));
        }
        else if (input.weapon2_Choice_Input)
        {
            input.weapon2_Choice_Input = false;

            coWeaponChange = StartCoroutine(ChangeWeapon(2));
        }
        else if (input.weapon3_Choice_Input)
        {
            input.weapon3_Choice_Input = false;

            coWeaponChange = StartCoroutine(ChangeWeapon(3));
        }
    }

    void Init()
    {
        playerDie = false;
        isDash = false;
        weaponArray[startWeaponNum].useAble = true;
        remainRevival = 1;
        currentDashStack = dashMaxStackNum;
        dashStackCoolTimer = 0;

        currentWeapon = weaponArray[startWeaponNum];

        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }

    internal void GetWeapon(int weaponNum)
    {
        weaponArray[weaponNum].useAble = true;

        EventManager.Instance.PlayerWeaponUIRefresh();
    }

    IEnumerator ChangeWeapon(int weaponNum)
    {
        if (true == weaponArray[weaponNum].useAble)
        {
            if (weaponSwapTimer < 0)
            {
                weaponSwapTimer = weaponSwapTime;

                currentWeapon.reloading = false;
                currentWeapon.SetBoolAnimation("WeaponPut", true);

                yield return null;
                yield return new WaitForSeconds(currentWeapon.GetAnimationTime());
                yield return null;

                currentWeapon.gameObject.SetActive(false);
                currentWeapon = weaponArray[weaponNum];
            }
        }
    }

    void DashCoolTimeProc()
    {
        // 대쉬 스택 최대면 리턴
        if (currentDashStack >= dashMaxStackNum)
        {
            return;
        }

        dashStackCoolTimer -= Time.deltaTime;

        if (dashStackCoolTimer < 0)
        {
            dashStackCoolTimer = dashStackCoolTime;

            currentDashStack += 1;
            Debug.Log("현재 대쉬 스택 : " + currentDashStack);
        }
    }

    void ReloadCheck()
    {
        if (false == playerDie)
        {
            if (input.r_Input || input.mouse1_Input)
            {
                input.r_Input = false;
                input.mouse1_Input = false;

                if (false == currentWeapon.reloading)
                {
                    // 장전 진행
                }
            }
        }
    }

    void ShootCheck()
    {
        if (input.mouse0_Input)
        {
            input.mouse0_Input = false;
            
            // 음악 시작전이면 리턴
            if (false == GameManager.Instance.musicStart)
            {
                return;
            }

            // 무기 장전중이면 리턴
            if (true == currentWeapon.reloading)
            {
                return;
            }

            // 노트가 멀면 리턴
            if (false == GameManager.Instance.GetNoteDisable())
            {
                return;
            }

            if (1 == GameManager.Instance.RhythmCheck())
            {
                Debug.Log("정박 성공!");
                EventManager.Instance.PlayerAddComboEvent();
            }
            else if (2 == GameManager.Instance.RhythmCheck())
            {
                Debug.Log("반박 성공!");
                EventManager.Instance.PlayerAddComboEvent();
            }
            else
            {
                Debug.Log("박자 타이밍 실패...");
                SoundManager.Instance.PlaySFX(SFX.RhythmFail);
                EventManager.Instance.PlayerReduceComboEvent();
            }

            GameManager.Instance.NotePush();
        }
    }

    void CameraRotation()
    {
        if (input.look.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            cinemachineTargetYaw += input.look.x * 1;
            cinemachineTargetPitch += input.look.y * 1;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        //cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);

        chestObj.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
        //transform.forward = new Vector3(cinemachineCameraTarget.transform.forward.x, 0, cinemachineCameraTarget.transform.forward.z);
    }

    float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    void Move()
    {
        float targetSpeed = isDash ? dashSpeed : moveSpeed;

        dashTimer -= Time.deltaTime;
        if (dashTimer + dashDurationTime < dashCoolTime && true == isDash)
        {
            isDash = false;
            targetSpeed = 0.0f;
        }

        if (false == isDash)
        {
            inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
        }

        if (true == input.lShift_Input)
        {
            input.lShift_Input = false;

            if (0 > dashTimer)
            {
                if (0 < currentDashStack)
                {
                    currentDashStack -= 1;

                    dashTimer = dashCoolTime;
                    isDash = true;
                }
            }
        }
        else
        {
            if (input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }
        }
        
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }
        
        if (false == isDash)
        {
            if (input.move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    RotationSmoothTime);
            }

            targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        }
        
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    void JumpAndGravity()
    {
        if (jumpTimeoutDelta >= 0.0f)
        {
            jumpTimeoutDelta -= Time.deltaTime;
        }

        if (Grounded)
        {
            fallTimeoutDelta = FallTimeout;

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (input.space_Input && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }
        else
        {
            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            //input.space_Input = false;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    public void AddRemainRevival(int _addCnt)
    {
        remainRevival += _addCnt;
    }

    public void SetPlayerDie(bool _playerDie)
    {
        // 살아있다가 죽은 경우
        if (true == _playerDie && false == playerDie)
        {
            playerDie = _playerDie;

            EventManager.Instance.PlayerDieEvent();
        }
        else if (false == _playerDie && true == playerDie)          // 죽었다가 부활한 경우
        {
            playerDie = _playerDie;

            EventManager.Instance.PlayerRevivalEvent();
        }
    }
}