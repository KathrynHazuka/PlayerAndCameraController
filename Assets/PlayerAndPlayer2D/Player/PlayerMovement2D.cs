using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(PlayerController2D))]
public class PlayerMovement2D : MonoBehaviour
{
    // reference to the controller
    PlayerController2D controller;
    // reference to the rigidbody
    new Rigidbody rigidbody;

    /// <summary>
    /// Gravity applied to the player when off the ground. (Default: -100)
    /// </summary>
    [Header("General Physics")]
    public float gravity = -100;
    /// <summary>
    /// Max velocity the player can reach when falling. (Default: -1000)
    /// </summary>
    public float terminalVelocity = -1000;
    /// <summary>
    /// Drag applied to the player in the air when not actively moving. (Default: 6)
    /// </summary>
    public float airDrag = 6;
    /// <summary>
    /// Drag applied to the player on the ground when not actively moving. (Default: 60)
    /// </summary>
    public float groundDrag = 60;

    /// <summary>
    /// Velocity applied to the player when moving. (Default: 10)
    /// </summary>
    [Header("Movement Physics")]
    public float moveVelocity = 10;
    /// <summary>
    /// Acceletation of the player when moving or running. (Default: 40)
    /// </summary>
    public float acceleration = 40;

    /// <summary>
    /// Velocity applied to the player when running. (Default: 16)
    /// </summary>
    [Space(10)]
    public float runVelocity = 16;

    /// <summary>
    /// Velocity applied to the player when crouching. (Default: 4)
    /// </summary>
    [Space(10)]
    public float crouchVelocity = 4;
    /// <summary>
    /// Ground drag applied to the player when crouch sliding. (Default: 30)
    /// </summary>
    public float crouchSlideDrag = 30;

    /// <summary>
    /// Stops the character when changing directions before applying new direction velocity for sharp turns while on ground. (Default: true)
    /// </summary>
    [Space(10)]
    public bool sharpGroundTurns = true;
    /// <summary>
    /// Stops the character when changing directions before applying new direction velocity for sharp turns while in air. (Default: true)
    /// </summary>
    public bool sharpAirTurns = true;

    /// <summary>
    /// Velocity applied to the player at instance of the jump. (Default: 35)
    /// </summary>
    [Header("Jump Physics")]
    public float jumpVelocity = 35;
    /// <summary>
    /// Velocity applied to the player at instance of jump cancel if apropriate. (Default: 10)
    /// </summary>
    public float shortJumpVelocity = 10;

    /// <summary>
    /// Velocity applied to the player at instance of the double jump. (Default: 30)
    /// </summary>
    [Space(10)]
    public float doubleJumpVelocity = 30;
    /// <summary>
    /// Velocity applied to the player at instance of double jump cancel if apropriate. (Default: 10)
    /// </summary>
    public float shortDoubleJumpVelocity = 10;

    /// <summary>
    /// Gravity applied to the player when wall sliding. (Default: -100)
    /// </summary>
    [Header("Wall Slide Physics")]
    public float wallSlideGravity = -100;
    /// <summary>
    /// Max velocity the player can reach when falling. (Default: -5)
    /// </summary>
    public float wallSlideTerminalVelocity = -5;

    [Header("Camera")]
    // player is followed by a camera (Default: true)
    [SerializeField] bool Camera2DFollower = true;
    // camera following the player
    Camera2D followCamera;

