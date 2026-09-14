using UnityEngine;

public class Landmine : MonoBehaviour
{
    [SerializeField] private Transform _explodeEffect;
    public int damge;
    public float damgeRange;
    public LayerMask layerMask;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Zombie"))
        {
            EnemyAI zombie = other.gameObject.GetComponent<EnemyAI>();
            if(zombie != null)
            {
                AudioManager.Instance.Play("Explosion");
                Explosion();
                Instantiate(_explodeEffect, transform.position, Quaternion.identity);
            }
        }
    }

    void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damgeRange, layerMask);
        foreach(Collider collider in hitColliders)
        {
            HealthBar hb = collider.GetComponentInChildren<HealthBar>();
            if(hb != null)
            {
                hb.GotHit(damge);
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.35f);

        Gizmos.DrawSphere(transform.position, damgeRange);
    }
}
