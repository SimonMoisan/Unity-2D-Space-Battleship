using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ennemy : MonoBehaviour
{
    [Header("Ennemy stats : ")]
    //Attributs d'un ennemy
    public float MaxHullPoints;
    public float MaxPlatePoints;
    public float MaxShieldPoints;
    [ReadOnly] public float hullPoints;
    [ReadOnly] public float platePoints;
    [ReadOnly] public float shieldPoints;

    public int scrapValue;  //Scrap given when destroyed
    public float uwChargeValue; //Charge given to the ultimate weapon when destroyed
    public bool isDestroyed = false;

    [Header("Attack management : ")]
    public int nbrSimAttack; //Number attack launch simultaneously
    public EnnemyAttack[] attacks;
    public int[] attackPaterns; //Indicate which attack belong to which coolDownUnits ([1,1,2] : two first attacks belong to the first cooldownUnit))
    public LinkedList<EnnemyAttack>[] coolDownUnits;

    [Header("Associated objects : ")]
    //Associated objects
    public WaveConfig waveConfig;
    public EnnemySquad squad;
    public Animator animator;
    public PolygonCollider2D collider;

    //Movement parameters
    [ReadOnly] public Transform[] waypoints;
    public float moveSpeed;
    [ReadOnly] int waypointIndex = 0;

    //Image
    public Image hullBar;
    public Image shieldBar;

    // Start is called before the first frame update
    void Awake()
    {
        collider = GetComponent<PolygonCollider2D>();
        animator = GetComponent<Animator>();

        //Initiate attack management
        attacks = GetComponentsInChildren<EnnemyAttack>();
        coolDownUnits = new LinkedList<EnnemyAttack>[nbrSimAttack];
        for (int i = 0; i < coolDownUnits.Length; i++)
        {
            coolDownUnits[i] = new LinkedList<EnnemyAttack>();
        }

        //Enqueue every attacks
        for (int i = 0; i < attacks.Length; i++)
        {
            enqueuAttack(attacks[i]);
        }
        
        hullPoints = MaxHullPoints;
        shieldPoints = MaxShieldPoints;
        platePoints = MaxPlatePoints;

        
        squad = GetComponentInParent<EnnemySquad>();

        //Move on its own if an ennemy doesn't have a squad
        if (squad == null)
        {
            waypoints = waveConfig.GetWaypoints();
            transform.position = waypoints[waypointIndex].transform.position;
        }

        //Can't have less than one attack
        if(nbrSimAttack <= 0)
        {
            nbrSimAttack = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            attacks[i].Targeter();
        }

        AttackManagement();

        //Move on its own if an ennemy doesn't have a squad
        if (squad == null)
        {
            MoveCible();
        }
    }

    private void enqueuAttack(EnnemyAttack attack)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            if(attack == attacks[i])
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
            if(!isDestroyed) //To avoid multiple destructions
            {
                StartCoroutine(destruction());
            }
        }

        //Update bars
        hullBar.fillAmount = hullPoints / MaxHullPoints;
        if(shieldBar != null && MaxShieldPoints > 0) shieldBar.fillAmount = shieldPoints / MaxShieldPoints;
    }

    //Function used to manage attacks
    protected void AttackManagement()
    {
        //Fire attacks ready to fire
        for (int i = 0; i < attacks.Length; i++)
        {
            if(attacks[i].isReadyToFire && attacks[i].playerDetector.target != null && !attacks[i].isFiring)
            {
                attacks[i].Fire();
                //Add the cooldownmanager to a random queue
                enqueuAttack(attacks[i]);

                for (int j = 0; j < coolDownUnits.Length; j++)
                {
                    if(coolDownUnits[j].Last != null && attacks[i] == coolDownUnits[j].Last.Value)
                    {
                        coolDownUnits[j].RemoveLast();
                    }
                }
            }
        }

        //Active cooldows in queues
        for (int i = 0; i < coolDownUnits.Length; i++)
        {
            if(coolDownUnits[i].Last != null)
            {
                coolDownUnits[i].Last.Value.CoolDownManager();
            }
        }
    }

    private void MoveCible()
    {
        if (waypointIndex <= waypoints.Length - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            if (waveConfig.dieAtEnd)
            {
                EnnemySpawner ennemySpawner = FindObjectOfType<EnnemySpawner>(); //Send to the ennemy spawn that he has been destroyed
                ennemySpawner.ennemyDestroyed++;
                ennemySpawner.ennemyDestroyedInWave++;
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator destruction()
    {
        //Disable collision with other projectiles
        collider.enabled = false;

        //Say to the ennemyspawner it was destroyed
        EnnemySpawner ennemySpawner = FindObjectOfType<EnnemySpawner>(); //Send to the ennemy spawn that he has been destroyed
        ennemySpawner.EnnemyDestroyed();
        isDestroyed = true;

        //Give scrap and uwCharge to the player as reward
        Battleship battleship = FindObjectOfType<Battleship>();
        battleship.getEnnemyReward(scrapValue, uwChargeValue);

        if(squad != null)
        {
            StartCoroutine(squad.imDestroyed());
        }

        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}


