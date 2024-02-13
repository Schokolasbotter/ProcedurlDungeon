using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public Camera characterCamera;
    public Animator animator;
    public float movementSpeed = 2f;
    public float rotationSpeed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        characterCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        Vector3 cameraPositionVector = characterCamera.transform.position;
        cameraPositionVector.y = transform.position.y;
        Vector3 VectorToCamera = cameraPositionVector - transform.position;
        VectorToCamera = VectorToCamera.normalized;
        Vector3 ForwardVector = VectorToCamera * -1;
        Vector3 RightVector = Vector3.Cross(Vector3.up, ForwardVector);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementVector = ForwardVector * verticalInput + RightVector * horizontalInput;
        movementVector *= movementSpeed * Time.deltaTime;
        GetComponent<CharacterController>().Move(movementVector);
        GetComponent<CharacterController>().Move(new Vector3(0f, -2f, 0f));

        //Animation
        if(movementVector.magnitude != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        //Rotation towards Walking Direction
        if (movementVector != Vector3.zero)
        {
            float angleToRotate = Quaternion.FromToRotation(transform.forward, movementVector).eulerAngles.y;
            if (angleToRotate > 180)
            {
                angleToRotate -= 360f;
            }
            transform.Rotate(Vector3.up, angleToRotate * Time.deltaTime * rotationSpeed);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward,1f);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == "Treasure")
                {
                    StartCoroutine(hit.collider.gameObject.GetComponent<TreasureScript>().OpenTreasure());
                }
                else
                {
                    Destroy(hit.collider.gameObject);
                }
            }
            
        }
    }
}
