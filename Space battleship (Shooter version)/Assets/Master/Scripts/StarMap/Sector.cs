using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SectorType { Normal, Start, End }
public enum SectorStatus { Explored, Unexplored, ExploredPlayer, UnexploredPlayer }

public class Sector : MonoBehaviour
{
    [Header("Sector Parameters :")]
    public SectorType sectorType; //start, end, battle, store, event, empty
    public SectorStatus sectorStatus;
    public Collider2D[] closestSectors;
    public List<Sector> linkedSectors;
    public List<SectorLink> inputLinks;
    public List<SectorLink> outputLinks;
    public SectorLink linkPrefab;
    public float maxRange;
    public float minRange;

    public LayerMask whatIsSector;
    public bool playerIsPresent; //Indicate if the player is present on this specific sector
    public bool sectorIsExplored;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public SpriteRenderer shipLogo;
    public SpriteRenderer dangerIndicator;
    public SpriteRenderer stationIndicator;

    [Header("Associated objects :")]
    //Associated objects
    public MenuManagerScript menuManager;
    public GameManagerScript gameManager;
    public EnnemySpawner ennemySpawner;
    public StarMapManagement starMapManager;
    public StarmapEvent sectorEvent;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
        ennemySpawner = FindObjectOfType<EnnemySpawner>();
        starMapManager = FindObjectOfType<StarMapManagement>();
        menuManager = FindObjectOfType<MenuManagerScript>();

        maxRange = starMapManager.maxDistanceBtwSectorsLink;
        minRange = starMapManager.minDistanceBtwSectorsLink;

        spriteRenderer = GetComponent<SpriteRenderer>();
        starMapManager = FindObjectOfType<StarMapManagement>();
        whatIsSector = starMapManager.whatIsSector;
    }

    void OnMouseDown()
    {
        if(playerIsPresent && !sectorIsExplored && menuManager.sectorClickable) //The player is already in this sector and wants to start this sector if not already explored
        {
            menuManager.enterStationButton.SetActive(false); //Reset before starting event

            gameManager.playNextStepEvent(); //Play first step event
        }
        else
        {
            for (int i = 0; i < linkedSectors.Count; i++)
            {
                if (linkedSectors[i].playerIsPresent && (linkedSectors[i].sectorIsExplored || sectorIsExplored)) //If is next to the sector the player is already present and this sector has been explored
                {
                    //Change sprite of the next sector
                    if (!sectorIsExplored) //if the next sector is unexplored
                    {
                        SwitchSprite(SectorStatus.UnexploredPlayer);
                        gameManager.sectorPlayer = this;
                        playerIsPresent = true;
                        linkedSectors[i].playerIsPresent = false;
                    }
                    else //The next sector is already explored
                    {
                        SwitchSprite(SectorStatus.ExploredPlayer);
                        gameManager.sectorPlayer = this;
                        playerIsPresent = true;
                        linkedSectors[i].playerIsPresent = false;
                    }

                    //Change sprite of previous sector
                    if(linkedSectors[i].sectorIsExplored)
                    {
                        linkedSectors[i].SwitchSprite(SectorStatus.Explored);
                    }
                    else
                    {
                        linkedSectors[i].SwitchSprite(SectorStatus.Unexplored);
                    }
                }
            }
        }
    }

    //Return a list of closest sectors, can be used to create links to those closest sectors
    public List<Sector> findClosestSectors(bool linkSectors)
    {
        if(sectorType == SectorType.End)
        {
            closestSectors = Physics2D.OverlapCircleAll(transform.position, (minRange - 0.001f) * 1.5f, whatIsSector);
        }
        else
        {
            closestSectors = Physics2D.OverlapCircleAll(transform.position, minRange - 0.001f, whatIsSector);
        }
        

        List<Sector> result = new List<Sector>();
        for (int i = 0; i < closestSectors.Length; i++)
        {
            if(linkSectors)
            {
                createLink(closestSectors[i].GetComponent<Sector>());
            }
            result.Add(closestSectors[i].GetComponent<Sector>());
        }
        return result;
    }

    public SectorLink createLink(Sector sector)
    {
        //Check if a link is already present or not
        if (!linkAlreadyPresent(sector) && sector.transform.position.y > transform.position.y)
        {
            Vector3 linkPosition = new Vector3(Mathf.Abs(sector.transform.position.x - transform.position.x), Mathf.Abs(sector.transform.position.y - transform.position.y), 0);
            SectorLink linkGO = Instantiate(linkPrefab, linkPosition, Quaternion.identity);
            linkGO.initiateLink(this, sector.GetComponent<Sector>());
            //Add in associated objects of this sector
            outputLinks.Add(linkGO);
            linkedSectors.Add(sector);
            //Add in associated objects of the other sector
            sector.outputLinks.Add(linkGO);
            sector.linkedSectors.Add(this);

            return linkGO;
        }
        return null;
    }

    public SectorLink linkAlreadyPresent(Sector otherSector)
    {
        for (int i = 0; i < outputLinks.Count; i++)
        {
            if(outputLinks[i].destinationSector == otherSector)
            {
                return outputLinks[i];
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        
        if (sectorType == SectorType.End)
        {
            Gizmos.DrawWireSphere(transform.position, (minRange - 0.001f) * 1.5f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, minRange);
        }
    }

    //Function used to switch sprite of a sector, 0 : explored, 1 : explored player, 2 : unexplored, 3 : unexplored player
    public void SwitchSprite(SectorStatus newSectorStatus)
    {
        int index = 0;

        sectorStatus = newSectorStatus;
        switch (newSectorStatus)
        {
            case SectorStatus.Explored:
                shipLogo.enabled = false;
                index = 0;
                break;
            case SectorStatus.ExploredPlayer:
                shipLogo.enabled = true;
                index = 0;
                break;
            case SectorStatus.Unexplored:
                shipLogo.enabled = false;
                index = 1;
                break;
            case SectorStatus.UnexploredPlayer:
                shipLogo.enabled = true;
                index = 1;
                break;
        }
        spriteRenderer.sprite = sprites[index];
    }
}
