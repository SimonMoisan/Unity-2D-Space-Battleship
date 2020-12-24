using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats : ")]
    public bool isPlaying;
    public int scrapsStored;
    public int energyCoreStored;
    public Turret turretActualySelected;

    [Header("Player related main objetcts : ")]
    public Battleship battleship;
    public Shield shield;

    [Header("Indicators : ")]
    public Image[] hullBars;
    public Image[] shieldBars;
    public Text[] hullIndicators;
    public Text[] shieldIndicators;
    public Image overdriveIndicator;
    public Image[] cooldownImages;
    public Text[] scrapTextIndicators;
    public Text[] energyCoreTextIndicators;

    private void OnValidate()
    {
        updateScrapStoredValue(scrapsStored);
        updateEnergyCoreStoredValue(energyCoreStored);

        updateHullIndicators();
        updateShieldIndicators();
        updateOverdriveIndicator();

        turretActualySelected = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        battleship = FindObjectOfType<Battleship>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < battleship.standardTurrets.Length; i++)
        {
            if(battleship.standardTurrets[i] != null)
            {
                cooldownImages[i].fillAmount = battleship.standardTurrets[i].GetCooldownFactor();
            }
        }
    }

    public void updateScrapStoredValue(int newValue)
    {
        scrapsStored = newValue;

        for (int i = 0; i < scrapTextIndicators.Length; i++)
        {
            scrapTextIndicators[i].text = "x " + scrapsStored;
        }
    }

    public void updateEnergyCoreStoredValue(int newValue)
    {
        energyCoreStored = newValue;

        for (int i = 0; i < energyCoreTextIndicators.Length; i++)
        {
            energyCoreTextIndicators[i].text = "x " + energyCoreStored;
        }
    }

    public void updateHullIndicators()
    {
        for (int i = 0; i < hullBars.Length; i++)
        {
            hullBars[i].fillAmount = battleship.hullPoints / battleship.MaxHullPoints;
        }
        
        for (int i = 0; i < hullIndicators.Length; i++)
        {
            hullIndicators[i].text = "" + battleship.hullPoints + " / " + battleship.MaxHullPoints;
        }
    }

    public void updateShieldIndicators()
    {
        for (int i = 0; i < shieldBars.Length; i++)
        {
            shieldBars[i].fillAmount = shield.shieldPoints / shield.maxShieldPoints;
        }
        
        for (int i = 0; i < shieldIndicators.Length; i++)
        {
            shieldIndicators[i].text = "" + shield.shieldPoints + " / " + shield.maxShieldPoints;
        }
    }

    public void updateOverdriveIndicator()
    {
        overdriveIndicator.fillAmount = battleship.actualOverdrive / battleship.maxOverdrive;
    }
}
