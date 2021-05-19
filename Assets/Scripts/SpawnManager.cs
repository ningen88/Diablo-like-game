using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // public section
    public GameObject insect;
    public GameObject sheep;

    // private section
    private float xSpawnRangeSheep = 20.0f;
    private float zSpawnRangeSheep = 20.0f;
    private float xSpawnInsect;
    private float zSpawnInsect;

    private float spawnDelay = 10.0f;
    private float spawnTime = 2.0f;
    private int maxInsect = 10;
    private int numInsect = 0;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnInsect", spawnTime, spawnDelay);
    }




    // spawn a spider in a random spawn position
    void spawnInsect()
    {
        int selectSpawn = Random.Range(0, 4);
        Vector3 position;

        if (selectSpawn == 0)
        {
            xSpawnInsect = -45.0f;
            zSpawnInsect = -5.0f;
        }
        else if (selectSpawn == 1)
        {
            xSpawnInsect = 5.0f;
            zSpawnInsect = -45.0f;

        }
        else if(selectSpawn == 2)
        {
            xSpawnInsect = 45.0f;
            zSpawnInsect = -8.0f;
        }
        else if(selectSpawn == 3)
        {
            xSpawnInsect = 8.0f;
            zSpawnInsect = 45.0f;
        }

        
        if (numInsect < maxInsect)
        {
            numInsect++;
            position = new Vector3(xSpawnInsect, 0.5f, zSpawnInsect);
            Instantiate(insect, position, insect.transform.rotation);
        }
        
    }

 
    void spawnSheep()
    {
        Vector3 position = getRandomPosition(xSpawnRangeSheep, xSpawnRangeSheep, zSpawnRangeSheep, zSpawnRangeSheep);

        Instantiate(sheep, position, sheep.transform.rotation);
    }

    Vector3 getRandomPosition(float xRangeMin, float xRangeMax, float zRangeMin, float zRangeMax)
    {
        float posX = Random.Range(-xRangeMin, xRangeMax);
        float posZ = Random.Range(-zRangeMin, zRangeMax);

        Vector3 position = new Vector3(posX, 0.5f, posZ);

        return position;
    }
}
