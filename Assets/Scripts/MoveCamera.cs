using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // public section
    public GameObject player;


    // private section
    private Vector3 offset = new Vector3(5.0f, 20.0f, -12.5f);
 

    // Update is called once per frame
    void Update()
    {

        transform.position = player.transform.position + offset;
    }

}
