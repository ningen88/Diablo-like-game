using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    // public section
    public List<GameObject> destinations;
    

    // private section
    private Rigidbody sheepRB;
    private Vector3 movementDirection;
    private NavMeshAgent agent;

    private Stack<GameObject> currentPath;

    private Animator anim;



    // Start is called before the first frame update
    void Start()
    {
        sheepRB = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        navigateToRandomTarget();
        StartCoroutine(moveCoroutine());
    }



    // searchNearestWaypoint return the waypoint nearest to currentWaypoint
    GameObject searchNearestWaypoint(GameObject currentWaypoint)
    {
        float finalDistance = Mathf.Infinity;                                                         // set the final distance to infinity. The final distance will be the
        GameObject nextWaypoint = null;                                                               // the closest to currentWaypoint
                                                                                              
        foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))                                                    
        {
            float distance = Vector3.Distance(currentWaypoint.transform.position, waypoint.transform.position);      // evaluate the distance between a neighbor waypoint and the currentWaypoint

            if (distance < finalDistance)                                                            // if the actual distance is lower than the final distance
            {
                nextWaypoint = waypoint;                                                             // set the next waypoint to the actual waypoint
                finalDistance = distance;                                                            // set the new distance to the new discovered value
            }
            else if(distance == finalDistance)                                                       // skip the waypoint if the distance is equal
            {
                continue;
            }    
        }

        if (nextWaypoint != null)                                                                    // make sure that we have find the next waypoint
        {
            return nextWaypoint;
        }
        else 
        {
            return null;
        }
    }


    IEnumerator moveCoroutine()
    {
        agent.speed = 1.5f;
        foreach (GameObject node in currentPath)
        {
            Vector3 nodePosition = node.transform.position;
            movementDirection = (nodePosition - transform.position).normalized;

            while (Vector3.Distance(transform.position, node.transform.position) > 1.0f)
            {
                anim.SetBool("Walk", true);

                agent.destination = nodePosition;

                yield return null;
            }            
        }

        anim.SetBool("Walk", false);
    }


    public void navigateToRandomTarget()
    {
        currentPath = new Stack<GameObject>();
        GameObject destination = destinations[Random.Range(0,4)];
        GameObject currentWaypoint = searchNearestWaypoint(gameObject);
        GameObject target = searchNearestWaypoint(destination);

        if(currentWaypoint == null || target == null || currentWaypoint == target)
        {
            return;
        }

        var openList = new SortedList<float, GameObject>();
        var closedList = new List<GameObject>();
        openList.Add(0, currentWaypoint);
        currentWaypoint.GetComponent<WaypointState>().previous = null;
        currentWaypoint.GetComponent<WaypointState>().distance = 0.0f;

        while (openList.Count > 0)
        {
            currentWaypoint = openList.Values[0];
            openList.RemoveAt(0);
            float dist = currentWaypoint.GetComponent<WaypointState>().distance;
            closedList.Add(currentWaypoint);

            if (currentWaypoint == target)
            {
                break;
            }

            foreach (var neighbor in currentWaypoint.GetComponent<WaypointState>().neighbors)
            {
                if (closedList.Contains(neighbor) || openList.ContainsValue(neighbor))
                {
                    continue;
                }

                neighbor.GetComponent<WaypointState>().previous = currentWaypoint;
                neighbor.GetComponent<WaypointState>().distance = dist + Vector3.Distance(neighbor.transform.position, currentWaypoint.transform.position);
                var distanceToTarget = Vector3.Distance(neighbor.transform.position, target.transform.position);
                openList.Add(neighbor.GetComponent<WaypointState>().distance + distanceToTarget, neighbor);
            }
        }

        if(currentWaypoint == target)
        {
            while (currentWaypoint.GetComponent<WaypointState>().previous != null)
            {
                currentPath.Push(currentWaypoint);
                currentWaypoint = currentWaypoint.GetComponent<WaypointState>().previous;
            }
            currentPath.Push(gameObject);
        }
        
    }

    public void Stop()
    {
        currentPath = null;
    }
}
