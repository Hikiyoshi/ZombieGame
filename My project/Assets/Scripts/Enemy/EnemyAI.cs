using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // public event Action<ZombieType> OnDeath;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    [Header("Chase")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Attack")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float timePerAttack = 1.2f;

    private float _attackTimer;
    private bool _hasAnimator;
    private bool _isDead;
    private GameObject _cam;
    private bool _isWaitingAfterAttack;
    private float _pathUpdateTimer;

    // Animator hashes
    private static readonly int AnimIDSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimIDAttack = Animator.StringToHash("Attack");
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
    }

    private void Start()
    {
        if (_cam == null)
        {
            _cam = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Update()
    {
        if (_isDead || player == null) return;

        Move();

        UpdateAnimator();

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

            HandleAttack();
        }
        else if (distance <= detectRange)
        {
            _agent.isStopped = false;

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

    private IEnumerator WaitTime()
    {
        _isWaitingAfterAttack = true;
        _agent.isStopped = true;
        yield return new WaitForSeconds(1.5f);
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

        // if (_hasAnimator)
        // {
        //     _animator.SetTrigger(AnimIDAttack);
        // }

        _attackTimer = timePerAttack;
    }

    private void UpdateAnimator()
    {
        if (!_hasAnimator) return;

        float speedPercent = _agent.velocity.magnitude / Mathf.Max(_agent.speed, 0.01f);
        _animator.SetFloat(AnimIDSpeed, speedPercent);
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

        if (_hasAnimator)
        {
            _animator.SetTrigger(AnimIDDie);
        }

        if (TryGetComponent<Collider>(out var col))
        {
            col.enabled = false;
        }

        Destroy(gameObject, 2.5f);
    }
}
