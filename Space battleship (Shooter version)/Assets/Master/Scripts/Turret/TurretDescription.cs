using Unity.Collections;
using UnityEngine;

public enum TurretType { Offensive, Defensive, Support }
public enum TurretSize { Standard, Heavy, Frontal }
public enum ProjectileType { Kinetic, Missile, Laser, Plasma, Ion, Shield }
public enum ModifierPrimaryType { Direct, Homming, Cluster, Dispersion, Beam }
public enum ModifierSecondaryType { None, Dispersion, Absorption, Bouncing, Chainer, ShieldPiercing, PlatePiercing, Piercing }

public enum SlotName { None, Cargo, Arsenal }

public class TurretDescritpion : TurretScheme
{
    [Header("When in cargo : ")]
    [ReadOnly] public int cargoId;      //-1 when not in cargo
    [Space]
    [Header("When in arsenal : ")]
    [ReadOnly] public int arsenalId;      //-1 when unequipped
    [ReadOnly] public SlotName slotName;  //Name of the slot where the turretDescription object is actually located, default : None
    public Turret turretPrefab;
    [Space]
    //Turret stats
    [Header("Turret actual stats :")]
    public int actualDamage;
    public int actualHealth;
    public int actualShotsPerSalve;
    public int actualNbrOfSalve;
    public int actualAmmo;          //if 0, no ammunitions and no reload
    public float actualFirerate;
    public float actualCooldown;
    public int actualSpeed;
    public int actualPrecision;
    [Space]
    //Turret upgrade rates
    [Header("Associated objects :")]
    private Battleship battleship;
    public TurretScheme schemeOrigin;

    private void OnValidate()
    {
        /*
        //Turret Scheme caracteritics
        turretType = schemeOrigin.turretType;
        turretSize = schemeOrigin.turretSize;
        projectileType = schemeOrigin.projectileType;
        modifierPrimaryType = schemeOrigin.modifierPrimaryType;
        modifierSecondaryType = schemeOrigin.modifierSecondaryType;
        //Turret upgrades stats values
        upgradeRateIndexes = schemeOrigin.upgradeRateIndexes; //0: Damage, 1: Health, 2: NbrOfSalve, 3: Ammo, 4: Firerate, 5: Cooldown, 6: Speed, 7: Precision 
        valuesDamage = schemeOrigin.valuesDamage;
        valuesHealth = schemeOrigin.valuesHealth;
        valuesNbrOfSalve = schemeOrigin.valuesNbrOfSalve;
        valuesAmmo = schemeOrigin.valuesAmmo;
        valuesFirerate = schemeOrigin.valuesFirerate;
        valuesCooldown = schemeOrigin.valuesCooldown;
        valuesSpeed = schemeOrigin.valuesSpeed;
        valuesPrecision = schemeOrigin.valuesPrecision;
        //Turret starting stats
        startingDamage = schemeOrigin.startingDamage;
        startingHealth = schemeOrigin.startingHealth;
        startingShotsPerSalve = schemeOrigin.startingShotsPerSalve;
        startingNbrOfSalve = schemeOrigin.startingNbrOfSalve;
        startingAmmo = schemeOrigin.startingAmmo;
        startingFirerate = schemeOrigin.startingAmmo;
        startingCooldown = schemeOrigin.startingCooldown;
        startingSpeed = schemeOrigin.startingSpeed;
        startingPrecision = schemeOrigin.startingPrecision;*/
    }

    //Function used to equip turret to battleship object
    public void equipTurret(MenuManagerScript menuManagerScript, int id)
    {
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
