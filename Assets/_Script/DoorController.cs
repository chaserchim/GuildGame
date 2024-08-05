using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        animator.SetTrigger("OpenDoor");
        animator.SetTrigger("CloseDoor");

        animator.ResetTrigger("OpenDoor");
        animator.ResetTrigger("CloseDoor");

    }
}
