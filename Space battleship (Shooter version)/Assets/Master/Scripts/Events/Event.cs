using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType { contextualEvent, battleEvent, shopEvent }
public class Event : MonoBehaviour
{
    [Header("Caracteritics :")]
    public EventType eventType;
}
