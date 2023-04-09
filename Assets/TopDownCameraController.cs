using UnityEngine;
using Cinemachine;

public class TopDownCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float cameraFollowSpeed = 5f;
    [SerializeField] private float cameraHeight = 10f;
    [SerializeField] private float cameraDistance = 10f;
    [SerializeField] private float cameraAngle = 45f;

    private CinemachineTransposer virtualCameraTransposer;
    private Vector3 targetCameraPosition;

    private void Awake()
    {
        virtualCameraTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void FixedUpdate()
    {
        // Calculate target camera position and rotation
        Vector3 cameraOffset = Quaternion.Euler(cameraAngle, 0f, 0f) * new Vector3(0f, cameraHeight, -cameraDistance);
        targetCameraPosition = transform.position + cameraOffset;
        Quaternion targetCameraRotation = Quaternion.Euler(cameraAngle, 0f, 0f);

        // Move camera position towards target position
        virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraFollowSpeed);
        //virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraFollowSpeed);


        // Set camera rotation to target rotation
        //virtualCamera.transform.rotation = targetCameraRotation;
    }
}