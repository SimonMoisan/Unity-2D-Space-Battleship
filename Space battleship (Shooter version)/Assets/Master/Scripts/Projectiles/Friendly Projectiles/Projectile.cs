using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isDestroyed = false;
    [SerializeField] public float bulletSpeed;

    //Animator
    public Animator animator;

    public Rigidbody2D rb2D;

    void start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Ennemy"))
        {
            StartCoroutine(Destruction());
        }
        else if (collision.name.Equals("BulletDestroyer"))
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
}
