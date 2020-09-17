using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float yMin = -20;
    public float yMax = 40;     //clamps for camera
    float xRotation = 0;
    float yRotation = 0;
    public Transform target;    //the focus
    public float distance = 10.0f;  //distance from target
    public float rotateSpeed = 5;   //camera speed/sensitivity
    public Transform groundPos;     //normalizes player movement

    public GameObject pauseMenu;
    private bool paused;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        paused = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            Pause();
        }

        Vector2 controlInput = new Vector2(Input.GetAxis("Mouse X")*rotateSpeed, Input.GetAxis("Mouse Y")*rotateSpeed);     //combines the player input
        if (paused)
        {
            controlInput = Vector2.zero;
        }

        xRotation += Mathf.Repeat(controlInput.x, 360.0f);      //allows for constant 360 rotation
        yRotation -= controlInput.y;
        yRotation = Mathf.Clamp(yRotation, yMin, yMax);         //clamps the y

        Quaternion newRotation = Quaternion.AngleAxis(xRotation, Vector3.up);   //calculates the rotation into a quaternion
        newRotation *= Quaternion.AngleAxis(yRotation, Vector3.right);

        transform.rotation = newRotation;
        transform.position = target.position - (transform.forward * distance);  //distances from the target

        groundPos.position = new Vector3(transform.position.x, target.position.y, transform.position.z);    //moves groundPos below the camera at player Y
        groundPos.LookAt(target);                                                                           //rotates the groundPos
        groundPos.position = target.position - (groundPos.forward * distance);                              //distances the groundPos to normalize player movement

    }

    public void Pause()
    {
        paused = !paused;
        pauseMenu.GetComponent<PauseMenu>().ShowMenu();
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
