using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameManagerScript : MonoBehaviour
{
    [Header("Camera management :")]
    public Camera battleZoneCamera;
    public CinemachineVirtualCamera battleZoneCM;
    public GameObject groupTarget;
    public Vector2 battlezoneCameraInitialPos;
    public Camera starmapCamera;
    [Space]
    [Header("Game states :")]
    public Sector sectorPlayer; //Sector where the player actually is
    public bool isInBattle;
    [Space]
    [Header("Game stats :")]
    public bool isInBattlezone;
    public int nbrSectorExplored;
    [Header("Other objects :")]
    public Animator crossfaderAnimator;
    public ProjectilePool[] poolers;

    public static GameManagerScript current;

    private void OnValidate()
    {
        current = this;
        poolers = GetComponentsInChildren<ProjectilePool>();
    }

    private void Awake()
    {
        battlezoneCameraInitialPos = battleZoneCM.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set Cameras
        battleZoneCamera.enabled = false;
        starmapCamera.enabled = true;

        sectorPlayer = StarMapManagement.current.startSector;
        //Launch starMap
        //starMapManager.InitializeStarMap();

        //////// Physic management ////////
        //Disable physics for cameraColliders
        Physics.IgnoreLayerCollision(11, 0);
        Physics.IgnoreLayerCollision(11, 8);
        Physics.IgnoreLayerCollision(11, 9);
        Physics.IgnoreLayerCollision(11, 10);
    }

    private void Update()
    {
        if(PlayerStats.current.isPlaying)
        {
            battleZoneCM.Follow = groupTarget.transform;
        }
        else
        {
            battleZoneCM.Follow = null;
            battleZoneCM.transform.position = battlezoneCameraInitialPos;
        }
    }

    public void playNextStepEvent(int nextStepIndex)
    {
        if (nextStepIndex < sectorPlayer.sectorEvent.eventSteps.Length)
        {
            EventStep eventStepToPlay = sectorPlayer.sectorEvent.eventSteps[nextStepIndex];

            switch (eventStepToPlay.eventStepType) //Play first step of the sector event
            {
                case EventStepType.battleEventStep:
                    MenuManagerScript.current.contextualCanvas.enabled = false;
                    startBattleEvent(eventStepToPlay);
                    break;

                case EventStepType.contextualEventStep:
                    StartCoroutine(startContextualEvent(eventStepToPlay));
                    break;

                case EventStepType.stationEventStep:
                    sectorPlayer.stationUnlocked = true;
                    startStationEvent(eventStepToPlay);
                    break;
            }

            sectorPlayer.sectorEvent.actualEventStepIndex = nextStepIndex;
        }
        else //No more step to play, go back to starmap
        {
            MenuManagerScript.current.contextualCanvas.enabled = false;
            sectorPlayer.sectorIsExplored = true;
            StartCoroutine(fadeBattlezoneToStarmap());
            AudioManager.current.StartCoroutine(AudioManager.current.musicSwitch(MusicType.Starmap));
            restObjectsStates();
        }
    }

    ///////// Battle step event /////////

    public void startBattleEvent(EventStep eventStepToPlay)
    {
        if (!isInBattlezone)
        {
            StartCoroutine(fadeStarmapToBattlezone());
            isInBattlezone = true;
        }
        AudioManager.current.StartCoroutine(AudioManager.current.musicSwitch(MusicType.BattlezoneAction));

        isInBattle = true;
        PlayerStats.current.isPlaying = true;

        EnnemySpawner.current.wavesToPlay = (eventStepToPlay as BattleEventStep).waves;
        EnnemySpawner.current.StartBattle();
    }

    public void endBattleEvent()
    {
        //Display information panel
        MenuManagerScript.current.informationWindowEndBattle.SetActive(true);
        isInBattle = false;
        PlayerStats.current.isPlaying = false;

        //Reset timers
        EnnemySpawner.current.timerEndSector = 4;
        EnnemySpawner.current.timerStartSector = 4;

        //Reset counter
        EnnemySpawner.current.ennemyCount = 0;
        EnnemySpawner.current.ennemyDestroyed = 0;
        EnnemySpawner.current.ennemySpawned = 0;
        EnnemySpawner.current.ennemyAliveInWave = 0;

        //Reset cooldowns on battleship
        Battleship.current.resetBattleship();

        //Get reward
        PlayerStats.current.updateScrapStoredValue(PlayerStats.current.scrapsStored + EnnemySpawner.current.scrapsToWin);
        PlayerStats.current.updateEnergyCoreStoredValue(PlayerStats.current.energyCoreStored + EnnemySpawner.current.energyCoreToWin);
    }

    ///////// Contextual step event /////////

    IEnumerator startContextualEvent(EventStep eventStepToPlay)
    {
        //Set background
        if((eventStepToPlay as ContextualEventStep).contextualBackgroundMaterial != null)
        {
            Material[] newMaterial = new Material[1];
            newMaterial[0] = (eventStepToPlay as ContextualEventStep).contextualBackgroundMaterial;
            MenuManagerScript.current.backgroundMesh.materials = newMaterial;

            MenuManagerScript.current.backgroundMesh.transform.localScale = (eventStepToPlay as ContextualEventStep).contextualBackgroundScaleRatios;
        }

        if(!isInBattlezone)
        {
            StartCoroutine(fadeStarmapToBattlezone());
            isInBattlezone = true;
        }
        AudioManager.current.StartCoroutine(AudioManager.current.musicSwitch(MusicType.BattlezoneCalm));

        yield return new WaitForSeconds(1.2f);

        MenuManagerScript.current.contextualCanvas.enabled = true;
        StoryReader.current.actualEventStep = eventStepToPlay;
        StoryReader.current.StartStory();
    }

    ///////// Station step event /////////

    public void startStationEvent(EventStep eventStepToPlay)
    {
        Debug.Log("Station unlocked");
        MenuManagerScript.current.enterStationButton.SetActive(true);
        //StartCoroutine(fadeBattlezoneToStarmap());
    }

    public void restObjectsStates()
    {
        sectorPlayer.switchSprite(SectorStatus.ExploredPlayer);
        sectorPlayer.sectorIsExplored = true;

        MenuManagerScript.current.startMapCanvas.SetActive(true);
    }

    public IEnumerator fadeStarmapToBattlezone()
    {
        crossfaderAnimator.Play("Crossfade-Anim-Appear");
        yield return new WaitForSeconds(1.2f);
        crossfaderAnimator.Play("Crossfade-Anim-Disappear");
        battleZoneCamera.enabled = true;
        starmapCamera.enabled = false;
        MenuManagerScript.current.starmapToBattleZone();
        isInBattlezone = true;
    }

    public IEnumerator fadeBattlezoneToStarmap()
    {
        crossfaderAnimator.Play("Crossfade-Anim-Appear");
        yield return new WaitForSeconds(1.2f);
        crossfaderAnimator.Play("Crossfade-Anim-Disappear");
        battleZoneCamera.enabled = false;
        starmapCamera.enabled = true;
        MenuManagerScript.current.battlezoneToStarmap();
        isInBattlezone = false;
    }
}
