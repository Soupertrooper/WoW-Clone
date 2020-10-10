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

    //CameraSmoothing
    float panAngle, panOffset;
    bool camXAdjust, camYAdjust;
    float rotationXCushion = 3, rotationXSpeed = 0;
    float yRotMin = 0, yRotMax = 20, rotationYSpeed = 0;

    //CamState
    public CameraState cameraState = CameraState.CameraNone;

    //Options
    [Range(0.25f, 1.75f)]
    public float cameraAdjustSpeed = 1;
    public CameraMoveState camMoveState = CameraMoveState.OnlyWhileMoving;

    //Collision
    public bool collisionDebug;
    public float collisionCushion = 0.35f;
    float adjustedDistance;
    public LayerMask collisionMask;
    Ray camRay;
    RaycastHit camRayHit;

    //References
    PlayerControls player;
    public Transform tilt;
    Camera mainCam;

    private void Awake()
    {
        transform.SetParent(null);
    }

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

        CameraCollisions();
        CameraInputs();
    }

    void LateUpdate()
    {
        panAngle = Vector3.SignedAngle(transform.forward, player.transform.forward, Vector3.up);

        switch(camMoveState)
        {
            case CameraMoveState.OnlyWhileMoving:
                if (player.inputNormalized.magnitude > 0 || player.rotation != 0)
                {
                    CameraXAdjust();
                    CameraYAdjust();
                }
                break;

            case CameraMoveState.OnlyHorizontalWhileMoving:
                if(player.inputNormalized.magnitude > 0 || player.rotation != 0)
                    CameraXAdjust();
                break;

            case CameraMoveState.AlwaysAdjust:
                CameraXAdjust();
                CameraYAdjust();
                break;

            case CameraMoveState.NeverAdjust:
                CameraNeverAdjust();
                break;
        }

        CameraTransforms();
    }

    void CameraCollisions()
    {
        float camDistance = currentDistance + collisionCushion;

        camRay.origin = transform.position;
        camRay.direction = -tilt.forward;

        if(Physics.Raycast(camRay, out camRayHit, camDistance, collisionMask))
        {
            adjustedDistance = Vector3.Distance(camRay.origin, camRayHit.point) - collisionCushion;
        }
        else
        {
            adjustedDistance = currentDistance;
        }

        if (collisionDebug)
            Debug.DrawLine(camRay.origin, camRay.origin + camRay.direction * camDistance, Color.cyan);
    }

    void CameraInputs()
    {
        if(cameraState != CameraState.CameraNone)
        {
            if (!camYAdjust && (camMoveState == CameraMoveState.AlwaysAdjust || camMoveState == CameraMoveState.OnlyWhileMoving))
                camYAdjust = true;

            if (cameraState == CameraState.CameraRotate)
            {
                if (!camXAdjust && camMoveState != CameraMoveState.NeverAdjust)
                    camXAdjust = true;

                if (player.steer)
                    player.steer = false;

                currentPan += Input.GetAxis("Mouse X") * CameraSpeed;
            }
            else if(cameraState == CameraState.CameraSteer || cameraState == CameraState.CameraRun)
            {
                if (!player.steer)
                {
                    Vector3 playerReset = player.transform.eulerAngles;
                    playerReset.y = transform.eulerAngles.y;

                    player.transform.eulerAngles = playerReset;

                    player.steer = true;
                }
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

    void CameraXAdjust()
    {
        if(cameraState != CameraState.CameraRotate)
        {
            if (camXAdjust)
            {
                rotationXSpeed += Time.deltaTime * cameraAdjustSpeed;

                if (Mathf.Abs(panAngle) > rotationXCushion)
                    currentPan = Mathf.Lerp(currentPan, currentPan + panAngle, rotationXSpeed);
                else
                    camXAdjust = false;
            }
            else
            {
                if (rotationXSpeed > 0)
                    rotationXSpeed = 0;

                currentPan = player.transform.eulerAngles.y;
            }
        }
    }

    void CameraYAdjust()
    {
        if (cameraState == CameraState.CameraNone)
        {
            if(camYAdjust)
            {
                rotationYSpeed += (Time.deltaTime / 2) * cameraAdjustSpeed;

                if (currentTilt >= yRotMax || currentTilt <= yRotMin)
                    currentTilt = Mathf.Lerp(currentTilt, yRotMax / 2, rotationYSpeed);
                else if (currentTilt < yRotMax && currentTilt > yRotMin)
                    camYAdjust = false;
            }
            else
            {
                if (rotationYSpeed > 0)
                    rotationYSpeed = 0;
            }
        }
    }

    void CameraNeverAdjust()
    {
        switch (cameraState)
        {
            case CameraState.CameraSteer:
            case CameraState.CameraRun:
                if(panOffset != 0)
                    panOffset = 0;

                currentPan = player.transform.eulerAngles.y;
                break;

            case CameraState.CameraNone:
                currentPan = player.transform.eulerAngles.y - panOffset;
                break;

            case CameraState.CameraRotate:
                panOffset = panAngle;
                break;
        }
    }

    void CameraTransforms()
    {
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
        mainCam.transform.position = transform.position + tilt.forward * -adjustedDistance;
    }

    public enum CameraState { CameraNone, CameraRotate, CameraSteer, CameraRun }

    public enum CameraMoveState { OnlyWhileMoving, OnlyHorizontalWhileMoving, AlwaysAdjust, NeverAdjust }
}
