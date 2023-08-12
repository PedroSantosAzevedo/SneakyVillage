
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.IO;

public class PlayerControllerForCharacterController : MonoBehaviour
{
    // Components
    private CharacterController controller;
    private Animator characterAnimator;
    private PlayerInput playerInput;

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

    //dash
    public bool isDashPressed = false;
    public float dashMultiplier;
    public float dashDuration;
    public bool isDashing = false;
    private float dashTimer;
    public float dashCooldown;
    public Vector3 dashDirection;


    //Player Input

    public bool isMoving = false;


    private void Awake()
    {
        playerInput = new PlayerInput();
        characterAnimator = GetComponent<Animator>();

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;
        playerInput.CharacterControls.Dash.started += onDash;
        playerInput.CharacterControls.Dash.canceled += onDash;


    }

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
        setJumpVariables();
        CheckForGroundRaycast();
        handlerAnimation();

        handleGravity();
        handleJump();
        handleDash();
    }

    private void FixedUpdate()
    {
        RotatePlayerTowardsDirectionOfMovement();
        MovePlayerBasedOnInputAxes();
        UpdateDashTimerAndCooldown();
        
    }

    void setJumpVariables() {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight / timeToApex);
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void handlerAnimation() {
        bool isWalking = characterAnimator.GetBool("isWalking");
        bool isRunning = characterAnimator.GetBool("isRunning");
       

        if (isMoving && !isWalking && !isDashing)
        {
            characterAnimator.SetBool("isWalking", true);
        }
        else if (!isMoving && isWalking && !isDashing)
        {
            characterAnimator.SetBool("isWalking", false);
        }
        else if (isDashing) {
            characterAnimator.SetBool("isRolling", true);
        }
        else if (!isDashing)
        {
            characterAnimator.SetBool("isRolling", false);
        }
    }

    void onMovementInput(InputAction.CallbackContext context) {
   
        movement.x = context.ReadValue<Vector2>().x;
        movement.z = context.ReadValue<Vector2>().y;
        isMoving = movement.x != 0 || movement.z != 0;
    }

    void onJump(InputAction.CallbackContext context) {
        isJumpPressed = context.ReadValueAsButton();
    }

    void handleJump() {
        if (isJumpPressed && isGrounded && !isJumping)
        {
            isJumping = true;
            movement.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && isGrounded && isJumping)
        {
            isJumping = false;
        }
    }

    void onDash(InputAction.CallbackContext context) {
        isDashPressed = context.ReadValueAsButton();
    }

    void handleDash() {

        if (!isJumping && isDashing)
        {
            controller.Move(dashDirection * dashMultiplier * Time.deltaTime);
        }

        if (isDashPressed && !isJumping && !isDashing && dashTimer <= 0) {
            StartCoroutine(setDash());
        }
    }

    IEnumerator setDash() {
        isDashing = true;
        dashDirection = movement;
        SetDashCooldown();
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        dashDirection = Vector3.zero;
    }

    //Rotate the player towards the direction of movement
    void RotatePlayerTowardsDirectionOfMovement()
    {
        Vector3 direction = new Vector3(movement.x, 0f, movement.z);
      
        if (direction.magnitude > 0.5f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void MovePlayerBasedOnInputAxes()
    {
        if (!isDashing)
        {
            controller.Move(movement * moveSpeed * Time.deltaTime);
        }
    }


    void handleGravity() {

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
/*
    // Check if the dash button is pressed and the dash timer has expired
    void CheckIfDashButtonPressedAndDashTimerExpired()
    {
        if (Input.GetKeyDown(dashButton) && dashTimer <= 0f)
        {
            // Start the dash coroutine
            StartCoroutine(Dash());
        }
    }*/

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

    void SetDashCooldown() {
        dashTimer = dashCooldown;
    }

   /* // Coroutine for dashing
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
    }*/
}
