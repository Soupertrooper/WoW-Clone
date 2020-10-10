using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBody : MonoBehaviour
{
    PlayerControls player;

    void Start()
    {
        player = FindObjectOfType<PlayerControls>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<PlayerControls>() == player)
        {
            if(!player.inWater)
                player.inWater = true;

            if (player.waterSurface != transform.position.y)
                player.waterSurface = transform.position.y;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerControls>() == player)
        {
            if (player.inWater)
                player.inWater = false;
        }
    }
}
