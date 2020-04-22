using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //Associated object
    public StarMapManagement starMapManager;
    public EnnemySpawner ennemySpawner;
    public Camera battleCamera;
    public Camera starmapCamera;

    [Space]

    //Game stats
    public Sector sectorPlayer; //Sector where the player actually is
    public int scraps;
    public int nbrSectorExplored;

    private void OnValidate()
    {
        //Find associated objects in the game environment
        starMapManager = FindObjectOfType<StarMapManagement>();
        ennemySpawner = FindObjectOfType<EnnemySpawner>();

        sectorPlayer = starMapManager.startSector;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set Cameras
        battleCamera.enabled = false;
        starmapCamera.enabled = true;

        //Launch starMap
        //starMapManager.InitializeStarMap();
    }

    public void StartBattleSector()
    {
        //Set objects states
        ennemySpawner.wavesToPlay = sectorPlayer.waves;
        battleCamera.enabled = true;
        starmapCamera.enabled = false;
        ennemySpawner.StartBattle();
    }

    public void EndBattleSector()
    {
        Debug.Log("End");

        //Reset objets states 
        sectorPlayer.SwitchSprite("Explored player");
        sectorPlayer.sectorIsExplored = true;
        battleCamera.enabled = false;
        starmapCamera.enabled = true;

        //Reset timers
        ennemySpawner.timerEndSector = 4;
        ennemySpawner.timerStartSector = 4;

        //Reset counter
        ennemySpawner.ennemyCount = 0;
        ennemySpawner.ennemyDestroyed = 0;
        ennemySpawner.ennemySpawned = 0;
        ennemySpawner.ennemyAliveInWave = 0;
    }
}
