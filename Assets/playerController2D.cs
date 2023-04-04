using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle, Jumping, runnning }

public class playerController2D : MonoBehaviour
{
    public float maxSpeed;
    public float acceleration;
    public float jumpForce;
    public float jumpAbortForce;
    public float stoppingSpeed;



    Vector2 moveVector;
    private Rigidbody2D body;

    PlayerState state;
    bool inputJump;
    float horizontalInput;
    float horizontalMove;


    bool jumpAbort;


    bool isGrounded;
    [SerializeField] Transform groundCheck;
    public LayerMask groundMask;


    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        state = PlayerState.Idle;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (transform.localScale.x * horizontalInput < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        ResetMotion();
        CheckForGround();
        UpdateHorizontal();


        switch (state)
        {
            case PlayerState.Idle:
                if (body.velocity.x != 0)
                {
                    state = PlayerState.runnning;
                }
                else
                {
                    CheckJump();
                    Attack();
                }
                break;
            case PlayerState.Jumping:
                if (body.velocity.y <= 0 && isGrounded)
                {
                    state = PlayerState.Idle;
                }
                else
                {

                    CheckAbortJump();
                }
                break;
            case PlayerState.runnning:
                if (body.velocity.x == 0)
                {
                    state = PlayerState.Idle;
                }
                else
                {
                    CheckJump();
                }
                break;
        }

        Debug.Log(state);

        body.velocity = moveVector;
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Ho attaccato");
        }
    }

    private void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
            inputJump = true;

        if (body.velocity.y > jumpAbortForce && Input.GetButtonUp("Jump"))
            jumpAbort = true;
    }

    private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundMask);

    }
    private void ResetMotion()
    {
        moveVector = body.velocity;

    }

    private void UpdateHorizontal()
    {


        horizontalMove += horizontalInput * acceleration * Time.fixedDeltaTime;
        if (horizontalInput == 0)
            horizontalMove = Mathf.Lerp(horizontalMove, 0, stoppingSpeed);


        horizontalMove = Mathf.Clamp(horizontalMove, -maxSpeed, maxSpeed);

        moveVector.x = horizontalMove;

    }

    private void CheckJump()
    {
        if (inputJump && isGrounded)
        {
            inputJump = false;
            moveVector = Vector2.up * jumpForce;

            //animator.SetTrigger("jump");
            state = PlayerState.Jumping;
        }



    }

    private void CheckAbortJump()
    {
        if (jumpAbort && body.velocity.y > jumpAbortForce)
        {
            moveVector = Vector2.up * jumpAbortForce;
            jumpAbort = false;
        }
    }
}
