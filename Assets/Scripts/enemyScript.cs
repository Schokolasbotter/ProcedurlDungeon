using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{
    /* This script controls the enemy NPCs
     * In manages the state between walking and standing, updates the animator
     * It rotates the object in the waling direction and controls the walking
     */

    private Animator animator;
    private CharacterController characterController;
    private float timer = 0;
    private float walkTime;
    private bool isWalking = false;
    private float movementSpeed = 5f;
    public Vector3 movementVector = Vector3.zero;
    private float rotationSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //State Management
        if(timer >= walkTime)
        {
            isWalking = !isWalking;
            if(isWalking)
            {
                movementVector = Vector3.forward * Random.Range(-1f,1f) + Vector3.Cross(Vector3.up, Vector3.forward) * Random.Range(-1f,1f);
                movementSpeed = Random.Range(1f, 3f);
                
            }
            else
            {
                movementVector = Vector3.zero;
            }
            timer = 0;
            walkTime = Random.Range(0.3f, 1f);
        }

        //Rotate in the walking  direction
        movementVector.y = 0f;
        if (movementVector != Vector3.zero)
        {
            float angleToRotate = Quaternion.FromToRotation(transform.forward, movementVector).eulerAngles.y;
            if (angleToRotate > 180)
            {
                angleToRotate -= 360f;
            }
            transform.Rotate(Vector3.up, angleToRotate * Time.deltaTime * rotationSpeed);
        }

        //Walk
        animator.SetBool("isWalking", isWalking);
        movementVector.y = -9f;
        characterController.Move(movementVector * movementSpeed * Time.deltaTime);
        timer += Time.deltaTime;
    }
}
