using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class PlyerMovement : MonoBehaviour
{
    [Header("Movement")]
    Rigidbody rb;
    [SerializeField] Transform orientation;
    public float moveSpeed = 10f;
    public float walkSpeed = 7f;
    public float moveMulti = 10f;
    
    [Header("Sprinting")]
    public float sprintSpeed = 15f;
    public float acceleration = 7f;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float airMulti;
    public float jumpCd;
    bool jumpable;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    float startYScale;
    
    [Header("Drag")]
    public float gndDrag = 6f;
    public float airDrag = 15f;

    [Header("GroundCheck")]
    [SerializeField] Transform gndPos;
    public float playerHeight;
    public LayerMask whatIsGnd;
    public float gndDistance = 0.2f;
    bool grounded;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
 
    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;

        jumpable = true;
    }

    private void Update()
    {
        MyInput();
        ControlDrag();
        ControlSpeed();
        StateHandler();

        grounded = Physics.CheckSphere(gndPos.position, gndDistance, whatIsGnd);
    }   

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && grounded && jumpable)
        {
            jumpable = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCd);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, acceleration);
        }
        else if(Input.GetKey(sprintKey) && grounded)
        {
            state = MovementState.sprinting;
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration);
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration);
        }
        else
        {
            state = MovementState.air;
        }
        
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveMulti, ForceMode.Acceleration);
        }
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveMulti * airMulti, ForceMode.Acceleration);
        }
        
    }

    void ControlSpeed()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        if (grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        
    }

    void ResetJump()
    {
        jumpable = true;
    }

    void ControlDrag()
    {
        if (grounded)
        {
            rb.linearDamping = gndDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

}
