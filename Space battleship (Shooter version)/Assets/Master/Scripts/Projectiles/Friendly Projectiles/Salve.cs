using Unity.Collections;
using UnityEngine;

public class Salve : MonoBehaviour
{
    public Projectile[] projectiles;
    [ReadOnly] public float globalDamage;
    [ReadOnly] public float globalHealth;
    [ReadOnly] public float globalbulletSpeed;

    [ReadOnly] int nbrProjectile;
    [ReadOnly] int nbrProjectileDestroyed = 0;

    // Start is called before the first frame update
    void Start()
    {
        nbrProjectile = projectiles.Length;
        for(int i=0;i<projectiles.Length;i++)
        {
            projectiles[i].damage = globalDamage;
            projectiles[i].health = globalHealth;
            projectiles[i].bulletSpeed = globalbulletSpeed;
            
            //projectiles[i].transform.GetComponent<Rigidbody2D>().AddForce(projectiles[i].transform.up * projectiles[i].bulletSpeed);
        }
    }

    private void Update()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (projectiles[i] != null)
            {
                projectiles[i].transform.position += projectiles[i].transform.up * Time.deltaTime * (projectiles[i].bulletSpeed / 50);
            }
        }
    }

    //Function called by projectile when its destroyed, destroy salve object if all projectiles are destroyed
    public void ImDestroyed()
    {
        nbrProjectileDestroyed++;
        if (nbrProjectile == nbrProjectileDestroyed)
        {
            Destroy(gameObject);
        }
    }
}
