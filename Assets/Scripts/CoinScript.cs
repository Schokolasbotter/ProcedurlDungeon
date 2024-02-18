using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the rotation of the coin and deletes the coin when the player touches the trigger
public class CoinScript : MonoBehaviour
{ 
    public float rotationSpeed =5f;

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
