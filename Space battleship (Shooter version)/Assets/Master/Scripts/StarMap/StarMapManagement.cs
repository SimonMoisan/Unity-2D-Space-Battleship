using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMapManagement : MonoBehaviour
{
    [Header("Level parameters")]
    public float xSize;
    public float ySize;
    public int minSectorNumber;
    public int maxSectorNumber;
    private int sectorNumber;
    public Sector[] sectors;
    public Sector startSector;
    public Sector endSector;
    private int index;

    public Sector sectorPrefab;
    public SectorLink linkPrefab;

    public float maxDistanceBtwSectorsLink;
    public float minDistanceBtwSectorsLink;
    public LayerMask whatIsSector;

    [Header("Waves parameters")]
    public Wave[] waves; //Every possible waves in this system
    public int minNbrWave;
    public int maxNbrWave;
 
    // Start is called before the first frame update
    void Start()
    {
        InitializeStarMap();
    }

    public void InitializeStarMap()
    {
        index = 2;
        sectorNumber = Random.Range(minSectorNumber, maxSectorNumber);
        sectors = new Sector[sectorNumber + 2];

        sectors[0] = startSector;
        sectors[1] = endSector;

        //Attribute waves to start and end sector
        //attributeWaveToSector(startSector);
        attributeWaveToSector(endSector);

        //generateSectorsFullRandom();
        generateSectorRecursively(startSector, 0);
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
                        attributeWaveToSector(newSectorGO);

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
    public void attributeWaveToSector(Sector sector)
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

    bool sectorCanBeCreated(Transform positionToTest, float rangeBtwSectors)
    {
        //Check if this position is out of bound
        if(positionToTest.position.x >= xSize / 2 || positionToTest.position.x <= -xSize / 2 ||
           positionToTest.position.y >= ySize / 2 || positionToTest.position.y >= -ySize / 2)
        {
            return false;
        }

        //Check if an other is too close from the given position
        Collider2D[] closestSectors = Physics2D.OverlapCircleAll(positionToTest.position, rangeBtwSectors, whatIsSector);
        if(closestSectors.Length > 1) //1 because the sector is counting himself
        {
            return false;
        }

        return true;
    }

    void generateSectorsFullRandom()
    {
        for(int i=0; i<sectorNumber; i++)
        {
            Vector2 sectorPosition = new Vector2(Random.Range((-xSize / 2) +2, (xSize / 2) -2), Random.Range((-ySize / 2)+1, (ySize / 2)-1));
            Sector sectorGO = Instantiate(sectorPrefab, sectorPosition, Quaternion.identity);
            sectorGO.transform.parent = gameObject.transform;

            sectors[index] = sectorGO;
            index++;
        }
    }

    //Create links between closest sectors
    void generateLinksBetweenSectors()
    {
        for (int i = 0; i < sectors.Length; i++)
        {
            sectors[i].findClosestSectors(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 mapRange = new Vector3(xSize, ySize, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, mapRange);
    }
}
