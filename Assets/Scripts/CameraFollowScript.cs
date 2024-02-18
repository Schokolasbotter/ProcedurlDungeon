using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script controls the player camera
//It follows the player constantly and keeps the camera at an angle so the user has an isometric view of the game
public class CameraFollowScript : MonoBehaviour
{
    public Transform PlayerTransform;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        transform.position = PlayerTransform.position + new Vector3(-1f, 1.4f, -1f);
    }
}
