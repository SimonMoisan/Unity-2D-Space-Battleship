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
    public Image cooldownImage1;
    public Image cooldownImage2;
    public Image cooldownImage3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shieldBar.fillAmount = shield.shieldPoints / shield.maxShieldPoints;
        hullBar.fillAmount = ship.hullPoints / ship.MaxHullPoints;
        cooldownImage1.fillAmount = ship.arsenal[0].GetCooldownFactor();
        cooldownImage2.fillAmount = ship.arsenal[1].GetCooldownFactor();
        cooldownImage3.fillAmount = ship.arsenal[2].GetCooldownFactor();
    }
}
