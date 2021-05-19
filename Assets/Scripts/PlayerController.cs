using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // public section
    public Camera cam;
    public NavMeshAgent agent;

    public uint life = 100;

    public TextMeshProUGUI deathText;
    public Slider healthBar;
    public GameObject castPoint;



    //private section

    private Rigidbody playerRB;
    private Animator anim;
    private Collider enemyCollider;

    private bool isHit = false;

    private StateMachine _stateMachine;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        _stateMachine = new StateMachine();
        

        // CONDITIONS
      
        Func<bool> mouseClick() => () => Input.GetMouseButtonDown(0);                                                                           // T1
        Func<bool> stopping() => () => agent.remainingDistance <= agent.stoppingDistance;                                                       // T2
        Func<bool> isDeath() => () => life == 0;
        Func<bool> shootingSpell() => () => Input.GetMouseButtonDown(1);
        Func<bool> stopShootingSpell() => () => !Input.GetMouseButtonDown(1);

        // STATES

        var idle = new IdleState(anim);
        var moving = new MovingState(this, anim, agent, cam);
        var death = new DeathState(anim);
        var shoot = new ShootingState(anim, cam, agent, this, castPoint);

        

        // TRANSITIONS

        _stateMachine.AddTransition(idle, moving, mouseClick());
        _stateMachine.AddTransition(moving, idle, stopping());
        _stateMachine.AddTransition(idle, shoot, shootingSpell());
        _stateMachine.AddTransition(shoot, idle, stopShootingSpell());

        _stateMachine.AddAnyTransition(death, isDeath());

        // START STATE

        _stateMachine.SetState(idle);

        // DEBUG STATE MACHINE

//        _stateMachine.transitionState();
//        _stateMachine.dictionaryState();
    }

    private void Update()
    {
        OnDeath();
        healthBar.value = life;
        _stateMachine.Tick();
        enemyDeath();
    }

    private void enemyDeath()
    {
        if(enemyCollider == null)
        {
            return;
        }

        if (enemyCollider.gameObject.GetComponent<ChasePlayer>().imDead == true)
        {
            Debug.Log("Enemy is dead!");
            isHit = false;
            StopCoroutine(takeDamage());
        }
    }

    void OnDeath()
    {
        if(life == 0)
        {
            StopAllCoroutines();
            deathText.gameObject.SetActive(true);
        }
    }

    IEnumerator takeDamage()
    {
        while (life != 0 && isHit)
        {
            life -= 5;

            yield return new WaitForSeconds(1);
        }     
    }


    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Spider"))
        {
            enemyCollider = other;


            Debug.Log("Exit damage");
            isHit = false;        
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Spider"))
        {
            isHit = true;
            StartCoroutine(takeDamage());
            agent.updateRotation = false;
            agent.ResetPath();
        }
    }
}
