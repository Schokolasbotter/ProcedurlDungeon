using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//This camera follows the player and looks from directly above. The camera this script is attached to is the camera which renders the mini map
public class minimapCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
        playerPosition.y = 5f;
        transform.position = playerPosition;
    }
}
