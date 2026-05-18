using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Di chuyển")]
    public float jumpForce = 12f;
    public int maxJumps = 2;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    [Header("Bay")]
    public bool allowFly = true;
    public float flySpeed = 6f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private PlayerStats stats;

    private float horizontal;
    private int jumpsLeft;
    private bool isGrounded;
    private bool jumpPressed;
    private bool canMove = true;
    private bool isFlying;
    private float defaultGravity;
    private bool canFly;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();

        defaultGravity = rb.gravityScale;
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        if (!canMove) return;

        ReadArrowInput();
        CheckGround();

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            jumpPressed = true;
            canFly = true;
        }

        isFlying = allowFly
                   && canFly
                   && !isGrounded
                   && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow));

        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpsLeft = maxJumps;

            if (!Input.GetKey(KeyCode.UpArrow))
                canFly = false;
        }

        if (horizontal > 0) sr.flipX = false;
        else if (horizontal < 0) sr.flipX = true;

        UpdateAnimator();
        // Debug.Log("Ground=" + isGrounded +
        //   " | CanFly=" + canFly +
        //   " | Flying=" + isFlying +
        //   " | H=" + horizontal +
        //   " | Y=" + rb.velocity.y);
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (isFlying)
        {
            rb.gravityScale = 0f;

            float flyY = 0f;

            // Bay lên
            if (Input.GetKey(KeyCode.UpArrow))
                flyY = flySpeed;

            // Bay xuống
            if (Input.GetKey(KeyCode.DownArrow))
                flyY = -flySpeed;

            rb.velocity = new Vector2(
                horizontal * stats.moveSpeed,
                flyY
            );

            return;
        }
        rb.gravityScale = defaultGravity;

        rb.velocity = new Vector2(
            horizontal * stats.moveSpeed,
            rb.velocity.y
        );

        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft--;
            jumpPressed = false;
        }
    }

    void ReadArrowInput()
    {
        horizontal = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;

        if (Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;
    }

    void CheckGround()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("Chưa gán GroundCheck trong Inspector!");
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void UpdateAnimator()
    {
        anim?.SetFloat("speed", Mathf.Abs(horizontal));
        anim?.SetBool("isGrounded", isGrounded);
        anim?.SetFloat("velocityY", rb.velocity.y);
        anim?.SetBool("isFlying", isFlying);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!value && rb != null)
            rb.velocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}