using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blindness : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("idle") == false)
            animator.Play("blind", 0, 0);
        CancelInvoke("StopBlind");
        Invoke("StopBlind", 1.5f);
    }

    void StopBlind()
    {
        animator.Play("blindExit");
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
