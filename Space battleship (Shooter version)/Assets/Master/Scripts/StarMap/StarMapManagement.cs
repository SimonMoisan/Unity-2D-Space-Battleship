using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMapManagement : MonoBehaviour
{
    [Header("Level parameters")]
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public float xOffset;
    public float yOffset;
    public List<Sector> sectors;
    public Sector startSector;
    public Sector endSector;
    private int index;

    [Header("Sectors spawn parameters")]
    public int sectorNumber;
    public int minSectorNumber;
    public int maxSectorNumber;
    public int numberOfShop;

    [Header("WaveLinks parameters")]
    public float maxDistanceBtwSectorsLink;
    public float minDistanceBtwSectorsLink;
    public LayerMask whatIsSector;

    [Header("Waves parameters")]
    public Wave[] waves; //Every possible waves in this system
    public int minNbrWave;
    public int maxNbrWave;

    [Header("Prefabs")]
    public Sector sectorPrefab;
    public BattleEvent battleEventPrefab;
    public ContextualEvent contextualEventPrefab;
    public ShopEvent shopEventPrefab;
    public SectorLink linkPrefab;

    // Start is called before the first frame update
    void Start()
    {
        InitializeStarMap();
    }

    public void InitializeStarMap()
    {
        index = 2;
        sectorNumber = Random.Range(minSectorNumber, maxSectorNumber);;

        sectors.Add(startSector);
        sectors.Add(endSector);

        //Attribute waves to start and end sector
        //attributeEventToSector(startSector);
        //attributeEventToSector(endSector);

        generateSectorsRandomly();
        createLinksBtwSectors();
        //generateSectorRecursively(startSector, 0);
    }

    public void generateSectorRecursively(Sector startingSector, int compteur)
    {
        List<Sector> closesrSectors = sectors[1].findClosestSectors(false);

        //if not close enought to the end sector
        if (!closesrSectors.Contains(startingSector))
        {
            int nbSectorGenerated = Random.Range(1, 1);
            for (int i = 0; i < nbSectorGenerated; i++) //number of sectors generated from the starting sector
            {
                if (compteur < sectorNumber)
                {
                    //First iteration
                    if(startingSector == startSector)
                    {

                    }


                    Collider2D[] closestSectors = Physics2D.OverlapCircleAll(startingSector.transform.position, minDistanceBtwSectorsLink - 0.4f, whatIsSector);

                    //if no sectors too close
                    if(closestSectors.Length <= 1)
                    {
                        int randomAngleDegree = Random.Range(60, 120);
                        float randomDistance = Random.Range(minDistanceBtwSectorsLink, maxDistanceBtwSectorsLink);
                        float newSectorX = startingSector.transform.position.x + randomDistance * Mathf.Cos(randomAngleDegree * Mathf.Deg2Rad);
                        float newSectorY = startingSector.transform.position.y + randomDistance * Mathf.Sin(randomAngleDegree * Mathf.Deg2Rad);

                        
                        Vector3 newSectorPosition = new Vector3(newSectorX, newSectorY, 0);
                        Sector newSectorGO = Instantiate(sectorPrefab, newSectorPosition, Quaternion.identity);
                        //newSectorGO.transform.parent = gameObject.transform;
                        SectorLink linkGO = startingSector.createLink(newSectorGO);
                        //linkGO.transform.parent = gameObject.transform;

                        //Attribute waves to this new Sector
                        attributeEventToSector(newSectorGO);

                        //Fill lists of sector and links
                        if (!newSectorGO.linkedSectors.Contains(startingSector))
                        {
                            newSectorGO.linkedSectors.Add(startingSector);
                        }
                        if(!newSectorGO.inputLinks.Contains(linkGO))
                        {
                            newSectorGO.inputLinks.Add(linkGO);
                        }
                        if (!startingSector.linkedSectors.Contains(newSectorGO))
                        {
                            startingSector.linkedSectors.Add(newSectorGO);
                        }
                        if (!startingSector.inputLinks.Contains(linkGO))
                        {
                            startingSector.inputLinks.Add(linkGO);
                        }

                        compteur++;
                        generateSectorRecursively(newSectorGO, compteur);
                    }
                }
            }
        }
        else
        {
            SectorLink lastLinkGO = startingSector.createLink(endSector);
            //Fill lists of sector and links for the two lasts sectors
            if (!endSector.linkedSectors.Contains(startingSector))
            {
                endSector.linkedSectors.Add(startingSector);
            }
            if (!endSector.inputLinks.Contains(lastLinkGO))
            {
                endSector.inputLinks.Add(lastLinkGO);
            }
            if (!startingSector.linkedSectors.Contains(endSector))
            {
                startingSector.linkedSectors.Add(endSector);
            }
            if (!startingSector.inputLinks.Contains(lastLinkGO))
            {
                startingSector.inputLinks.Add(lastLinkGO);
            }
        }
    }

    //Function used to attribute waves to a sector
    public void attributeEventToSector(Sector sector)
    {
        int nbrWave = Random.Range(minNbrWave, maxNbrWave);
        List<Wave> wavesToAttribute = new List<Wave>();

        for(int i = 0; i < nbrWave; i++)
        {
            int randomIndex = Random.Range(0, waves.Length - 1);
            if(waves.Length > 0 && !wavesToAttribute.Contains(waves[randomIndex]))
            {
                wavesToAttribute.Add(waves[randomIndex]);
            }
        }
        sector.waves = wavesToAttribute;
    }

    private bool sectorCanBeCreated(Vector2 testPosition)
    {
        for (int i = 0; i < sectors.Count; i++)
        {
            if(Mathf.Abs(Vector2.Distance(testPosition, sectors[i].transform.position)) < minDistanceBtwSectorsLink)
            {
                return false;
            }
        }
        return true;
    }

    private void generateSectorsRandomly()
    {
        for(int i=0; i<sectorNumber; i++)
        {
            int failedSpawn = 0;
            while(failedSpawn < 10)
            {
                Vector2 sectorPosition = new Vector2(Random.Range(xMin + 2, xMax - 2) + xOffset, Random.Range(yMin + 1, yMax - 1) + yOffset);
                if (sectorCanBeCreated(sectorPosition))
                {
                    Sector sectorGO = Instantiate(sectorPrefab, sectorPosition, Quaternion.identity);
                    sectorGO.transform.parent = gameObject.transform;

                    attributeEventToSector(sectorGO);
                    sectors.Add(sectorGO);
                    index++;
                    break;
                }
                else
                {
                    failedSpawn++;
                }
            }
        }
    }

    private void createLinksBtwSectors()
    {
        for (int i = 0; i < sectors.Count; i++)
        {
            for (int j = 0; j < sectors.Count; j++)
            {
                if(sectors[i] != sectors[j] && Vector2.Distance(sectors[i].transform.position, sectors[j].transform.position) < maxDistanceBtwSectorsLink)
                {
                    sectors[i].createLink(sectors[j]);
                }
            }
        }
    }

    //Create links between closest sectors
    void generateLinksBetweenSectors()
    {
        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].findClosestSectors(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 mapRange = new Vector3(xMax - xMin, yMax - yMin, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, mapRange);
    }
}
