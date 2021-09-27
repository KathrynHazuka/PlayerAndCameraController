using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

    // reference to camera component of this object, if it has one
    new Camera camera;
    // current horizontal size of the camera
    float cameraXSize;
    // current vertical size of the camera
    float cameraYSize;

    // reference to the box collider (trigger), used for detecting bounds if in camera mode
    BoxCollider boxCollider;
    
    /// <summary>
    /// Target this object should follow.
    /// </summary>
    public Transform target;
    // reference to the current position of the target, used for ease of access
    Vector3 targetPos;
    // predicted X position this object will move to
    float predictedX;
    // predicted Y position this object will move tu next
    float predictedY;
    // previous position of this object, used for cutting out redundant position updates.
    Vector3 previousPos;

    /// <summary>
    /// Follow the target. (Default: true)
    /// </summary>
    [Header("General")]
    public bool follow = true;
    /// <summary>
    /// Follow the target in horizontal directions. (Default: true)
    /// </summary>
    public bool followX = true;
    /// <summary>
    /// Follow the target in vertical directions. (Default: true)
    /// </summary>
    public bool followY = true;

    /// <summary>
    /// Use the bounds system when in SmoothDamp following method. (Default: true)
    /// </summary>
    [Header("Bound Controls (for SmoothDamp following method)")]
    public bool useBounds = true;
    /// <summary>
    /// Custom right boundry tag. (Default: RightBound)
    /// </summary>
    [SerializeField] string rightBoundTag = "RightBound";
    /// <summary>
    /// Custom left boundry tag. (Default: LeftBound)
    /// </summary>
    [SerializeField] string leftBoundTag = "LeftBound";
    /// <summary>
    /// Custom up boundry tag. (Default: UpperBound)
    /// </summary>
    [SerializeField] string upperBoundTag = "UpperBound";
    /// <summary>
    /// Custom down boundry tag. (Default: BottomBound)
    /// </summary>
    [SerializeField] string bottomBoundTag = "BottomBound";

    // should the right boundry be applied?
    bool applyRightBound;
    // should the left boundry be applied?
    bool applyLeftBound;
    // should the up boundry be applied?
    bool applyUpperBound;
    // should the down boundry be applied?
    bool applyBottomBound;

    // x position of right bound
    float rightBound;
    // x position of left bound
    float leftBound;
    // y position of upper bound
    float upperBound;
    // y position of bottom bound
    float bottomBound;

    /// <summary>
    /// Follow target in horizontal directions using SmoothDamp to smooth out movement. (Default: true)
    /// </summary>
    [Header("Following Methods")]
    public bool useSmoothDampX = true;
    /// <summary>
    /// Follow target in vertical directions using SmoothDamp to smooth out movement. (Default: true)
    /// </summary>
    public bool useSmoothDampY = true;

    /// <summary>
    /// Follow target in horizontal directions using LevelSystem (older games style). (Default: false)
    /// </summary>
    [Space(10)]
    public bool useLevelSystemX = false;
    /// <summary>
    /// Follow target in vertical directions using LevelSystem (older games style). (Default: false)
    /// </summary>
    public bool useLevelSystemY = false;

    /// <summary>
    /// Follow target in horizontal directions with no smoothing. (Default: false)
    /// </summary>
    [Space(10)]
    public bool useDirectFollowX = false;
    /// <summary>
    /// Follow target in vertical directions with no smoothing. (Default: false)
    /// </summary>
    public bool useDirectFollowY = false;

    /// <summary>
    /// Damp time for horizontal smoothing when using SmoothDamp following method. (Default: 0.05f)
    /// </summary>
    [Header("SmoothDamp Controls")]
    public float dampTimeX = 0.05f;
    /// <summary>
    /// Damp time for vertical smoothing when using SmoothDamp following method. (Default: 0.035f)
    /// </summary>
    public float dampTimeY = 0.035f;

    /// <summary>
    /// Offset the following position upwards based on values in lookOffset when using SmoothDamp or DirectFollow following methods.
    /// </summary>
    [HideInInspector] public bool lookUp;
    /// <summary>
    /// Offset the following position downwards based on values in lookOffset when using SmoothDamp or DirectFollow following methods.
    /// </summary>
    [HideInInspector] public bool lookDown;
    /// <summary>
    /// Offset the following position leftwards based on values in lookOffset when using SmoothDamp or DirectFollow following methods.
    /// </summary>
    [HideInInspector] public bool lookLeft;
    /// <summary>
    /// Offset the following position rightwards based on values in lookOffset when using SmoothDamp or DirectFollow following methods.
    /// </summary>
    [HideInInspector] public bool lookRight;

    /// <summary>
    /// Should the look offset be applied? (Default: true)
    /// </summary>
    [Space(10)]
    public bool useLookOffset = true;
    /// <summary>
    /// Original offset the object will return to if look offset is applied. (Default: 0,0)
    /// </summary>
    public Vector2 originalOffset = Vector2.zero;
    /// <summary>
    /// Offset the object should follow if player looks that way. Used in Direct and SmoothDamp follow methods. (Default: 5,5.5f)
    /// </summary>
    public Vector2 lookOffset = new Vector2(5, 5.5f);
    // current offset
    Vector2 offset;

    /// <summary>
    /// Shift the  horizontal offset to horizontal look offset gradually
    /// </summary>
    [Space(10)]
    public bool shiftLookX = true;
    /// <summary>
    /// Shift the vertical offset to vertical look offset gradually
    /// </summary>
    public bool shiftLookY = true;
    /// <summary>
    /// Time it takes in seconds to go from centered offset to horizontal directional look offset (Default: 0.35f)
    /// </summary>
    public float shiftLookXTime = 0.35f;
    /// <summary>
    /// Time it takes in seconds to go from centered offset to vertical directional look offset (Default: 0.35f)
    /// </summary>
    public float shiftLookYTime = 0.35f;

    /// <summary>
    /// Time it takes in seconds before the look offset starts being applied. (Default: 0)
    /// </summary>
    [Space(10)]
    public float lookXDelay = 0;
    /// <summary>
    /// Time it takes in seconds before the look offset starts being applied. (Default: 1)
    /// </summary>
    public float lookYDelay = 1;
    // Timer for looking up
    float lookUpTimer = 0;
    // Timer for looking down
    float lookDownTimer = 0;
    // Timer for looking left
    float lookLeftTimer = 0;
    // Timer for looking right
    float lookRightTimer = 0;

    /// <summary>
    /// Centers the offset when the target stops moving in horizontal directions. (Default: true)
    /// </summary>
    [Space(10)]
    public bool centerLookX = true;
    /// <summary>
    /// Centers the offset when the target stops moving in vertical directions. (Default: true)
    /// </summary>
    public bool centerLookY = true;
    /// <summary>
    /// Time it takes in seconds before centering horizontal offset. (Default: 3)
    /// </summary>
    public float centerLookXTime = 3;
    /// <summary>
    /// Time it takes in seconds before centering vertical offset. (Default: 0)
    /// </summary>
    public float centerLookYTime = 0;
    // timer for centering horizontal offset
    float centerLookXTimer = 0;
    // timer for centering vertical offset
    float centerLookYTimer = 0;

    /// <summary>
    /// Smooth damp time for transitioning between horizontal levels. (Default: 0.035f)
    /// </summary>
    [Header("Level System Controls")]
    public float levelDampTimeX = 0.035f;
    /// <summary>
    /// Smooth damp time for transitioning between vertical levels. (Default: 0.035f)
    /// </summary>
    public float levelDampTimeY = 0.035f;

    /// <summary>
    /// Screen percentage for the horizontal levels considered to be a "level" in the LevelSystem. (Default: 1)
    /// </summary>
    [Space(10)]
    [Range(0,1)]
    public float percentPerLevelX = 1;
    /// <summary>
    /// Screen percentage for the horizontal levels considered to be a "level" in the LevelSystem. (Default: 1)
    /// </summary>
    [Range(0,1)]
    public float percentPerLevelY = 1;

    /// <summary>
    /// Horizonal offset of levels when using Level System. (Default: 0)
    /// </summary>
    [Space(10)]
    public float levelXOffset = 0;
    /// <summary>
    /// Vertical offset of levels when using Level System. (Default: 0)
    /// </summary>
    public float levelYOffset = 0;

    void Awake()
    {
        camera = GetComponent<Camera>();
        target.SendMessage("SetFollower", this);
        boxCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        //setup initial predicted X and Y values, and previous position
        predictedX = transform.position.x;
        predictedY = transform.position.y;
        previousPos = transform.position;
    }


    /// <summary>
    /// Resets the look offset.
    /// </summary>
    public void ResetLookOffsets()
    {
        offset.x = originalOffset.x;
        offset.y = originalOffset.y;
    }

    void Update()
    {
        // if the object should follow the target
        if (follow)
        {
            // get camera's horizontal and vertical size
            cameraXSize = camera.orthographicSize * 2 * camera.aspect * (percentPerLevelX);
            cameraYSize = camera.orthographicSize * 2 * (percentPerLevelY);

            // set variables for later calculations
            targetPos = target.position;
            if (useBounds)
                boxCollider.size = new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, 1000);

            // set / center offset if using SmoothDamp following method
            if (useSmoothDampX || useSmoothDampY)
            {
                // set the offset
                SetOffset();
                CenterLookOffsets();
            }

            // choose a following method
            ChooseFollowMethod();
        }
    }

    // chooses the following method based on what following methods are selected
    void ChooseFollowMethod()
    {
        // choose the horizontal follow method
        if (followX)
        {
            if (useSmoothDampX)
                SmoothDampX();
            else if (useLevelSystemX)
                LevelSystemX();
            else if (useDirectFollowX)
                DirectFollowX();
        }

        // choose the vertical follow method
        if (followY)
        {
            if (useSmoothDampY)
                SmoothDampY();
            else if (useLevelSystemY)
                LevelSystemY();
            else if(useDirectFollowY)
                DirectFollowY();
        }
    }

    // smooth out horizontal movement when following
    void SmoothDampX()
    {
        // set up SmoothDamp() and get a predicted position
        var velocity = Vector3.zero;
        var destination = new Vector3(targetPos.x + offset.x, transform.position.y, transform.position.z);
        predictedX = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTimeX * Time.deltaTime).x;
        var predictedPos = new Vector3(predictedX, transform.position.y, transform.position.z);

        // if applying bounds, constrain movement to not surpass the bounds.
        if (useBounds)
        {
            if (applyRightBound)
            {
                var rightThreshhold = rightBound - (cameraXSize / 2);
                if (predictedX < rightThreshhold)
                    MoveToPosition(predictedPos);
                else
                    MoveToPosition(new Vector3(rightThreshhold, transform.position.y, transform.position.z));
            }
            else if (applyLeftBound)
            {
                var leftThreshhold = leftBound + (cameraXSize / 2);
                if (predictedX > leftThreshhold)
                    MoveToPosition(predictedPos);
                else
                    MoveToPosition(new Vector3(leftThreshhold, transform.position.y, transform.position.z));
            }
            else
            {
                MoveToPosition(predictedPos);
            }
        }
        // if not applying bounds, set position right away.
        else
        {
            MoveToPosition(predictedPos);
        }

    }

    // smooth out vertucal movement when following
    void SmoothDampY()
    {
        // set up SmoothDamp() and get a predicted position
        var velocity = Vector3.zero;
        var destination = new Vector3(transform.position.x, targetPos.y + offset.y, transform.position.z);
        predictedY = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTimeY * Time.deltaTime).y;
        var predictedPos = new Vector3(transform.position.x, predictedY, transform.position.z);

        // if applying bounds, constrain movement to not surpass the bounds.
        if (useBounds)
        {
            if (applyUpperBound)
            {
                var upperThreshhold = upperBound - (cameraYSize / 2);
                if (predictedY < upperThreshhold)
                    MoveToPosition(predictedPos);
                else
                    MoveToPosition(new Vector3(transform.position.x, upperThreshhold, transform.position.z));
            }
            else if (applyBottomBound)
            {
                var bottomThreshhold = bottomBound + (cameraYSize / 2);
                if (predictedY > bottomThreshhold)
                    MoveToPosition(predictedPos);
                else
                    MoveToPosition(new Vector3(transform.position.x, bottomThreshhold, transform.position.z));
            }
            else
            {
                MoveToPosition(predictedPos);
            }
        }
        // if not applying bounds, set position right away.
        else
        {
            MoveToPosition(predictedPos);
        }
    }

    // sets the offset of the object to follow
    void SetOffset()
    {
        if (useSmoothDampX)
        {
            if (lookLeft)
            {
                if (lookLeftTimer < lookXDelay)
                    lookLeftTimer += Time.deltaTime;
                else
                    if (shiftLookX)
                    offset.x = Mathf.Max(offset.x - lookOffset.x * Time.deltaTime / shiftLookXTime, -lookOffset.x);
                else
                    offset.x = -lookOffset.x;
            }
            else
            {
                lookLeftTimer = 0;
            }

            if (lookRight)
            {
                if (lookRightTimer < lookXDelay)
                    lookRightTimer += Time.deltaTime;
                else
                    if (shiftLookX)
                    offset.x = Mathf.Min(offset.x + lookOffset.x * Time.deltaTime / shiftLookXTime, lookOffset.x);
                else
                    offset.x = lookOffset.x;
            }
            else
            {
                lookRightTimer = 0;
            }
        }

        if (useSmoothDampY)
        {
            if (lookUp)
            {
                if (lookUpTimer < lookYDelay)
                    lookUpTimer += Time.deltaTime;
                else
                    if (shiftLookY)
                    offset.y = Mathf.Min(offset.y + lookOffset.y * Time.deltaTime / shiftLookYTime, lookOffset.y);
                else
                    offset.y = lookOffset.y;
            }
            else
            {
                lookUpTimer = 0;
            }

            if (lookDown)
            {
                if (lookDownTimer < lookYDelay)
                    lookDownTimer += Time.deltaTime;
                else
                    if (shiftLookY)
                    offset.y = Mathf.Max(offset.y - lookOffset.y * Time.deltaTime / shiftLookYTime, -lookOffset.y);
                else
                    offset.y = -lookOffset.y;
            }
            else
            {
                lookDownTimer = 0;
            }
        }
    }

    // centers the offset when target is not moving.
    void CenterLookOffsets()
    {
        if (centerLookX)
        {
            if (!lookRight && !lookLeft)
            {
                if (centerLookXTimer < centerLookXTime)
                    centerLookXTimer += Time.deltaTime;
                else
                    offset.x = originalOffset.x;
            }
            else
            {
                centerLookXTimer = 0;
            }
        }

        if (centerLookY)
        {
            if (!lookUp && !lookDown)
            {
                if (centerLookYTimer < centerLookYTime)
                    centerLookYTimer += Time.deltaTime;
                else
                    offset.y = originalOffset.y;
            }
            else
            {
                centerLookYTimer = 0;
            }
        }
    }

    // move in horizontal directions with a Level System
    void LevelSystemX()
    {
        float levelX = (targetPos.x - levelXOffset) / cameraXSize / percentPerLevelX;
        if (targetPos.x < 0)
            levelX -= 0.5f;
        else
            levelX += 0.5f;

        // smooth out the transition between levels
        var velocity = Vector3.zero;
        var destination = new Vector3((int)levelX * cameraXSize * percentPerLevelX + levelXOffset, transform.position.y, transform.position.z);
        var predictedPos = Vector3.SmoothDamp(transform.position, destination, ref velocity, levelDampTimeX);
        MoveToPosition(predictedPos);
    }

    // move in vertical directions with a Level System
    void LevelSystemY()
    {
        float levelY = (targetPos.y - levelYOffset) / cameraYSize / percentPerLevelY;
        if (targetPos.y < 0)
            levelY -= 0.5f;
        else
            levelY += 0.5f;

        // smooth out the transition between levels
        var velocity = Vector3.zero;
        var destination = new Vector3(transform.position.x, (int)levelY * cameraYSize * percentPerLevelY + levelYOffset, transform.position.z);
        var predictedPos = Vector3.SmoothDamp(transform.position, destination, ref velocity, levelDampTimeY);
        MoveToPosition(predictedPos);
    }

    // follows the target directly with no smoothing
    void DirectFollowX()
    {
        MoveToPosition(new Vector3(targetPos.x + offset.x, transform.position.y, transform.position.z));
    }

    // follows the target directly with no smoothing
    void DirectFollowY()
    {
        MoveToPosition(new Vector3(transform.position.x, targetPos.y + offset.y, transform.position.z));
    }

    // moves the object to a position unless it is already at that position
    void MoveToPosition(Vector3 pos)
    {
        // if the previous position is not the same as requested position, move to position
        if (pos != previousPos)
            transform.position = pos;
        // set previous position
        previousPos = transform.position;
    }

    // runs when a trigger enters this object's collider
    void OnTriggerEnter(Collider other)
    {
        // if using bounds, check if the trigger detected is a bound, and if so, set up that bound.
        if (useBounds)
        {
            if (useSmoothDampX)
            {
                if (other.CompareTag(rightBoundTag))
                    SetUpBound(other.transform.position.x, "right");
                else if (other.CompareTag(leftBoundTag))
                    SetUpBound(other.transform.position.x, "left");
            }

            if (useSmoothDampY)
            {
                if (other.CompareTag(upperBoundTag))
                    SetUpBound(other.transform.position.y, "up");
                else if (other.CompareTag(bottomBoundTag))
                    SetUpBound(other.transform.position.y, "down");
            }
        }
    }

    // runs when a trigger exits this object's collider
    void OnTriggerExit(Collider other)
    {
        // if using bounds, check if the trigger detected is a bound, and if so, reset that bound.
        if (useBounds)
        {
            if (useSmoothDampX)
            {
                if (other.CompareTag(rightBoundTag))
                    ResetBound("right");
                else if (other.CompareTag(leftBoundTag))
                    ResetBound("left");
            }

            if (useSmoothDampY)
            {
                if (other.CompareTag(upperBoundTag))
                    ResetBound("up");
                else if (other.CompareTag(bottomBoundTag))
                    ResetBound("down");
            }
        }
    }

    // sets up a bound, given that bound's X/Y position and direction
    void SetUpBound(float boundPos, string dir)
    {
        // set up the appropriate bound
        switch (dir)
        {
            case "right":
                applyRightBound = true;
                rightBound = boundPos;
                break;
            case "left":
                applyLeftBound = true;
                leftBound = boundPos;
                break;
            case "up":
                applyUpperBound = true;
                upperBound = boundPos;
                break;
            case "down":
                applyBottomBound = true;
                bottomBound = boundPos;
                break;
        }
    }

    // resets a bound, given that bound's direction
    void ResetBound(string dir)
    {
        // reset the appropriate bound
        switch (dir)
        {
            case "right":
                applyRightBound = false;
                break;
            case "left":
                applyLeftBound = false;
                break;
            case "up":
                applyUpperBound = false;
                break;
            case "down":
                applyBottomBound = false;
                break;
        }
    }
}
