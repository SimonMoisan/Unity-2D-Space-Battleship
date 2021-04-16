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

    [Header("Player related main objetcts : ")]
    public Shield shield;

    [Header("Indicators : ")]
    public Image[] hullBars;
    public Image[] shieldBars;
    public Text[] hullIndicators;
    public Text[] shieldIndicators;
    public Image overdriveGauge;
    public Text overdriveIndicator;
    [Space]
    public Image[] rotationTurretGauges;
    public Image[] standardTurretSetIndicators;
    public Image[] heavyTurretSetIndicators;
    public Sprite noSetSprite;
    public Sprite[] turretSetSpritesIndicator; //0 : 1, 1 : 2, 2 : 3
    public Sprite[] turretSetSpritesHighlightIndicator; //0 : 1, 1 : 2, 2 : 3
    public GameObject[] turretSets;
    public GameObject actualTurretSet;
    public Image[] turretsImages;
    public Image[] frontalTurretsGauges;
    [Space]
    public Text[] scrapTextIndicators;
    public Text[] energyCoreTextIndicators;

    public static PlayerStats current;

    private void OnValidate()
    {
        current = this;

        updateScrapStoredValue(scrapsStored);
        updateEnergyCoreStoredValue(energyCoreStored);

        updateHullIndicators();
        updateShieldIndicators();
        updateOverdriveIndicator();
    }

    private void Awake()
    {
        initializeRotationTurretsIndicators();

        //Initialize turret sets
        StartCoroutine(initialiseTurretSets());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Battleship.current.standardRotationTurrets.Length; i++)
        {
            rotationTurretGauges[i].fillAmount = Battleship.current.standardRotationTurrets[i].GetCooldownFactor();
        }

        int chainId = -1;
        for (int i = 0; i < Battleship.current.allFrontalTurrets.Length; i++)
        {
            if (Battleship.current.allFrontalTurrets[i].idTurret != chainId)
            {
                chainId = Battleship.current.allFrontalTurrets[i].idTurret;
            }
            frontalTurretsGauges[chainId].fillAmount = Battleship.current.allFrontalTurrets[i].GetCooldownFactor();
        }
    }

    public void initializeRotationTurretsIndicators()
    {
        int totalNbrOfSets = Battleship.current.nbrStandardRotationTurretSets + Battleship.current.nbrHeavyRotationTurretSets;

        //Check which sets are available
        for (int i = 0; i < 3; i++)
        {
            if(i >= Battleship.current.nbrStandardRotationTurretSets)
            {
                standardTurretSetIndicators[i].sprite = noSetSprite;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if(i >= Battleship.current.nbrHeavyRotationTurretSets)
            {
                heavyTurretSetIndicators[i].sprite = noSetSprite;
            }
        }

        //Set indicator for standard turrets set by default
        standardTurretSetIndicators[0].sprite = turretSetSpritesHighlightIndicator[0];
    }

    public IEnumerator initialiseTurretSets()
    {
        for (int i = 1; i < turretSets.Length; i++)
        {
            turretSets[i].GetComponent<Animator>().Play("Out");
        }
        yield return new WaitForSeconds(0.2f);

        turretSets[0].GetComponent<Animator>().Play("In");
    }

    public void changeSet(int id, SetTypeActive setTypeActive)
    {
        //Animate sets
        GameObject previousTurretSet = actualTurretSet;
        if(setTypeActive == SetTypeActive.Standard)
        {
            actualTurretSet = turretSets[id];
        }
        else
        {
            actualTurretSet = turretSets[id + 3];
        }
        previousTurretSet.GetComponent<Animator>().Play("Out");
        actualTurretSet.GetComponent<Animator>().Play("In");

        //Change indicator
        for (int i = 0; i < standardTurretSetIndicators.Length; i++)
        {
            if (i < Battleship.current.nbrStandardRotationTurretSets)
            {
                standardTurretSetIndicators[i].sprite = turretSetSpritesIndicator[i];
            }
            else
            {
                standardTurretSetIndicators[i].sprite = noSetSprite;
            }
        }

        for (int i = 0; i < heavyTurretSetIndicators.Length; i++)
        {
            if (i < Battleship.current.nbrHeavyRotationTurretSets)
            {
                heavyTurretSetIndicators[i].sprite = turretSetSpritesIndicator[i];
            }
            else
            {
                heavyTurretSetIndicators[i].sprite = noSetSprite;
            }
        }
        
        if (setTypeActive == SetTypeActive.Standard)
        {
            standardTurretSetIndicators[id].sprite = turretSetSpritesHighlightIndicator[id];
        }
        else
        {
            heavyTurretSetIndicators[id].sprite = turretSetSpritesHighlightIndicator[id];
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
            hullBars[i].fillAmount = Battleship.current.hullPoints / Battleship.current.MaxHullPoints;
        }
        
        for (int i = 0; i < hullIndicators.Length; i++)
        {
            hullIndicators[i].text = "" + Battleship.current.hullPoints.ToString("F0") + " / " + Battleship.current.MaxHullPoints.ToString("F0");
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
            shieldIndicators[i].text = "" + shield.shieldPoints.ToString("F0") + " / " + shield.maxShieldPoints.ToString("F0");
        }
    }

    public void updateOverdriveIndicator()
    {
        overdriveGauge.fillAmount = Battleship.current.actualOverdrive / Battleship.current.maxOverdrive;
        overdriveIndicator.text = Battleship.current.actualOverdrive.ToString("F0") + "/" + Battleship.current.maxOverdrive.ToString("F0");
    }
}
