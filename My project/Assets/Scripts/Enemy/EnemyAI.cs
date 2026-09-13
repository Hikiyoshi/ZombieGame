using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public event Action<ZombieType> OnDeath;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator _animator;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private NavMeshAgent _agent;

    [Header("Attributes")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float aggressiveTriggerInterval = 1.5f;

    [Header("Attack")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float timePerAttack = 1.2f;

    [SerializeField] private ZombieType _zombieType;

    private float _attackTimer;
    private bool _hasAnimator;
    private bool _isDead;
    private GameObject _cam;
    private bool _isWaitingAfterAttack;
    private float _pathUpdateTimer;

    //Mutation
    private bool _isMutation;
    private bool _isAgg;
    private float _aggressiveTriggerInterval;

    // Animator hashes
    private static readonly int AnimIDSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimIDAttack = Animator.StringToHash("Attack");
    private static readonly int AnimIDAgg = Animator.StringToHash("Aggressive");
    private static readonly int AnimIDDie = Animator.StringToHash("Death");

    private AudioSource _chaseAaudioS;
    private AudioSource _aggAudioS;
    private AudioSource _deathAudioS;

    private void Awake()
    {
        _hasAnimator = _animator != null;
        _agent.speed = moveSpeed;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        _isMutation = _zombieType == ZombieType.Mutation;
        _aggressiveTriggerInterval = aggressiveTriggerInterval;
    }

    private void Start()
    {
        if (_cam == null)
        {
            _cam = GameObject.FindGameObjectWithTag("MainCamera");
        }

        _chaseAaudioS = AudioManager.Instance.GetAudio("ZomHiss").source;
        _aggAudioS = AudioManager.Instance.GetAudio("ZomAgg").source;
        _deathAudioS = AudioManager.Instance.GetAudio("ZomDeath").source;
    }

    private void Update()
    {
        if (_isDead || player == null) return;

        if (_healthBar.Health <= 0)
        {
            Die();
            return;
        }

        Move();

        UpdateAnimator();

        HandleMutation();

        if (_attackTimer > 0f)
        {
            _attackTimer -= Time.deltaTime;
        }
    }

    private void Move()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (!_isWaitingAfterAttack)
            {
                StartCoroutine("WaitTime");
            }
        }
        else if (distance <= detectRange)
        {
            if (_isWaitingAfterAttack)
            {
                _agent.isStopped = false;
                return;
            }

            _pathUpdateTimer -= Time.deltaTime;
            if (_pathUpdateTimer <= 0f)
            {
                _agent.SetDestination(player.position);
                _pathUpdateTimer = 1f;
            }
        }
        else
        {
            _agent.isStopped = true;
        }

        FacePlayer();
    }

    private void StopAttackSound()
    {
        AudioManager.Instance.Stop("ZomHiss");
    }

    public void PlayAttackSound()
    {
        AudioManager.Instance.Play("ZomHiss");
    }

    private IEnumerator WaitTime()
    {
        HandleAttack();

        _isWaitingAfterAttack = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(2.5f);

        _agent.isStopped = false;
        _isWaitingAfterAttack = false;
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f);
    }

    private void HandleAttack()
    {
        if (_attackTimer > 0f)
            return;

        if (_hasAnimator)
        {
            _animator.SetTrigger(AnimIDAttack);
        }

        PlayAttackSound();
        _attackTimer = timePerAttack;
    }

    public void DealDamage()
    {
        if (player == null)
            return;

        if (Vector3.Distance(transform.position, player.position) > attackRange + 0.5f)
            return;

        if (player.TryGetComponent<TopDown3DController>(out var controller))
        {
            controller.TakeDamge(damage);
        }
    }

    private void UpdateAnimator()
    {
        if (!_hasAnimator) return;

        _animator.SetFloat(AnimIDSpeed, _agent.velocity.magnitude);
    }

    public void GotHit(int amount)
    {
        if (_isDead) return;

        _healthBar.GotHit(amount);

        StartCoroutine(KnockBack(2));
    }

    private IEnumerator KnockBack(int knockback)
    {
        _agent.speed -= knockback;
        yield return new WaitForSeconds(.5f);
        _agent.speed = moveSpeed;
    }

    private void Die()
    {
        _isDead = true;
        _agent.isStopped = true;
        _healthBar.gameObject.SetActive(false);

        StopAttackSound();
        AudioManager.Instance.Play("ZombieDeath");

        if (_hasAnimator)
        {
            _animator.SetTrigger(AnimIDDie);
        }

        if (TryGetComponent<Collider>(out var col))
        {
            col.enabled = false;
        }

        OnDeath?.Invoke(GetZombieType());
        Destroy(gameObject, 2.5f);
    }

    public ZombieType GetZombieType()
    {
        return _zombieType;
    }

    private void HandleMutation()
    {
        if (_isAgg)
            return;

        if (!(_isMutation && GameManager.Instance.IsInCameraView(_cam.GetComponent<Camera>(), transform)))
            return;

        int trigger = UnityEngine.Random.Range(1, 100);
        
        if (_aggressiveTriggerInterval <= 0f)
        {
            if (trigger < 30)
            {
                _isAgg = true;
                _agent.speed = runSpeed;
                StartCoroutine("WaitAggress");
            }

            _aggressiveTriggerInterval = aggressiveTriggerInterval;
        }

        _aggressiveTriggerInterval -= Time.deltaTime;
    }

    private IEnumerator WaitAggress()
    {
        _agent.isStopped = true;
        _animator.SetTrigger(AnimIDAgg);
        AudioManager.Instance.Play("ZomAgg");
        yield return new WaitForSeconds(2.5f);
        _agent.isStopped = false;
    }
}