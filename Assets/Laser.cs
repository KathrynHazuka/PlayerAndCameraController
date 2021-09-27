using UnityEngine;
using System.Collections;
using System;

public class Laser : MonoBehaviour {

    float height;
    float distance;

    Action movementType;

    [SerializeField] GameObject buildingBlock;

    Transform laserTranform;

    // Changes the way that the lazer is positioned from vertical (default, false) to horizontal (true)
    [SerializeField] bool horizontal = false;
    [SerializeField] bool move = false;

    void Awake()
    {
        var heightTransform = transform.Find("Height").transform;
        var distanceTransform = transform.Find("Distance").transform;

        height = heightTransform.position.y - transform.position.y;
        distance = distanceTransform.position.x - transform.position.x;

        Destroy(heightTransform.gameObject);
        Destroy(distanceTransform.gameObject);
        Destroy(GetComponent<SpriteRenderer>());
    }

	// Use this for initialization
	void Start ()
    {
        Action buildType = BuildVertical;
        if (horizontal)
            buildType = BuildHorizontal;

        if (move)
        {
            movementType = MoveHorizontally;

            if (horizontal)
                movementType = MoveVertically;
        }

        BuildInitial();
        buildType();
    }

    void Update()
    {
        if (move)
            movementType();
    }

    void BuildInitial()
    {

    }

    void BuildHorizontal()
    {
        if (move)
        {
            for (int i = 0; i < 2; i++)
            {
                var colliderObj = new GameObject();
                colliderObj.transform.parent = transform;
                colliderObj.transform.localPosition = new Vector3(transform.position.y + (i * distance), height / 2, transform.position.z);
                var colliderComponent = colliderObj.AddComponent<BoxCollider>();
                colliderComponent.size = new Vector3(1, height + 1, 0.2f);
                colliderObj.name = "Collider";
                colliderObj.tag = "Wall";
            }
        }


    }

    void BuildVertical()
    {
        if (move)
        {
            for (int i = 0; i < 2; i++)
            {
                var colliderObj = new GameObject();
                colliderObj.transform.parent = transform;
                colliderObj.transform.localPosition = new Vector3(distance / 2, transform.position.y + (i * height), transform.position.z);
                var colliderComponent = colliderObj.AddComponent<BoxCollider>();
                colliderComponent.size = new Vector3(distance + 1, 1, 0.2f);
                colliderObj.name = "Collider";
                colliderObj.tag = "Ground";
            }
        }

        var laserBackground = Instantiate(buildingBlock);
        laserBackground.transform.parent = transform;
        laserBackground.transform.localPosition = new Vector3(0, height / 2, 0);
        laserBackground.transform.localScale = new Vector3(0.2f, height - 1, 1);
        laserBackground.name = "LaserBG";
        laserBackground.GetComponent<SpriteRenderer>().color = new Color32(255, 120, 120, 150);
    }

    void MoveHorizontally()
    {

    }

    void MoveVertically()
    {

    }
}
