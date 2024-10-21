using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof)]
public class EnemyController : MonoBehaviour
{
    #region Variables
    private CharacterController controller;
    [SerializeField]
    private LayerMask groundLayerMask;
    public float gravity = -9.81f;

    private NavMeshAgent agent;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    //private Animator animator;

    //readonly int moveHash = Animator.StringToHash("Move");
    //readonly int fallingHash = Animator.StringToHash("Falling");

    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GetComponent<ControllerCharacter>().gameObject;

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.transform.position);

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            controller.Move(agent.velocity * Time.deltaTime);
            //animator.SetBool(moveHash, true);
        }
        else
        {
            controller.Move(Vector3.zero);
            //animator.SetBool(moveHash, false);
        }

        if (agent.isOnOffMeshLink)
        {

        }
    }
}
