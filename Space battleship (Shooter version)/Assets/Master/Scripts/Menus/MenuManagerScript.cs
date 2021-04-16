using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManagerScript : MonoBehaviour
{
    public bool sectorClickable;

    [Header("Assotiated objects :")]
    public PlayerStats playerStats;
    public Cargo cargo;
    public ArsenalPanel arsenalPanel;
    public UpgraderPanel upgraderPanel;
    public Image dragableImage;
    public GameManagerScript gameManager;
    private CargoItemSlot draggedSlot;
    private Battleship battleship;
    [Space]
    [Header("Base canvas :")]
    public GameObject pauseMenuWindow;
    private float previousTimeFactor;
    [Space]
    [Header("Station Menu :")]
    public int actualRepairStationAmount;
    public int actualRepairStationPrice;
    public GameObject stationMenuCanvas; //This menu can be open in the starmap to change turrets of the battleship
    public GameObject startMapCanvas;
    public GameObject helpWindowStationMenu;
    public GameObject warningWindowStationMenu;
    public GameObject enterStationButton;
    [Space]
    public GameObject shieldUpgrader;
    public Text upgradeShieldIndicator;
    public Text repairHullIndicator;
    [Space]
    [Header("Battlezone :")]
    public GameObject battlezoneCanvas;
    public GameObject informationWindowEndBattle;
    public Canvas contextualCanvas;
    public MeshRenderer backgroundMesh;
    //Report window
    public GameObject reportWindow;
    public GameObject openReportWindowButton;
    public Text gainScrapValue;
    public Text gainEnergyCoreValue;
    public Text gainHullValue;
    public Text loseScrapValue;
    public Text loseEnergyCoreValue;
    public Text loseHullValue;
    [Header("Battle report :")]
    public Text scrapRewardValue;
    public Text energyCoreRewardValue;
    [Header("Starmap :")]
    public GameObject starmapCanvas;

    //Sprites to display
    public GameObject battleshipSprite;

    public static MenuManagerScript current;
    
    private void Awake()
    {
        //Setup events :
        //Begin drag
        cargo.OnBeginingDragEvent += beginDrag;
        arsenalPanel.OnBeginingDragEvent += beginDrag;
        //End drag
        cargo.OnEndingDragEvent += endDrag;
        arsenalPanel.OnEndingDragEvent += endDrag;
        //Drag
        cargo.OnDragEvent += drag;
        arsenalPanel.OnDragEvent += drag;
        //Drop
        cargo.OnDropEvent += drop;
        arsenalPanel.OnDropEvent += drop;

        //Initiate menus animations
        stationMenuCanvas.SetActive(true);
        stationMenuCanvas.GetComponent<Animator>().Play("Close");
        //stationMenuCanvas.GetComponent<Animator>().Play("Close-Help");
        //stationMenuCanvas.GetComponent<Animator>().Play("Close-Warning");
    }

    // Start is called before the first frame update
    void OnValidate()
    {
        current = this;

        playerStats = FindObjectOfType<PlayerStats>();
        battleship = FindObjectOfType<Battleship>();
        gameManager = FindObjectOfType<GameManagerScript>();

        updateShieldUpgradePrice();
        updateRepairPriceIndicator();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            openPauseMenu();
        }
    }

    /*private void equipTurretToUpgrader(CargoItemSlot cargoItemSlot)
    {
        TurretDescritpion turretToEquip = cargoItemSlot.CargoItem as TurretDescritpion;
        if(turretToEquip != null)
        {
            equipTurretToUpgrader(turretToEquip);
        }
    }

    public void equipTurretToUpgrader(TurretDescritpion turret)
    {
        upgraderPanel.addTurret(turret);

        upgraderPanel.turretName.text = turret.name;
        upgraderPanel.damageValue.text = "" + turret.actualDamage;
        upgraderPanel.healthValue.text = "" + turret.actualHealth;
        upgraderPanel.shotBySalveValue.text = turret.actualShotsPerSalve + " x " + turret.actualNbrOfSalve;
        upgraderPanel.cooldownValue.text = "" + turret.actualCooldown;
        upgraderPanel.speedValue.text = "" + turret.actualSpeed;

        upgraderPanel.turretType.text = "" + turret.turretType;
        upgraderPanel.projectileType.text = "" + turret.projectileType;
        upgraderPanel.modifierType.text = "" + turret.modifierPrimaryType;
    }*/

    public void cargoLeftClick(CargoItemSlot cargoItemSlot)
    {
        if (cargoItemSlot.CargoItem is TurretDescription)
        {
            equipTurretToArsenal((TurretDescription)cargoItemSlot.CargoItem);
        }
    }

    public void arsenalLeftClick(CargoItemSlot cargoItemSlot)
    {
        if (cargoItemSlot.CargoItem is TurretDescription)
        {
            unequipTurret((TurretDescription)cargoItemSlot.CargoItem, cargoItemSlot.slotId);
        }
    }

    public void equipTurretToArsenal(TurretDescription turret)
    {
        if (cargo.removeItem(turret))
        {
            int turretSlotId = arsenalPanel.addTurret(turret);
            if (turretSlotId != -1)
            {
                turret.equipTurret(this, turretSlotId);
                turret.slotName = SlotName.Arsenal;
            }
            else
            {
                cargo.addItem(turret);
            }
        }
    }

    public void unequipTurret(TurretDescription turret, int slotId)
    {
        if (!cargo.isFull() && arsenalPanel.removeTurret(turret))
        {
            turret.unequipTurret(this, slotId);
            cargo.addItem(turret);
        }
        else
        {
            arsenalPanel.addTurret(turret);
        }
    }

    private void beginDrag(CargoItemSlot cargoItemSlot)
    {
        if (cargoItemSlot.CargoItem != null && !cargoItemSlot.isUsed && !cargoItemSlot.isLocked)
        {
            draggedSlot = cargoItemSlot;
            dragableImage.sprite = cargoItemSlot.turretImage.sprite;
            dragableImage.transform.position = Input.mousePosition;
            dragableImage.enabled = true;
            dragableImage.SetNativeSize();
        }
    }

    private void endDrag(CargoItemSlot cargoItemSlot)
    {
        dragableImage.enabled = false;
    }

    private void drag(CargoItemSlot cargoItemSlot)
    {
        Debug.Log(Input.mousePosition);
        dragableImage.transform.position = Input.mousePosition;
    }

    private void drop(CargoItemSlot dropCargoItemSlot)
    {
        if(draggedSlot != null)
        {
            TurretDescription dragItem = draggedSlot.CargoItem as TurretDescription;    //item dragged by the player
            TurretDescription dropItem = dropCargoItemSlot.CargoItem as TurretDescription;   //item present or not in the cargoItemSlot

            int slotIdDraggedItem = -1;

            //We want to know the slotId of the dragged item, and if its a cargo or an arsenal slot
            if (dragItem.slotName == SlotName.Arsenal) //Turret Slot
            {
                //Get slotId of the turret to unequip from arsenal
                slotIdDraggedItem = dragItem.arsenalId;
            }
            else if (dragItem.slotName == SlotName.Cargo) //Cargo Slot
            {
                //Get slotId of the turret to unequip from cargo
                slotIdDraggedItem = dragItem.cargoId;
            }
            //Debug.Log(slotIdDraggedItem);


            //Case 1 : drop item in empty arsenal slot (OK)
            if (dropCargoItemSlot is TurretSlot && dragItem != null && dropItem == null)
            {
                if (dragItem is TurretDescription) //if dragged is a turret
                {
                    if ((draggedSlot as TurretSlot) != null) //if dragged item come from arsenal (OK)
                    {
                        //Switch turrets
                        dragItem.unequipTurret(this, draggedSlot.slotId);
                        dragItem.equipTurret(this, dropCargoItemSlot.slotId); //equip dragTurret in arsenal slot 

                        //Switch hidders
                        (draggedSlot as TurretSlot).hidderEmpty.SetActive(true);
                        (dropCargoItemSlot as TurretSlot).hidderEmpty.SetActive(false);

                        //Switch visual
                        dropCargoItemSlot.CargoItem = draggedSlot.CargoItem;
                        draggedSlot.CargoItem = null;

                        //Update id
                        (dropCargoItemSlot as TurretSlot).cargoId = (draggedSlot as TurretSlot).cargoId;
                        (draggedSlot as TurretSlot).cargoId = -1;

                        //Update price
                        (dropCargoItemSlot as TurretSlot).updateUpgradePrice(dragItem.actualUpgradePriceScraps, dragItem.actualUpgradePriceEnergyCore);
                        (draggedSlot as TurretSlot).updateUpgradePrice(dropItem.actualUpgradePriceScraps, dropItem.actualUpgradePriceEnergyCore);
                    }
                    else //if dragged item come from cargo 
                    {
                        dragItem.equipTurret(this, dropCargoItemSlot.slotId);

                        //Switch hidders
                        (dropCargoItemSlot as TurretSlot).hidderEmpty.SetActive(false);
                        draggedSlot.isUsed = true;
                        draggedSlot.manageHidders();

                        //Switch visual
                        dropCargoItemSlot.CargoItem = draggedSlot.CargoItem;

                        //Update id
                        (dropCargoItemSlot as TurretSlot).cargoId = draggedSlot.slotId;

                        //Update price
                        (dropCargoItemSlot as TurretSlot).updateUpgradePrice(dragItem.actualUpgradePriceScraps, dragItem.actualUpgradePriceEnergyCore);
                    }
                }
            }

            //Case 2 : drop item in occupied arsenal slot (OK)
            if (dropCargoItemSlot is TurretSlot && dragItem != null && dropItem != null && draggedSlot != dropCargoItemSlot)
            {
                if (dragItem is TurretDescription) //if dragged is a turret
                {
                    if ((draggedSlot as TurretSlot) != null) //if dragged item come from arsenal (OK)
                    {
                        dragItem.unequipTurret(this, draggedSlot.slotId);
                        dropItem.unequipTurret(this, dropCargoItemSlot.slotId);

                        dragItem.equipTurret(this, dropCargoItemSlot.slotId);
                        dropItem.equipTurret(this, draggedSlot.slotId);

                        //Switch items
                        CargoItem draggedItem = draggedSlot.CargoItem;
                        draggedSlot.CargoItem = dropCargoItemSlot.CargoItem;
                        dropCargoItemSlot.CargoItem = draggedItem;

                        //Update price
                        (dropCargoItemSlot as TurretSlot).updateUpgradePrice(dragItem.actualUpgradePriceScraps, dragItem.actualUpgradePriceEnergyCore);
                        (draggedSlot as TurretSlot).updateUpgradePrice(dropItem.actualUpgradePriceScraps, dropItem.actualUpgradePriceEnergyCore);
                    }
                    else if(!draggedSlot.isUsed) //if dragged item come from cargo and its not an already used turret (OK)
                    {
                        dropItem.unequipTurret(this, dropCargoItemSlot.slotId); //unequip turret in arsenal slot
                        dragItem.equipTurret(this, dropCargoItemSlot.slotId); //equip dragTurret in arsenal slot

                        //Send dropItem item back to its cargo slot
                        cargo.itemSlots[(dropCargoItemSlot as TurretSlot).cargoId].isUsed = false;
                        cargo.itemSlots[draggedSlot.slotId].isUsed = true;
                        cargo.itemSlots[(dropCargoItemSlot as TurretSlot).cargoId].manageHidders();
                        cargo.itemSlots[draggedSlot.slotId].manageHidders();

                        //Change item in arsenal slot
                        dropCargoItemSlot.CargoItem = draggedSlot.CargoItem;

                        //Set slot id in arsenal slot according to the new turret
                        (dropCargoItemSlot as TurretSlot).cargoId = draggedSlot.slotId;

                        //Update price
                        (dropCargoItemSlot as TurretSlot).updateUpgradePrice(dragItem.actualUpgradePriceScraps, dragItem.actualUpgradePriceEnergyCore);

                        dropItem.cargoId = dropCargoItemSlot.slotId;
                    }
                }
            }

            /*//Case 3 : drop item in empty cargo slot (OK)
            if (!(dropCargoItemSlot is TurretSlot) && dragItem != null && dropItem == null)
            {
                Debug.Log("Case 3");
                if (dragItem is TurretDescritpion) //if dragged is a turret
                {
                    if ((dragItem as TurretDescritpion).slotName == SlotName.Arsenal) //if dragged item come from arsenal (OK)
                    {
                        dragItem.unequipTurret(this, slotIdDraggedItem);
                        dragItem.cargoId = dropCargoItemSlot.slotId; ;
                    }
                    else if ((dragItem as TurretDescritpion).slotName == SlotName.Cargo) //if dragged item come from cargo (OK)
                    {
                        dragItem.cargoId = dropCargoItemSlot.slotId;
                    }
                }
            }*/

            /*//Case 4 : drop item in occupied cargo slot (OK)
            if (!(dropCargoItemSlot is TurretSlot) && dragItem != null && dropItem != null)
            {
                Debug.Log("Case 4");
                if (dragItem is TurretDescritpion) //if dragged is a turret
                {
                    if ((dragItem as TurretDescritpion).slotName == SlotName.Arsenal) //if dragged item come from arsenal (OK)
                    {
                        dragItem.unequipTurret(this, slotIdDraggedItem); //unequip drag Turret from arsenal slot
                        dropItem.equipTurret(this, slotIdDraggedItem); //equip drop Turret from cargo in arsenal slot

                        dragItem.cargoId = dropCargoItemSlot.slotId;
                    }
                    else if ((dragItem as TurretDescritpion).slotName == SlotName.Cargo) //if dragged item come from cargo (OK)
                    {
                        dragItem.cargoId = dropCargoItemSlot.slotId;
                        dropItem.cargoId = slotIdDraggedItem;
                    }
                }
            }*/

            //Case 4 : drop item in occupied cargo slot (Ok)
            if (!(dropCargoItemSlot is TurretSlot) && dragItem != null && dropItem != null)
            {
                if (dragItem is TurretDescription && (draggedSlot as TurretSlot) != null) //if dragged is a turret
                {
                    if ((draggedSlot as TurretSlot).cargoId == dropCargoItemSlot.slotId && dropCargoItemSlot.isUsed) //if cargo slot is the origin slot of the dragged item, then the turret become empty
                    {
                        dragItem.unequipTurret(this, draggedSlot.slotId); //unequip drag Turret from arsenal slot

                        //Switch hidder
                        cargo.itemSlots[(draggedSlot as TurretSlot).cargoId].isUsed = false;
                        cargo.itemSlots[(draggedSlot as TurretSlot).cargoId].manageHidders();
                        (draggedSlot as TurretSlot).hidderEmpty.SetActive(true);

                        //Set slot id in arsenal slot to empty
                        (draggedSlot as TurretSlot).cargoId = -1;

                        //Empty arsenal slot
                        draggedSlot.CargoItem = null;

                        //Update price
                        (draggedSlot as TurretSlot).updateUpgradePrice(-1, -1);
                    }
                    else if(!dropCargoItemSlot.isUsed)
                    {
                        dragItem.unequipTurret(this, draggedSlot.slotId); //unequip drag Turret from arsenal slot
                        dropItem.equipTurret(this, draggedSlot.slotId); //equip drop Turret from cargo in arsenal slot

                        //Switch hidder
                        cargo.itemSlots[(draggedSlot as TurretSlot).cargoId].isUsed = false;
                        cargo.itemSlots[(draggedSlot as TurretSlot).cargoId].manageHidders();
                        dropCargoItemSlot.isUsed = true;
                        dropCargoItemSlot.manageHidders();

                        //Change item in arsenal slot
                        draggedSlot.CargoItem = dropCargoItemSlot.CargoItem;

                        //Set slot id in arsenal slot according to the new turret
                        (draggedSlot as TurretSlot).cargoId = dropCargoItemSlot.slotId;

                        //Update price
                        (draggedSlot as TurretSlot).updateUpgradePrice(dropItem.actualUpgradePriceScraps, dropItem.actualUpgradePriceEnergyCore);
                    }

                    dragItem.cargoId = dropCargoItemSlot.slotId;
                }
            }

            /*
            if (dragItem is TurretDescritpion)
            {
                Debug.Log("DragItem");
                if (dragItem != null) dragItem.unequipTurret(this);
                if (dropItem != null) dragItem.equipTurret(this, dropCargoItemSlot.slotId);
            }
            if (dropItem is TurretDescritpion)
            {
                Debug.Log("DropItem");
                if (dragItem != null) dragItem.equipTurret(this, dropCargoItemSlot.slotId);
                if (dropItem != null) dragItem.unequipTurret(this);
            }*/

            //Reverse items positions
            /*CargoItem draggedItem = draggedSlot.CargoItem;
            draggedSlot.CargoItem = dropCargoItemSlot.CargoItem;
            dropCargoItemSlot.CargoItem = draggedItem;*/
        }
    }

    public void openPauseMenu()
    {
        pauseMenuWindow.SetActive(true);
        previousTimeFactor = Time.timeScale;
        Time.timeScale = 0;
        VolumeEffectManager.current.activateVolumeEffect(1, false);

        PlayerStats.current.isPlaying = false;
    }

    public void clonePauseMenu()
    {
        pauseMenuWindow.SetActive(false);
        Time.timeScale = previousTimeFactor;
        VolumeEffectManager.current.deactivateVolumeEffect(1);

        PlayerStats.current.isPlaying = true;
    }

    public void closeBattleshipMenu()
    {
        sectorClickable = true;
        stationMenuCanvas.SetActive(false);
        startMapCanvas.SetActive(true);
    }

    public void openBattleshipMenu()
    {
        sectorClickable = false;
        stationMenuCanvas.SetActive(true);
        startMapCanvas.SetActive(false);
    }

    public void closeInformationPanel()
    {
        GameManagerScript.current.restObjectsStates();
        GameManagerScript.current.StartCoroutine(GameManagerScript.current.fadeBattlezoneToStarmap());
        AudioManager.current.StartCoroutine(AudioManager.current.musicSwitch(MusicType.Starmap));

        EnnemySpawner ennemySpawner = FindObjectOfType<EnnemySpawner>();

        //Give scraps to player
        playerStats.scrapsStored += ennemySpawner.scrapsToWin;
        playerStats.energyCoreStored += ennemySpawner.energyCoreToWin;

        informationWindowEndBattle.SetActive(false);
    }

    public void openHelpWindowStationMenu()
    {
        stationMenuCanvas.GetComponent<Animator>().Play("Open-Help");
        //helpWindowStationMenu.SetActive(true);
    }

    public void closeHelpWindowStationMenu()
    {
        stationMenuCanvas.GetComponent<Animator>().Play("Close-Help");
        //helpWindowStationMenu.SetActive(false);
    }

    public void closeWarningWindowStationMenu()
    {
        stationMenuCanvas.GetComponent<Animator>().Play("Close-Warning");
        //warningWindowStationMenu.SetActive(false);
    }

    public void openStationMenu()
    {
        stationMenuCanvas.GetComponent<Animator>().Play("Open");
        //stationMenuCanvas.SetActive(true);
    }

    public void closeStationMenu()
    {
        for (int i = 0; i < ArsenalPanel.current.turretSlots.Length; i++)
        {
            if(ArsenalPanel.current.turretSlots[i].CargoItem == null)
            {
                stationMenuCanvas.GetComponent<Animator>().Play("Open-Warning");
                //warningWindowStationMenu.SetActive(true);
                return;
            }
        }

        stationMenuCanvas.GetComponent<Animator>().Play("Close");
        //stationMenuCanvas.SetActive(false);
    }

    public void upgradeShieldButton()
    {
        Shield shield = FindObjectOfType<Shield>();

        if(shield.actualTier < shield.shieldTiers.Length && playerStats.energyCoreStored >= shield.shieldTiers[shield.actualTier + 1].energyCorePrice)
        {
            playerStats.updateEnergyCoreStoredValue(playerStats.energyCoreStored - shield.shieldTiers[shield.actualTier + 1].energyCorePrice);
            shield.updateShieldStats(shield.actualTier + 1);
            updateShieldUpgradePrice();
        }
    }

    public void repairHullButton()
    {
        if(battleship.hullPoints < battleship.MaxHullPoints && playerStats.scrapsStored >= actualRepairStationPrice)
        {
            playerStats.updateScrapStoredValue(playerStats.scrapsStored - actualRepairStationPrice);
            battleship.repairHull(actualRepairStationAmount);
        }
        else
        {
            Debug.Log("Repair impossible");
        }
    }

    public void updateShieldUpgradePrice()
    {
        Shield shield = FindObjectOfType<Shield>();
        if (shield.actualTier < shield.shieldTiers.Length)
        {
            upgradeShieldIndicator.text = "x " + shield.shieldTiers[shield.actualTier + 1].energyCorePrice;
        }
        else
        {
            shieldUpgrader.SetActive(false);
        }
    }

    public void updateRepairPriceIndicator()
    {
        repairHullIndicator.text = "x " + actualRepairStationPrice;
    }

    public void openReportWindow()
    {
        reportWindow.SetActive(true);
    }

    public void closeReportWindow()
    {
        reportWindow.SetActive(false);
        gainScrapValue.text = "+ 0";
        gainEnergyCoreValue.text = "+ 0";
        gainHullValue.text = "+ 0";
        loseScrapValue.text = "- 0";
        loseEnergyCoreValue.text = "- 0";
        loseHullValue.text = "- 0";
    }
}
