using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        transform.position = PlayerTransform.position + new Vector3(-1f, 1.39999998f, -1f);
    }
}
