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
    public float currenteDecceleration;

    [Header("Projectile accelerable")]
    public bool isAccelerating;
    public float delayBeforeAcceleration;
    public float accelerationRate;
    public float accelerationSpeed;
    public float currenteAcceleration;

    [Header("Associated objects")]
    public Animator animator;
    public CapsuleCollider2D col;
    public Salve salve;

    //Coroutines
    public IEnumerator accDecCoroutine;

    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        salve = GetComponentInParent<Salve>();

        if (isAccelerating && !isDeccelerating)
        {
            accDecCoroutine = Acceleration();
            StartCoroutine(accDecCoroutine);
        }
        if (isDeccelerating && !isAccelerating)
        {
            accDecCoroutine = Decceleration();
            StartCoroutine(accDecCoroutine);
        }

        //Destroy(gameObject, 15);
    }

    private void absorbProjectile()
    {

    }

    public IEnumerator Acceleration()
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

    public IEnumerator Decceleration()
    {
        bulletSpeed = 0;
        while (true)
        {
            currenteDecceleration += deccelerationSpeed;

            bulletSpeed -= currenteDecceleration;

            /*if(rb2D.velocity.x >= 0 || rb2D.velocity.y >= 0)
            {
                GetComponent<Rigidbody2D>().AddForce(transform.up * bulletSpeed);
            }
            else
            {
                rb2D.velocity = Vector2.zero;
            }*/

            yield return new WaitForSeconds(deccelerationRate);
        }
    }

    public void destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Invoke("destroy", 5f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
