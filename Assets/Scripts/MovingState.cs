using UnityEngine;
using UnityEngine.AI;

public class MovingState : IState
{
    // private section
    private readonly PlayerController _player;
    private readonly Animator _animator;
    private readonly NavMeshAgent _agent;
    private readonly Camera _cam;




    private Plane playerPlane;
    private Ray mousePos;
    private Vector3 targetPoint;

    public MovingState(PlayerController player, Animator animator, NavMeshAgent agent, Camera cam)
    {
        _player = player;
        _animator = animator;
        _agent = agent;
        _cam = cam;
    }

    public void Tick()
    {
        
        float dist;

        // If we click again the mouse button we have to recalculate the position

        if (Input.GetMouseButton(0))                                             
        {
            playerPlane = new Plane(Vector3.up, _player.transform.position);
            mousePos = _cam.ScreenPointToRay(Input.mousePosition);
        }

        if (playerPlane.Raycast(mousePos, out dist))
        {
            targetPoint = mousePos.GetPoint(dist);
            _agent.destination = targetPoint;
        }
    }

    public void FixTick()
    {

    }

    public void OnEnter()
    {
        playerPlane = new Plane(Vector3.up, _player.transform.position);
        mousePos = _cam.ScreenPointToRay(Input.mousePosition);

        _agent.isStopped = false;
        _agent.updateRotation = true;
        _agent.speed = 10.0f;
        _animator.SetInteger("Run", 1);
    }

    public void OnExit()
    {
        _agent.isStopped = true;
        _agent.updateRotation = false;
        _agent.speed = 0;
        _animator.SetInteger("Run", 0);
    }
}
