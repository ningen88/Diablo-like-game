using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPooler : MonoBehaviour
{
    // public section
    public List<GameObject> listObjects;
    public int maxPool = 5;
    public GameObject objectToPool;

    public static ObjectPooler Instance;                                                     // using singleton


    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        listObjects = new List<GameObject>();

        for(int i = 0; i < maxPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.SetActive(false);
            listObjects.Add(obj);
        }
    }

    public GameObject getObjectFromPool()
    {
        foreach(GameObject obj in listObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}
