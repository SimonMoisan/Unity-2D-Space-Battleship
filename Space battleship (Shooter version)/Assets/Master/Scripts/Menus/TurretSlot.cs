public class TurretSlot : CargoItemSlot
{


    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = "Turret slot " + slotId;
    }
}
