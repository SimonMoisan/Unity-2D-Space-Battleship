using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ennemy : MonoBehaviour
{
    [Header("Ennemy stats : ")]
    public float MaxHullPoints;
    public float MaxPlatePoints;
    public float MaxShieldPoints;
    [ReadOnly] public float hullPoints;
    [ReadOnly] public float platePoints;
    [ReadOnly] public float shieldPoints;
    [Space]
    public int dangerIndicator;
    public bool isDestroyed;
    public bool isMoving;
    public float moveSpeed;
    [Header("Reaward when killed")]
    public int minScrapValue;  
    public int maxScrapValue;
    public int chancePercToDropEnergyCore; //percentage to drop an energyCore
    public float overdriveCharge; //Charge given to the ultimate weapon when destroyed
    [Space]
    [Header("Attack management : ")]
    public int nbrSimAttack; //Number attack launch simultaneously
    public EnnemyAttack[] attacks;
    public int[] attackPaterns; //Indicate which attack belong to which coolDownUnits ([1,1,2] : two first attacks belong to the first cooldownUnit))
    public LinkedList<EnnemyAttack>[] coolDownUnits;
    [Space]
    [Header("Associated objects : ")]
    public Animator animator;
    public CapsuleCollider2D col;
    public EnnemySpawner ennemySpawner;
    public EnnemySquad squad;
    public GameObject reactor;
    [Space]
    public Image hullBar;
    public Image shieldBar;

    protected void Awake()
    {
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        ennemySpawner = FindObjectOfType<EnnemySpawner>(); //Send to the ennemy spawn that he has been destroyed

        hullPoints = MaxHullPoints;
        shieldPoints = MaxShieldPoints;
        platePoints = MaxPlatePoints;

        isMoving = true;
        squad = GetComponentInParent<EnnemySquad>();
    }

    protected void enqueuAttack(EnnemyAttack attack)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attack == attacks[i])
            {
                int coolDownUnitIndex = attackPaterns[i]; //Identify to which cooldownUnit this attack belongs to
                coolDownUnits[coolDownUnitIndex].AddFirst(attack);
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
            if (!isDestroyed) //To avoid multiple destructions
            {
                StartCoroutine(destruction(true));
            }
        }
        else
        {
            animator.Play("Damaged");
        }

        //Update bars
        hullBar.fillAmount = hullPoints / MaxHullPoints;
        if (shieldBar != null && MaxShieldPoints > 0) shieldBar.fillAmount = shieldPoints / MaxShieldPoints;
    }

    //Function used to manage attacks
    protected void AttackManagement()
    {
        //Fire attacks ready to fire
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].isReadyToFire && attacks[i].playerDetector.target != null && !attacks[i].isFiring)
            {
                attacks[i].Fire();
                //Add the cooldownmanager to a random queue
                enqueuAttack(attacks[i]);

                for (int j = 0; j < coolDownUnits.Length; j++)
                {
                    if (coolDownUnits[j].Last != null && attacks[i] == coolDownUnits[j].Last.Value)
                    {
                        coolDownUnits[j].RemoveLast();
                    }
                }
            }
        }

        //Active cooldows in queues
        for (int i = 0; i < coolDownUnits.Length; i++)
        {
            if (coolDownUnits[i].Last != null)
            {
                coolDownUnits[i].Last.Value.CoolDownManager();
            }
        }
    }

    public IEnumerator destruction(bool destroyedByPlayer)
    {
        if(reactor != null)
        {
            reactor.SetActive(false);
        }

        //Disable collision with other projectiles
        col.enabled = false;

        //Say to the ennemyspawner it was destroyed
        ennemySpawner.EnnemyDestroyed();
        isDestroyed = true;

        if(squad != null)
        {
            StartCoroutine(squad.imDestroyed());
        }

        //Give scrap and uwCharge to the player as reward
        if (destroyedByPlayer)
        {
            Battleship battleship = FindObjectOfType<Battleship>();
            battleship.chargeOverdrive(overdriveCharge);

            //Reward
            int scrapValue = Random.Range(minScrapValue, maxScrapValue);
            int energyCoreValue = 0;

            int energyCoreDropChance = Random.Range(0, 100);
            if(energyCoreDropChance <= chancePercToDropEnergyCore)
            {
                energyCoreValue = 1;
            }
            
            ennemySpawner.scrapsToWin += scrapValue;
            ennemySpawner.energyCoreToWin += energyCoreValue; 
        }

        //animator.SetBool("isDead", true);
        animator.Play("Explosion");

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}