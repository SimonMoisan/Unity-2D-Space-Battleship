using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CargoItemSlot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] Image image;
    [SerializeField] protected CargoItem _item;
    public int slotId;

    Vector2 originalPosition;

    public event Action<CargoItemSlot> OnRightClickEvent;
    public event Action<CargoItemSlot> OnLeftClickEvent;
    public event Action<CargoItemSlot> OnBeginingDragEvent;
    public event Action<CargoItemSlot> OnEndingDragEvent;
    public event Action<CargoItemSlot> OnDragEvent;
    public event Action<CargoItemSlot> OnDropEvent;

    private Color normalColor = Color.white;
    private Color disableColor = new Color(1,1,1,0);

    public CargoItem CargoItem
    {
        get { return _item; }
        set
        {
            _item = value;
            if(_item == null)
            {
                image.color = disableColor;
            }
            else
            {
                image.sprite = _item.cargoIcone;
                image.color = normalColor;
            }
        }
    }

    protected virtual void OnValidate()
    {
        if(image == null)
        {
            image = GetComponent<Image>();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClickEvent(this);
        }
        else if (eventData != null && eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClickEvent(this);
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
