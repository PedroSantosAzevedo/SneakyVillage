using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMeshComponent : MonoBehaviour
{

    // Components
    public CharacterController controller;
    public Animator characterAnimator;
 

    // Movement speed and rotation speed
    public float moveSpeed;
    public float rotateSpeed = 5f;
    public Vector3 movement = Vector3.zero;


    /*  // Dash speed and duration
      public float dashSpeed = 10f;
      public float dashDuration = 0.5f;
      public float verticalVelocity;

      // Dash cooldown
      public float dashCooldown = 2f;
      private float dashTimer;*/


    // Gravity 
    public float gravity = 9.81f;
    public bool isGrounded;

    //jump
    public bool isJumpPressed = false;
    public float maxJumpHeight = 1.0f;
    public float maxJumpTime = 0.5f;
    public bool isJumping = false;
    public float initialJumpVelocity;
    public float jumpRayCast;

    //Player Input

    public bool isMoving = false;

    public NavMeshAgent agent;
    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        CheckForGroundRaycast();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(MoveCharaterToLocation());
    }



    IEnumerator MoveCharaterToLocation() {

        while (true)
        {

            if (target != null)
            {
                Vector3 direction = new Vector3(target.position.x, 0f, target.position.z);
                agent.SetDestination(direction);
                characterAnimator.SetBool("isWalking", true);
                Debug.Log("andou");
            }
            else
            {
                characterAnimator.SetBool("isWalking", false);
                Debug.Log("parou de andar");
            }

            yield return new WaitForSeconds(moveSpeed);

        }
    }

    void Update()
    {
       
       // handleGravity();
     
    }

    private void FixedUpdate()
    {
       // RotatePlayerTowardsDirectionOfMovement();

    }


    void handleGravity()
    {

        if (isGrounded && !isJumpPressed)
        {
            movement.y = -9.5f * Time.deltaTime;
        }
        else
        {
            movement.y += gravity * Time.deltaTime;
        }
    }

    void CheckForGroundRaycast()
    {
        int groundLayers = ~(1 << LayerMask.NameToLayer("NotGround"));
        isGrounded = Physics.Raycast(transform.position, Vector3.down, jumpRayCast, groundLayers);

    }

    void RotatePlayerTowardsDirectionOfMovement()
    {
        Vector3 direction = new Vector3(target.transform.position.x, 0f, target.transform.position.z);

        if (direction.magnitude > 0.5f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
