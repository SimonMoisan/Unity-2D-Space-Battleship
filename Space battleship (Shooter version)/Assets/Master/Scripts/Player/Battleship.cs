using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Battleship : MonoBehaviour
{
    [Header("Battleship caracteristics : ")]
    //Battleship caracteristics
    public float MaxHullPoints;
    public float MaxPlatePoints;
    [ReadOnly] public float hullPoints;
    [ReadOnly] public float platePoints;
    [ReadOnly] public bool hasTakenDamagesRecently;
    [Space]
    [Header("UW weapons informations : ")]
    [ReadOnly] public bool uwIsRunning = false;
    public float uwChargeMax; //Charge to reach to use UW
    [ReadOnly] public float uwCharge; //Start to 0, need to reach the limit to be used
    [Space]
    [Header("Movement parameters : ")]
    //Parameters used for movement
    private Transform initialPosition;       // Start position for every battl phases
    [ReadOnly] public float velocity;        // Current Travelling Velocity
    public float maxVelocity;     // Maxima Velocity
    public float acc;             // Current Acceleration
    public float accSpeed;        // Amount to increase Acceleration with.
    public float maxAcc;          // Max Acceleration
    public float minAcc;          // Min Acceleration
    public float accRate;         // Acceleration and Decceleration rate
    [ReadOnly] public string actualDirection;     //Indicate in which direction the ship is actually going
    [ReadOnly] public float minY;                 // Minimum height the battleship can reach
    [ReadOnly] public float maxY;                 // Maximum height the battleship can reach
    public float defaultX;             // Default X position
    public float maxX;                 // Maximum X position the ship can reach when he decelerate

    //Coroutines
    Coroutine accelerationCoroutine;             //Acceleration coroutine
    Coroutine deccelerationCoroutine;            //Decceleration coroutine

    [Space]
    [Header("Associated objects : ")]
    //Associated objects
    public PlayerStats stats;
    [Header("Turrets parents : ")]
    public Transform arsenalStandardTurret;
    public Transform arsenalFrontalTurret;
    [Header("Turrets : ")]
    public Turret[] standardTurrets;    //Array containing turrets gameobjects
    public Turret[] spinalTurrets;      //Spinal turrets are special turrets with unique effects
    [Header("Standard turrets associated objects : ")]
    public Viseur[] viseurs;
    public Transform[] turretPositions; //Positions where turrets will be created
    [Header("Ultimate weapons associated objects : ")]
    public Gate gate;
    public UltimateWeapon ultimateWeapon;
    public UWProjectile uwProjectile;
    public Shield shield;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform;
        stats = FindObjectOfType<PlayerStats>();
        animator = gameObject.GetComponent<Animator>();
        hullPoints = MaxHullPoints;
        platePoints = MaxPlatePoints;
        hasTakenDamagesRecently = false;
        standardTurrets = arsenalStandardTurret.GetComponentsInChildren<Turret>(); //Find turret already installed on battlship
        spinalTurrets = arsenalFrontalTurret.GetComponentsInChildren<Turret>();
        viseurs = FindObjectsOfType<Viseur>();

        //Attribute turret id and viseur
        if(standardTurrets.Length > 0) 
        {
            for (int i = 0; i < standardTurrets.Length; i++)
            {
                standardTurrets[i].idTurret = i;
                standardTurrets[i].viseur = viseurs[i];
            }
        }
        else
        {
            standardTurrets = new Turret[turretPositions.Length];
        }
    }

    // Update is called once per frame
    void Update()
    {
        NewMoveBattleshipAcc();
        StaticAnimation();
        UWdeployer();

        Vector3 shipPos = new Vector3(transform.position.x, transform.position.y, 2);
        shipPos.y = Mathf.Clamp(shipPos.y, minY, maxY);
        transform.position = shipPos;
    }

    //Function when the player wants to use the UW device
    public void UWdeployer()
    {
        if (Input.GetKeyDown("u") && !uwIsRunning)
        {
            StartCoroutine(UWdeployment());
        }
    }

    //Fonction qui gère les dégats subis par le vaisseau
    public void TakingDamages(float damageInput)
    {
        //Add shield regeneration cooldown if the battleship taken damages
        hasTakenDamagesRecently = true;
        shield.cooldownTimer += 0.5f;

        if (platePoints > 0 && shield.shieldPoints <= 0)
        {
            platePoints -= damageInput;
        }
        else if(platePoints <= 0 && shield.shieldPoints <= 0)
        {
            hullPoints -= damageInput;
        }
    }

    //Function for battleship movement
    public void NewMoveBattleshipAcc()
    {
        ////////////////////////////////////////////////////////////////////////////
        //Upward movement
        if (Input.GetKeyDown("z") && actualDirection != "DOWN")
        {
            actualDirection = "UP";
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("UP", maxAcc, minAcc, maxVelocity, accSpeed));
            if(deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
        }
        if (Input.GetKeyUp("z") && accelerationCoroutine != null)
        {
            StopCoroutine(accelerationCoroutine);
            acc = 0;
            deccelerationCoroutine = StartCoroutine(DeccelerateContinuously("UP", maxAcc, minAcc, maxVelocity, accSpeed));
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        //Downward movement
        if (Input.GetKeyDown("s") && actualDirection != "UP")
        {
            actualDirection = "DOWN";
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("DOWN", maxAcc, minAcc, maxVelocity, accSpeed));
            if (deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
        }
        if (Input.GetKeyUp("s") && accelerationCoroutine != null)
        {
            StopCoroutine(accelerationCoroutine);
            acc = 0;
            deccelerationCoroutine = StartCoroutine(DeccelerateContinuously("DOWN",maxAcc ,minAcc, maxVelocity ,accSpeed));
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        //Gère le déplacement en cas d'interruption de décelération
        if (Input.GetKeyDown("z") && actualDirection == "DOWN")
        {
            actualDirection = "UP";
            acc = 0;
            StopCoroutine(accelerationCoroutine); 
            if (deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("UP", maxAcc, minAcc, maxVelocity, accSpeed));
        }
        if (Input.GetKeyDown("s") && actualDirection == "UP")
        {
            actualDirection = "DOWN";
            acc = 0;
            StopCoroutine(accelerationCoroutine);
            if (deccelerationCoroutine != null)
            {
                StopCoroutine(deccelerationCoroutine);
            }
            accelerationCoroutine = StartCoroutine(AccelerateContinuously("DOWN", maxAcc, minAcc, maxVelocity, accSpeed));
        }
        ////////////////////////////////////////////////////////////////////////////
    }

    //Animation when static
    public void StaticAnimation()
    {
        if(actualDirection == "NONE")
        { 
            transform.position = transform.position + new Vector3(0, Mathf.Sin(Time.time * 2.0f) * 0.002f, 2);
        }
    }

    IEnumerator AccelerateContinuously(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine d'accelleration du vaisseau
    {
        while (true)
        {
            if(direction == "UP")
            {
                acc += accSpeed;
            }
            else if (direction == "DOWN")
            {
                acc -= accSpeed;
            }

            if (acc > maxAcc)
            {
                acc = maxAcc;
            }
            else if (acc < minAcc)
            {
                acc = minAcc;
            }

            velocity += acc;

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
        }
    }

    IEnumerator DeccelerateContinuously(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine de déccelleration du vaisseau
    {
        while (velocity != 0f)
        {
            if(direction == "UP") //si la direction initiale est UP on réduit la velocity jusqu'à atteindre 0 à coup sur
            {
                velocity -= 0.50f;
                if(velocity < 0) //si on a trop diminué la velocity on remet à 0
                {
                    velocity = 0;
                }
            }
            else if (direction == "DOWN") //si la direction initiale est DOWN on augmente la velocity jusqu'à atteindre 0 à coup sur
            {
                velocity += 0.50f;
                if (velocity > 0) //si on a trop augmenté la velocity on remet à 0
                {
                    velocity = 0;
                }
            }

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
        }
        actualDirection = "NONE";
    }

    IEnumerator MovingBackward(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine de ralentissement du vaisseau ce qu'il lui permet de reculer
    {
        while (true)
        {
            acc += accSpeed;

            if (acc > maxAcc)
            {
                acc = maxAcc;
            }
            else if (acc < minAcc)
            {
                acc = minAcc;
            }

            velocity += acc;

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
        }
    }
     
    IEnumerator BackToInitialPosition(string direction, float maxAcc, float minAcc, float maxVelocity, float accSpeed) //Coroutine où le vaisseau regagne ça position X initiale
    {
        while (true)
        {
            if (direction == "UP")
            {
                acc += accSpeed;
            }
            else if (direction == "DOWN")
            {
                acc -= accSpeed;
            }

            if (acc > maxAcc)
            {
                acc = maxAcc;
            }
            else if (acc < minAcc)
            {
                acc = minAcc;
            }

            velocity += acc;

            if (velocity > maxVelocity)
            {
                velocity = maxVelocity;
            }
            else if (velocity < -maxVelocity)
            {
                velocity = -maxVelocity;
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, velocity);
            yield return new WaitForSeconds(accRate);
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
            TakingDamages(collision.GetComponent<EnnemyProjectile>().damage);
        }
    }

    public void chargeUW(float ammount)
    {
        uwCharge += ammount;
        if (uwCharge > uwChargeMax)
        {
            uwCharge = uwChargeMax;
        }
    }

    //Function called at the end of a battle phase to reset shield and cooldowns
    public void reset()
    {
        Debug.Log("Reset");

        shield.shieldPoints = shield.maxShieldPoints;
        for (int i = 0; i < standardTurrets.Length; i++)
        {
            if(standardTurrets[i] != null)
            {
                standardTurrets[i].cooldownTimer = 0;
                standardTurrets[i].actualAmmo = standardTurrets[i].maxAmmo;
            }
        }
        for (int i = 0; i < spinalTurrets.Length; i++)
        {
            if(spinalTurrets[i] != null)
            {
                spinalTurrets[i].cooldownTimer = 0;
                spinalTurrets[i].actualAmmo = spinalTurrets[i].maxAmmo;
            }
        }
        
    }
}
