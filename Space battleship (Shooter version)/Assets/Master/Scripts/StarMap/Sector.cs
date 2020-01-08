using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    [Header("Sector Parameters :")]
    public string sectorType; //start, end, battle, store, event, empty
    public Collider2D[] closestSectors;
    public List<Sector> linkedSectors;
    public List<SectorLink> inputLinks;
    public List<SectorLink> outputLinks;
    public StarMapManagement mapManager;
    public SectorLink linkPrefab;
    public float maxRange;
    public float minRange;

    public LayerMask whatIsSector;
    public bool playerIsPresent; //Indicate if the player is present on this specific sector
    public bool sectorIsExplored;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public Wave waves; //Waves presents in this sectors

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mapManager = FindObjectOfType<StarMapManagement>();
        maxRange = mapManager.maxDistanceBtwSectorsLink;
        minRange = mapManager.minDistanceBtwSectorsLink;
        whatIsSector = mapManager.whatIsSector;
    }

    void OnMouseDown()
    {
        if(playerIsPresent) //The player is already in this sector
        {
            sectorIsExplored = true;
            SwitchSprite("Explored player");
        }
        else
        {
            if(linkedSectors.Count == 0)
            {
                Debug.Log("No linked sectors");
            }

            for (int i = 0; i < linkedSectors.Count; i++)
            {
                if (linkedSectors[i].playerIsPresent && linkedSectors[i].sectorIsExplored) //If is next to the sector the player is already present and this sector has been explored
                {
                    if(!sectorIsExplored) //if the next sector is unexplored
                    {
                        SwitchSprite("Unexplored player");
                    }
                    else //The next sector is already explored
                    {
                        SwitchSprite("Explored player");
                    }
                    playerIsPresent = true;
                    linkedSectors[i].playerIsPresent = false;
                    linkedSectors[i].SwitchSprite("Explored");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Return a list of closest sectors, can be used to create links to those closest sectors
    public List<Sector> findClosestSectors(bool linkSectors)
    {
        if(sectorType == "End")
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

            outputLinks.Add(linkGO);
            linkedSectors.Add(sector);
            return linkGO;
        }
        return null;
    }

    public bool linkAlreadyPresent(Sector otherSector)
    {
        for (int i = 0; i < outputLinks.Count; i++)
        {
            if(this.outputLinks[i].destinationSector == otherSector)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        

        if (sectorType == "End")
        {
            Gizmos.DrawWireSphere(transform.position, (minRange - 0.001f) * 1.5f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, minRange);
        }
    }

    //Function used to switch sprite of a sector, 0 : explored, 1 : explored player, 2 : unexplored, 3 : unexplored player
    public void SwitchSprite(string type)
    {
        int index = 0;
        switch (type)
        {
            case "Explored":
                index = 0;
                break;
            case "Explored player":
                index = 1;
                break;
            case "Unexplored":
                index = 2;
                break;
            case "Unexplored player":
                index = 3;
                break;
        }
        spriteRenderer.sprite = sprites[index];
    }
}
