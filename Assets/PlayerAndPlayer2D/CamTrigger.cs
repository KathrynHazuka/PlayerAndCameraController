using UnityEngine;
using System.Collections;

public class CamTrigger : MonoBehaviour {

    MenuManager menuManager;
    public int camMode;

    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (camMode)
            {
                case 0:
                    menuManager.DampXYSystem();
                    break;
                case 1:
                    menuManager.LevelXYSystem();
                    break;
            }
        }
    }
}
