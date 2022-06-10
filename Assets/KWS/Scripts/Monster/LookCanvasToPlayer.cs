using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCanvasToPlayer : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(transform.position * 2 - Camera.main.transform.position);
    }
}
