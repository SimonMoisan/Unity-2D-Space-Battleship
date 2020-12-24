﻿using System.Collections;
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
    public Collider2D col;
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

    private void Start()
    {
        ennemySalve = GetComponentInParent<EnnemySalve>();
    }

    // Start is called before the first frame update
    void OnValidate()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("Firing", false);
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
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
        bulletSpeed = 0;
        rb2D.isKinematic = false;
        rb2D.WakeUp();
        col.isTrigger = false;
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0f;
        animator.SetBool("BeingDestroyed", true);
        yield return new WaitForSeconds(0.4f);
        ennemySalve.ImDestroyed();
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}
