using UnityEngine.UI;

public class TurretSlot : CargoItemSlot
{
    public Text turretName;
    public Text damageValue;
    public Text healthValue;
    public Text shotBySalveValue; //Format : shotPerSalve x nbrOfShot 
    public Text firerateValue;
    public Text cooldownValue;

    public new CargoItem CargoItem
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
            {
                image.color = disableColor;
                turretName.text = "Empty slot";
                damageValue.text = "-";
                healthValue.text = "-";
                shotBySalveValue.text = "-";
                firerateValue.text = "-";
                cooldownValue.text = "-";
            }
            else
            {
                image.sprite = _item.arsenalIcon;
                image.color = normalColor;
                turretName.text = _item.ItemName;
                damageValue.text = "" + (_item as TurretDescritpion).actualDamage;
                healthValue.text = "" + (_item as TurretDescritpion).actualHealth;
                shotBySalveValue.text = "" + (_item as TurretDescritpion).actualShotsPerSalve + " x " + (_item as TurretDescritpion).actualNbrOfSalve;
                firerateValue.text = "" + (_item as TurretDescritpion).actualDamage;
                cooldownValue.text = "" + (_item as TurretDescritpion).actualDamage;
            }
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = "Turret slot " + slotId;
    }
}
