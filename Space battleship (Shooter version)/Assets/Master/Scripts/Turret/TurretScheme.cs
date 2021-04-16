using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class TurretScheme : CargoItem
{
    public int overdriveCost;
    [Serializable]
    public class TurretTier
    {
        public int upgradeTier; // 2 to 10
        public int scrapPrice;
        public int energyCorePrice;

        public float upgradedDamageValue;
        public float upgradedHealthValue;
        public int upgradedShotPerSalveValue;
        public int upgradedNbrOfSalveValue;
        public int upgradedAmmoValue;
        public float upgradedFirerateValue;
        public float upgradedCooldownValue;
        public float upgradedSpeedValue;
        public float upgradedDeviationValue;
    }

    [Header("Turret Scheme caracteritics :")]
    public TurretType turretType;
    public TurretSize turretSize;
    public TurretAim turretAim;
    public ProjectileType projectileType;
    public ModifierPrimaryType modifierPrimaryType;
    public ModifierSecondaryType modifierSecondaryType;
    [Space]
    [Header("Turret upgrades tiers :")] //0: Damage, 1: Health, 2: NbrOfSalve, 3: Ammo, 4: Firerate, 5: Cooldown, 6: Speed, 7: Deviation 
    public int maxTier;
    public TurretTier[] turretTiersArray;
}


