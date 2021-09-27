using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PlayerController2D : MonoBehaviour
{
    // -------------- Ground Check Variables: --------------
    /// <summary>
    /// BoxCollider of the player.
    /// </summary>
    [Header("Detection")]
    public BoxCollider playerRegion;

    /// <summary>
    /// Tags used for detecting ground. (Default: {"Ground"})
    /// </summary>
    [Space(10)]
    public string[] groundTags = new string[] { "Ground", "BottomBound" };
    /// <summary>
    /// Tags used for detecting walls. (Default: {"Ground", "Wall", "RightBound", "LeftBound"})
    /// </summary>
    public string[] wallTags = new string[] { "Ground", "Wall", "RightBound", "LeftBound" };
    // the y compensation for ground calculations. makes sure the raycast doesnt start inside the ground collider.
    float yCompensation = 0.005f;
    // the x compensation for wall calculations. makes sure the raycast doesnt start inside the wall collider.
    float xCompensation = 0.005f;
    /// <summary>
    /// Range of the ground test.
    /// </summary>
    public float groundRange;
    /// <summary>
    /// Range of the wall slide test.
    /// </summary>
    public float wallSlideRange;

    /// <summary>
    /// Tag used for detecting ledges. (Default: "Ledge")
    /// </summary>
    [Space(10)]
    public string ledgeTag = "Ledge";
    /// <summary>
    /// Size of ledges. (Default: Vector3(0.1f, 0.2f, 0.2f))
    /// </summary>
    public Vector3 ledgeSize = new Vector3(0.1f, 0.2f, 0.2f);

    // -------------- Control Restrictions: --------------
    /// <summary>
    /// This player can run. (Default: false)
    /// </summary>
    [Header("Run Ability")]
    public bool usesRun = false;

    /// <summary>
    /// This player can crouch. (Default: false)
    /// </summary>
    [Header("Crouch Ability")]
    public bool usesCrouch = false;
    /// <summary>
    /// Crouching makes the player slide. (Default: false)
    /// </summary>
    public bool crouchSlide = false;

    /// <summary>
    /// This player can wall slide. (Default: false)
    /// </summary>
    [Header("Wall Slide Abilities")]
    public bool usesWallSlide = false;
    /// <summary>
    /// Pressing down cancels a wall slide as long as it is pressed. (Default: false) 
    /// </summary>
    public bool downCancelsWallSlide = false;

    /// <summary>
    /// This player can jump from walls when wall sliding. (Default: true)
    /// </summary>
    [Space(10)]
    public bool jumpFromWalls = true;
    /// <summary>
    /// Time it takes in seconds to unlock horizontal movement after a wall jump (Default: 0.3f)
    /// </summary>
    public float wallJumpLockTime = 0.3f;
    // locks the right direction after a wall jump to the right
    bool wallJumpLockRight = false;
    // locks the left direction after a wall jump to the left
    bool wallJumpLockLeft = false;
    // timer for the right direction lock
    float wallJumpLockRightTimer = 0;
    // timer for the left direction lock
    float wallJumpLockLeftTimer = 0;

    /// <summary>
    /// Player sticks to walls when wall sliding. (Default: true)
    /// </summary>
    [Space(10)]
    public bool sticksToWalls = true;
    /// <summary>
    /// Time it takes in seconds to stop sticking to a wall when wall sliding. (Default: 0.25f)
    /// </summary>
    public float stickToWallTime = 0.25f;
    //// timer for the right direction lock when wall sliding
    float stickToRightWallTimer = 0;
    //// timer for the left direction lock when wall sliding
    float stickToLeftWallTimer = 0;

    /// <summary>
    /// This player can double jump. (Default: false)
    /// </summary>
    [Header("Double Jump Abilities")]
    public bool usesDoubleJump = false;
    // can the character double jump right now?
    bool canDoubleJump;
    /// <summary>
    /// This player's double jump resets when grabbing a ledge. (Default: false)
    /// </summary>
    public bool ledgeResetsDoubleJump = false;
    /// <summary>
    /// This player's double jump resets when sliding on a wall. (Defualt: false)
    /// </summary>
    public bool wallResetsDoubleJump = false;

    // -------------- Control Properties: --------------
    /// <summary>
    /// Player is on the ground.
    /// </summary>
    public bool grounded { get; private set; }
    /// <summary>
    /// Player is sliding on a wall
    /// </summary>
    public bool wallSlide { get; private set; }
    /// <summary>
    /// Wall on right = true, wall on left = false.
    /// </summary>
    public bool wallOnRight { get; private set; }

    /// <summary>
    /// Block player's input
    /// </summary>
    public bool blockInput { get; set; }

    /// <summary>
    /// Player is pressing the up key.
    /// </summary>
    public bool up { get; private set; }
    /// <summary>
    /// Player is pressing the down key.
    /// </summary>
    public bool down { get; private set; }
    /// <summary>
    /// Player is pressing the left key.
    /// </summary>
    public bool left { get; private set; }
    /// <summary>
    /// Player is pressing the right key.
    /// </summary>
    public bool right { get; private set; }
    /// <summary>
    /// Player is pressing the run key.
    /// </summary>
    public bool run { get; private set; }
    /// <summary>
    /// Player is pressing the crouch key.
    /// </summary>
    public bool crouch { get; private set; }

    /// <summary>
    /// Player is pressing the jump key. Must be set to false after executing the jump.
    /// </summary>
    public bool jump { get; set; }
    /// <summary>
    /// Player is performing a jump cancel after jumping and before landing. Must be set to false after executing jump cancel.
    /// </summary>
    public bool jumpCancel { get; set; }
    /// <summary>
    /// Player is pressing the jump key after jumping and before landing. Must be set to false after executing the double jump.
    /// </summary>
    public bool doubleJump { get; set; }
    /// <summary>
    /// Player is performing a double jump cancel after double jumping and before landing. Must be set to false after executing the double jump cancel.
    /// </summary>
    public bool doubleJumpCancel { get; set; }

    /// <summary>
    /// Playes is trying to grab a ledge
    /// </summary>
    public bool grabLedge { get; set; }
    /// <summary>
    /// Player is grabbing a ledge. (Set by the movement script)
    /// </summary>
    public bool grabbingLedge { get; set; }
    /// <summary>
    /// Ledge on right = true, ledge on left = false.
    /// </summary>
    public bool ledgeOnRight { get; set; }
    /// <summary>
    ///  X and Y positions of the ledge the player can grab or is grabbing. (top left/right of the platform)
    /// </summary>
    public Vector2 ledge { get; set; }

    void Start()
    {
        // set default values
        canDoubleJump = true;
    }
    
    // runs once per frame
    void Update()
    {
        if (!blockInput)
        {
            DirectionalInput();
            ModInput();
            JumpInput();
        }
        else
        {
            ResetInputs();
        }
    }

    // runs every fixed timestep
    void FixedUpdate()
    {
        GroundTest();
        WallSlideTest();
    }

    // sets directional properties based on player input
    void DirectionalInput()
    {
        // unlock locked directions based on the lock timer.
        if (wallJumpLockLeft)
        {
            if (wallJumpLockLeftTimer < wallJumpLockTime)
                wallJumpLockLeftTimer += Time.deltaTime;
            else
                wallJumpLockLeft = false;
        }

        if (wallJumpLockRight)
        {
            if (wallJumpLockRightTimer < wallJumpLockTime)
                wallJumpLockRightTimer += Time.deltaTime;
            else
                wallJumpLockRight = false;
        }

        // set up/down/left/right controls
        up = (Input.GetAxisRaw("Vertical") > 0) ? true : false;
        down = (Input.GetAxisRaw("Vertical") < 0) ? true : false;
        left = (Input.GetAxisRaw("Horizontal") < 0 && !wallJumpLockLeft) ? true : false;
        right = (Input.GetAxisRaw("Horizontal") > 0 && !wallJumpLockRight) ? true : false;

        if (sticksToWalls)
        {
            if (left)
            {
                if (wallSlide && wallOnRight)
                {
                    if (stickToRightWallTimer < stickToWallTime)
                    {
                        stickToRightWallTimer += Time.deltaTime;
                        left = false;
                    }
                    else
                    {
                        left = true;
                    }
                }
            }
            else
            {
                stickToRightWallTimer = 0;
            }

            if (right)
            {
                if (wallSlide && !wallOnRight)
                {
                    if (stickToLeftWallTimer < stickToWallTime)
                    {
                        stickToLeftWallTimer += Time.deltaTime;
                        right = false;
                    }
                    else
                    {
                        right = true;
                    }
                }
            }
            else
            {
                stickToLeftWallTimer = 0;
            }
        }
    }

    //void StickToWalls()
    //{
    //    if (sticksToWalls)
    //    {
    //        if (wallSlide && !grabbingLedge && !grounded)
    //        {
    //            if (wallOnRight && Input.GetAxisRaw("Horizontal") > 0)
    //            {
    //                stickToRightWall = true;
    //                stickToRightWallTimer = 0;
    //            }

    //            if (!wallOnRight && Input.GetAxisRaw("Horizontal") < 0)
    //            {
    //                stickToLeftWall = true;
    //                stickToLeftWallTimer = 0;
    //            }

    //            if (stickToRightWall)
    //            {
    //                if (stickToRightWallTimer < stickToWallTime)
    //                    stickToRightWallTimer += Time.fixedDeltaTime;
    //                else
    //                    stickToRightWall = false;
    //            }

    //            if (stickToLeftWall)
    //            {
    //                if (stickToLeftWallTimer < stickToWallTime)
    //                    stickToLeftWallTimer += Time.fixedDeltaTime;
    //                else
    //                    stickToLeftWall = false;
    //            }
    //        }
    //        else
    //        {
    //            stickToRightWall = false;
    //            stickToLeftWall = false;
    //        }
    //    }
    //    else
    //    {
    //        stickToRightWall = false;
    //        stickToLeftWall = false;
    //    }
    //}

    // sets modifier properties based on player input

    void ModInput()
    {
        // set run controls
        if (usesRun)
            run = (Input.GetButton("Run")) ? true : false;
        else
            run = false;

        if (usesCrouch)
            crouch = (Input.GetButton("Crouch")) ? true : false;
        else
            crouch = false;
    }

    // sets jump properties based on player input
    void JumpInput()
    {
        // set jump controls
        if (Input.GetButtonDown("Jump"))
        {
            if (grounded || grabbingLedge || jumpFromWalls && wallSlide)
            {
                jump = true;

                if (wallSlide && wallOnRight && !grabbingLedge)
                {
                    wallJumpLockRight = true;
                    wallJumpLockRightTimer = 0;
                }
                else if (wallSlide && !wallOnRight && !grabbingLedge)
                {
                    wallJumpLockLeft = true;
                    wallJumpLockLeftTimer = 0;
                }

                if (wallResetsDoubleJump && wallSlide)
                    canDoubleJump = true;

                if (ledgeResetsDoubleJump && grabbingLedge)
                    canDoubleJump = true;
            }
        }

        if (Input.GetButtonUp("Jump") && !grounded)
            jumpCancel = true;

        // set double jump controls
        if (usesDoubleJump && !jump)
        {
            if (grounded)
                canDoubleJump = true;

            if (!wallResetsDoubleJump && wallSlide)
                canDoubleJump = false;

            if (Input.GetButtonDown("Jump") && canDoubleJump && !grounded && !wallSlide)
            {
                doubleJump = true;
                canDoubleJump = false;
            }

            if (Input.GetButtonUp("Jump") && !canDoubleJump && !grounded && !wallSlide)
                doubleJumpCancel = true;
        }
    }

    // resets all input properties
    void ResetInputs()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        run = false;

        jump = false;
        jumpCancel = false;
        doubleJump = false;
        doubleJumpCancel = false;
    }

    // tests if the player is currently on the ground
    void GroundTest()
    {
        Ray bottomRight = new Ray(new Vector3(transform.position.x + (playerRegion.size.x / 2), transform.position.y - (playerRegion.size.y / 2) + yCompensation, 0), -Vector3.up);
        Ray bottomLeft = new Ray(new Vector3(transform.position.x - (playerRegion.size.x / 2), transform.position.y - (playerRegion.size.y / 2) + yCompensation, 0), -Vector3.up);
        RaycastHit hit;

        bool isGrounded = false;
        if (Physics.Raycast(bottomRight, out hit, groundRange + yCompensation))
        {
            foreach (string groundTag in groundTags)
            {
                if (hit.transform.CompareTag(groundTag))
                {
                    isGrounded = true;
                    CorrectY(hit.transform);
                }
            }
        }

        if (Physics.Raycast(bottomLeft, out hit, groundRange + yCompensation))
        {
            foreach (string groundTag in groundTags)
            {
                if (hit.transform.CompareTag(groundTag))
                {
                    isGrounded = true;
                    CorrectY(hit.transform);
                }
            }
        }

        if (isGrounded)
            grounded = true;
        else
            grounded = false;
    }

    // tests if the player is currently wall sliding
    void WallSlideTest()
    {
        Ray bottomRight = new Ray(new Vector3(transform.position.x + (playerRegion.size.x / 2) - xCompensation, transform.position.y - (playerRegion.size.y / 2), 0), Vector3.right);
        Ray topRight = new Ray(new Vector3(transform.position.x + (playerRegion.size.x / 2) - xCompensation, transform.position.y + (playerRegion.size.y / 2), 0), Vector3.right);
        Ray bottomLeft = new Ray(new Vector3(transform.position.x - (playerRegion.size.x / 2) + xCompensation, transform.position.y - (playerRegion.size.y / 2), 0), -Vector3.right);
        Ray topLeft = new Ray(new Vector3(transform.position.x - (playerRegion.size.x / 2) + xCompensation, transform.position.y + (playerRegion.size.y / 2), 0), -Vector3.right);
        RaycastHit hit;

        bool isSliding = false;
        if (Physics.Raycast(bottomRight, out hit, wallSlideRange + xCompensation) || Physics.Raycast(topRight, out hit, wallSlideRange + xCompensation))
        {
            foreach (string wallTag in wallTags)
            {
                if (hit.transform.CompareTag(wallTag) && !grounded)
                {
                    CorrectX(hit.transform, true);
                    if (sticksToWalls)
                    {
                        isSliding = true;
                        wallOnRight = true;
                    }
                    else
                    {
                        if (right)
                        {
                            isSliding = true;
                            wallOnRight = true;
                        }
                    }
                }
            }
        }

        if (Physics.Raycast(bottomLeft, out hit, wallSlideRange + xCompensation) || Physics.Raycast(topLeft, out hit, wallSlideRange + xCompensation))
        {
            foreach (string wallTag in wallTags)
            {
                if (hit.transform.CompareTag(wallTag) && !grounded)
                {
                    CorrectX(hit.transform, false);
                    if (sticksToWalls)
                    {
                        isSliding = true;
                        wallOnRight = false;
                    }
                    else
                    {
                        if (left)
                        {
                            isSliding = true;
                            wallOnRight = false;
                        }
                    }
                }
            }
        }

        if (usesWallSlide && isSliding)
        {
            if (downCancelsWallSlide && down)
                wallSlide = false;
            else
                wallSlide = true;
        }
        else
        {
            wallSlide = false;
        }
    }

    // corrects the y position when on ground to prevent getting stuck on colliders
    void CorrectY(Transform ground)
    {
        float yPos = ground.transform.position.y + ((ground.transform.GetComponent<BoxCollider>().size.y / 2) * ground.localScale.y) + (playerRegion.size.y / 2) + groundRange - yCompensation;
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    // corrects the x position when sliding on walls to prevent getting stuck on colliders
    void CorrectX(Transform ground, bool toRight)
    {
        float xPos;
        if(toRight)
            xPos = ground.transform.position.x - ((ground.transform.GetComponent<BoxCollider>().size.x / 2) * ground.localScale.x) - (playerRegion.size.x / 2) - wallSlideRange + xCompensation;
        else
            xPos = ground.transform.position.x + ((ground.transform.GetComponent<BoxCollider>().size.x / 2) * ground.localScale.x) + (playerRegion.size.x / 2) + wallSlideRange - xCompensation;

        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
    }
}
