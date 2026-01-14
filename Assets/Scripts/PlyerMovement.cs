using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        ControlDrag();
        ControlSpeed();

        grounded = Physics.CheckSphere(gndPos.position, gndDistance, whatIsGnd);

        if(Input.GetKey(jumpKey) && grounded)
        {
            Jump();
        }
    }   

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
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
        if(grounded && Input.GetKey(sprintKey))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        
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
