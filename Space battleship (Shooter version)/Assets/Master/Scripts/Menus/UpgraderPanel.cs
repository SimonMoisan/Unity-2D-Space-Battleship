using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    //Upgrade functions
    public void changeStat(string statToUpgrade, int grade) //-1 : downgrade, 1 : upgrade
    {
        switch (statToUpgrade)
        {
            case "Damage":
                (upgradeSlot.CargoItem as TurretDescritpion).actualDamage += (upgradeSlot.CargoItem as TurretDescritpion).upgradeRateDamage;
                damageValue.text = "" + (upgradeSlot.CargoItem as TurretDescritpion).actualDamage;
                break;
            case "Firerate":
                (upgradeSlot.CargoItem as TurretDescritpion).actualFirerate += (upgradeSlot.CargoItem as TurretDescritpion).upgradeRateFirerate;
                firerateValue.text = "" + (upgradeSlot.CargoItem as TurretDescritpion).actualFirerate;
                break;
            case "Cooldown":
                (upgradeSlot.CargoItem as TurretDescritpion).actualCooldown += (upgradeSlot.CargoItem as TurretDescritpion).upgradeRateCooldown;
                cooldownValue.text = "" + (upgradeSlot.CargoItem as TurretDescritpion).actualCooldown;
                break;
            case "Speed":
                (upgradeSlot.CargoItem as TurretDescritpion).actualSpeed += (upgradeSlot.CargoItem as TurretDescritpion).upgradeRateSpeed;
                speedValue.text = "" + (upgradeSlot.CargoItem as TurretDescritpion).actualSpeed;
                break;
        }
    }
}
