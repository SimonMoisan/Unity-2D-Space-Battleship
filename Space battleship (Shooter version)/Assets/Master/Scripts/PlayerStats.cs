using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Player related main objetcts : ")]
    public Battleship battleship;
    public Shield shield;

    [Header("Indicators : ")]
    public Image shieldBar;
    public Image hullBar;
    public Image[] cooldownImages;
    public Text scrapTextIndicator;

    //Scraps
    public int scraps;

    // Start is called before the first frame update
    void Start()
    {
        battleship = FindObjectOfType<Battleship>();
    }

    // Update is called once per frame
    void Update()
    {
        
        shieldBar.fillAmount = shield.shieldPoints / shield.maxShieldPoints;
        hullBar.fillAmount = battleship.hullPoints / battleship.MaxHullPoints;

        for (int i = 0; i < battleship.standardTurrets.Length; i++)
        {
            if(battleship.standardTurrets[i] != null)
            {
                cooldownImages[i].fillAmount = battleship.standardTurrets[i].GetCooldownFactor();
            }
        }
    }

    public void updateScrapValue(int newValue)
    {
        scraps = newValue;
        scrapTextIndicator.text = "Scraps : " + scraps;
    }
}
