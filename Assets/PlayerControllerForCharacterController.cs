
using UnityEngine;
using System.Collections;

public class PlayerControllerForCharacterController : MonoBehaviour
{
    // Character Controller component
    private CharacterController controller;

    // Movement speed and rotation speed
    public float moveSpeed;
    public float rotateSpeed = 5f;


    // Dash speed and duration
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float verticalVelocity;

    public float jumpHeight;
    public bool isJumping = false;

    // Dash cooldown
    public float dashCooldown = 2f;
    private float dashTimer;

    // Input axes for movement and rotation
    private float horizontalInput;
    private float verticalInput;

    // Dash button
    public KeyCode dashButton = KeyCode.Space;

    // Jump button
    public KeyCode jumpButton = KeyCode.Space;

    // Gravity 
    public float gravity = 9.81f;
    public bool isGrounded;

    private float jumpTimer;
    public float jumpDuration;
   

    // Start is called before the first frame update
    void Start()
    {
        // Get the Character Controller component
        controller = GetComponent<CharacterController>();
        isGrounded = controller.isGrounded;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGroundRaycast();
        GetInputAxes();
        CheckIfDashButtonPressedAndDashTimerExpired();
    }

    private void FixedUpdate()
    {
        RotatePlayerTowardsDirectionOfMovement();
        MovePlayerBasedOnInputAxes();
        
        CheckIfJumpButton();
        UpdateDashTimerAndCooldown();
    }

    // Get the input axes for movement and rotation
    void GetInputAxes()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    //Rotate the player towards the direction of movement
    void RotatePlayerTowardsDirectionOfMovement()
    {
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput);

        if (direction.magnitude > 0.5f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void MovePlayerBasedOnInputAxes()
    {

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        Vector3 velocity = direction * moveSpeed;

        if (isGrounded)
        {

            ///verticalVelocity = -1f;
            if (Input.GetKeyDown(jumpButton))
            {
                verticalVelocity = jumpHeight;
                Debug.Log("pulou");
            }
        }
        else if (jumpTimer <= 0)
        {
            verticalVelocity = -gravity;
            Debug.Log("no chao");
        }
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    void CheckForGround() 
    {
        isGrounded = controller.isGrounded;
    }

    void CheckForGroundRaycast()
    {
        int groundLayers = ~(1 << LayerMask.NameToLayer("NotGround"));
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f, groundLayers);

    }

    // Check if the dash button is pressed and the dash timer has expired
    void CheckIfDashButtonPressedAndDashTimerExpired()
    {
        if (Input.GetKeyDown(dashButton) && dashTimer <= 0f)
        {
            // Start the dash coroutine
            StartCoroutine(Dash());
        }
    }

    void CheckIfJumpButton()
    {
        if (Input.GetKeyDown(jumpButton) && isGrounded)
        {
            // Start the dash coroutine
            Debug.Log("jump");
            StartCoroutine(Jump());
        }
    }

    // Update the dash timer and cooldown
    void UpdateDashTimerAndCooldown()
    {
        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }
        else if (dashTimer <= 0f)
        {
            dashTimer = 0f;
        }
    }

    // Coroutine for dashing
    IEnumerator Dash()
    {
        // Set the dash timer and disable movement
        dashTimer = dashCooldown;
        float timer = dashDuration;
        //moveSpeed = dashSpeed;

        // Dash in the direction of movement
        Vector3 dashDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        while (timer > 0f)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        // Reset the movement speed
        //moveSpeed = 5f;
    }

    IEnumerator Jump()
    {
        // Set the dash timer and disable movement
        jumpTimer = dashCooldown;
        //das = dashDuration;

        // Dash in the direction of movement
        Vector3 dashDirection = new Vector3(horizontalInput, jumpHeight, horizontalInput).normalized;
        while (jumpTimer > 0f)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            isJumping = true;
            jumpTimer -= Time.deltaTime;
            yield return null;
        }

        
        yield return null;
    }
}
