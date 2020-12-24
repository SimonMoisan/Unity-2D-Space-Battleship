using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    [Header("Associated objects :")]
    public StarMapManagement starMapManager;
    public EnnemySpawner ennemySpawner;
    public MenuManagerScript menuManager;
    public Battleship battleship;
    public PlayerStats playerStats;
    public Camera battleCamera;
    public Camera starmapCamera;
    public Animator crossfaderAnimator;
    public StoryReader storyReader;
    [Space]
    [Header("Game states :")]
    public Sector sectorPlayer; //Sector where the player actually is
    public bool isInBattle;
    [Space]
    [Header("Game stats :")]
    public bool isInBattlezone;
    public int nbrSectorExplored;

    public static GameManagerScript current;

    private void OnValidate()
    {
        current = this;

        //Find associated objects in the game environment
        starMapManager = FindObjectOfType<StarMapManagement>();
        ennemySpawner = FindObjectOfType<EnnemySpawner>();
        menuManager = FindObjectOfType<MenuManagerScript>();
        battleship = FindObjectOfType<Battleship>();
        playerStats = FindObjectOfType<PlayerStats>();
        storyReader = FindObjectOfType<StoryReader>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set Cameras
        battleCamera.enabled = false;
        starmapCamera.enabled = true;

        sectorPlayer = starMapManager.startSector;
        //Launch starMap
        //starMapManager.InitializeStarMap();
    }

    public void playNextStepEvent()
    {
        int eventStepIndex = sectorPlayer.sectorEvent.actualEventStepIndex;
        menuManager.contextualCanvas.enabled = false;

        if (eventStepIndex < sectorPlayer.sectorEvent.eventSteps.Length)
        {
            EventStep eventStepToPlay = sectorPlayer.sectorEvent.eventSteps[sectorPlayer.sectorEvent.actualEventStepIndex];

            switch (eventStepToPlay.eventStepType) //Play first step of the sector event
            {
                case EventStepType.battleEventStep:
                    startBattleEvent(eventStepToPlay);
                    break;

                case EventStepType.contextualEventStep:
                    StartCoroutine(startContextualEvent(eventStepToPlay));
                    break;

                case EventStepType.stationEventStep:
                    startStationEvent(eventStepToPlay);
                    break;
            }

            sectorPlayer.sectorEvent.actualEventStepIndex++;
        }
        else //No more step to play, go back to starmap
        {
            sectorPlayer.sectorIsExplored = true;
            StartCoroutine(fadeBattlezoneToStarmap());
        }
    }

    ///////// Battle step event /////////

    public void startBattleEvent(EventStep eventStepToPlay)
    {
        isInBattle = true;
        playerStats.isPlaying = true;

        Debug.Log(eventStepToPlay.waves.Count);
        ennemySpawner.wavesToPlay = eventStepToPlay.waves;

        StartCoroutine(fadeStarmapToBattlezone());

        ennemySpawner.StartBattle();
    }

    public void endBattleEvent()
    {
        //Display information panel
        menuManager.informationWindowEndBattle.SetActive(true);
        isInBattle = false;
        playerStats.isPlaying = false;

        //Reset timers
        ennemySpawner.timerEndSector = 4;
        ennemySpawner.timerStartSector = 4;

        //Reset counter
        ennemySpawner.ennemyCount = 0;
        ennemySpawner.ennemyDestroyed = 0;
        ennemySpawner.ennemySpawned = 0;
        ennemySpawner.ennemyAliveInWave = 0;

        //Reset cooldowns on battleship
        battleship.reset();
    }

    ///////// Contextual step event /////////

    IEnumerator startContextualEvent(EventStep eventStepToPlay)
    {
        if(!isInBattlezone)
        {
            StartCoroutine(fadeStarmapToBattlezone());
            isInBattlezone = true;
        }
        yield return new WaitForSeconds(1.2f);

        menuManager.contextualCanvas.enabled = true;
        storyReader.inkJSONAsset = eventStepToPlay.storyJson;
        storyReader.StartStory();
    }

    ///////// Station step event /////////

    public void startStationEvent(EventStep eventStepToPlay)
    {
        menuManager.enterStationButton.SetActive(true);
    }

    public void restObjectsStates()
    {
        Debug.Log("reset");
        sectorPlayer.SwitchSprite(SectorStatus.ExploredPlayer);
        sectorPlayer.sectorIsExplored = true;

        StartCoroutine(fadeBattlezoneToStarmap());

        menuManager.startMapCanvas.SetActive(true);
    }

    IEnumerator fadeStarmapToBattlezone()
    {
        crossfaderAnimator.Play("Crossfade-Anim-Appear");
        yield return new WaitForSeconds(1.2f);
        crossfaderAnimator.Play("Crossfade-Anim-Disappear");
        battleCamera.enabled = true;
        starmapCamera.enabled = false;
        menuManager.starmapToBattleZone();
        isInBattlezone = true;
    }

    IEnumerator fadeBattlezoneToStarmap()
    {
        crossfaderAnimator.Play("Crossfade-Anim-Appear");
        yield return new WaitForSeconds(1.2f);
        crossfaderAnimator.Play("Crossfade-Anim-Disappear");
        battleCamera.enabled = false;
        starmapCamera.enabled = true;
        menuManager.battlezoneToStarmap();
        isInBattlezone = false;
    }
}
