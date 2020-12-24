using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum BossState { passive, agressive, rush } //passive : move and stay to passive points,  aggressive : follow points toward the player, rush : rush in front of the player, stop and lauch an attack
public class BossEnnemy : Ennemy
{
    [Header("Boss stats :")]
    public int passiveSpeed;
    public int aggroSpeed;
    [Header("Boss state :")]
    public BossState actualBossState;
    public BossState previousBossState;
    public bool isActive;
    public bool aggroStateAvailable;
    public bool passiveStateAvailable;
    public bool rushStateAvailable;
    [Header("Timer and cooldowns :")]
    private float changeStateTimer;
    public float cooldownChangeState;
    [Header("Associated objects : ")]
    public AIPath aIPath;
    public AIDestinationSetter aIDestinationSetter;
    public Seeker seeker;
    [Space]
    public Transform[] passiveWaypoints;
    public Transform[] aggroWaypoints;
    public Transform temporaryPoint;
    public Transform targetPoint;
    [Space]
    public Transform attackPaternsParent;
    public List<EnnemyAttackPatern> ennemyAttackPaternsAggro;
    public List<EnnemyAttackPatern> ennemyAttackPaternsPassive;
    public List<EnnemyAttackPatern> ennemyAttackPaternsRush;
    public LinkedList<EnnemyAttackPatern> attackPaternAggroQueue;
    public LinkedList<EnnemyAttackPatern> attackPaternPassiveQueue;
    public LinkedList<EnnemyAttackPatern> attackPaternRushQueue;

    public IEnumerator combatCoroutine;

    private void OnValidate()
    {
        aIDestinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();

        //Initiate queues
        attackPaternAggroQueue = new LinkedList<EnnemyAttackPatern>();
        attackPaternPassiveQueue = new LinkedList<EnnemyAttackPatern>();
        attackPaternRushQueue = new LinkedList<EnnemyAttackPatern>();

        //Reset lists
        ennemyAttackPaternsAggro.Clear();
        ennemyAttackPaternsPassive.Clear();
        ennemyAttackPaternsRush.Clear();

        EnnemyAttackPatern[] allAttackPaterns = attackPaternsParent.GetComponentsInChildren<EnnemyAttackPatern>();
        for (int i = 0; i < allAttackPaterns.Length; i++)
        {
            //Add attack paterns to their rescpective list and respective queue
            if(allAttackPaterns[i].attackPaternType == AttackPaternType.Aggro)
            {
                ennemyAttackPaternsAggro.Add(allAttackPaterns[i]);
                attackPaternAggroQueue.AddFirst(allAttackPaterns[i]);
            }
            else if(allAttackPaterns[i].attackPaternType == AttackPaternType.Passive)
            {
                ennemyAttackPaternsPassive.Add(allAttackPaterns[i]);
                attackPaternPassiveQueue.AddFirst(allAttackPaterns[i]);
            }
            else if(allAttackPaterns[i].attackPaternType == AttackPaternType.Rush)
            {
                ennemyAttackPaternsRush.Add(allAttackPaterns[i]);
                attackPaternRushQueue.AddFirst(allAttackPaterns[i]);
            }
            else
            {
                int randomQueue = Random.Range(0, 2);
                if(randomQueue == 0)
                {
                    ennemyAttackPaternsAggro.Add(allAttackPaterns[i]);
                    attackPaternAggroQueue.AddFirst(allAttackPaterns[i]);
                }
                else if(randomQueue == 1)
                {
                    ennemyAttackPaternsPassive.Add(allAttackPaterns[i]);
                    attackPaternPassiveQueue.AddFirst(allAttackPaterns[i]);
                }
                else if(randomQueue == 2)
                {
                    ennemyAttackPaternsRush.Add(allAttackPaterns[i]);
                    attackPaternRushQueue.AddFirst(allAttackPaterns[i]);
                }
            }
        }
    }

    private void Awake()
    {
        //Initiate
        isActive = true;

        parentPassiveWP parentPassiveWP = FindObjectOfType<parentPassiveWP>();
        parentAggroWP parentAggroWP = FindObjectOfType<parentAggroWP>();

        passiveWaypoints = parentPassiveWP.GetComponentsInChildren<Transform>();
        aggroWaypoints = parentAggroWP.GetComponentsInChildren<Transform>();

    }

    private void Update()
    {
        if(isActive)
        {
            combatPhaseRoutine();
        }
    }

    private void changeState()
    {
        if(actualBossState == BossState.agressive)
        {
            actualBossState = BossState.passive;
        }
        else
        {
            actualBossState = BossState.agressive;
        }
    }

