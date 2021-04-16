using Unity.Collections;
using UnityEngine;
using System;

public enum TurretType { Offensive, Defensive, Support }
public enum TurretSize { Standard, Heavy }
public enum TurretAim { Rotate, Frontal }
public enum ProjectileType { Kinetic, Missile, Laser, Plasma, Ion, Shield }
public enum ModifierPrimaryType { Direct, Homming, Cluster, Dispersion, Beam }
public enum ModifierSecondaryType { None, Dispersion, Absorption, Bouncing, Chainer, ShieldPiercing, PlatePiercing, Piercing }

public enum SlotName { None, Cargo, Arsenal }

[CreateAssetMenu]
public class TurretDescription : TurretScheme
{
    [Header("When in cargo : ")]
    [ReadOnly] public int cargoId;      //-1 when not in cargo
    [Space]
    [Header("When in arsenal : ")]
    [ReadOnly] public int arsenalId;      //-1 when unequipped
    [ReadOnly] public SlotName slotName;  //Name of the slot where the turretDescription object is actually located, default : None
    public RotationTurret turretPrefab;
    [Space]
    //Turret stats
    [Header("Turret actual stats :")]
    public int priceToUnlock;
    public float actualDamage;
    public float actualHealth;
    public int actualShotsPerSalve;
    public int actualNbrOfSalve;
    public int actualAmmo;          //if 0, no ammunitions and no reload
    public float actualFirerate;
    public float actualCooldown;
    public float actualSpeed;
    public float actualDeviation;
    [Space]
    //Turret upgrade rates
    public int actualTier;
    public int actualUpgradePriceScraps;
    public int actualUpgradePriceEnergyCore;
    [Header("Associated objects :")]
    private Battleship battleship;
    public TurretScheme schemeOrigin;

    private void OnValidate()
    {
        applySchemeCarac(1);
    }

    public void applySchemeCarac(int tier)
    {
        //Turret Scheme caracteritics
        turretType = schemeOrigin.turretType;
        turretSize = schemeOrigin.turretSize;
        projectileType = schemeOrigin.projectileType;
        modifierPrimaryType = schemeOrigin.modifierPrimaryType;
        modifierSecondaryType = schemeOrigin.modifierSecondaryType;

        actualTier = tier;
        maxTier = schemeOrigin.maxTier;

        //Set price to upgrade to next tier
        if(tier < schemeOrigin.maxTier)
        {
            actualUpgradePriceScraps = schemeOrigin.turretTiersArray[tier].scrapPrice;
            actualUpgradePriceEnergyCore = schemeOrigin.turretTiersArray[tier].energyCorePrice; ;
        }
        else //Can't be upgrade anymore
        {
            actualUpgradePriceScraps = -1;
            actualUpgradePriceEnergyCore = -1;
        }

        //Set turretDescription carac according to its origin scheme and its tier
        if(tier <= schemeOrigin.maxTier && tier <= schemeOrigin.turretTiersArray.Length)
        {
            actualDamage = schemeOrigin.turretTiersArray[tier-1].upgradedDamageValue;
            actualHealth = schemeOrigin.turretTiersArray[tier-1].upgradedHealthValue;
            actualShotsPerSalve = schemeOrigin.turretTiersArray[tier-1].upgradedShotPerSalveValue;
            actualNbrOfSalve = schemeOrigin.turretTiersArray[tier-1].upgradedNbrOfSalveValue;
            actualAmmo = schemeOrigin.turretTiersArray[tier-1].upgradedAmmoValue;
            actualFirerate = schemeOrigin.turretTiersArray[tier-1].upgradedFirerateValue;
            actualCooldown = schemeOrigin.turretTiersArray[tier-1].upgradedCooldownValue;
            actualSpeed = schemeOrigin.turretTiersArray[tier-1].upgradedSpeedValue;
            actualDeviation = schemeOrigin.turretTiersArray[tier-1].upgradedDeviationValue;
        }

        overdriveCost = schemeOrigin.overdriveCost;
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
        RotationTurret turretGO = Instantiate(turretPrefab, battleship.rotationStandardTurretPositions[id], false);
        turretGO.transform.parent = battleship.rotationStandardTurretPositions[id];

        //Initiate turret
        turretGO.idTurret = arsenalId;
        turretGO.turretButtonId = turretGO.idTurret % 4;
        if (turretGO.GetComponent<RotationTurret>() != null)
        {
            turretGO.GetComponent<RotationTurret>().viseur = battleship.viseurs[arsenalId];
        }

        //Initiate turret in battlsehip object
        if(turretGO.GetComponent<RotationTurret>() != null)
        {
            battleship.standardRotationTurrets[id] = turretGO.GetComponent<RotationTurret>();
        }
        else if(turretGO.GetComponent<FrontalTurret>() != null)
        {
            battleship.standardFrontalTurrets[id] = turretGO.GetComponent<FrontalTurret>();
        }

        //Set image turret hud
        PlayerStats.current.turretsImages[arsenalId].sprite = cargoIcone;
        PlayerStats.current.turretsImages[arsenalId].rectTransform.localScale = new Vector3(0.8f, 0.8f, 1);
        PlayerStats.current.turretsImages[arsenalId].SetNativeSize();
    }

    //Function used to unequip turret from battleship object
    public void unequipTurret(MenuManagerScript menuManagerScript, int id)
    {
        arsenalId = -1;
        cargoId = id;
        slotName = SlotName.Cargo;

        battleship = FindObjectOfType<Battleship>();

        //Find the turret gameObject in battleship's arsenal and delete it
        if (battleship.standardRotationTurrets[id] != null)
        {
            GameObject turetGO = battleship.standardRotationTurrets[id].gameObject;
            Destroy(turetGO);
        }
    }
}
