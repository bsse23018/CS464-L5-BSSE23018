using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 6f;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayer = ~0; // Default to all layers
    [SerializeField] float groundCheckDistance = 0.2f;

    Rigidbody rb;
    CapsuleCollider col;
    Vector3 moveInput;
    Collider[] groundHits = new Collider[4];
    bool isGrounded;
    bool jumpQueued;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // 1. Read movement input every visual frame
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(h, 0f, v).normalized;

        // 2. Perform a reliable ground check (excluding own collider)
        Vector3 bottom = transform.position + col.center - Vector3.up * (col.height * 0.5f - col.radius);
        int hitCount = Physics.OverlapSphereNonAlloc(bottom, col.radius + groundCheckDistance, groundHits, groundLayer, QueryTriggerInteraction.Ignore);
        isGrounded = false;
        for (int i = 0; i < hitCount; i++)
        {
            if (groundHits[i] != col)
            {
                isGrounded = true;
                break;
            }
        }

        // 3. Queue jump if grounded
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpQueued = true;
        }
    }

    void FixedUpdate()
    {
        // Calculate horizontal target velocity
        Vector3 targetVelocity = moveInput * speed;

        // Unity 6: rb.linearVelocity | Older Unity (2022 and below): rb.velocity
        Vector3 currentVel = rb.linearVelocity; // Change to rb.velocity if on Unity 2022 or older

        // Apply movement while preserving current vertical velocity
        rb.linearVelocity = new Vector3(targetVelocity.x, currentVel.y, targetVelocity.z);

        // Handle Jump
        if (jumpQueued)
        {
            // ForceMode.VelocityChange ensures consistent jump height regardless of mass
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumpQueued = false;
            isGrounded = false;
        }
    }
}