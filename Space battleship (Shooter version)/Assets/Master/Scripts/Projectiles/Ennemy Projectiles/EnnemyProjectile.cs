using System.Collections;
using UnityEngine;

public class EnnemyProjectile : Projectile
{
    private void Start()
    {
        base.Start();
    }

    // Start is called before the first frame update
    void OnValidate()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.SetBool("Firing", false);
        col = GetComponent<CapsuleCollider2D>();
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
                if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
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
        col.enabled = false;

        if (animator.runtimeAnimatorController != null)
        {
            animator.SetBool("BeingDestroyed", true);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(1).length);
        }
        if (impactParticleSystem != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            impactParticleSystem.Play();
            Debug.Log(impactParticleSystem.main.duration);
            yield return new WaitForSeconds(impactParticleSystem.main.duration);
            GetComponent<SpriteRenderer>().enabled = true;
        }

        salve.ImDestroyed();
        destroy();
    }
}
