using UnityEngine;
using System.Collections.Generic;

public class LedgeBox2D : MonoBehaviour
{
    // reference to the player controller
    PlayerController2D controller;
    Rigidbody playerRigidbody;

    // true = this is the right box, false = this is the left box
    [SerializeField] bool rightSideBox;

    // list of currently created ledges for testing
    List<LedgeInfo> ledges = new List<LedgeInfo>();

    void Start()
    {
        // set the controller reference
        controller = transform.parent.GetComponent<PlayerController2D>();
        playerRigidbody = transform.parent.GetComponent<Rigidbody>();
    }

    // runs when a collider enters this object's trigger(s)
    void OnTriggerEnter(Collider other)
    {
        // check if the collider detected is a ground. If so, create a ledge for that ground
        foreach (string groundTag in controller.groundTags)
        {
            if (other.CompareTag(groundTag))
            {
                AddLedge(other);
            }
        }
    }

    // runs when a collider stays inside this object's trigger(s)
    void OnTriggerStay(Collider other)
    {
        // check if the collider detected is a ledge. If player is moving towords the ledge, tell contoller that player is trying to grab the ledge
        if (other.CompareTag(controller.ledgeTag) && playerRigidbody.velocity.y < 0)
        {
            if (rightSideBox && controller.right || !rightSideBox && controller.left)
            {
                GrabLedge(other);
            }
        }
    }

    // runs when a collider exits this object's trigger(s)
    void OnTriggerExit(Collider other)
    {
        // check if the collider detected is a ground. If so, het rid of the ledge as it's no longer needed
        foreach (string groundTag in controller.groundTags)
        {
            if (other.CompareTag(groundTag))
            {
                RemoveLedge(other);
            }
        }
    }

    // adds a ledge and its info to the running list of ledges for testing
    void AddLedge(Collider ground)
    {
        float xPos;
        if (rightSideBox)
            xPos = ground.transform.position.x - ((ground.GetComponent<BoxCollider>().size.x * ground.transform.localScale.x) / 2) + (controller.ledgeSize.x / 2);
        else
            xPos = ground.transform.position.x + ((ground.GetComponent<BoxCollider>().size.x * ground.transform.localScale.x) / 2) - (controller.ledgeSize.x / 2);
        float yPos = ground.transform.position.y + ((ground.GetComponent<BoxCollider>().size.y * ground.transform.localScale.y) / 2) - (controller.ledgeSize.y / 2);

        var ledgeInfo = new LedgeInfo();
        ledgeInfo.ground = ground;
        ledgeInfo.ledge = new GameObject("Ledge").AddComponent<BoxCollider>();
        ledgeInfo.ledge.transform.position = new Vector3(xPos, yPos, ground.transform.position.z);
        ledgeInfo.ledge.size = controller.ledgeSize;
        ledgeInfo.ledge.tag = controller.ledgeTag;
        ledges.Add(ledgeInfo);
    }

    // tells the controller the player is trying to grab the ledge.
    void GrabLedge(Collider ledge)
    {
        foreach (LedgeInfo ledgeInfo in ledges)
        {
            if (ledgeInfo.ledge == ledge)
            {
                float xPos;
                if (rightSideBox)
                    xPos = ledgeInfo.ledge.transform.position.x - (controller.ledgeSize.x / 2);
                else
                    xPos = ledgeInfo.ledge.transform.position.x + (controller.ledgeSize.x / 2);
                float yPos = ledgeInfo.ledge.transform.position.y + (controller.ledgeSize.y / 2);

                controller.grabLedge = true;
                controller.ledge = new Vector2(xPos, yPos);
                controller.ledgeOnRight = rightSideBox;
                
                ledges.Remove(ledgeInfo);
                Destroy(ledgeInfo.ledge.gameObject);
                break;
            }
        }
    }

    // removes the ledge and its info from the running list as its no longer needed.
    void RemoveLedge(Collider ground)
    {
        foreach (LedgeInfo ledgeInfo in ledges)
        {
            if (ledgeInfo.ground == ground)
            {
                ledges.Remove(ledgeInfo);
                Destroy(ledgeInfo.ledge.gameObject);
                break;
            }
        }
    }

    // struct for ledge info. Holds the ground related to the ledge and the ledge itself.
    struct LedgeInfo
    {
        public Collider ground;
        public BoxCollider ledge;
    }
}
