using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    Camera2D camera2D;

    void Awake()
    {
        Player.OnPlayerKilled += Player_OnPlayerKilled;
        Player.OnPlayerRespawned += Player_OnPlayerRespawned;

        camera2D = GetComponent<Camera2D>();
    }

    private void Player_OnPlayerKilled()
    {
        camera2D.follow = false;
        camera2D.ResetLookOffsets();
    }

    private void Player_OnPlayerRespawned()
    {
        camera2D.follow = true;
    }
}
