using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EventGenerator : MonoBehaviour
{
    /*[Header("Battle events caracteritics :")]
    public BattleEvent battleEventPrefab;
    public BattleEventList[] battleEvents;
    public WaveList[] possibleWaves; //array of array of wave, sort by level
    [Space]
    [Header("Contextual events caracteritics :")]
    public ContextualEvent contextualEventPrefab;
    public List<ContextualEvent> possibleContextualEvents;
    [Space]
    [Header("Station events caracteritics :")]
    public StationEvent stationEventPrefab;
    public int itemIdCounter;
    public int minNbrShop;
    public int maxNbrShop;
    public int minNbrItems;
    public int maxNbrItems;
    public List<StationEvent> shopEvents;
    public TurretScheme[] possibleTurretScheme;
    [Space]
    [Header("Parents :")]
    public Transform parentBattleEvents;
    public Transform parentContextualEvents;
    public Transform parentShopEvents;

    // Start is called before the first frame update
    void OnValidate()
    {
        battleEvents = new BattleEventList[2];
        for (int i = 0; i < battleEvents.Length; i++)
        {
            battleEvents[i] = new BattleEventList(i+1);
        }
    }

    private void Start()
    {
       
    }

    public void multiBattleEventGeneration()
    {
        generateBattleEvent(1, 2, 3, 50, 100);
        generateBattleEvent(1, 2, 3, 50, 100);
        generateBattleEvent(1, 2, 3, 50, 100);
        generateBattleEvent(1, 2, 3, 50, 100);

        generateBattleEvent(2, 3, 5, 150, 300);
        generateBattleEvent(2, 3, 5, 150, 300);
        generateBattleEvent(2, 3, 5, 150, 300);
        generateBattleEvent(2, 3, 5, 150, 300);
    }

    public void generateShops()
    {
        int numberShop = Random.Range(minNbrShop, maxNbrShop);
        for (int i = 0; i < numberShop; i++)
        {
            generateShopEvent();
        }
    }

    public void generateShopEvent()
    {
        int randomNbrItem = Random.Range(minNbrItems, maxNbrItems);

        StationEvent shopEvent = Instantiate(stationEventPrefab, parentShopEvents);

        for (int i = 0; i < randomNbrItem; i++)
        {
            int randomItemIndex = Random.Range(0, possibleTurretScheme.Length - 1);
            string itemName = possibleTurretScheme[randomItemIndex].ItemName + " (" + itemIdCounter + ")";
            TurretDescritpion turretDescritpion = generateTurretDescription(itemName, possibleTurretScheme[randomItemIndex]);
            shopEvent.turretsToSold.Add(turretDescritpion);
            itemIdCounter++;
        }

        shopEvents.Add(shopEvent);
    }

    public BattleEvent generateBattleEvent(int tier, int minWave, int maxWave, int minDangerIndicator, int maxDangerIndicator)
    {
        List<Wave> wavesToAttribute = new List<Wave>();
        int actualNbrWave = 0;
        int actualDangerIndicator = 0;
        
        //Attribute waves randomly
        while((actualNbrWave < maxWave && actualDangerIndicator < maxDangerIndicator && actualDangerIndicator > minDangerIndicator) || actualNbrWave < minWave)
        {
            int randomWaveTier = Random.Range(1, possibleWaves.Length);
            
            int randomWaveIndex = Random.Range(0, possibleWaves[randomWaveTier - 1].waves.Length - 1);

            Wave randomWave = possibleWaves[randomWaveTier - 1].waves[randomWaveIndex];
            wavesToAttribute.Add(randomWave);

            actualNbrWave++;
            actualDangerIndicator += randomWave.dangerIndicator;
        }

        //Instantiate battle event
        BattleEvent battleEventGO = Instantiate(battleEventPrefab, parentBattleEvents);
        battleEventGO.waves = wavesToAttribute;
        battleEventGO.dangerIndicator = actualDangerIndicator;
        battleEventGO.tier = tier;

        //Add to battleEventsList
        battleEvents[tier - 1].battleEvents.Add(battleEventGO);

        return battleEventGO;
    }

    //Function called to generate a turretDescription from turretSchemes
    public static TurretDescritpion generateTurretDescription(string name, TurretScheme scheme)
    {
        /*TurretDescritpion asset = ScriptableObject.CreateInstance<TurretDescritpion>();
        asset.schemeOrigin = scheme;

        AssetDatabase.CreateAsset(asset, "Assets/Master/Turrets/Turrets Descriptions/Turrets from Shop/" + name + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
        
        return asset;
        return null;
    }*/
}