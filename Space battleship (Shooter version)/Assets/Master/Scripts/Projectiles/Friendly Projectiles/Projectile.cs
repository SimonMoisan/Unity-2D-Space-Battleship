using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isDestroyed = false;
    [SerializeField] public float bulletSpeed;
    public float damage;

    //Animator
    public Animator animator;

    public Rigidbody2D rb2D;

    void start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.tag.Equals("Ennemy"))
        {
            Ennemy ennemyGO = collider.GetComponent<Ennemy>();
            if(ennemyGO != null)
            {
                ennemyGO.TakingDamage(damage);
            }

            StartCoroutine(Destruction());
        }
        else if (collider.name.Equals("BulletDestroyer"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Destruction()
    {
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0f;
        animator.SetBool("BeingDestroyed", true);
        yield return new WaitForSeconds(0.5f);
        isDestroyed = true;
        Destroy(gameObject);
    }

    public bool BulletIsDestroyed()
    {
        return isDestroyed;
    }

    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }

    public void SetProjectileDamage(float amount)
    {
        damage = amount;
    }
}
