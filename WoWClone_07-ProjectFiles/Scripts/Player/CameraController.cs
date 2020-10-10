using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Input Variables
    KeyCode leftMouse = KeyCode.Mouse0, rightMouse = KeyCode.Mouse1, middleMouse = KeyCode.Mouse2;

    //Camera Variables
    public float CameraHeight = 1.75f, CameraMaxDistance = 25;
    float CameraMaxTilt = 90;
    [Range(0,4)]
    public float CameraSpeed = 2;
    float currentPan, currentTilt = 10, currentDistance = 5;
    [HideInInspector]
    public bool autoRunReset = false;

    //CamState
    public CameraState cameraState = CameraState.CameraNone;

    //References
    PlayerControls player;
    public Transform tilt;
    Camera mainCam;

    void Start()
    {
        player = FindObjectOfType<PlayerControls>();
        player.mainCam = this;
        mainCam = Camera.main;

        transform.position = player.transform.position + Vector3.up * CameraHeight;
        transform.rotation = player.transform.rotation;

        tilt.eulerAngles = new Vector3(currentTilt, transform.eulerAngles.y, transform.eulerAngles.z);
        mainCam.transform.position += tilt.forward * -currentDistance;
    }

    void Update()
    {
        if (!Input.GetKey(leftMouse) && !Input.GetKey(rightMouse) && !Input.GetKey(middleMouse)) //if no mouse button is pressed
            cameraState = CameraState.CameraNone;
        else if (Input.GetKey(leftMouse) && !Input.GetKey(rightMouse) && !Input.GetKey(middleMouse)) //if left mouse button is pressed
            cameraState = CameraState.CameraRotate;
        else if (!Input.GetKey(leftMouse) && Input.GetKey(rightMouse) && !Input.GetKey(middleMouse)) //if right mouse button is pressed
            cameraState = CameraState.CameraSteer;
        else if ((Input.GetKey(leftMouse) && Input.GetKey(rightMouse)) || Input.GetKey(middleMouse)) //if left and right mouse button or middle mouse button is pressed
            cameraState = CameraState.CameraRun;

        CameraInputs();
    }

    void LateUpdate()
    {
        CameraTransforms();
    }

    void CameraInputs()
    {
        if(cameraState != CameraState.CameraNone)
        {
            if (cameraState == CameraState.CameraRotate)
            {
                if (player.steer)
                    player.steer = false;

                currentPan += Input.GetAxis("Mouse X") * CameraSpeed;
            }
            else if(cameraState == CameraState.CameraSteer || cameraState == CameraState.CameraRun)
            {
                if (!player.steer)
                    player.steer = true;
            }

            currentTilt -= Input.GetAxis("Mouse Y") * CameraSpeed;
            currentTilt = Mathf.Clamp(currentTilt, -CameraMaxTilt, CameraMaxTilt);
        }
        else
        {
            if (player.steer)
                player.steer = false;
        }

        currentDistance -= Input.GetAxis("Mouse ScrollWheel") * 2;
        currentDistance = Mathf.Clamp(currentDistance, 0, CameraMaxDistance);
    }

    void CameraTransforms()
    {
        switch(cameraState)
        {
            case CameraState.CameraSteer:
            case CameraState.CameraRun:
            case CameraState.CameraNone:
                currentPan = player.transform.eulerAngles.y;
                break;
        }

        if(cameraState == CameraState.CameraNone)
            currentTilt = 10;

        if(cameraState == CameraState.CameraRun)
        {
            player.autoRun = true;

            if(!autoRunReset)
                autoRunReset = true;
        }
        else
        {
            if(autoRunReset)
            {
                player.autoRun = false;
                autoRunReset = false;
            }
        }

        transform.position = player.transform.position + Vector3.up * CameraHeight;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentPan, transform.eulerAngles.z);
        tilt.eulerAngles = new Vector3(currentTilt, tilt.eulerAngles.y, tilt.eulerAngles.z);
        mainCam.transform.position = transform.position + tilt.forward * -currentDistance;
    }

    public enum CameraState { CameraNone, CameraRotate, CameraSteer, CameraRun }
}
