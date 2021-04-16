using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum xDirection { None, Left, Right }
public enum yDirection { None, Up, Down }
public enum SetTypeActive { Standard, Heavy }

public class Battleship : MonoBehaviour
{
    [Header("Battleship caracteristics :")]
    //Battleship caracteristics
    public float MaxHullPoints;
    public float MaxPlatePoints;
    [ReadOnly] public float hullPoints;
    [ReadOnly] public float platePoints;
    [ReadOnly] public bool hasTakenDamagesRecently;
    [Space]
    [Header("UW weapons informations :")]
    [ReadOnly] public bool uwIsRunning = false;
    public float actualOverdrive;
    public float maxOverdrive;
    public bool overdriveIsActive;
    public bool overdriveEndEffectActivated;
    public OverdriveEffect overdriveEffect;
    [Space]
    [Header("Movement parameters :")]
    //Parameters used for movement
    private Transform initialPosition;       // Start position for every battl phases
    public float maxYVelocity;
    public float ySpeed;
    public float xSpeed;
    [Space]
    [ReadOnly] public float minY;                 // Minimum height the battleship can reach
    [ReadOnly] public float maxY;                 // Maximum height the battleship can reach
    [Space]
    [ReadOnly] public float defaultX;             // Default X position
    [ReadOnly] public float minX;                 // Minimum length the battleship can reach
    [ReadOnly] public float maxX;                 // Maximum length the battleship can reach

    //Buttons states
    public xDirection xButtonState;
    public yDirection yButtonState;

    [Space]
    [Header("Associated objects : ")]
    public PlayerStats playerStats;
    public Animator animator;
    public Rigidbody2D rb2;
    [Header("Turrets parents : ")]
    public Transform arsenalRotationStandardTurret;
    public Transform arsenalRotationHeavyTurret;
    public Transform arsenalFrontalStandardTurret;
    public Transform arsenalFrontalHeavyTurret;
    [Header("Rotations turrets : ")]
    public RotationTurret[] standardRotationTurrets;    //Array containing turrets gameobjects
    public RotationTurret[] heavyRotationTurrets;    //Array containing turrets gameobjects
    public SetTypeActive setTypeActive;
    public int indexRotationStandardTurretsSet;
    public int nextIndexRotationStandardTurretsSet;
    public int indexRotationHeavyTurretsSet;
    public int nextIndexRotationHeavyTurretsSet;
    public int nbrStandardRotationTurretSets;
    public int nbrHeavyRotationTurretSets;
    public bool alternativeFire;
    public bool turretsSelectedFiring;
    public List<RotationTurret> turretSelectedToFire;
    [Header("Frontal turrets : ")]
    public FrontalTurret[] standardFrontalTurrets;      //Spinal turrets are special turrets with unique effects
    public FrontalTurret[] heavyFrontalTurrets;      //Spinal turrets are special turrets with unique effects
    public FrontalTurret[] allFrontalTurrets;       // order : lighter turrets recharge first, they are also used first
    public float cumulativeCooldown;
    public int actualMaxId;
    [Space]
    public Viseur[] viseurs;
    public Transform[] rotationStandardTurretPositions; //Positions where turrets will be created
    [Header("Ultimate weapons associated objects : ")]
    public Gate gate;
    public UltimateWeapon ultimateWeapon;
    public UWProjectile uwProjectile;
    public Shield shield;

    public static Battleship current;

    public Coroutine rotationCoroutine;

    private void OnValidate()
    {
        current = this;

        rb2 = GetComponent<Rigidbody2D>();
        //initialPosition = transform;
        playerStats = FindObjectOfType<PlayerStats>();
        animator = gameObject.GetComponent<Animator>();
        hullPoints = MaxHullPoints;
        platePoints = MaxPlatePoints;
        hasTakenDamagesRecently = false;

        //Initialise rotation turrest
        standardRotationTurrets = arsenalRotationStandardTurret.GetComponentsInChildren<RotationTurret>(); //Find turret already installed on battlship
        heavyRotationTurrets = arsenalRotationHeavyTurret.GetComponentsInChildren<RotationTurret>();
        viseurs = FindObjectsOfType<Viseur>();
        if (standardRotationTurrets.Length > 0) //Attribute turret id and viseur to rotation turrets
        {
            for (int i = 0; i < standardRotationTurrets.Length; i++)
            {
                standardRotationTurrets[i].idTurret = i;
                standardRotationTurrets[i].viseur = viseurs[i];
            }
        }
        else
        {
            standardRotationTurrets = new RotationTurret[rotationStandardTurretPositions.Length];
        }

        //Initialise frontal turrest
        standardFrontalTurrets = arsenalFrontalStandardTurret.GetComponentsInChildren<FrontalTurret>();
        heavyFrontalTurrets = arsenalFrontalHeavyTurret.GetComponentsInChildren<FrontalTurret>();
        allFrontalTurrets = new FrontalTurret[standardFrontalTurrets.Length + heavyFrontalTurrets.Length];

        int j = 0;
        for (int i = 0; i < standardFrontalTurrets.Length; i++)
        {
            allFrontalTurrets[j] = standardFrontalTurrets[i];
            j++;
        }
        for (int i = 0; i < heavyFrontalTurrets.Length; i++)
        {
            allFrontalTurrets[j] = heavyFrontalTurrets[i];
            j++;
        }
    }

