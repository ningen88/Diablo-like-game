using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;



public class ShootingState : IState
{
    //private section
    private readonly PlayerController _player;
    private readonly Animator _anim;
    private readonly Camera _cam;
    private readonly NavMeshAgent _agent;
    private readonly GameObject _castPoint;

    

    private RaycastHit hitInfo;
    private float castSpeed = 10.0f;
    private float rotSpeed = 180.0f;
    private GameObject spell;
    private bool isShooting = false;
    
    private MonoBehaviour mono; 


    public ShootingState(Animator anim, Camera cam, NavMeshAgent agent, PlayerController player, GameObject castPoint)
    {
        _anim = anim;
        _cam = cam;
        _agent = agent;
        _player = player;
        _castPoint = castPoint;
    }
    public void Tick()
    {
        Ray mousePos = _cam.ScreenPointToRay(Input.mousePosition);
        

        if (Physics.Raycast(mousePos, out hitInfo))
        {
            if (hitInfo.collider.CompareTag("Spider") && !isShooting)
            {
                isShooting = true;
                

                Vector3 targetDir = hitInfo.collider.transform.position - _player.transform.position;
                float step = rotSpeed * Time.deltaTime;

                Quaternion lookDir = Quaternion.LookRotation(targetDir, Vector3.up);
                _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, lookDir, step);


                _anim.SetInteger("Shoot", 1);

                mono.StartCoroutine(castSpell());
            }
        }
    }

    public void FixTick()
    {

    }

    public void OnEnter()
    {
        mono = GameObject.FindObjectOfType<MonoBehaviour>();                                                                     // this is slow!
        _agent.updateRotation = true;
    }

    public void OnExit()
    {
        _anim.SetInteger("Shoot", 0);
        _agent.updateRotation = false;
    }

    IEnumerator castSpell()
    {
        spell = ObjectPooler.Instance.getObjectFromPool();
           
        if(spell != null && isShooting)
        {
            spell.SetActive(true);
            spell.transform.position = _castPoint.transform.position;
            spell.GetComponent<Rigidbody>().AddForce(_castPoint.transform.forward * castSpeed, ForceMode.Impulse);

            spell.GetComponent<Rigidbody>().velocity = _castPoint.transform.forward * castSpeed;

            yield return new WaitForSeconds(1.4f);

            isShooting = false;
        }      
    }
}
