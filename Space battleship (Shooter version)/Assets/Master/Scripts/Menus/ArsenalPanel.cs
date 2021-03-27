using UnityEngine;
using System;

public class ArsenalPanel : MonoBehaviour
{
    [SerializeField] CargoItemSlot[] startingCargoTurrets; //starting turrets from cargo
    public TurretSlot[] turretSlots;

    //public event Action<CargoItemSlot> OnLeftClickEvent;
    public event Action<CargoItemSlot> OnBeginingDragEvent;
    public event Action<CargoItemSlot> OnEndingDragEvent;
    public event Action<CargoItemSlot> OnDragEvent;
    public event Action<CargoItemSlot> OnDropEvent;

    private void Start()
    {
        for (int i = 0; i < turretSlots.Length; i++)
        {
            turretSlots[i].slotId = i;

            //turretSlots[i].OnLeftClickEvent += OnLeftClickEvent;
            turretSlots[i].OnBeginingDragEvent += OnBeginingDragEvent;
            turretSlots[i].OnEndingDragEvent += OnEndingDragEvent;
            turretSlots[i].OnDragEvent += OnDragEvent;
            turretSlots[i].OnDropEvent += OnDropEvent;
        }
    }

    private void OnValidate()
    {
        turretSlots = GetComponentsInChildren<TurretSlot>();

        refreshUI();
    }

    private void refreshUI()
    {
        int i = 0;

        //Equip starting turret in the arsenal and on the battleship
        for (; i < startingCargoTurrets.Length && i < turretSlots.Length; i++)
        {
            turretSlots[i].CargoItem = startingCargoTurrets[i].CargoItem;
            turretSlots[i].cargoId = startingCargoTurrets[i].slotId;

            if (turretSlots[i].hidderEmpty != null)
            {
                turretSlots[i].hidderEmpty.SetActive(false);
            }

            //Manage hidders for turret and cargo slot
            startingCargoTurrets[i].isUsed = true;
            startingCargoTurrets[i].manageHidders();
            (startingCargoTurrets[i].CargoItem as TurretDescription).cargoId = i;

            //Display price for next upgrade
            turretSlots[i].updateUpgradePrice((startingCargoTurrets[i].CargoItem as TurretDescription).actualUpgradePriceScraps, (startingCargoTurrets[i].CargoItem as TurretDescription).actualUpgradePriceEnergyCore);
        }

        for (; i < turretSlots.Length; i++)
        {
            if(turretSlots[i].hidderEmpty != null)
            {
                turretSlots[i].hidderEmpty.SetActive(true);
            }
            turretSlots[i].CargoItem = null;
        }

        for (int j = 0; j < startingCargoTurrets.Length; j++)
        {
            if (startingCargoTurrets[j].CargoItem is TurretDescription)
            {
                (startingCargoTurrets[j].CargoItem as TurretDescription).slotName = SlotName.Cargo;
                (startingCargoTurrets[j].CargoItem as TurretDescription).arsenalId = -1;
            }
        }
    }

    //Add turret to a slot of the arsenal panel, return the id of the slot or -1 if arsenalPanel is full
    public int addTurret(TurretDescription turret)
    {
        for (int i = 0; i < turretSlots.Length; i++)
        {
            if (turretSlots[i].CargoItem == null) //if slot is free
            {
                turretSlots[i].CargoItem = turret;
                return i;
            }
        }
        Debug.Log("Arsenal full");
        return -1;
    }

    public bool removeTurret(TurretDescription turret)
    {
        for (int i = 0; i < turretSlots.Length; i++)
        {
            if (turretSlots[i].CargoItem == turret)
            {
                turretSlots[i].CargoItem = null;
                return true;
            }
        }
        return false;
    }
}
