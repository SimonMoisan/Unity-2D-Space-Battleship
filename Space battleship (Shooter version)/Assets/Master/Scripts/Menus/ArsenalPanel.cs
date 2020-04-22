using UnityEngine;
using System;

public class ArsenalPanel : MonoBehaviour
{
    public TurretSlot[] turretSlots;

    public event Action<CargoItemSlot> OnLeftClickEvent;
    public event Action<CargoItemSlot> OnBeginingDragEvent;
    public event Action<CargoItemSlot> OnEndingDragEvent;
    public event Action<CargoItemSlot> OnDragEvent;
    public event Action<CargoItemSlot> OnDropEvent;

    private void Start()
    {
        for (int i = 0; i < turretSlots.Length; i++)
        {
            turretSlots[i].slotId = i;

            turretSlots[i].OnLeftClickEvent += OnLeftClickEvent;
            turretSlots[i].OnBeginingDragEvent += OnBeginingDragEvent;
            turretSlots[i].OnEndingDragEvent += OnEndingDragEvent;
            turretSlots[i].OnDragEvent += OnDragEvent;
            turretSlots[i].OnDropEvent += OnDropEvent;
        }
    }

    private void OnValidate()
    {
        turretSlots = GetComponentsInChildren<TurretSlot>();
    }

    //Add turret to a slot of the arsenal panel, return the id of the slot or -1 if arsenalPanel is full
    public int addTurret(TurretDescritpion turret)
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

    public bool removeTurret(TurretDescritpion turret)
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
