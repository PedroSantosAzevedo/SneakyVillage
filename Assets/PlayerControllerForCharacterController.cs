
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

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
    private float horizontalInput;
    private float verticalInput;


    // Dash speed and duration
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float verticalVelocity;

    // Dash cooldown
    public float dashCooldown = 2f;
    private float dashTimer;


    // Dash button
    public KeyCode dashButton = KeyCode.Space;

    // Jump button
    public KeyCode jumpButton = KeyCode.Space;

    // Gravity 
    public float gravity = 9.81f;
    public bool isGrounded;

    public bool isJumpPressed = false;
    public float maxJumpHeight = 1.0f;
    public float maxJumpTime = 0.5f;
    public bool isJumping = false;
    public float initialJumpVelocity;

/*    private float jumpTimer;
    public float jumpDuration;
    public float jumpHeight;
    public float initialJumpVelocity;*/

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

        if (isMoving && !isWalking)
        {
            characterAnimator.SetBool("isWalking", true);
        }
        else if (!isMoving && isWalking) {
            characterAnimator.SetBool("isWalking", false);
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
        if (isJumpPressed && controller.isGrounded && !isJumping)
        {
            isJumping = true;
            movement.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && controller.isGrounded && isJumping)
        {
            isJumping = false;
        }


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
        CheckIfDashButtonPressedAndDashTimerExpired();
        handlerAnimation();
    }

    private void FixedUpdate()
    {
        RotatePlayerTowardsDirectionOfMovement();
       
       
        MovePlayerBasedOnInputAxes();

        UpdateDashTimerAndCooldown();
       
        handleGravity();
        handleJump();
    }

    // Get the input axes for movement and rotatio

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
    
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }


    void handleGravity() {
        if (isGrounded)
        {
            movement.y += -9.5f * Time.deltaTime;
        }
        else 
        {
            movement.y += gravity * Time.deltaTime;
            Debug.Log("no chao");
        }
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
}
