using UnityEngine.UI;
using UnityEngine;

public class TurretSlot : CargoItemSlot
{
    public int cargoId; //id equals to the cargo slot's id, -1 if no turret equipped
    [Header("Associated objects turret slots")]
    public GameObject hidderEmpty;
    public GameObject upgradeSlot;
    public Text scrapPriceValue;
    public Text energyCorePriceValue;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = "Turret slot " + slotId;

        if(_item != null)
        {
            updateUpgradePrice((_item as TurretDescritpion).actualUpgradePriceScraps, (_item as TurretDescritpion).actualUpgradePriceEnergyCore);
        }
    }

    public void updateUpgradePrice(int newScrapPrice, int newEnergyCorePrice)
    {
        if(newScrapPrice != -1 && newEnergyCorePrice != -1)
        {
            upgradeSlot.SetActive(true);
            scrapPriceValue.text = "x " + newScrapPrice;
            energyCorePriceValue.text = "x " + newEnergyCorePrice;
        }
        else
        {
            upgradeSlot.SetActive(false);
        }
    }

    public void updateLevel(int newLevel)
    {

    }

    public void upgradeTurret()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        Debug.Log((_item as TurretDescritpion).actualTier < (_item as TurretDescritpion).maxTier);

        if((_item as TurretDescritpion) != null && 
            (_item as TurretDescritpion).actualTier < (_item as TurretDescritpion).maxTier &&
            playerStats.scrapsStored >= (_item as TurretDescritpion).actualUpgradePriceScraps && 
            playerStats.energyCoreStored >= (_item as TurretDescritpion).actualUpgradePriceEnergyCore)
        {
            Debug.Log("Turret upgraded");

            //Paiement
            playerStats.updateScrapStoredValue(playerStats.scrapsStored - (_item as TurretDescritpion).actualUpgradePriceScraps);
            playerStats.updateEnergyCoreStoredValue(playerStats.energyCoreStored - (_item as TurretDescritpion).actualUpgradePriceEnergyCore);

            //Update turret description in turret slot
            int newTier = (_item as TurretDescritpion).actualTier + 1;
            (_item as TurretDescritpion).actualTier++;
            (_item as TurretDescritpion).applySchemeCarac(newTier);
            updateItemInformationDisplay();

            //Apply update in cargo slot
            Cargo cargo = FindObjectOfType<Cargo>();
            cargo.itemSlots[cargoId].CargoItem = _item;
            cargo.itemSlots[cargoId].updateItemInformationDisplay();

            //Apply update on the turret itself
            Battleship battleship = FindObjectOfType<Battleship>();
            battleship.standardTurrets[slotId].descritpion = _item as TurretDescritpion;
            battleship.standardTurrets[slotId].updateTurretStats();

            //Update price for the next upgrade
            if ((_item as TurretDescritpion).actualTier < (_item as TurretDescritpion).maxTier)
            {
                updateUpgradePrice((_item as TurretDescritpion).actualUpgradePriceScraps, (_item as TurretDescritpion).actualUpgradePriceEnergyCore);
            }
            else
            {
                updateUpgradePrice(-1, -1);
            }
        }
        else
        {
            Debug.Log("Not enought minerals");
        }
    }
}
