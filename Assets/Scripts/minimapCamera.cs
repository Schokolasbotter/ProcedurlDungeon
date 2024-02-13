using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
