using System.Collections;
using Unity.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    //Configuration parameters
    [Header("Base turret parameters :")]
    public int idTurret;               //id de la tourelle
    public float damage;
    public float bulletHealth;          
    public float fireRate;                
    public int nbrBullet;               //number of salve per burst
    public float cooldown;                //time to wait between two burst
    public float cooldownTimer;
    public float bulletSpeed;
    public float deviationFactor;         //factor of bullet dispersion
    [Space]
    //Beam type turret
    public bool isTurretBeam;
    public float beamDuration;            //duration of the beam  
    public Beam beam;                     //Only here if its a beam turret
    [Space]
    //Ammo type turret
    public int actualAmmo;
    public int maxAmmo;
    [Space]
    //Stats
    public bool isFiring = false;         //indique si la tourelle est en train de tirer ou non
    [Space]
    //Associated objects
    public Animator animator;
    public GameObject Bullet;                              //type de balle tiré par la tourelle
    public TurretDescription descritpion;
    public SoundEffectsGenerator soundEffectsGenerator;
    public ProjectilePool pooler;
    //Coroutines
    public Coroutine manualFiringCoroutine;                //coroutine de tir manuel de la tourelle 
    
    public void OnValidate()
    {
        //Initiate turrets stats from its description
        if (descritpion != null)
        {
            updateTurretStats();

            if (descritpion.modifierPrimaryType == ModifierPrimaryType.Beam) //Beam turret
            {
                isTurretBeam = true;
            }

            if (descritpion.actualAmmo > 0) //Turret with ammunitions
            {
                maxAmmo = descritpion.actualAmmo;
                actualAmmo = maxAmmo;
            }
        }

        soundEffectsGenerator = GetComponentInChildren<SoundEffectsGenerator>();
    }

    // Start is called before the first frame update
    public void Awake()
    {
        if(!isTurretBeam)
        {
            for (int i = 0; i < GameManagerScript.current.poolers.Length; i++)
            {
                if (GameManagerScript.current.poolers[i].salvePrefab == Bullet.GetComponent<Salve>())
                {
                    pooler = GameManagerScript.current.poolers[i];
                }
            }
        }
        
        //Find associated objects
        animator = gameObject.GetComponent<Animator>();
        if(isTurretBeam)
        {
            beam = GetComponentInChildren<Beam>();
        }
    }

    public void updateTurretStats()
    {
        damage = descritpion.actualDamage;
        bulletHealth = descritpion.actualHealth;
        nbrBullet = descritpion.actualNbrOfSalve;
        fireRate = descritpion.actualFirerate;
        cooldown = descritpion.actualCooldown;
        bulletSpeed = descritpion.actualSpeed;
        deviationFactor = descritpion.actualDeviation;
    }

    //Fonction qui gère le cooldown minimum entre deux rafales
    public void CoolDownManager()
    {
        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if(cooldownTimer < 0)
        {
            if(maxAmmo > 0) //if its a tower with ammunition
            {
                if(actualAmmo < maxAmmo)
                {
                    actualAmmo += nbrBullet;
                    if(actualAmmo != maxAmmo)
                    {
                        cooldownTimer = cooldown;
                    }
                }
                else
                {
                    cooldownTimer = 0;
                }
            }
            else //if its a normal turret
            {
                cooldownTimer = 0;
            }

            animator.Play("ReadyToFire");
        }
    }

    //bullet = Instantiate(Bullet, bulletPosition, Quaternion.Euler(new Vector3(0, 0, -90)));

    public float GetCooldownFactor()
    {
        return cooldownTimer / cooldown;
    } 
}
