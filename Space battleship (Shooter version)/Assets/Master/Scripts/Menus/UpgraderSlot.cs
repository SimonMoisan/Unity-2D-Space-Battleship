using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgraderSlot : CargoItemSlot
{
    protected override void OnValidate()
    {
        base.OnValidate();
        if(_item != null)
        {
            gameObject.name = "Upgrade : " + _item.ItemName;
        }
    }
}
