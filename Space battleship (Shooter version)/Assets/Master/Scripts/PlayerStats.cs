using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public Battleship ship;
    public Shield shield;

    public Image shieldBar;
    public Image hullBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shieldBar.fillAmount = shield.shieldPoints / shield.maxShieldPoints;
        hullBar.fillAmount = ship.hullPoints / ship.MaxHullPoints;
    }
}
