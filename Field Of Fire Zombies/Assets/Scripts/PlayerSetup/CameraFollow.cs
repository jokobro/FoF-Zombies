using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    private Vector3 lastCameraPosition;
    private bool hasInitialized = false;

    private void Start()
    {
        if (cameraPosition != null)
        {
            transform.position = cameraPosition.position;
            lastCameraPosition = cameraPosition.position;
            hasInitialized = true;
        }
    }

    private void LateUpdate()
    {
        if (cameraPosition == null || !hasInitialized) return;

        Vector3 currentTargetPosition = cameraPosition.position;

        if (currentTargetPosition != lastCameraPosition)
        {
            transform.position = currentTargetPosition;
            lastCameraPosition = currentTargetPosition;
        }
    }

    // Method to manually set camera target (useful for camera switches)
    public void SetCameraTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            cameraPosition = newTarget;
            transform.position = newTarget.position;
            lastCameraPosition = newTarget.position;
            hasInitialized = true;
        }
    }
}
