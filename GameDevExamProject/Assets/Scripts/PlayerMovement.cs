// Cleaned version: keeps only walk + run + jump. 
// Crouch, ladder, footsteps, headbob, trampoline, and audio removed.
// Original jumping and movement logic untouched.

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private float inputX = 0;
    private float inputY = 0;

    [Header("Movement Vars")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public bool limitDiagonalSpeed = true;
    public bool toggleRun = false;
    public float jumpSpeed = 8.0f;
    public float defGravity = 20.0f;
    public float fallingDamageThreshold = 10.0f;
    public float fallMultiplier = 1.85f;
    public bool airControl = false;

    private float antiBumpFactor = .75f;
    private int antiBunnyHopFactor = 1;

    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public bool running = false;

    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;

    private float gravity = 0;
    private float slideLimit;
    private float defSlopeLimit = 45;

    private bool isWorking = true;
    private bool onPlatform = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
        gravity = defGravity;
        defSlopeLimit = controller.slopeLimit;
    }

    void FixedUpdate()
    {
        if (!isWorking) return;

        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

        if (!onPlatform)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }

        if (grounded)
        {
            bool sliding = false;
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit) sliding = true;

                else if (Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit))
                    if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit) sliding = true;

            if (falling)
            {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                {
                    print("Ouch! Fell " + (fallStartLevel - myTransform.position.y) + " units!");
                }
            }

            running = (speed > walkSpeed && (inputX != 0 || inputY != 0));

            if (!sliding)
            {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }

            if (!Input.GetButton("Jump"))
                jumpTimer++;
            else if (jumpTimer >= antiBunnyHopFactor)
            {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
            }
        }
        else
        {
            if (airControl && playerControl)
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }

            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }
        }
    }

    void Update()
    {
        if (!isWorking) return;

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        if (toggleRun && grounded)
        {
            if (Input.GetButtonDown("Run"))
            {
                speed = (speed == walkSpeed ? runSpeed : walkSpeed);
            }
        }
        else
        {
            speed = Input.GetButton("Run") ? runSpeed : walkSpeed;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }

    public void SetWorking(bool workingState)
    {
        isWorking = workingState;
    }
}