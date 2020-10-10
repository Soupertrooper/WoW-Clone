using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public RectTransform compass, playerP;

    Transform player;
    public Camera mapCam;

    [Range(1,5)]
    public int zoomLevel = 1;
    public bool pointNorth = true;

    void Start()
    {
        player = FindObjectOfType<PlayerControls>().transform;  
    }

    void Update()
    {
        transform.position = player.position;

        if (pointNorth)
        {
            if (transform.forward != Vector3.forward)
            {
                transform.forward = Vector3.forward;
                compass.eulerAngles = Vector3.zero;
            }

            playerP.eulerAngles = new Vector3(0, 0, 180 - player.eulerAngles.y);
        }
        else
        {
            if (playerP.eulerAngles != new Vector3(0, 0, 180))
                playerP.eulerAngles = new Vector3(0, 0, 180);

            transform.forward = player.forward;
            compass.eulerAngles = new Vector3(0, 0, player.eulerAngles.y);
        }

        if (mapCam.orthographicSize != zoomLevel * 5)
            mapCam.orthographicSize = zoomLevel * 5;
    }

    public void Zoom(int zLevel)
    {
        zoomLevel += zLevel;
        zoomLevel = Mathf.Clamp(zoomLevel, 1, 5);
    }
}
