using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum upgradableStats { Damage, Health, NbrShots, Firerate, Cooldown, Speed, Precision, Ammunition}

public class UpgraderPanel : MonoBehaviour
{
    [Header("Associated objects :")]
    public Cargo cargo;
    [ReadOnly] public UpgraderSlot upgradeSlot;
    [Space]
    [Header("Values to display :")]
    //Values to display
    public Sprite turretImage;
    public Text turretName;
    public Text damageValue;
    public Text healthValue;
    public Text shotBySalveValue; //Format : shotPerSalve x nbrOfShot 
    public Text firerateValue;
    public Text cooldownValue;
    public Text speedValue;
    public Text ammoValue;
    [Space]
    [Header("Informations to display :")]
    //Informations to display
    public Text turretType;
    public Text projectileType;
    public Text modifierType;   

    private void OnValidate()
    {
        upgradeSlot = GetComponentInChildren<UpgraderSlot>();
    }

    private void Start()
    {
        cargo = FindObjectOfType<Cargo>();
    }

    public void addTurret(TurretDescritpion turret)
    {
        upgradeSlot.CargoItem = turret;
    }

    public void upgradeStat(string statToUpgrade)
    {
        changeStat(statToUpgrade, 1);
    }

    public void downgradeStat(string statToDowngrade)
    {
        changeStat(statToDowngrade, -1);
    }

    //Upgrade functions
    public void changeStat(string statToModify, int grade) //-1 : downgrade, 1 : upgrade
    {
       
        if (upgradeSlot.CargoItem != null)
        {
            TurretDescritpion turretD = (upgradeSlot.CargoItem as TurretDescritpion);
            switch (statToModify)
            {
                case "Damage" :
                    if(turretD.upgradeRateIndexes[0] < turretD.valuesDamage.Length -1 && turretD.upgradeRateIndexes[0] > 0)
                    {
                        turretD.upgradeRateIndexes[0] += 1 * grade;
                        turretD.actualDamage = turretD.valuesDamage[turretD.upgradeRateIndexes[0]];
                        damageValue.text = "" + turretD.actualDamage;
                    }
                    break;
                case "Health":
                    if (turretD.upgradeRateIndexes[1] < turretD.valuesHealth.Length - 1 && turretD.upgradeRateIndexes[1] > 0)
                    {
                        turretD.upgradeRateIndexes[1] += 1 * grade;
                        turretD.actualHealth = turretD.valuesHealth[turretD.upgradeRateIndexes[1]];
                        healthValue.text = "" + turretD.actualHealth;
                    }
                    break;
                case "NbrSalves" :
                    if (turretD.upgradeRateIndexes[2] < turretD.valuesDamage.Length - 1 && turretD.upgradeRateIndexes[2] > 0)
                    {
                        turretD.upgradeRateIndexes[2] += 1 * grade;
                        turretD.actualNbrOfSalve = turretD.valuesNbrOfSalve[turretD.upgradeRateIndexes[2]];
                        shotBySalveValue.text = "" + turretD.actualNbrOfSalve;
                    }
                    break;
                case "Ammunition":
                    if (turretD.upgradeRateIndexes[3] < turretD.valuesDamage.Length - 1 && turretD.upgradeRateIndexes[3] > 0)
                    {
                        turretD.upgradeRateIndexes[3] += 1 * grade;
                        turretD.actualAmmo = turretD.valuesAmmo[turretD.upgradeRateIndexes[3]];
                        ammoValue.text = "" + turretD.actualAmmo;
                    }
                    break;
                case "Firerate":
                    if (turretD.upgradeRateIndexes[4] < turretD.valuesDamage.Length - 1 && turretD.upgradeRateIndexes[4] > 0)
                    {
                        turretD.upgradeRateIndexes[4] += 1 * grade;
                        turretD.actualFirerate = turretD.valuesFirerate[turretD.upgradeRateIndexes[4]];
                        firerateValue.text = "" + turretD.actualFirerate;
                    }
                    break;
                case "Cooldown":
                    if (turretD.upgradeRateIndexes[5] < turretD.valuesDamage.Length - 1 && turretD.upgradeRateIndexes[5] > 0)
                    {
                        turretD.upgradeRateIndexes[5] += 1 * grade;
                        turretD.actualCooldown = turretD.valuesCooldown[turretD.upgradeRateIndexes[5]];
                        cooldownValue.text = "" + turretD.actualCooldown;
                    }
                    break;
                case "Speed":
                    if (turretD.upgradeRateIndexes[6] < turretD.valuesDamage.Length - 1 && turretD.upgradeRateIndexes[6] > 0)
                    {
                        turretD.upgradeRateIndexes[6] += 1 * grade;
                        turretD.actualSpeed = turretD.valuesSpeed[turretD.upgradeRateIndexes[6]];
                        speedValue.text = "" + turretD.actualSpeed;
                    }
                    break;
                case "Precision":
                    if (turretD.upgradeRateIndexes[7] < turretD.valuesDamage.Length - 1 && turretD.upgradeRateIndexes[7] > 0)
                    {
                        turretD.upgradeRateIndexes[7] += 1 * grade;
                        turretD.actualPrecision = turretD.valuesPrecision[turretD.upgradeRateIndexes[7]];
                        speedValue.text = "" + turretD.actualSpeed;
                    }
                    break;
            }
        }
    }
}
