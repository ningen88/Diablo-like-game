using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDestroy : MonoBehaviour
{

    // private section
    private Vector3 actualPosition;
    private Vector3 playerPosition;
    private float distance;
    private float maxLimit = 30.0f;
    private GameObject player;

    // public section

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        actualPosition = transform.position;
        playerPosition = player.transform.position;
        distance = Vector3.Distance(playerPosition, actualPosition);

        if(distance > maxLimit)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spider"))
        {
            gameObject.SetActive(false);
        }
    }

}