    private void changePoint()
    {
        int randomIndex;
        switch (actualBossState)
        {
            case BossState.passive:
                randomIndex = Random.Range(1, passiveWaypoints.Length - 1);
                targetPoint = passiveWaypoints[randomIndex];
                //movementTimer = Random.Range(minPassiveCD, maxPassiveCD);
                aIPath.maxSpeed = passiveSpeed;
                break;

            case BossState.agressive:
                randomIndex = Random.Range(1, aggroWaypoints.Length - 1);
                targetPoint = aggroWaypoints[randomIndex];
                //movementTimer = Random.Range(minPassiveCD, maxPassiveCD);
                aIPath.maxSpeed = aggroSpeed;
                break;

            case BossState.rush:
                randomIndex = Random.Range(1, aggroWaypoints.Length - 1);
                Transform randomAggroPoint = aggroWaypoints[randomIndex];
                if(temporaryPoint != null)
                {
                    Destroy(temporaryPoint.gameObject);
                }
                temporaryPoint = Instantiate(randomAggroPoint, randomAggroPoint);
                targetPoint = temporaryPoint;
                break;
        }
        aIDestinationSetter.target = targetPoint;
    }

    private void passiveModeRoutine()
    {

    }

    private void agressiveModeRoutine()
    {

    }

    public void combatPhaseRoutine()
    {
        //Fire attack paterns ready to fire
        Debug.Log(attackPaternAggroQueue.Last.Value.isReadyToFire);
        if (attackPaternAggroQueue.Last.Value != null && attackPaternAggroQueue.Last.Value.isReadyToFire && aggroStateAvailable)
        {
            StartCoroutine(attackPaternAggroQueue.Last.Value.fireAllAttacks()); //Execute all attack from this patern

            if (attackPaternAggroQueue.Last.Value.attackPaternType == AttackPaternType.Any)
            {
                queuePaternRandomly(attackPaternAggroQueue.Last.Value);
            }
            else
            {
                //Get random patern from this type of patern to add to the cooldown queue
                int randomIndex = Random.Range(0, ennemyAttackPaternsAggro.Count);
                attackPaternAggroQueue.AddFirst(ennemyAttackPaternsAggro[randomIndex]);
            }

            attackPaternAggroQueue.RemoveLast(); //Remove the alreasy used patern from the queue

            //Change actual bossState according to its attack patern
            previousBossState = actualBossState;
            actualBossState = BossState.agressive;
            changePoint();

        }
        else if (attackPaternPassiveQueue.Last.Value != null && attackPaternPassiveQueue.Last.Value.isReadyToFire && passiveStateAvailable)
        {
            StartCoroutine(attackPaternPassiveQueue.Last.Value.fireAllAttacks()); //Execute all attack from this patern

            if (attackPaternAggroQueue.Last.Value.attackPaternType == AttackPaternType.Any)
            {
                queuePaternRandomly(attackPaternAggroQueue.Last.Value);
            }
            else
            {
                //Get random patern from this type of patern to add to the cooldown queue
                int randomIndex = Random.Range(0, ennemyAttackPaternsPassive.Count);
                attackPaternPassiveQueue.AddFirst(ennemyAttackPaternsPassive[randomIndex]);
            }

            attackPaternPassiveQueue.RemoveLast(); //Remove the alreasy used patern from the queue

            //Change actual bossState according to its attack patern
            previousBossState = actualBossState;
            actualBossState = BossState.passive;
            changePoint();
        }
        else if (attackPaternRushQueue.Last.Value != null && attackPaternRushQueue.Last.Value.isReadyToFire && rushStateAvailable)
        {
            StartCoroutine(attackPaternRushQueue.Last.Value.fireAllAttacks()); //Execute all attack from this patern

            if (attackPaternAggroQueue.Last.Value.attackPaternType == AttackPaternType.Any)
            {
                queuePaternRandomly(attackPaternAggroQueue.Last.Value);
            }
            else
            {
                //Get random patern from this type of patern to add to the cooldown queue
                int randomIndex = Random.Range(0, ennemyAttackPaternsRush.Count);
                attackPaternRushQueue.AddFirst(ennemyAttackPaternsRush[randomIndex]);
            }

            //Change actual bossState according to its attack patern
            previousBossState = actualBossState;
            actualBossState = BossState.rush;
            changePoint();

            attackPaternRushQueue.RemoveLast(); //Remove the alreasy used patern from the queue
        }

        //Cooldwon managers
        if (attackPaternAggroQueue.Last != null && aggroStateAvailable && attackPaternAggroQueue.Last.Value)
        {
            attackPaternAggroQueue.Last.Value.cooldownManager();
        }
        if (attackPaternPassiveQueue.Last != null && passiveStateAvailable)
        {
            attackPaternPassiveQueue.Last.Value.cooldownManager();
        }
        if (attackPaternRushQueue.Last != null && rushStateAvailable)
        {
            attackPaternRushQueue.Last.Value.cooldownManager();
        }
    }

    private void queuePaternRandomly(EnnemyAttackPatern attackPatern)
    {
        int randomQueue = Random.Range(0, 2);
        if (randomQueue == 0)
        {
            attackPaternAggroQueue.AddFirst(attackPatern);
        }
        else if (randomQueue == 1)
        {
            attackPaternPassiveQueue.AddFirst(attackPatern);
        }
        else if (randomQueue == 2)
        {
            attackPaternRushQueue.AddFirst(attackPatern);
        }
    }
}
