using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float maxStepHeight = 0.5f;
    [SerializeField] private float maxSlopeAngle = 45f;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Get input from joystick
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Move player in move direction, taking collisions and slopes into account
        moveDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        MovePlayer();

        // Rotate player in move direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private void MovePlayer()
    {
        // Move player in move direction, taking collisions and slopes into account
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        // Check for collisions with the ground and slopes
        RaycastHit groundHit;
        if (Physics.Raycast(rb.position, -Vector3.up, out groundHit, maxStepHeight))
        {
            // Check if the ground is a slope
            float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);
            if (slopeAngle <= maxSlopeAngle)
            {
                // Check if there's a slope in front of the player
                RaycastHit slopeHit;
                if (Physics.Raycast(rb.position + Vector3.up * maxStepHeight, movement.normalized, out slopeHit, 1f))
                {
                    // Check if the slope is below the max slope angle
                    float slopeAngle2 = Vector3.Angle(Vector3.up, slopeHit.normal);
                    if (slopeAngle2 <= maxSlopeAngle)
                    {
                        // Check if the slope is a step
                        if (slopeHit.distance <= maxStepHeight)
                        {
                            // Move up the step
                            movement += Vector3.up * (maxStepHeight - slopeHit.distance);
                        }
                        else
                        {
                            // Move up the slope
                            movement += ProjectOnPlane(movement, slopeHit.normal);
                        }
                    }
                }
            }
        }

        rb.MovePosition(rb.position + movement);
    }

    private Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
    {
        return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
    }

    private void OnDrawGizmos()
    {
        // Draw the ground and slope raycasts in the scene view
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -Vector3.up * maxStepHeight);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up * maxStepHeight, transform.forward);
    }
}
