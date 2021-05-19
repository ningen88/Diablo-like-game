using UnityEngine;
using UnityEngine.AI;

public class IdleState : IState
{

    // private section
    private readonly Animator _animator;


    public IdleState(Animator animator)
    {
        _animator = animator;

    }

    public void Tick()
    {

    }

    public void FixTick()
    {

    }

    public void OnEnter()
    {
        _animator.SetInteger("Run", 0);
        _animator.SetInteger("Shoot", 0);
    }

    public void OnExit()
    {

    }
}
