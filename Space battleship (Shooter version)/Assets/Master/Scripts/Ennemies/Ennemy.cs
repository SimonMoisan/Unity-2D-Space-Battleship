using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    //Attributs d'un ennemy
    [SerializeField] public float MaxHullPoints;
    [SerializeField] public float MaxPlatePoints;
    [SerializeField] public float MaxShieldPoints;
    [SerializeField] public float hullPoints;
    [SerializeField] public float platePoints;
    [SerializeField] public float shieldPoints;
    [SerializeField] public float cooldown;              //time to wait between two burst
    [SerializeField] public float cooldownTimer;
    [SerializeField] public float bulletSpeed;           //vitesse des projectiles tirés par l'ennemi
    [SerializeField] public float fireRate;              //cadence de tir de l'ennemi
    [SerializeField] public int nbrShots;                //nombre de tir par rafale
    [SerializeField] public float angleViseur;           //angle de visé par rapport à la cible

    [SerializeField] public bool isTakingConstantDamages = false;
    [SerializeField] public bool isActive = false;          //indique si la tourelle est active (contrôlé par le joueur) ou non
    [SerializeField] public bool isFiring = false;          //indique si la tourelle est en train de tirer ou non

    //Associated objects
    [SerializeField] public GameObject Bullet;              //type de balle tiré par la tourelle
    [SerializeField] public PlayerDetector playerDetector;

    //Coroutines
    public Coroutine firingCoroutine;                //coroutine de tir 


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        if (cooldownTimer == 0 && playerDetector.GetTarget() != null) //quand le cooldown est écoulé et que l'ennemie à identifier le joueur
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
            Destroy(gameObject);
        }
    }

    //Fonction qui donne l'angle de visé
    public void Targeter()
    {
        if (playerDetector.GetTarget() != null)
        {
            Vector3 dif = playerDetector.GetTarget().transform.position - transform.position;
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
