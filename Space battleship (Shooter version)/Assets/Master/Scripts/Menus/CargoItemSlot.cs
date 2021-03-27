using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CargoItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Update and unlock")]
    public int unlockEnergyCorePrice;
    public int upgradPriceScraps;
    public int upgradPriceEnergyCore;
    [Header("States")]
    public bool isUsed;
    public bool isLocked;
    [Header("Associted objects")]
    public Text turretName;
    public Text turretLevelValue;
    public Text damageText;
    public Text healthText;
    public Text damageValue;
    public Text healthValue;
    public Text shotBySalveValue; //Format : shotPerSalve x nbrOfShot 
    public Text cooldownValue;
    public Text deviationValue; //values : None, Low, Med, High
    public Text speedValue; //values : Slow, Med, High, Inf
    public Text energyCoreToUnlockValue;
    [Space]
    public GameObject hidderLocked;
    public GameObject hidderUsed;
    public GameObject hidderCommingSoon;
    [Space]
    public Image turretTypeImage;
    public Sprite[] turretTypeSprites; //0 : Kinetic, 1 : Missile, 2 : Laser, 3 : Plasme, 4 : Ion, 5 : Shield
    [Space]
    [SerializeField] protected Image globalImage;
    public Image turretImage;
    [SerializeField] protected CargoItem _item;
    public int slotId;

    Vector2 originalPosition;
    //public event Action<CargoItemSlot> OnRightClickEvent;
    //public event Action<CargoItemSlot> OnLeftClickEvent;
    public event Action<CargoItemSlot> OnBeginingDragEvent;
    public event Action<CargoItemSlot> OnEndingDragEvent;
    public event Action<CargoItemSlot> OnDragEvent;
    public event Action<CargoItemSlot> OnDropEvent;

    protected Color normalColor = Color.white;
    protected Color disableColor = new Color(1,1,1,0);

    public CargoItem CargoItem
    {
        get { return _item; }
        set
        {
            _item = value;
            updateItemInformationDisplay();
        }
    }

    protected virtual void OnValidate()
    {
        gameObject.name = "Cargo slot " + slotId;

        if (globalImage == null)
        {
            globalImage = GetComponent<Image>();
        }

        if (hidderUsed != null && hidderLocked != null && hidderCommingSoon != null)
        {
            manageHidders();
        }

        turretImage.SetNativeSize();
    }

    public void updateItemInformationDisplay()
    {
        if (_item == null)
        {
            turretImage.color = disableColor;

            //Set turret information displayed
            if (turretName != null && healthText != null && damageText != null && healthValue != null && damageValue != null)
            {
                healthText.enabled = false;
                damageText.enabled = true;
                healthValue.enabled = false;
                damageValue.enabled = true;

                turretName.text = "Empty slot";
                damageValue.text = "-";
                healthValue.text = "-";
                shotBySalveValue.text = "-";
                cooldownValue.text = "-";
                deviationValue.text = "-";
                speedValue.text = "-";

                //Update price to unlock
                if (energyCoreToUnlockValue != null)
                {
                    energyCoreToUnlockValue.text = "";
                }

                //Update level
                if (turretLevelValue != null)
                {
                    turretLevelValue.text = "";
                }
            }

            if (turretTypeImage != null)
            {
                turretTypeImage.enabled = false;
            }
        }
        else
        {
            if (this is TurretSlot)
            {
                turretImage.sprite = _item.cargoIcone;
            }
            else
            {
                turretImage.sprite = _item.cargoIcone;
            }
            turretImage.color = normalColor;

            //Set turret information displayed
            if (turretName != null)
            {
                turretImage.SetNativeSize();
                turretName.text = _item.ItemName;

                if ((_item as TurretDescription).actualDamage > 0)
                {
                    healthText.enabled = false;
                    damageText.enabled = true;
                    healthValue.enabled = false;
                    damageValue.enabled = true;
                    damageValue.text = "" + (_item as TurretDescription).actualDamage;
                }
                else
                {
                    healthText.enabled = true;
                    damageText.enabled = false;
                    healthValue.enabled = true;
                    damageValue.enabled = false;
                    healthValue.text = "" + (_item as TurretDescription).actualHealth;
                }

                shotBySalveValue.text = "" + (_item as TurretDescription).actualShotsPerSalve + " x " + (_item as TurretDescription).actualNbrOfSalve;
                cooldownValue.text = "" + (_item as TurretDescription).actualCooldown;

                //Define deviation
                if ((_item as TurretDescription).actualDeviation <= 0)
                {
                    deviationValue.text = "None";
                }
                else if ((_item as TurretDescription).actualDeviation > 0 && (_item as TurretDescription).actualDeviation <= 10)
                {
                    deviationValue.text = "Low";
                }
                else if ((_item as TurretDescription).actualDeviation > 10 && (_item as TurretDescription).actualDeviation <= 15)
                {
                    deviationValue.text = "Med";
                }
                else
                {
                    deviationValue.text = "High";
                }

                //Define speed
                if ((_item as TurretDescription).actualSpeed > 0 && (_item as TurretDescription).actualSpeed <= 200)
                {
                    speedValue.text = "Slow";
                }
                else if ((_item as TurretDescription).actualSpeed > 200 && (_item as TurretDescription).actualSpeed <= 500)
                {
                    speedValue.text = "Med";
                }
                else
                {
                    speedValue.text = "High";
                }

                //Define price
                if (energyCoreToUnlockValue != null)
                {
                    energyCoreToUnlockValue.text = "x " + (_item as TurretDescription).priceToUnlock;
                }

                //Define turret type image
                if (turretTypeImage != null)
                {
                    turretTypeImage.enabled = true;

                    switch ((_item as TurretDescription).projectileType)
                    {
                        case ProjectileType.Kinetic:
                            turretTypeImage.sprite = turretTypeSprites[0];
                            break;
                        case ProjectileType.Missile:
                            turretTypeImage.sprite = turretTypeSprites[1];
                            break;
                        case ProjectileType.Laser:
                            turretTypeImage.sprite = turretTypeSprites[2];
                            break;
                        case ProjectileType.Plasma:
                            turretTypeImage.sprite = turretTypeSprites[3];
                            break;
                        case ProjectileType.Ion:
                            turretTypeImage.sprite = turretTypeSprites[4];
                            break;
                        case ProjectileType.Shield:
                            turretTypeImage.sprite = turretTypeSprites[5];
                            break;
                    }
                }

                //Update level
                if (turretLevelValue != null)
                {
                    turretLevelValue.text = "" + (_item as TurretDescription).actualTier;
                }
            }
        }

        if (hidderUsed != null && hidderLocked != null && hidderCommingSoon != null)
        {
            manageHidders();
        }
    }

    public void manageHidders()
    {
        hidderUsed.SetActive(false);
        hidderLocked.SetActive(false);
        hidderCommingSoon.SetActive(false);

        if (_item != null && !isLocked && !isUsed) //No hiders
        {
            return;
        }
        if (_item != null && isLocked) //Hidder locked
        {
            hidderLocked.SetActive(true);
            return;
        }
        if (_item != null && !isLocked && isUsed) //Hidder used
        {
            hidderUsed.SetActive(true);
            return;
        }
        if (_item == null) //Hidder comming soon
        {
            hidderCommingSoon.SetActive(true);
            return;
        }
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClickEvent(this);
        }
        else if (eventData != null && eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClickEvent(this);
        }
    }*/

    public void unlockItem()
    {
        Debug.Log("hi");
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if(isLocked && playerStats.energyCoreStored >= unlockEnergyCorePrice)
        {
            playerStats.updateEnergyCoreStoredValue(playerStats.energyCoreStored -= unlockEnergyCorePrice);
            isLocked = false;
            manageHidders();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //originalPosition = image.transform.position;
        OnDragEvent(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //image.transform.position = originalPosition;
        OnBeginingDragEvent(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //image.transform.position = Input.mousePosition;
        OnEndingDragEvent(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnDropEvent(this);
    }
}
