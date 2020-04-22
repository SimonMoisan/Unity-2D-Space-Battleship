using System.Collections;
using UnityEngine;

public class EnnemyProjectile : MonoBehaviour
{
    [Header("Projectile parameters")]
    public float bulletSpeed;
    public float damage;
    public DamageType damageType;

    [Header("Associated objects")]
    public Animator animator;
    public Rigidbody2D rb2D;
    public PolygonCollider2D col;
    public EnnemySalve ennemySalve;

    //Projectile can be destroyed by ennemy projectile (case of shields)
    [Header("Projectile destroyable")]
    public bool isDestroyable;
    public float health;

    [Header("Projectile explosive")]
    public bool canExplode;
    public float explosionRadius;

    [Header("Projectile scatter")]
    public bool canScatter;

    private bool hasJoint;

    // Start is called before the first frame update
    void Start()
    {
        ennemySalve = GetComponentInParent<EnnemySalve>();
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("Firing", false);
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Completed-Shield") || collision.name.Equals("Vaisseau"))
        {
            StartCoroutine(Destruction());
        }
        else if (collision.name.Equals("BulletDestroyer"))
        {
            StartCoroutine(Destruction());
        }
        else if(collision.tag.Equals("FriendlyProjectile"))
        {
            if(collision.GetComponent<Projectile>().isDestroyable)
            {
                if (collision.gameObject.GetComponent<Rigidbody2D>() != null && !hasJoint)
                {
                    //Stick the projectile impact to a destroyable friendly projectile
                    /*rb2D.velocity = Vector2.zero;
                    rb2D.angularVelocity = 0f;

                    gameObject.AddComponent<FixedJoint2D>();
                    gameObject.GetComponent<FixedJoint2D>().connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
                    hasJoint = true;*/
                }
                StartCoroutine(Destruction());
            }
        }
    }

    IEnumerator Destruction()
    {
        col.isTrigger = false;
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0f;
        animator.SetBool("BeingDestroyed", true);
        yield return new WaitForSeconds(0.5f);
        ennemySalve.ImDestroyed();
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}
