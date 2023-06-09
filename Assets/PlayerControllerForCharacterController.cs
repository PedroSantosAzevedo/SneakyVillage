
using UnityEngine;
using System.Collections;

public class PlayerControllerForCharacterController : MonoBehaviour
{
    // Character Controller component
    private CharacterController controller;

    // Movement speed and rotation speed
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;


    // Dash speed and duration
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;

    // Dash cooldown
    public float dashCooldown = 2f;
    private float dashTimer;

    // Input axes for movement and rotation
    private float horizontalInput;
    private float verticalInput;

    // Dash button
    public KeyCode dashButton = KeyCode.Space;

    // Gravity 
    public float gravity = 9.81f;
    public bool isGrounded;

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
        GetInputAxes();

    }

    private void FixedUpdate()
    {
        //RotatePlayerTowardsDirectionOfMovement();
        MovePlayerBasedOnInputAxes();
        CheckIfDashButtonPressedAndDashTimerExpired();
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

    // Move the player based on the input axes
    void MovePlayerBasedOnInputAxes()
    {
        isGrounded = controller.isGrounded;
        Vector3 movement = Vector3.zero;
        if (!controller.isGrounded)
        {
            movement.y -= gravity * Time.deltaTime;
            controller.Move(movement * Time.deltaTime);

        }



        Vector3 moveDirection = new Vector3(horizontalInput, movement.y, verticalInput).normalized;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
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
        moveSpeed = dashSpeed;

        // Dash in the direction of movement
        Vector3 dashDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        while (timer > 0f)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        // Reset the movement speed
        moveSpeed = 5f;
    }
}
