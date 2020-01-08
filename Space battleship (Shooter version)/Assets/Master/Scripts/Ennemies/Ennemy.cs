using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    //Attributs d'un ennemy
    public float MaxHullPoints;
    public float MaxPlatePoints;
    public float MaxShieldPoints;
    public float hullPoints;
    public float platePoints;
    public float shieldPoints;
    public float cooldown;              //time to wait between two burst
    public float cooldownTimer;
    public float bulletSpeed;           //vitesse des projectiles tirés par l'ennemi
    public float fireRate;              //cadence de tir de l'ennemi
    public int nbrShots;                //nombre de tir par rafale
    public float angleViseur;           //angle de visé par rapport à la cible

    public bool isTakingConstantDamages = false;
    public bool isActive = false;          //indique si la tourelle est active (contrôlé par le joueur) ou non
    public bool isFiring = false;          //indique si la tourelle est en train de tirer ou non

    //Associated objects
    public GameObject Bullet;              //type de balle tiré par la tourelle
    public PlayerDetector playerDetector;
    public WaveConfig waveConfig;
    public EnnemySquad squad;

    //Coroutines
    public Coroutine firingCoroutine;     //coroutine de tir 


    // Start is called before the first frame update
    void Start()
    {
        hullPoints = MaxHullPoints;
        shieldPoints = MaxShieldPoints;
        platePoints = MaxPlatePoints;

        playerDetector = GetComponent<PlayerDetector>();
    }

    //Fonction qui gère le cooldown minimum entre deux rafales
    protected void CoolDownManager()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if (cooldownTimer < 0)
        {
            cooldownTimer = 0;
        }
    }

    //Fonction qui gère l'intégralité de la routine de tir de la tourelle
    protected void Fire(string fireMode)
    {
        CoolDownManager();
        if (cooldownTimer == 0 && playerDetector.target != null) //quand le cooldown est écoulé et que l'ennemie à identifier le joueur
        {
            if (!isFiring) //on lance le burst de tir
            {
                isActive = false;
                isFiring = true;
                firingCoroutine = StartCoroutine(BurstFire(fireMode));
            }
        }
    }

    public void TakingDamage(float damageTaken)
    {
        if (shieldPoints > 0)
        {
            shieldPoints -= damageTaken;
        }
        else if (platePoints > 0)
        {
            platePoints -= damageTaken;
        }
        else
        {
            hullPoints -= damageTaken;
        }
        if (hullPoints <= 0)
        {
            if(squad != null)
            {
                squad.imDestroyed(); //Send to his squad that he has been destroyed
            }
            EnnemySpawner ennemySpawner = FindObjectOfType<EnnemySpawner>(); //Send to the ennemy spawn that he has been destroyed
            ennemySpawner.ennemDestroyed++;
            Destroy(gameObject);
        }
    }

    //Fonction qui donne l'angle de visé
    public void Targeter()
    {
        if (playerDetector.target != null)
        {
            Vector3 dif = playerDetector.target.transform.position - transform.position;
            angleViseur = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        }   
    }



    public IEnumerator BurstFire(string firemode)
    {
        yield return new WaitForSeconds(0.1f);
        // rate of fire in weapons is in rounds per minute (RPM), therefore we should calculate how much time passes before firing a new round in the same burst.

        if(firemode == "Normal")
        {
            for (int i = 0; i < nbrShots; i++)
            {
                Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 1);
                GameObject bullet = Instantiate
                    (Bullet,
                    bulletPosition,
                    Quaternion.Euler(new Vector3(0, 0, angleViseur - 90)));

                yield return new WaitForSeconds(fireRate); // wait till the next round
            }
        }
        else if(firemode == "Static")
        {
            for (int i = 0; i < nbrShots; i++)
            {
                Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, 1);
                GameObject bullet = Instantiate
                    (Bullet,
                    bulletPosition,
                    Quaternion.Euler(new Vector3(0, 0, 90)));

                yield return new WaitForSeconds(fireRate); // wait till the next round
            }
        }
        
        isFiring = false;
        cooldownTimer = cooldown;
    }
}
