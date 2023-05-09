using UnityEngine;


public class GroundCheck : MonoBehaviour
{
    public PlayerController controller;

    private void Start()
    {
        controller = GetComponentInParent<PlayerController>();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, controller.groundCheckRadius);
    }
}