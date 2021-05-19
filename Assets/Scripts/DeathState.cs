using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : IState
{
    // private section
    private readonly Animator _anim;


    public DeathState(Animator anim)
    {
        _anim = anim;
    }

    public void Tick()
    {

    }

    public void FixTick()
    {

    }

    public void OnEnter()
    {
        Debug.Log("You are dead!");
        _anim.SetBool("Death", true);
    }

    public void OnExit()
    {
        _anim.SetBool("Death", false);
    }
}
