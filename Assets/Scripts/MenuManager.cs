using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    Camera2D followCamera;
    PlayerController2D controller;

    void Awake()
    {
        followCamera = Camera.main.GetComponent<Camera2D>();
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
    }

    public void ToggleUsesRun()
    {
        if (controller.usesRun)
            controller.usesRun = false;
        else
            controller.usesRun = true;
    }

    public void ToggleUsesDoubleJump()
    {
        if (controller.usesDoubleJump)
            controller.usesDoubleJump = false;
        else
            controller.usesDoubleJump = true;
    }

    public void ToggleFollow()
    {
        if (followCamera.follow)
            followCamera.follow = false;
        else
            followCamera.follow = true;
    }

    public void ToggleFollowX()
    {
        if (followCamera.followX)
            followCamera.followX = false;
        else
            followCamera.followX = true;
    }

    public void ToggleFollowY()
    {
        if (followCamera.followY)
            followCamera.followY = false;
        else
            followCamera.followY = true;
    }

    public void DampX()
    {
        followCamera.useLevelSystemX = false;
        followCamera.useSmoothDampX = true;
    }

    public void DampY()
    {
        followCamera.useLevelSystemY = false;
        followCamera.useSmoothDampY = true;
    }

    public void LevelX()
    {
        followCamera.useSmoothDampX = false;
        followCamera.useLevelSystemX = true;
    }

    public void LevelY()
    {
        followCamera.useSmoothDampY = false;
        followCamera.useLevelSystemY = true;
    }

    public void DampXYSystem()
    {
        followCamera.useLevelSystemX = false;
        followCamera.useLevelSystemY = false;
        followCamera.useSmoothDampX = true;
        followCamera.useSmoothDampY = true;
    }

    public void LevelXYSystem()
    {
        followCamera.useSmoothDampX = false;
        followCamera.useSmoothDampY = false;
        followCamera.useLevelSystemX = true;
        followCamera.useLevelSystemY = true;
    }
}
