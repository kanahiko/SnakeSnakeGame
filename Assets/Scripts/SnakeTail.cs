using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeTail : MonoBehaviour {
    
    private Vector3 velocity = new Vector3(0, 0, 0);

    public void MovePart(Vector3 target,float smoothTime)
    {
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }
}
