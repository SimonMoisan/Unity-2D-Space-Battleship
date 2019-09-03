using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyProjectile : MonoBehaviour
{
    public bool isDestroyed = false;
    [SerializeField] public float bulletSpeed;

    //Animator
    public Animator animator;
    public Rigidbody2D rb2D;
    public Ennemy ennemy;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("Firing", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Completed-Shield") || collision.name.Equals("Vaisseau"))
        {
            StartCoroutine(Destruction());
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
