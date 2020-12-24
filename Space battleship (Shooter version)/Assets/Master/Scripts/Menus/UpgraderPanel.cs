using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum upgradableStats { Damage, Health, NbrShots, Firerate, Cooldown, Speed, Deviation, Ammunition}

public class UpgraderPanel : MonoBehaviour
{
    /*[Header("Associated objects :")]
    public Cargo cargo;
    public PlayerStats playerStats;
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
        playerStats = FindObjectOfType<PlayerStats>();
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
                    //Check if the player have enought scrap to upgrad
                    if (grade == 1 && turretD.upgradeRateIndexes[0] < turretD.valuesDamage.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesDamage.scrapPrice[turretD.upgradeRateIndexes[0]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[0] += 1 * grade;
                        turretD.actualDamage = turretD.valuesDamage.values[turretD.upgradeRateIndexes[0]];
                        playerStats.scrapsStored -= turretD.valuesDamage.scrapPrice[turretD.upgradeRateIndexes[0]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[0] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[0] += 1 * grade;
                        turretD.actualDamage = turretD.valuesDamage.values[turretD.upgradeRateIndexes[0]];
                        playerStats.scrapsStored += turretD.valuesDamage.scrapPrice[turretD.upgradeRateIndexes[0]];
                    }
                    
                    damageValue.text = "" + turretD.actualDamage;
                    break;
                case "Health":
                    if (grade == 1 && turretD.upgradeRateIndexes[1] < turretD.valuesHealth.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesHealth.scrapPrice[turretD.upgradeRateIndexes[1]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[1] += 1 * grade;
                        turretD.actualHealth = turretD.valuesHealth.values[turretD.upgradeRateIndexes[1]];
                        playerStats.scrapsStored -= turretD.valuesHealth.scrapPrice[turretD.upgradeRateIndexes[1]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[0] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[1] += 1 * grade;
                        turretD.actualHealth = turretD.valuesHealth.values[turretD.upgradeRateIndexes[1]];
                        playerStats.scrapsStored += turretD.valuesHealth.scrapPrice[turretD.upgradeRateIndexes[1]];
                    }
                    break;
                case "NbrSalves" :
                    if (grade == 1 && turretD.upgradeRateIndexes[2] < turretD.valuesNbrOfSalve.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesNbrOfSalve.scrapPrice[turretD.upgradeRateIndexes[2]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[2] += 1 * grade;
                        turretD.actualNbrOfSalve = turretD.valuesNbrOfSalve.values[turretD.upgradeRateIndexes[2]];
                        playerStats.scrapsStored -= turretD.valuesNbrOfSalve.scrapPrice[turretD.upgradeRateIndexes[2]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[2] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[2] += 1 * grade;
                        turretD.actualNbrOfSalve = turretD.valuesNbrOfSalve.values[turretD.upgradeRateIndexes[2]];
                        playerStats.scrapsStored += turretD.valuesNbrOfSalve.scrapPrice[turretD.upgradeRateIndexes[2]];
                    }
                    break;
                case "Ammunition":
                    if (grade == 1 && turretD.upgradeRateIndexes[3] < turretD.valuesAmmo.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesAmmo.scrapPrice[turretD.upgradeRateIndexes[3]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[3] += 1 * grade;
                        turretD.actualAmmo = turretD.valuesAmmo.values[turretD.upgradeRateIndexes[3]];
                        playerStats.scrapsStored -= turretD.valuesAmmo.scrapPrice[turretD.upgradeRateIndexes[3]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[3] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[3] += 1 * grade;
                        turretD.actualAmmo = turretD.valuesAmmo.values[turretD.upgradeRateIndexes[3]];
                        playerStats.scrapsStored += turretD.valuesAmmo.scrapPrice[turretD.upgradeRateIndexes[3]];
                    }
                    break;
                case "Firerate":
                    if (grade == 1 && turretD.upgradeRateIndexes[4] < turretD.valuesFirerate.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesFirerate.scrapPrice[turretD.upgradeRateIndexes[4]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[4] += 1 * grade;
                        turretD.actualFirerate = turretD.valuesFirerate.values[turretD.upgradeRateIndexes[4]];
                        playerStats.scrapsStored -= turretD.valuesFirerate.scrapPrice[turretD.upgradeRateIndexes[4]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[4] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[4] += 1 * grade;
                        turretD.actualFirerate = turretD.valuesFirerate.values[turretD.upgradeRateIndexes[4]];
                        playerStats.scrapsStored += turretD.valuesFirerate.scrapPrice[turretD.upgradeRateIndexes[4]];
                    }
                    break;
                case "Cooldown":
                    if (grade == 1 && turretD.upgradeRateIndexes[5] < turretD.valuesCooldown.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesCooldown.scrapPrice[turretD.upgradeRateIndexes[5]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[5] += 1 * grade;
                        turretD.actualCooldown = turretD.valuesCooldown.values[turretD.upgradeRateIndexes[5]];
                        playerStats.scrapsStored -= turretD.valuesCooldown.scrapPrice[turretD.upgradeRateIndexes[5]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[5] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[5] += 1 * grade;
                        turretD.actualCooldown = turretD.valuesCooldown.values[turretD.upgradeRateIndexes[5]];
                        playerStats.scrapsStored += turretD.valuesCooldown.scrapPrice[turretD.upgradeRateIndexes[5]];
                    }
                    break;
                case "Speed":
                    if (grade == 1 && turretD.upgradeRateIndexes[6] < turretD.valuesSpeed.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesSpeed.scrapPrice[turretD.upgradeRateIndexes[6]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[6] += 1 * grade;
                        turretD.actualSpeed = turretD.valuesSpeed.values[turretD.upgradeRateIndexes[6]];
                        playerStats.scrapsStored -= turretD.valuesSpeed.scrapPrice[turretD.upgradeRateIndexes[6]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[6] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[6] += 1 * grade;
                        turretD.actualSpeed = turretD.valuesSpeed.values[turretD.upgradeRateIndexes[6]];
                        playerStats.scrapsStored += turretD.valuesSpeed.scrapPrice[turretD.upgradeRateIndexes[6]];
                    }
                    break;
                case "Deviation":
                    if (grade == 1 && turretD.upgradeRateIndexes[7] < turretD.valuesDeviation.values.Length - 1 && playerStats.scrapsStored >= turretD.valuesDeviation.scrapPrice[turretD.upgradeRateIndexes[7]]) //Upgrade
                    {
                        turretD.upgradeRateIndexes[7] += 1 * grade;
                        turretD.actualDeviation = turretD.valuesDeviation.values[turretD.upgradeRateIndexes[7]];
                        playerStats.scrapsStored -= turretD.valuesDeviation.scrapPrice[turretD.upgradeRateIndexes[7]];
                    }
                    else if (grade == -1 && turretD.upgradeRateIndexes[7] > 0)//Downgrade
                    {
                        turretD.upgradeRateIndexes[0] += 1 * grade;
                        turretD.actualDeviation = turretD.valuesDeviation.values[turretD.upgradeRateIndexes[7]];
                        playerStats.scrapsStored += turretD.valuesDeviation.scrapPrice[turretD.upgradeRateIndexes[7]];
                    }
                    break;
            }
        }
    }*/
}
