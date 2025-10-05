using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSetup : MonoBehaviour
{
    public Movement movement;
    public Camera playerCamera; // or GameObject playerCameraObject

    public void IslocalPlayer()
    {
        Debug.Log("IslocalPlayer() called on " + gameObject.name);
        movement.enabled = true;
        playerCamera.gameObject.SetActive(true); // enable the camera
    }
}