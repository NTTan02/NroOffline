using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    public Transform player;

    public float normalScaleX = 0.7f;
    public float normalScaleY = 0.18f;
    public float minScale = 0.35f;
    public float maxHeight = 3f;

    private Vector3 startLocalPos;
    private Rigidbody2D rb;

    void Start()
    {
        startLocalPos = transform.localPosition;
        rb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null || rb == null) return;

        float heightFactor = Mathf.Clamp01(
            Mathf.Abs(rb.velocity.y) / maxHeight
        );

        float scale = Mathf.Lerp(1f, minScale, heightFactor);

        transform.localScale = new Vector3(
            normalScaleX * scale,
            normalScaleY * scale,
            1f
        );

        transform.localPosition = startLocalPos;
    }
}