    // runs before start
    void Awake()
    {
        // set component references
        controller = GetComponent<PlayerController2D>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // runs once per fixed timestep
    void FixedUpdate ()
    {
        ApplyGravity();
        ApplyDrag();
        Movement();
        Jumping();

        if (controller.grabLedge)
            GrabLedge();
    }

    // applies gravity to the player when not on ground
    void ApplyGravity()
    {
        if (controller.wallSlide)
        {
            float yVelocity = Mathf.Max((rigidbody.velocity.y + (wallSlideGravity * 0.008f)), wallSlideTerminalVelocity);
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, yVelocity, rigidbody.velocity.z);
        }
        else if (!controller.grounded)
        {
            float yVelocity = Mathf.Max((rigidbody.velocity.y + (gravity * 0.008f)), terminalVelocity);
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, yVelocity, rigidbody.velocity.z);
        }
        else
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.y);
    }

    // applies air or ground drag to the player when appropriate
    void ApplyDrag()
    {
        // check is player is actively moving, and if not, apply the appropriate drag
        if (!controller.left && !controller.right)
        {
            float drag;
            if (controller.grounded)
                drag = groundDrag;
            else
                drag = airDrag;

            float newVelocityX;
            if (rigidbody.velocity.x > 0)
                newVelocityX = Mathf.Max(rigidbody.velocity.x - (drag * 0.008f), 0);
            else
                newVelocityX = Mathf.Min(rigidbody.velocity.x + (drag * 0.008f), 0);

            rigidbody.velocity = new Vector3(newVelocityX, rigidbody.velocity.y, rigidbody.velocity.z);
        }
        else
        {
            if(controller.crouchSlide && controller.crouch)
            {
                float newVelocityX;
                if (rigidbody.velocity.x > 0)
                    newVelocityX = Mathf.Max(rigidbody.velocity.x - (crouchSlideDrag * 0.008f), 0);
                else
                    newVelocityX = Mathf.Min(rigidbody.velocity.x + (crouchSlideDrag * 0.008f), 0);

                rigidbody.velocity = new Vector3(newVelocityX, rigidbody.velocity.y, rigidbody.velocity.z);
            }
        }
    }

    // applies movement velocities
    void Movement()
    {
        // if player presses down
        if (controller.down)
        {
            // tell camera to look down
            if (Camera2DFollower)
                followCamera.lookDown = true;

            // let player fall if grabbing ledge.
            if (controller.grabbingLedge)
            {
                rigidbody.isKinematic = false;
                controller.grabbingLedge = false;
            }
        }
        // otherwise tell camera to not look down anymore.
        else
        {
            if (Camera2DFollower)
                followCamera.lookDown = false;
        }

        // if player presses up
        if (controller.up)
        {
            // tell camera to look up
            if (Camera2DFollower)
                followCamera.lookUp = true;
        }
        // otherwise tell camera to not look up anymore.
        else
        {
            if (Camera2DFollower)
                followCamera.lookUp = false;
        }

        // if player presses right
        if (controller.right)
        {
            // tell camera to look right
            if (Camera2DFollower)
                followCamera.lookRight = true;

            // apply correct x velocity
            if(sharpGroundTurns && controller.grounded || sharpAirTurns && !controller.grounded)
                if (rigidbody.velocity.x < 0)
                    rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, rigidbody.velocity.z);

            float maxVelocity = moveVelocity;
            if (controller.crouch && !controller.crouchSlide)
                maxVelocity = crouchVelocity;
            else if (controller.run)
                maxVelocity = runVelocity;

            float xVelocity;
            if (controller.wallSlide && controller.wallOnRight)
                xVelocity = 0;
            else
                xVelocity = Mathf.Min(rigidbody.velocity.x + (acceleration * 0.008f), maxVelocity);

            if (!(controller.crouch && controller.crouchSlide && controller.grounded))
                rigidbody.velocity = new Vector3(xVelocity, rigidbody.velocity.y, rigidbody.velocity.z);
        }
        // otherwise, tell camera to not look right anymore.
        else
        {
            if (Camera2DFollower)
                followCamera.lookRight = false;
        }

        // if player presses left
        if (controller.left)
        {
            // tell camera to look left
            if (Camera2DFollower)
                followCamera.lookLeft = true;

            // apply correct x velocity
            if (sharpGroundTurns && controller.grounded || sharpAirTurns && !controller.grounded)
                if (rigidbody.velocity.x > 0)
                    rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, rigidbody.velocity.z);

            float maxVelocity = moveVelocity;
            if (controller.crouch && !controller.crouchSlide)
                maxVelocity = crouchVelocity;
            else if (controller.run)
                maxVelocity = runVelocity;

            float xVelocity;
            if (controller.wallSlide && !controller.wallOnRight)
                xVelocity = 0;
            else
                xVelocity = Mathf.Max(rigidbody.velocity.x - (acceleration * 0.008f), -maxVelocity);

            if (!(controller.crouch && controller.crouchSlide && controller.grounded))
                rigidbody.velocity = new Vector3(xVelocity, rigidbody.velocity.y, rigidbody.velocity.z);
        }
        // otherwise, tell camera to not look left anymore.
        else
        {
            if (Camera2DFollower)
                followCamera.lookLeft = false;
        }
    }

    // applies jump velocities
    void Jumping()
    {
        // if player presses jump
        if (controller.jump)
        {
            float xVelocity;
            // if jumping off of a wall, set the correct x velocity.
            if (controller.jumpFromWalls && controller.wallSlide && !controller.grabbingLedge)
            {
                if (controller.wallOnRight)
                    xVelocity = -moveVelocity;
                else
                    xVelocity = moveVelocity;
            }
            else if (controller.grabbingLedge)
            {
                if (controller.ledgeOnRight && controller.left)
                    xVelocity = -moveVelocity;
                else if(!controller.ledgeOnRight && controller.right)
                    xVelocity = moveVelocity;
                else
                    xVelocity = rigidbody.velocity.x;
            }
            else
            {
                xVelocity = rigidbody.velocity.x;
            }

            // if grabbing ledge, let the player move again and tell controller that player is no longer grabbing the ledge.
            if (controller.grabbingLedge)
            {
                rigidbody.isKinematic = false;
                controller.grabbingLedge = false;
            }

            rigidbody.velocity = new Vector3(xVelocity, jumpVelocity, rigidbody.velocity.z);
            controller.jump = false;
        }
        // else if player presses double jump
        else if (controller.doubleJump)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, doubleJumpVelocity, rigidbody.velocity.z);
            controller.doubleJump = false;
        }

        // if player performes a jump cancel
        if (controller.jumpCancel)
        {
            if (rigidbody.velocity.y > shortJumpVelocity)
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, shortJumpVelocity, rigidbody.velocity.z);
            controller.jumpCancel = false;
        }

        // if player performes a double jump cancel
        if (controller.doubleJumpCancel)
        {
            if (rigidbody.velocity.y > shortDoubleJumpVelocity)
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, shortDoubleJumpVelocity, rigidbody.velocity.z);
            controller.doubleJumpCancel = false;
        }
    }

    // grabs the a ledge if controller detects one
    void GrabLedge()
    {
        // stop the character from moving
        rigidbody.isKinematic = true;
        controller.grabLedge = false;

        // move character to the ledge
        float xPos;
        if (controller.ledgeOnRight)
            xPos = controller.ledge.x - (controller.playerRegion.size.x / 2);
        else
            xPos = controller.ledge.x + (controller.playerRegion.size.x / 2);
        float yPos = controller.ledge.y - (controller.playerRegion.size.y / 2);

        transform.position = new Vector3(xPos, yPos, transform.position.z);
        controller.grabbingLedge = true;
    }

    // sets the follower camera
    void SetFollower(Camera2D follower)
    {
        followCamera = follower;
    }

    // reset the follower camera
    void ResetFollower()
    {
        followCamera = null;
    }
}
