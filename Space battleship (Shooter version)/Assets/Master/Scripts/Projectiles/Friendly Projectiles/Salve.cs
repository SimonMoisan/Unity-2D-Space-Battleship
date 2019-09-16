using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salve : MonoBehaviour
{
    public Projectile[] projectiles;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<projectiles.Length;i++)
        {
            projectiles[i].SetProjectileDamage(damage);
            projectiles[i].transform.GetComponent<Rigidbody2D>().AddForce(projectiles[i].transform.up * projectiles[i].GetBulletSpeed());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsProjectilesAllDestroyed())
        {
            Destroy(gameObject);
        }
    }

    public bool IsProjectilesAllDestroyed()
    {
        int nbrProjectileDestroyed = 0;
        for(int i=0;i<projectiles.Length;i++)
        {
            if (projectiles[i].isDestroyed)
            {
                nbrProjectileDestroyed++;
            }
            if(projectiles.Length == nbrProjectileDestroyed)
            {
                return true;
            }
        }
        return false;
    }

    public void SetSalveDamage(float amount)
    {
        damage = amount;
    }
}