    private void Awake()
    {
        turretSelectedToFire = new List<RotationTurret>();

        //Initialize frontal turrets cumulative cooldowns
        float cumulativeCooldown = 0;
        int actualMaxId = -1;

        for (int i = 0; i < allFrontalTurrets.Length; i++)
        {
            if (allFrontalTurrets[i].idTurret > actualMaxId)
            {
                actualMaxId = allFrontalTurrets[i].idTurret;
                cumulativeCooldown += allFrontalTurrets[i].cooldown;
            }
            allFrontalTurrets[i].cooldown = cumulativeCooldown;
            allFrontalTurrets[i].cooldownTimer = allFrontalTurrets[i].cooldown;

            allFrontalTurrets[i].animator.Play("OnCD");
        }

        equipNextTurretSet();
    }

    // Update is called once per frame
    void Update()
    {
        StaticAnimation();
        if (playerStats.isPlaying)
        {
            inputMovementManagement();
            UWdeployer();
            rotationTurretsFireManagement();
            frontalTurretsFireManagement();
            overdriveManagement();
        }
        
        Vector3 shipPos = new Vector3(transform.position.x, transform.position.y, 2);
        shipPos.y = Mathf.Clamp(shipPos.y, minY, maxY);
        transform.position = shipPos;
    }

    private void FixedUpdate()
    {
        if (playerStats.isPlaying)
        {
            movementBattleship();
        }
    }

