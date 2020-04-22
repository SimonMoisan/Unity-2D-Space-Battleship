using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManagerScript : MonoBehaviour
{
    public Cargo cargo;
    public ArsenalPanel arsenalPanel;
    public UpgraderPanel upgraderPanel;
    public Image dragableImage;

    public GameObject menuBattleshipCanvas; //This menu can be open in the starmap to change turrets of the battleship
    public GameObject startMapCanvas;

    private CargoItemSlot draggedSlot;

    //Associated objects
    private Battleship battleship;

    //Sprites to display
    public GameObject battleshipSprite;
    
    private void Awake()
    {
        //Setup events :
        //Left click
        cargo.OnLeftClickEvent += cargoLeftClick;
        arsenalPanel.OnLeftClickEvent += arsenalLeftClick;
        //Right click
        cargo.OnRightClickEvent += equipTurretToUpgrader;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        battleship = FindObjectOfType<Battleship>();
    }

    private void equipTurretToUpgrader(CargoItemSlot cargoItemSlot)
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
        upgraderPanel.healthValue.text = "" + turret.actualProjectileHealth;
        upgraderPanel.shotBySalveValue.text = turret.actualShotsPerSalve + " x " + turret.actualNbrOfSalve;
        upgraderPanel.cooldownValue.text = "" + turret.actualCooldown;
        upgraderPanel.speedValue.text = "" + turret.actualSpeed;

        upgraderPanel.turretType.text = "" + turret.turretType;
        upgraderPanel.projectileType.text = "" + turret.projectileType;
        upgraderPanel.modifierType.text = "" + turret.modifierPrimaryType;
    }

    public void cargoLeftClick(CargoItemSlot cargoItemSlot)
    {
        if (cargoItemSlot.CargoItem is TurretDescritpion)
        {
            equipTurretToArsenal((TurretDescritpion)cargoItemSlot.CargoItem);
        }
    }

    public void arsenalLeftClick(CargoItemSlot cargoItemSlot)
    {
        if (cargoItemSlot.CargoItem is TurretDescritpion)
        {
            unequipTurret((TurretDescritpion)cargoItemSlot.CargoItem, cargoItemSlot.slotId);
        }
    }

    public void equipTurretToArsenal(TurretDescritpion turret)
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

    public void unequipTurret(TurretDescritpion turret, int slotId)
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
        if(cargoItemSlot.CargoItem != null)
        {
            draggedSlot = cargoItemSlot;
            dragableImage.sprite = cargoItemSlot.CargoItem.cargoIcone;
            dragableImage.transform.position = Input.mousePosition;
            dragableImage.enabled = true;
        }
    }

    private void endDrag(CargoItemSlot cargoItemSlot)
    {
        draggedSlot = null;
        dragableImage.enabled = false;
    }

    private void drag(CargoItemSlot cargoItemSlot)
    {
        dragableImage.transform.position = Input.mousePosition;
    }

    private void drop(CargoItemSlot dropCargoItemSlot)
    {
        TurretDescritpion dragItem = draggedSlot.CargoItem as TurretDescritpion;    //item dragged by the player
        TurretDescritpion dropItem = dropCargoItemSlot.CargoItem as TurretDescritpion;   //item present or not in the cargoItemSlot

        int slotIdDraggedItem = -1;

        //We want to know the slotId of the dragged item, and if its a cargo or an arsenal slot
        if(dragItem.slotName == SlotName.Arsenal) //Turret Slot
        {
            //Get slotId of the turret to unequip from arsenal
            slotIdDraggedItem = dragItem.arsenalId;
        }
        else if(dragItem.slotName == SlotName.Cargo) //Cargo Slot
        {
            //Get slotId of the turret to unequip from cargo
            slotIdDraggedItem = dragItem.cargoId;
        }
        Debug.Log(slotIdDraggedItem);


        //Case 1 : drop item in empty arsenal slot (OK)
        if (dropCargoItemSlot is TurretSlot && dragItem != null && dropItem == null)
        {
            Debug.Log("Case 1");
            if (dragItem is TurretDescritpion) //if dragged is a turret
            {
                if ((dragItem as TurretDescritpion).slotName == SlotName.Arsenal) //if dragged item come from arsenal (OK)
                {
                    dragItem.unequipTurret(this, slotIdDraggedItem);
                    dragItem.equipTurret(this, dropCargoItemSlot.slotId); //equip dragTurret in arsenal slot (OK)
                }
                else if ((dragItem as TurretDescritpion).slotName == SlotName.Cargo) //if dragged item come from cargo 
                {
                    dragItem.equipTurret(this, dropCargoItemSlot.slotId);
                }
            }
        }

        //Case 2 : drop item in occupied arsenal slot (OK)
        if (dropCargoItemSlot is TurretSlot && dragItem != null && dropItem != null)
        {
            Debug.Log("Case 2");
            if (dragItem is TurretDescritpion) //if dragged is a turret
            {
                if ((dragItem as TurretDescritpion).slotName == SlotName.Arsenal) //if dragged item come from arsenal (OK)
                {
                    dragItem.unequipTurret(this, slotIdDraggedItem);
                    dropItem.unequipTurret(this, dropCargoItemSlot.slotId);

                    dragItem.equipTurret(this, dropCargoItemSlot.slotId);
                    dropItem.equipTurret(this, slotIdDraggedItem);
                }
                else if ((dragItem as TurretDescritpion).slotName == SlotName.Cargo) //if dragged item come from cargo (OK)
                {
                    dropItem.unequipTurret(this, dropCargoItemSlot.slotId); //unequip turret in arsenal slot
                    dragItem.equipTurret(this, dropCargoItemSlot.slotId); //equip dragTurret in arsenal slot
                    
                    dropItem.cargoId = dropCargoItemSlot.slotId;
                }
            }
        }

        //Case 3 : drop item in empty cargo slot (OK)
        if (!(dropCargoItemSlot is TurretSlot) && dragItem != null && dropItem == null)
        {
            Debug.Log("Case 3");
            if(dragItem is TurretDescritpion) //if dragged is a turret
            {
                if((dragItem as TurretDescritpion).slotName == SlotName.Arsenal) //if dragged item come from arsenal (OK)
                {
                    dragItem.unequipTurret(this, slotIdDraggedItem);
                    dragItem.cargoId = dropCargoItemSlot.slotId; ;
                }
                else if((dragItem as TurretDescritpion).slotName == SlotName.Cargo) //if dragged item come from cargo (OK)
                {
                    dragItem.cargoId = dropCargoItemSlot.slotId;
                }
            }
        }

        //Case 4 : drop item in occupied cargo slot (OK)
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

        CargoItem draggedItem = draggedSlot.CargoItem;
        draggedSlot.CargoItem = dropCargoItemSlot.CargoItem;
        dropCargoItemSlot.CargoItem = draggedItem;
    }

    public void closeBattleshipMenu()
    {
        menuBattleshipCanvas.SetActive(false);
        startMapCanvas.SetActive(true);
    }

    public void openBattleshipMenu()
    {
        menuBattleshipCanvas.SetActive(true);
        startMapCanvas.SetActive(false);
    }
}
