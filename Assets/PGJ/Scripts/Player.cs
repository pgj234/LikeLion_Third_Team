using System.Collections;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Player")]
    public int maxHp;
    int currentHp;

    [Space(5)]
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
    GameManager gameManager;
    EventManager eventManager;
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
        gameManager = GameManager.Instance;
        eventManager = EventManager.Instance;

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
        //ReloadCheck();
        //ShootCheck();
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
        currentHp = maxHp;
        playerDie = false;
        isDash = false;
        weaponArray[startWeaponNum].useAble = true;
        remainRevival = 1;
        currentDashStack = dashMaxStackNum;
        dashStackCoolTimer = 0;

        currentWeapon = weaponArray[startWeaponNum];
        currentWeapon.SetAnimationSpeed(1.8f);

        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }

    internal void GetDamage(int _dmg)
    {
        currentHp -= _dmg;

        if (currentHp <= 0)
        {
            SetPlayerDie(true);
        }
        eventManager.OnPlayerDamageAction(currentHp);
    }

    internal void GetWeapon(int weaponNum)
    {
        weaponArray[weaponNum].useAble = true;

        bool[] weaponUseAbleArray = new bool[weaponArray.Length];
        for (int i=0; i<weaponArray.Length; i++)
        {
            weaponUseAbleArray[i] = weaponArray[i].useAble;
        }
        eventManager.PlayerWeaponUIRefresh(weaponUseAbleArray);
    }

    IEnumerator ChangeWeapon(int weaponNum)
    {
        if (true == weaponArray[weaponNum].useAble)
        {
            if (weaponSwapTimer < 0)
            {
                weaponSwapTimer = weaponSwapTime;

                currentWeapon.reloading = false;
                currentWeapon.SetTrggierAnimation("WeaponPut");

                yield return null;
                yield return new WaitForSeconds(currentWeapon.GetAnimationTime());
                yield return null;

                currentWeapon.gameObject.SetActive(false);
                currentWeapon = weaponArray[weaponNum];
                currentWeapon.gameObject.SetActive(true);

                eventManager.PlayerCurrentBulletUIRefresh(currentWeapon.GetCurrentAmmo());
                eventManager.PlayerMaxBulletUIRefresh(currentWeapon.GetMaxAmmo());
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

    void CameraRotation()
    {
        if (input.look.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            cinemachineTargetYaw += input.look.x * 1;
            cinemachineTargetPitch += input.look.y * 1;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        chestObj.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
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

            eventManager.PlayerDieEvent();
        }
        else if (false == _playerDie && true == playerDie)          // 죽었다가 부활한 경우
        {
            playerDie = _playerDie;

            eventManager.PlayerRevivalEvent();
        }
    }
}