    public void rotationTurretsFireManagement()
    {
        if(Input.GetMouseButtonDown(2))
        {
            equipNextTurretSet();
        }


        if (setTypeActive == SetTypeActive.Standard)
        {
            for (int i = 0; i < standardRotationTurrets.Length; i++)
            {
                RotationTurret turretToCheck = standardRotationTurrets[i];
                int key = turretToCheck.turretButtonId + 1;

                if ((turretToCheck.maxAmmo > 0 && turretToCheck.actualAmmo > 0) || (turretToCheck.maxAmmo == 0 && turretToCheck.cooldownTimer == 0)
                    && Input.GetKeyDown(key.ToString()) && turretToCheck.idTurret >= 4 * indexRotationStandardTurretsSet && turretToCheck.idTurret < (indexRotationStandardTurretsSet + 1) * 4)
                {
                    if(alternativeFire)
                    {
                        if(!turretSelectedToFire.Contains(turretToCheck)) //Turret is not inside the list
                        {
                            if (overdriveIsActive && actualOverdrive >= turretToCheck.GetComponent<Turret>().descritpion.overdriveCost)
                            {
                                actualOverdrive -= turretToCheck.GetComponent<Turret>().descritpion.overdriveCost;
                                turretToCheck.GetComponent<RotationTurret>().turretOverdriveActivated = true;
                                turretSelectedToFire.Add(turretToCheck.GetComponent<RotationTurret>());
                                lockFiringRotationTurrets();
                            }
                            else
                            {
                                turretSelectedToFire.Add(turretToCheck.GetComponent<RotationTurret>());
                                lockFiringRotationTurrets();
                            }
                        }
                        else //Turret is inside the list, then remove it
                        {
                            if (turretToCheck.GetComponent<RotationTurret>().turretOverdriveActivated)
                            {
                                actualOverdrive += turretToCheck.GetComponent<Turret>().descritpion.overdriveCost;
                                turretToCheck.GetComponent<RotationTurret>().turretOverdriveActivated = false;
                            }
                            turretSelectedToFire.Remove(turretToCheck.GetComponent<RotationTurret>());
                        }
                    }
                    else
                    {
                        turretToCheck.GetComponent<RotationTurret>().fire();
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < heavyRotationTurrets.Length; i++)
            {
                RotationTurret turretToCheck = heavyRotationTurrets[i];
                int key = turretToCheck.turretButtonId + 1;

                if ((turretToCheck.maxAmmo > 0 && turretToCheck.actualAmmo > 0) || (turretToCheck.maxAmmo == 0 && turretToCheck.cooldownTimer == 0)
                    && Input.GetKeyDown(key.ToString()) && turretToCheck.idTurret >= 4 * indexRotationHeavyTurretsSet && turretToCheck.idTurret < (indexRotationHeavyTurretsSet + 1) * 4)
                {
                    if (alternativeFire)
                    {
                        turretSelectedToFire.Add(turretToCheck.GetComponent<RotationTurret>());
                        lockFiringRotationTurrets();
                    }
                    else
                    {
                        turretToCheck.GetComponent<RotationTurret>().fire();
                    }
                }
            }
        }

        //Alternative fire
        if(alternativeFire && Input.GetMouseButtonDown(0) && turretSelectedToFire != null && turretSelectedToFire.Count > 0)
        {
            for (int i = 0; i < turretSelectedToFire.Count; i++)
            {
                turretSelectedToFire[i].fire();
            }
            turretSelectedToFire.Clear();
        }
    }

    private void lockFiringRotationTurrets()
    {
        List<RotationTurret> result = new List<RotationTurret>();
        for (int i = 0; i < standardRotationTurrets.Length; i++)
        {
            if (standardRotationTurrets[i].isFiring)
            {
                result.Add(standardRotationTurrets[i]);
            }
        }
        for (int i = 0; i < heavyRotationTurrets.Length; i++)
        {
            if (heavyRotationTurrets[i].isFiring)
            {
                result.Add(heavyRotationTurrets[i]);
            }
        }

        if (result.Count > 0)
        {
            for (int i = 0; i < result.Count; i++)
            {
                Debug.Log(result[i]);
                result[i].lockMode = true;
                result[i].viseur.isLocked = true;
                result[i].viseur.GetComponent<Animator>().Play("Locked");
            }
        }
    }

    private void equipNextTurretSet()
    {
        if (indexRotationStandardTurretsSet < nbrStandardRotationTurretSets)
        {
            indexRotationStandardTurretsSet = nextIndexRotationStandardTurretsSet;
            PlayerStats.current.changeSet(indexRotationStandardTurretsSet, SetTypeActive.Standard);
            nextIndexRotationStandardTurretsSet++;
        }
        else
        {
            if (indexRotationHeavyTurretsSet < nbrHeavyRotationTurretSets)
            {
                indexRotationHeavyTurretsSet = nextIndexRotationHeavyTurretsSet;
                PlayerStats.current.changeSet(indexRotationHeavyTurretsSet, SetTypeActive.Heavy);
                nextIndexRotationHeavyTurretsSet++;
            }
            if (nextIndexRotationHeavyTurretsSet == nbrHeavyRotationTurretSets) //Go back to standard turret sets
            {
                nextIndexRotationHeavyTurretsSet = 0;
                nextIndexRotationStandardTurretsSet = 0;
                setTypeActive = SetTypeActive.Standard;
            }
        }

        if (nbrHeavyRotationTurretSets == 0 && nextIndexRotationStandardTurretsSet == nbrStandardRotationTurretSets) //if no heavy turret set, go back to the first standard turret set
        {
            nextIndexRotationStandardTurretsSet = 0;
        }
    }

    public void frontalTurretsFireManagement()
    {
        if(Input.GetMouseButtonDown(1))
        {
            int chainId = -1;

            //Fire all turrets with this id
            for (int i = 0; i < allFrontalTurrets.Length; i++)
            {
                if(allFrontalTurrets[i].cooldownTimer == 0 && chainId == -1)
                {
                    allFrontalTurrets[i].fire();
                    chainId = allFrontalTurrets[i].idTurret;
                }
                else if(allFrontalTurrets[i].cooldownTimer == 0 && allFrontalTurrets[i].idTurret == chainId)
                {
                    allFrontalTurrets[i].fire();
                }
                
                if(chainId != -1 && allFrontalTurrets[i].idTurret > chainId)
                {
                    Debug.Log("Break");
                    break;
                }
            }
        }
    }

    //Function when the player wants to use the UW device
    public void UWdeployer()
    {
        if (Input.GetKeyDown("u") && !uwIsRunning && actualOverdrive == maxOverdrive)
        {
            StartCoroutine(UWdeployment());
        }
    }

    public void overdriveManagement()
    {
        //Input for continuous overdrive effect
        if(overdriveEffect.overdriveExecType == OverdriveExecType.ContinuousUse && actualOverdrive > 0 && Input.GetKeyDown("space") && !overdriveIsActive)
        {
            overdriveIsActive = true;
        }
        else if(overdriveEffect.overdriveExecType == OverdriveExecType.ContinuousUse && Input.GetKeyDown("space") && overdriveIsActive)
        {
            overdriveIsActive = false;
        }

        //Input for single overdrive effect
        if (overdriveEffect.overdriveExecType == OverdriveExecType.SingleUse && actualOverdrive == maxOverdrive && Input.GetKeyDown("space") && !overdriveIsActive)
        {
            overdriveIsActive = true;
            overdriveEffect.overdriveExecution();
        }


        //Continuous Effect
        if (overdriveEffect.overdriveExecType == OverdriveExecType.ContinuousUse && overdriveIsActive && actualOverdrive > 0)
        {
            overdriveEffect.overdriveExecution();
            actualOverdrive -= Time.deltaTime * overdriveEffect.overdriveCost;
            playerStats.updateOverdriveIndicator();
            overdriveEndEffectActivated = false;
        }
        else if(overdriveEffect.overdriveExecType == OverdriveExecType.ContinuousUse && (actualOverdrive == 0 || !overdriveEndEffectActivated))
        {
            overdriveEffect.overdriveEndEffect();
            overdriveEndEffectActivated = true;
        }
    }

    //Fonction qui gère les dégats subis par le vaisseau
    public void takingDamages(EnnemyProjectile ennemyProjectile)
    {
        //Add shield regeneration cooldown if the battleship taken damages
        hasTakenDamagesRecently = true;
        shield.cooldownTimer += 0.5f;

        float finalDamage = ennemyProjectile.damage;

        if (platePoints > 0 && shield.shieldPoints <= 0)
        {
            //Calculate damages taken
            switch (ennemyProjectile.damageType)
            {
                case DamageType.Kinetic:
                    finalDamage *= 1.3f;
                    break;
                case DamageType.Laser:
                    finalDamage *= 1f;
                    break;
                case DamageType.Plasma:
                    finalDamage *= 1.1f;
                    break;
                case DamageType.Ion:
                    finalDamage *= 0.5f;
                    break;
                case DamageType.Explosive:
                    finalDamage *= 1.5f;
                    break;
            }

            platePoints -= finalDamage;
        }
        else if(platePoints <= 0 && shield.shieldPoints <= 0)
        {
            //Calculate damages taken
            switch (ennemyProjectile.damageType)
            {
                case DamageType.Kinetic:
                    finalDamage *= 1f;
                    break;
                case DamageType.Laser:
                    finalDamage *= 1f;
                    break;
                case DamageType.Plasma:
                    finalDamage *= 1.3f;
                    break;
                case DamageType.Ion:
                    finalDamage *= 0.5f;
                    break;
                case DamageType.Explosive:
                    finalDamage *= 1f;
                    break;
            }

            hullPoints -= finalDamage;
            playerStats.updateHullIndicators();

            animator.Play("Damaged");
        }
    }

    public void takingDamages(float damageInput)
    {
        Debug.LogWarning("Beam damages not implemented");
    }

    public void repairHull(int healAmount)
    {
        hullPoints += healAmount;
        if(hullPoints > MaxHullPoints)
        {
            hullPoints = MaxHullPoints;
        }

        playerStats.updateHullIndicators();
    }

    public void inputMovementManagement()
    {
        if (Input.GetKey("z")) yButtonState = yDirection.Up;
        else if (Input.GetKey("s")) yButtonState = yDirection.Down;
        else yButtonState = yDirection.None;

        if (Input.GetKey("q")) xButtonState = xDirection.Left;
        else if (Input.GetKey("d")) xButtonState = xDirection.Right;
        else xButtonState = xDirection.None;
    }

    public void movementBattleship()
    {
        if(rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        if(yButtonState == yDirection.Up && transform.position.y < maxY)
        {
            rotationCoroutine = StartCoroutine(battleshipRotation(10));
            if (rb2.velocity.y < 0)
            {
                rb2.velocity = new Vector2(rb2.velocity.x, 0);
            }
            rb2.AddForce(transform.up * ySpeed);
        }
        else if(yButtonState == yDirection.Down && transform.position.y > minY)
        {
            rotationCoroutine = StartCoroutine(battleshipRotation(-10));
            if (rb2.velocity.y > 0)
            {
                rb2.velocity = new Vector2(rb2.velocity.x, 0);
            }
            rb2.AddForce(-transform.up * ySpeed);
        }
        else
        {
            rb2.velocity = new Vector2(rb2.velocity.x, rb2.velocity.y * 0.9f);
        }

        if (xButtonState == xDirection.Right && transform.position.x < maxX)
        {
            LeanTween.cancelAll();
            if (rb2.velocity.x < 0)
            {
                rb2.velocity = new Vector2(0, rb2.velocity.y);
            }
            rb2.AddForce(transform.right * xSpeed);
        }
        else if (xButtonState == xDirection.Left && transform.position.x > minX)
        {
            LeanTween.cancelAll();
            if (rb2.velocity.x > 0)
            {
                rb2.velocity = new Vector2(0, rb2.velocity.y);
            }
            rb2.AddForce(-transform.right * xSpeed);
        }
        else
        {
            rb2.velocity = new Vector2(rb2.velocity.x * 0.9f, rb2.velocity.y);
        }

        //Manage overshield depanding on ship movement
        if (rb2.velocity.x < 0.09 && rb2.velocity.x > -0.09 && rb2.velocity.y < 0.09 && rb2.velocity.y > -0.09)
        {
            shield.activateOvershield();
        }
        else
        {
            shield.deactivateOvershield();
        }

        //Reset x position
        if (xButtonState == xDirection.None && rb2.velocity.x < 0.09 && rb2.velocity.x > -0.09)
        {
            LeanTween.moveX(this.gameObject, 0, 1.5f);
        }

        //Reset rotation
        if(yButtonState == yDirection.None)
        {
           rotationCoroutine = StartCoroutine(battleshipRotation(0.000001f));
        }

        //Regulate max y velocity
        if(rb2.velocity.y > maxYVelocity)
        {
            rb2.velocity = new Vector3(rb2.velocity.x, maxYVelocity, 0);
        }
        if(rb2.velocity.y < -maxYVelocity)
        {
            rb2.velocity = new Vector3(rb2.velocity.x, -maxYVelocity, 0);
        }
    }

    IEnumerator battleshipRotation(float targetAngle)
    {
        while (transform.rotation.y != targetAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, targetAngle), 1f * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        yield return null;
    }

    //Animation when static
    public void StaticAnimation()
    {
        if(rb2.velocity == new Vector2(0,0))
        { 
            transform.position = transform.position + new Vector3(0, Mathf.Sin(Time.time * 1.0f) * 0.001f, 2);
        }
    }
     
    IEnumerator UWdeployment()
    {
        uwIsRunning = true;

        //Open gate
        gate.SetGateState(true);
        yield return new WaitForSeconds(1);

        //Deploy UW
        ultimateWeapon.SetUWState(true);
        yield return new WaitForSeconds(1);

        //Fire
        uwProjectile.setBeamState(1);
        uwProjectile.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        uwProjectile.setBeamState(2);
        yield return new WaitForSeconds(5);
        uwProjectile.setBeamState(3);
        yield return new WaitForSeconds(0.5f);
        uwProjectile.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        uwProjectile.setBeamState(0);

        //Redeply UW
        ultimateWeapon.SetUWState(false);
        yield return new WaitForSeconds(1);

        //Close gate
        gate.SetGateState(false);

        uwIsRunning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("EnnemyProjectile"))
        {
            takingDamages(collision.GetComponent<EnnemyProjectile>());
        }
    }

    public void chargeOverdrive(float ammount)
    {
        actualOverdrive += ammount;
        if (actualOverdrive > maxOverdrive)
        {
            actualOverdrive = maxOverdrive;
        }

        playerStats.updateOverdriveIndicator();
    }

    //Function called at the end of a battle phase to reset shield and cooldowns
    public void resetBattleship()
    {
        //Reset movemement
        rb2.velocity = new Vector2(0,0);

        //Reset shield and turrets's cooldowns values
        shield.shieldPoints = shield.maxShieldPoints;
        playerStats.updateShieldIndicators();

        for (int i = 0; i < standardRotationTurrets.Length; i++)
        {
            if(standardRotationTurrets[i] != null)
            {
                standardRotationTurrets[i].cooldownTimer = 0;
                standardRotationTurrets[i].actualAmmo = standardRotationTurrets[i].maxAmmo;
            }
        }
        for (int i = 0; i < standardFrontalTurrets.Length; i++)
        {
            if(standardFrontalTurrets[i] != null)
            {
                standardFrontalTurrets[i].cooldownTimer = 0;
                standardFrontalTurrets[i].actualAmmo = standardFrontalTurrets[i].maxAmmo;
            }
        }
        
    }
}
