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
            updateUpgradePrice((_item as TurretDescription).actualUpgradePriceScraps, (_item as TurretDescription).actualUpgradePriceEnergyCore);
        }

        turretImage.SetNativeSize();
    }

    public void Start()
    {
        turretImage.SetNativeSize();
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

        Debug.Log((_item as TurretDescription).actualTier < (_item as TurretDescription).maxTier);

        if((_item as TurretDescription) != null && 
            (_item as TurretDescription).actualTier < (_item as TurretDescription).maxTier &&
            playerStats.scrapsStored >= (_item as TurretDescription).actualUpgradePriceScraps && 
            playerStats.energyCoreStored >= (_item as TurretDescription).actualUpgradePriceEnergyCore)
        {
            Debug.Log("Turret upgraded");

            //Paiement
            playerStats.updateScrapStoredValue(playerStats.scrapsStored - (_item as TurretDescription).actualUpgradePriceScraps);
            playerStats.updateEnergyCoreStoredValue(playerStats.energyCoreStored - (_item as TurretDescription).actualUpgradePriceEnergyCore);

            //Update turret description in turret slot
            int newTier = (_item as TurretDescription).actualTier + 1;
            (_item as TurretDescription).actualTier++;
            (_item as TurretDescription).applySchemeCarac(newTier);
            updateItemInformationDisplay();

            //Apply update in cargo slot
            Cargo cargo = FindObjectOfType<Cargo>();
            cargo.itemSlots[cargoId].CargoItem = _item;
            cargo.itemSlots[cargoId].updateItemInformationDisplay();

            //Apply update on the turret itself
            Battleship battleship = FindObjectOfType<Battleship>();
            battleship.standardRotationTurrets[slotId].descritpion = _item as TurretDescription;
            battleship.standardRotationTurrets[slotId].updateTurretStats();

            //Update price for the next upgrade
            if ((_item as TurretDescription).actualTier < (_item as TurretDescription).maxTier)
            {
                updateUpgradePrice((_item as TurretDescription).actualUpgradePriceScraps, (_item as TurretDescription).actualUpgradePriceEnergyCore);
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
