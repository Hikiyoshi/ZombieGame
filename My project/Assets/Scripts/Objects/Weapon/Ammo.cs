using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private bool _isMulti = false;
    [SerializeField] private Transform _hitEffect;
    public int damge {get; set;}
    private bool _hasHit = false;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Zombie"))
        {
            if(_hasHit && !_isMulti) return;

            EnemyAI zombie = other.gameObject.GetComponent<EnemyAI>();
            if(zombie != null)
            {
                zombie.GotHit(damge);
                _hasHit = true;
                // Instantiate(_hitEffect, other.transform);
            }
        }
    }
}
