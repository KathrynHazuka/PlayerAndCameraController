using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public delegate void PlayerKilled();
    public static event PlayerKilled OnPlayerKilled;

    public delegate void PlayerRespawned();
    public static event PlayerRespawned OnPlayerRespawned;

    new Rigidbody rigidbody;
    SpriteRenderer spriteRenderer;

    float respawnTime = 1;
    Vector3 respawnPos;
    Vector3 deadPos = new Vector3(-100000, 0, 0);

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

	// Use this for initialization
	void Start ()
    {
        respawnPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void Kill()
    {
        spriteRenderer.enabled = false;
        transform.position = deadPos;
        rigidbody.isKinematic = true;

        if (OnPlayerKilled != null)
            OnPlayerKilled();

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        transform.position = respawnPos;
        spriteRenderer.enabled = true;
        rigidbody.isKinematic = false;

        if (OnPlayerRespawned != null)
            OnPlayerRespawned();
    }
}
