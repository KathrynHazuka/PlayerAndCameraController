using UnityEngine;
using System.Collections;

/// <summary>
/// Manages game values and game states.
/// </summary>
public class GameManager : MonoBehaviour {

    public delegate void LivingTagsUpdated(string[] livingTags);
    public static event LivingTagsUpdated OnLivingTagsUpdated;

    // Array of tags related to living things
    [SerializeField]
    string[] livingTags;

    // Use this for initialization
    void Start ()
    {
        if (OnLivingTagsUpdated != null)
            OnLivingTagsUpdated(livingTags);

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
