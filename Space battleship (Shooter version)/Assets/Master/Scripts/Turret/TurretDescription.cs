using Unity.Collections;
using UnityEngine;

public enum TurretType { Offensive, Defensive, Support }
public enum TurretSize { Standard, Heavy, Frontal }
public enum ProjectileType { Kinetic, Missile, Laser, Plasma, Ion, Shield }
public enum ModifierPrimaryType { Direct, Homming, Cluster, Dispersion, Beam }
public enum ModifierSecondaryType { None, Dispersion, Absorption, Bouncing, Chainer, ShieldPiercing, PlatePiercing, Piercing }

public enum SlotName { None, Cargo, Arsenal }

[CreateAssetMenu]
public class TurretDescritpion : CargoItem
{
    public TurretType turretType;
    public TurretSize turretSize;
    public ProjectileType projectileType;   
    public ModifierPrimaryType modifierPrimaryType;
    public ModifierSecondaryType modifierSecondaryType;
    [Space]
    [Header("When in cargo : ")]
    [ReadOnly] public int cargoId;      //-1 when not in cargo
    [Space]
    [Header("When in arsenal : ")]
    [ReadOnly] public int arsenalId;      //-1 when unequipped
    [ReadOnly] public SlotName slotName;  //Name of the slot where the turretDescription object is actually located, default : None
    public Turret turretPrefab;
    [Space]
    //Turret stats
    [Header("Turret actual stats")]
    public int actualDamage;
    public int actualProjectileHealth;
    public int actualShotsPerSalve;
    public int actualNbrOfSalve;
    public int actualAmmo;          //if 0, no ammunitions and reload
    public float actualFirerate;
    public float actualCooldown;
    public int actualSpeed;
    public int actualPrecisionFactor;
    [Space]
    //Turret max stats
    [Header("Turret min stats")]
    public int minDamage;
    public int minProjectileHealth;
    public int minNbrOfSalve;
    public int minAmmo;
    public float minFirerate;
    public float minCooldown;
    public int minSpeed;
    public int minPrecisionFactor;
    [Space]
    //Turret min stats
    [Header("Turret max stats")]
    public int maxDamage;
    public int maxProjectileHealth;
    public int maxNbrOfSalve;
    public int maxAmmo;
    public float maxFirerate;
    public float maxCooldown;
    public int maxSpeed;
    public int maxPrecisionFactor;
    [Space]
    //Turret upgrade rates
    [Header("Turret upgrade rate")]
    public int upgradeRateDamage;
    public int upgradeRateNbrOfSalve;
    public int upgradeRateAmmo;
    public float upgradeRateFirerate;
    public float upgradeRateCooldown;
    public int upgradeRateSpeed;
    public int upgradeRatePrecisionFactor;
    public int upgradeRateProjectileHealth;
    [Space]
    //Turret starting stats (stats by default on this turret)
    [Header("Turret starting stats")]
    public int startingDamage;
    public int startingProjectileHealth;
    public int startingShotsPerSalve;
    public int startingNbrOfSalve;
    public int startingRateAmmo;
    public float startingFirerate;
    public float startingCooldown;
    public int startingSpeed;
    public int startingPrecisionFactor;

    private Battleship battleship;

    //Function used to equip turret to battleship object
    public void equipTurret(MenuManagerScript menuManagerScript, int id)
    {
        Debug.Log("Equip : " + id);
        arsenalId = id;
        cargoId = -1;
        slotName = SlotName.Arsenal;

        battleship = FindObjectOfType<Battleship>();

        //Instantiate turret GO and its position
        Quaternion turretOrientation = new Quaternion(0, 0, -90, 0);
        Turret turretGO = Instantiate(turretPrefab, battleship.turretPositions[id], false);
        turretGO.transform.parent = battleship.turretPositions[id];

        //Initiate turret
        turretGO.idTurret = arsenalId;
        turretGO.viseur = battleship.viseurs[arsenalId];

        //Initiate turret in battlsehip object
        battleship.standardTurrets[id] = turretGO;
    }

    //Function used to unequip turret from battleship object
    public void unequipTurret(MenuManagerScript menuManagerScript, int id)
    {
        Debug.Log("Unequip : "+ id);
        arsenalId = -1;
        cargoId = id;
        slotName = SlotName.Cargo;

        battleship = FindObjectOfType<Battleship>();

        //Find the turret gameObject in battleship's arsenal and delete it
        if (battleship.standardTurrets[id] != null)
        {
            GameObject turetGO = battleship.standardTurrets[id].gameObject;
            Destroy(turetGO);
        }
    }
}
