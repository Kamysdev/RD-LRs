using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CatPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 22f;
    [SerializeField] private float dashForce = 8f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private bool rotateTowardsMovement = true;
    [SerializeField] private bool rotateWhileMovingBackward = false;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Paw Push")]
    [SerializeField] private float pawPushForce = 7f;
    [SerializeField] private float pawPushRadius = 1.1f;
    [SerializeField] private float pawPushDistance = 1.2f;
    [SerializeField] private float pawPushUpwardForce = 1f;
    [SerializeField] private float pawPushCooldown = 0.35f;

    [Header("Safety")]
    [SerializeField] private float fallY = -5f;
    [SerializeField] private Vector3 respawnPosition = new Vector3(0f, 1f, 0f);

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("References")]
    [SerializeField] private Transform movementReference;

    private Rigidbody rb;
    private Vector3 rawMoveInput;
    private Vector3 moveInput;
    private bool dashQueued;
    private bool jumpQueued;
    private bool pawPushQueued;
    private bool controlsLocked;
    private float pawPushReadyTime;

    public bool ControlsLocked => controlsLocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (movementReference == null && Camera.main != null)
        {
            movementReference = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (!controlsLocked && Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashQueued = true;
        }

        if (!controlsLocked && Input.GetKeyDown(KeyCode.Space))
        {
            jumpQueued = true;
        }

        if (!controlsLocked && Input.GetKeyDown(KeyCode.E))
        {
            pawPushQueued = true;
        }

        if (transform.position.y < fallY)
        {
            Respawn();
        }
    }

    private void FixedUpdate()
    {
        if (controlsLocked)
        {
            return;
        }

        rawMoveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        moveInput = GetMovementDirection(rawMoveInput);

        ApplyMovement();
        UpdateRotation();

        if (dashQueued)
        {
            Dash();
            dashQueued = false;
        }

        if (jumpQueued)
        {
            Jump();
            jumpQueued = false;
        }

        if (pawPushQueued)
        {
            PawPush();
            pawPushQueued = false;
        }
    }

    public void SetControlsLocked(bool isLocked)
    {
        controlsLocked = isLocked;

        if (controlsLocked)
        {
            moveInput = Vector3.zero;
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void ApplyMovement()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 targetVelocity = moveInput * moveSpeed;
        float currentAcceleration = moveInput.sqrMagnitude > 0.001f ? acceleration : deceleration;

        Vector3 newHorizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            currentAcceleration * Time.fixedDeltaTime);

        if (newHorizontalVelocity.magnitude > maxSpeed)
        {
            newHorizontalVelocity = newHorizontalVelocity.normalized * maxSpeed;
        }

        rb.linearVelocity = new Vector3(newHorizontalVelocity.x, rb.linearVelocity.y, newHorizontalVelocity.z);
    }

    private void Dash()
    {
        if (moveInput.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Vector3 dashDirection = moveInput.normalized;
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        ClampHorizontalSpeed();
    }

    private void Jump()
    {
        if (!IsGrounded())
        {
            return;
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void PawPush()
    {
        if (Time.time < pawPushReadyTime)
        {
            return;
        }

        Vector3 pushDirection = transform.forward;
        pushDirection.y = 0f;
        pushDirection.Normalize();

        if (pushDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Vector3 pushCenter = transform.position + Vector3.up * 0.5f + pushDirection * pawPushDistance;
        Collider[] hitColliders = Physics.OverlapSphere(
            pushCenter,
            pawPushRadius,
            ~0,
            QueryTriggerInteraction.Ignore);

        bool pushedAnyObject = false;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Rigidbody hitBody = hitColliders[i].attachedRigidbody;

            if (hitBody == null || hitBody == rb)
            {
                continue;
            }

            Vector3 forceDirection = (hitBody.worldCenterOfMass - transform.position).normalized;
            forceDirection.y = 0f;

            if (forceDirection.sqrMagnitude <= 0.001f)
            {
                forceDirection = pushDirection;
            }

            forceDirection.Normalize();
            Vector3 finalForce = forceDirection * pawPushForce + Vector3.up * pawPushUpwardForce;
            hitBody.AddForce(finalForce, ForceMode.Impulse);
            pushedAnyObject = true;
        }

        if (pushedAnyObject)
        {
            pawPushReadyTime = Time.time + pawPushCooldown;
        }
    }

    private void ClampHorizontalSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude <= maxSpeed)
        {
            return;
        }

        Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
        rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
    }

    private void Respawn()
    {
        transform.position = respawnPosition;
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void UpdateRotation()
    {
        if (!rotateTowardsMovement || moveInput.sqrMagnitude <= 0.001f)
        {
            return;
        }

        if (!rotateWhileMovingBackward && rawMoveInput.z < -0.1f && Mathf.Abs(rawMoveInput.x) < 0.1f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveInput.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * 360f * Time.fixedDeltaTime);
    }

    private Vector3 GetMovementDirection(Vector3 rawInput)
    {
        rawInput = Vector3.ClampMagnitude(rawInput, 1f);

        if (rawInput.sqrMagnitude <= 0.001f)
        {
            return Vector3.zero;
        }

        if (movementReference == null)
        {
            return rawInput;
        }

        Vector3 forward = movementReference.forward;
        Vector3 right = movementReference.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * rawInput.z + right * rawInput.x;
        return Vector3.ClampMagnitude(direction, 1f);
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float rayDistance = groundCheckDistance + 0.1f;
        return Physics.Raycast(origin, Vector3.down, rayDistance, groundLayers, QueryTriggerInteraction.Ignore);
    }
}
