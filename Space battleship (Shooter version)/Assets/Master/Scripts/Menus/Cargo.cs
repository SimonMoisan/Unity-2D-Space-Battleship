using System.Collections.Generic;
using UnityEngine;
using System;

public class Cargo : MonoBehaviour
{
    [SerializeField] List<CargoItem> startingItems;
    [SerializeField] Transform itemsParent;
    public CargoItemSlot[] itemSlots;

    public event Action<CargoItemSlot> OnRightClickEvent;
    public event Action<CargoItemSlot> OnLeftClickEvent;
    public event Action<CargoItemSlot> OnBeginingDragEvent;
    public event Action<CargoItemSlot> OnEndingDragEvent;
    public event Action<CargoItemSlot> OnDragEvent;
    public event Action<CargoItemSlot> OnDropEvent;

    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].slotId = i;

            itemSlots[i].OnRightClickEvent += OnRightClickEvent;
            itemSlots[i].OnLeftClickEvent += OnLeftClickEvent;
            itemSlots[i].OnBeginingDragEvent += OnBeginingDragEvent;
            itemSlots[i].OnEndingDragEvent += OnEndingDragEvent;
            itemSlots[i].OnDragEvent += OnDragEvent;
            itemSlots[i].OnDropEvent += OnDropEvent;
        }
    }

    private void OnValidate()
    {
        if(itemsParent != null)
        {
            itemSlots = itemsParent.GetComponentsInChildren<CargoItemSlot>();
        }

        refreshUI();
    }


    private void refreshUI()
    {
        int i = 0;
        for (; i < startingItems.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].CargoItem = startingItems[i];
            (startingItems[i] as TurretDescritpion).cargoId = i;
        }

        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].CargoItem = null;
        }

        for (int j = 0; j < startingItems.Count; j++)
        {
            if(startingItems[j] is TurretDescritpion)
            {
                (startingItems[j] as TurretDescritpion).slotName = SlotName.Cargo;
                (startingItems[j] as TurretDescritpion).arsenalId = -1;
            }
        }
    }

    public bool addItem(CargoItem item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].CargoItem == null)
            {
                itemSlots[i].CargoItem = item;
                if(itemSlots[i].CargoItem is TurretDescritpion)
                {
                    (itemSlots[i].CargoItem as TurretDescritpion).slotName = SlotName.Cargo;
                }

                return true;
            }
        }
        return false;
    }

    public bool removeItem(CargoItem item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].CargoItem == item)
            {
                itemSlots[i].CargoItem = null;
                return true;
            }
        }
        return false;
    }

    public bool isFull()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].CargoItem == null)
            {
                return false;
            }
        }
        return true;
    }
}
