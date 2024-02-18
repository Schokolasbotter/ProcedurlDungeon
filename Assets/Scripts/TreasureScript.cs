using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// This script manages the treasure. Sets the animator state and loads the scene after the treasure has been interacted with
public class TreasureScript : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public IEnumerator OpenTreasure()
    {
        animator.SetTrigger("OpenTreasure");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
