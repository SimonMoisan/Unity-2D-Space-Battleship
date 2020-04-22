using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public enum DamageType { Laser, Kinetic, Ion, Explosive }

public class Projectile : MonoBehaviour
{
    //Projectile information
    [Header("Projectile parameters")]
    [ReadOnly] public float bulletSpeed;
    [ReadOnly] public float maxSpeed;
    public DamageType damageType;
    public float damage;

    //Projectile can be destroyed by ennemy projectile (case of shields)
    [Header("Projectile destroyable")]
    public bool isDestroyable;
    [ReadOnly] public float health;

    [Header("Projectile explosive")]
    public bool canExplode;
    public float explosionRadius;

    [Header("Projectile scatter")]
    public bool canScatter;

    [Header("Projectile fade with time")]
    public bool willFade;
    public float timeBeforeFadding;

    [Header("Projectile deccelerable")]
    public bool isDeccelerating;
    public float delayBeforeDecceleration;
    public float deccelerationRate;
    public float deccelerationSpeed;
    private float currenteDecceleration;

    [Header("Projectile accelerable")]
    public bool isAccelerating;
    public float delayBeforeAcceleration;
    public float accelerationRate;
    public float accelerationSpeed;
    private float currenteAcceleration;

    [Header("Associated objects")]
    public Animator animator;
    public Rigidbody2D rb2D;
    public PolygonCollider2D col;
    public Salve salve;

    //Coroutines
    IEnumerator accDecCoroutine;

    void Start()
    {
        salve = GetComponentInParent<Salve>();
        animator = gameObject.GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();

        if (isAccelerating && !isDeccelerating)
        {
            accDecCoroutine = Acceleration();
            StartCoroutine(accDecCoroutine);
        }
        if(isDeccelerating && !isAccelerating)
        {
            accDecCoroutine = Decceleration();
            StartCoroutine(accDecCoroutine);
        }
    }

    private void Update()
    {
        if(willFade)
        {
            if(timeBeforeFadding <= 0)
            {
                StartCoroutine(Destruction());
            }
            else
            {
                timeBeforeFadding -= Time.deltaTime;
            }
        }
    }

    private void dealDamages(Ennemy ennemyGO)
    {
        if (ennemyGO != null)
        {
            switch(damageType)
            {
                case DamageType.Ion:
                    if (ennemyGO.shieldPoints > 0)
                    {
                        float finalDamage = damage * 2;
                        ennemyGO.TakingDamage(finalDamage);
                    }
                    else
                    {
                        ennemyGO.TakingDamage(damage);
                    }
                    break;

                case DamageType.Kinetic:
                    if (ennemyGO.shieldPoints > 0)
                    {
                        float finalDamage = damage * 0.4f;
                        ennemyGO.TakingDamage(finalDamage);
                    }
                    else
                    {
                        ennemyGO.TakingDamage(damage);
                    }
                    break;

                case DamageType.Laser:
                    if (ennemyGO.shieldPoints > 0)
                    {
                        float finalDamage = damage * 0.6f;
                        ennemyGO.TakingDamage(finalDamage);
                    }
                    else
                    {
                        ennemyGO.TakingDamage(damage);
                    }
                    break;

                case DamageType.Explosive:
                    ennemyGO.TakingDamage(damage);
                    break;
            } 
        }
    }

    private void absorbProjectile()
    {

    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.tag.Equals("Ennemy") && damage > 0)
        {
            Ennemy ennemyGO = collider.GetComponent<Ennemy>();
            dealDamages(ennemyGO);

            StartCoroutine(Destruction());
        }
        else if (collider.name.Equals("BulletDestroyer"))
        {
            StartCoroutine(Destruction());
        }
        else if(collider.tag.Equals("EnnemyProjectile") && isDestroyable)
        {
            EnnemyProjectile ennemyProjectileGO = collider.GetComponent<EnnemyProjectile>();
            health -= ennemyProjectileGO.damage;

            if (health <= 0)
            {
                StartCoroutine(Destruction());
            }
        }
    }

    IEnumerator Destruction()
    {
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0f;
        animator.SetBool("BeingDestroyed", true);
        yield return new WaitForSeconds(0.4f);
        salve.ImDestroyed();
        Destroy(gameObject);
    }

    IEnumerator Acceleration()
    {
        while(true)
        {
            currenteAcceleration += accelerationSpeed;

            bulletSpeed += currenteAcceleration;

            if (bulletSpeed > maxSpeed)
            {
                bulletSpeed = maxSpeed;
            }

            GetComponent<Rigidbody2D>().AddForce(transform.up * bulletSpeed);
            yield return new WaitForSeconds(accelerationRate);
        }
    }

    IEnumerator Decceleration()
    {
        bulletSpeed = 0;
        while (true)
        {
            currenteDecceleration += deccelerationSpeed;

            bulletSpeed -= currenteDecceleration;

            if(rb2D.velocity.x >= 0 || rb2D.velocity.y >= 0)
            {
                GetComponent<Rigidbody2D>().AddForce(transform.up * bulletSpeed);
            }
            else
            {
                rb2D.velocity = Vector2.zero;
            }

            yield return new WaitForSeconds(deccelerationRate);
        }
    }
}
