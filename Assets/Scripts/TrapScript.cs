using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    public Animator animator;
    private BoxCollider boxCollider;
    private bool isActive = false;
    private float timer = 0f;
    private float cycleTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider>();
        SetBoxCollider(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= cycleTime)
        {
            isActive = !isActive;
            animator.SetBool("isActive", isActive);
            SetBoxCollider(isActive);
            timer = 0f;
            cycleTime = Random.Range(1f, 5f);
        }
        timer += Time.deltaTime;
    }

    private void SetBoxCollider(bool isActive)
    {
        if(!isActive)
        {
            boxCollider.size = new Vector3(boxCollider.size.x, 0.085f, boxCollider.size.z);
            boxCollider.center = new Vector3(boxCollider.center.x, 0.0425f, boxCollider.center.z);
        }
        else
        {
            boxCollider.size = new Vector3(boxCollider.size.x, 0.5f, boxCollider.size.z);
            boxCollider.center = new Vector3(boxCollider.center.x, 0.25f, boxCollider.center.z);
        }
    }
}
