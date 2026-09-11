using System;
using System.Collections;
using UnityEngine;

public class TopDown3DController : MonoBehaviour
{
    [Header("Player Attributes"), Space]
    [SerializeField] private float moveSpeed = 5.335f;
    [SerializeField] private float speedChangeRate = 10f;
    [SerializeField] private float gravity = -15.0f;

    [Range(0.0f, 0.3f)]
    [SerializeField] private float RotationSmoothTime = 0.12f;

    [Header("Attack")]
    [SerializeField] private float animateAttackInterval = 2f;
    [SerializeField] private float ammoDestroy = .3f;
    // [SerializeField] private ParticleSystem bloodPS;

    [Header("References"), Space]
    [SerializeField] private AssetsInputSystems input;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthBar healthBar;


    [Header("Player Grounded")]
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float GroundedOffset = -0.14f;
    [SerializeField] private float GroundedRadius = 0.28f;
    [SerializeField] private LayerMask GroundLayers;
    [SerializeField] private Transform GunMuzzle;

    // Animation IDs
    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private int _animIDAttack;
    private int _animIDDeath;

    // Player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private bool _hasAnimator;
    private bool _isDie;

    //Weapon
    private Gun _gun;
    private float _attackInterval;
    private float _animateAttackInterval;
    private bool _isRecoil;
    // private float _BombInterval;

    private GameObject mainCamera;

    private void Awake()
    {
        // get a reference to our main camera
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        Setup();

        AssignAnimationIDs();
    }

    private void Setup()
    {
        _hasAnimator = animator != null ? true : false;
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDDeath = Animator.StringToHash("Death");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void Update()
    {
        ApplyGravity();
        GroundedCheck();
        Move();
        Attack();
    }

    private void Attack()
    {
        if (input.attackTrigger)
        {
            if (_attackInterval <= 0f)
            {
                if (_hasAnimator && _animateAttackInterval <= 0f)
                {
                    animator.SetTrigger(_animIDAttack);

                    _animateAttackInterval = animateAttackInterval;
                }

                //Attack
                _isRecoil = true;

                string guntype = _gun.gunType.ToString();

                // AudioManager.Instance.Play(guntype);

                Transform ammoPrefab = _gun.ammoPrefabTransform;
                Transform ammo = Instantiate(ammoPrefab, GunMuzzle);
                ammo.GetComponent<Ammo>().damge = _gun.damagePerTime;

                Destroy(ammo.gameObject, ammoDestroy);
                // Destroy(Instantiate(_gun.vfxPrefabTransform, GunMuzzle).gameObject, ammoDestroy);

                _attackInterval = _gun.timePerAttk;
            }
        }
        else
        {
        	_isRecoil = false;
        }

        _attackInterval -= Time.deltaTime;
        _animateAttackInterval -= Time.deltaTime;
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (input.moveInput == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = input.IsAnalogMovement() ? input.moveInput.magnitude : 1f;

        //Apply Recoil
        if (_isRecoil && input.moveInput != Vector2.zero)
        {
            targetSpeed = targetSpeed - _gun.knockback;
        }

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);

        // normalise input direction
        Vector3 inputDirection = new Vector3(input.moveInput.x, 0.0f, input.moveInput.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (input.moveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            animator.SetFloat(_animIDSpeed, _animationBlend);
            animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public void SetGun(Gun gun)
	{
		_attackInterval = -1f;
		_gun = gun;
	}

    public void TakeDamge(int damage)
    {
        if (_isDie) return;

        healthBar.GotHit(damage);
        // bloodPS.Play();
    }

    public HealthBar GetHealthBar()
    {
        return this.healthBar;
    }
}
