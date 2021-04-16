using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyProjectile : Projectile
{
    void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (willFade)
        {
            if (timeBeforeFadding <= 0)
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
            switch (damageType)
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
        else if (collider.tag.Equals("EnnemyProjectile") && isDestroyable)
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

        yield return new WaitForSeconds(0.5f);
        salve.ImDestroyed();
        destroy();
    }
}
