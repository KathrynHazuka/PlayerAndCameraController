using UnityEngine;
using System.Collections;

/// <summary>
/// Area that kills any living entity. Requires the entity to have a Kill method.
/// </summary>
public class KillArea : MonoBehaviour {

    // kills entity on trigger exit instead of trigger enter
    [SerializeField] bool killOnExit;

    string[] livingTags;

    void Awake()
    {
        GameManager.OnLivingTagsUpdated += GameManager_OnLivingTagsUpdated;
    }

    // Sets living tags
    void GameManager_OnLivingTagsUpdated(string[] livingTags)
    {
        this.livingTags = livingTags;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!killOnExit)
            VerifyAndKillEntity(col);
    }

    void OnTriggerExit(Collider col)
    {
        if(killOnExit)
            VerifyAndKillEntity(col);
    }

    void VerifyAndKillEntity(Collider col)
    {
        for (int i = 0; i < livingTags.Length; i++)
        {
            if (col.CompareTag(livingTags[i]))
            {
                col.SendMessage("Kill");
                break;
            }
        }
    }
}
