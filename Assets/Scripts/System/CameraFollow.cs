using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 8f;
    public Vector3 offset = new Vector3(2f, 1f, -10f);

    [Header("Map Bounds")]
    public bool useBounds = true;

    // Đây là mép thật của map
    public float mapMinX;
    public float mapMaxX;
    public float mapMinY;
    public float mapMaxY;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;

        if (useBounds && cam != null)
        {
            float cameraHalfHeight = cam.orthographicSize;
            float cameraHalfWidth = cameraHalfHeight * cam.aspect;

            desired.x = Mathf.Clamp(
                desired.x,
                mapMinX + cameraHalfWidth,
                mapMaxX - cameraHalfWidth
            );

            desired.y = Mathf.Clamp(
                desired.y,
                mapMinY + cameraHalfHeight,
                mapMaxY - cameraHalfHeight
            );
        }

        desired.z = -10f;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            1f / smoothSpeed
        );
    }

    void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3((mapMinX + mapMaxX) / 2f, (mapMinY + mapMaxY) / 2f, 0),
            new Vector3(mapMaxX - mapMinX, mapMaxY - mapMinY, 0)
        );
    }
}