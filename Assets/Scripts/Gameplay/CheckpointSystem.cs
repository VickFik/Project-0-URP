using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetFloat("Position_X", transform.position.x);
            PlayerPrefs.SetFloat("Position_Y", transform.position.y);

            Debug.Log("Position \n x: " + PlayerPrefs.GetFloat("Position_X") + " ,y: " + PlayerPrefs.GetFloat("Position_Y"));
        }
    }
}
