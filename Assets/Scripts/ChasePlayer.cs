using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class ChasePlayer : MonoBehaviour
{
    // public section
    public List<GameObject> destinations;
    public HelthBar healthBar;
    public bool imDead { get; private set; }

    // private section
    private NavMeshAgent agent;
    private Rigidbody wolfRB;
    private GameObject player;
    private float speed = 6.0f;
    private float pushImpulse = 1000.0f;
    private float rotationSpeed = 90.0f;
    Vector3 playerDirection;
    Vector3 nodeDirection;

    private Animator anim;
    private float chaseDistance = 22.0f;
    private float attackDistance = 1.5f;

    private int health = 50;

    private Stack<GameObject> currentPath;




 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        wolfRB = GetComponent<Rigidbody>();

        healthBar.SetMaxHealth(health);

        navigateToTarget();
        StartCoroutine(movingCoroutine());
    }

    void Update()
    {
        onDeath();
    }



    IEnumerator chasePlayer()
    {

        Stop();
        StopCoroutine(movingCoroutine());

        anim.SetBool("Walk Forward", false);
        anim.SetBool("Run Forward", true);
        anim.ResetTrigger("Stab Attack");
        agent.speed = 3.5f;

        while (Vector3.Distance(transform.position, player.transform.position) > 0.5f)
        {
            Vector3 playerPosition = player.transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            agent.destination = playerPosition;

            if (distanceToPlayer > chaseDistance)
            {
                break;
            }

            if (distanceToPlayer > attackDistance)
            {
                break;
            }

            if (distanceToPlayer < attackDistance)
            {
                yield return attackCoroutine();
            }

            yield return null;
        }
    }

    // Is better to use Observer pattern for hit damage???

    IEnumerator attackCoroutine()
    {
        StopCoroutine(chasePlayer());

        anim.SetBool("Run Forward", false);
        anim.SetTrigger("Stab Attack");
        agent.speed = 0;

        yield return new WaitForSeconds(1);

    }


    IEnumerator movingCoroutine()
    {
        StopCoroutine(chasePlayer());

        anim.SetBool("Run Forward", false);
        anim.SetBool("Walk Forward", true);
        agent.speed = 1.5f;

        foreach (GameObject node in currentPath)
        {
            Vector3 nodePosition = node.transform.position;
            nodeDirection = (nodePosition - transform.position).normalized;

            while (Vector3.Distance(nodePosition, transform.position) > 0.5f)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanceToPlayer < chaseDistance)
                {
                    
                    yield return chasePlayer();
                    break;
                }
                else
                {
                    agent.destination = nodePosition;
                    yield return null;
                }
            }
        }

        navigateToTarget();
        yield return movingCoroutine();      
    }



    GameObject searchNearestWaypoint(GameObject currentWaypoint)
    {
        float finalDistance = Mathf.Infinity;                                                         // set the final distance to infinity. The final distance will be the
        GameObject nextWaypoint = null;                                                               // the closest to currentWaypoint

        foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("WaypointW"))
        {
            float distance = Vector3.Distance(currentWaypoint.transform.position, waypoint.transform.position);      // evaluate the distance between a neighbor waypoint and the currentWaypoint

            if (distance < finalDistance)                                                            // if the actual distance is lower than the final distance
            {
                nextWaypoint = waypoint;                                                             // set the next waypoint to the actual waypoint
                finalDistance = distance;                                                            // set the new distance to the new discovered value
            }
            else if (distance == finalDistance)                                                       // skip the waypoint if the distance is equal
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



    public void navigateToTarget()
    {
        currentPath = new Stack<GameObject>();
        GameObject currentWaypoint = searchNearestWaypoint(gameObject);
        GameObject target = searchNearestWaypoint(destinations[Random.Range(0,8)]);

        if (currentWaypoint == null || target == null || currentWaypoint == target)
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

        if (currentWaypoint == target)
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

    void onDeath()
    {
        if (health == 0)
        {
            transform.position = new Vector3(10000, -10000, 10000);
             
            StartCoroutine(waitAndDestroy());
            Stop();
//            StopAllCoroutines();
        }
    }

    IEnumerator waitAndDestroy()
    {
        Debug.Log("Starting destroy enemy...");
        yield return new WaitForSeconds(2);

        gameObject.SetActive(false);

        Destroy(gameObject);
        Debug.Log("Enemy destroyed!");
    }

    private void OnDestroy()
    {
        imDead = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spell1"))
        {
            StartCoroutine(chasePlayer());
            health -= 10;
            healthBar.SetHealth(health);
            Debug.Log("Spider health: " + health);
        }
    }
}
