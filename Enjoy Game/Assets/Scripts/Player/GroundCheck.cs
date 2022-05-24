using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private PlayerAnimation playerAnimation;

    public bool isGrounded;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == gameObject.transform.parent)
            return;

        isGrounded = true;
        playerAnimation.PlayerLanded();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform == gameObject.transform.parent)
            return;

        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}