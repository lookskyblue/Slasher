using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private string scene_name;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            GameManager.instance.LoadScene(scene_name);
        }
    }
}
