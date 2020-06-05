using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TurretScheme : CargoItem
{
    [Header("Turret Scheme caracteritics :")]
    public TurretType turretType;
    public TurretSize turretSize;
    public ProjectileType projectileType;
    public ModifierPrimaryType modifierPrimaryType;
    public ModifierSecondaryType modifierSecondaryType;
    [Space]
    [Header("Turret upgrades stats values :")]
    public int[] upgradeRateIndexes; //0: Damage, 1: Health, 2: NbrOfSalve, 3: Ammo, 4: Firerate, 5: Cooldown, 6: Speed, 7: Precision 
    public int[] valuesDamage;
    public int[] valuesHealth;
    public int[] valuesNbrOfSalve;
    public int[] valuesAmmo;
    public float[] valuesFirerate;
    public float[] valuesCooldown;
    public int[] valuesSpeed;
    public int[] valuesPrecision;
    [Space]
    //Turret starting stats (stats by default on this turret)
    [Header("Turret starting stats")]
    public int startingDamage;
    public int startingHealth;
    public int startingShotsPerSalve;
    public int startingNbrOfSalve;
    public int startingAmmo;
    public float startingFirerate;
    public float startingCooldown;
    public int startingSpeed;
    public int startingPrecision;
}
