using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    Transform cam;
    public bool prevHeight = false;
    Vector3 originalScale; 
    void Start() {
        originalScale = transform.localScale; 
        cam = GameObject.Find("Main Camera").transform;
    }
    void Update() {
        cam = GameObject.Find("Main Camera").transform;
        transform.rotation = Quaternion.Euler(cam.eulerAngles.x, cam.eulerAngles.y, cam.eulerAngles.z);
        if (prevHeight != cam.GetComponent<CameraMovement>().aboveHeight) {
            if (cam.GetComponent<CameraMovement>().aboveHeight) {
                transform.localScale = new Vector3(2, 1.76f, 2);
            } else {
                transform.localScale = originalScale; 
            }
        }
        prevHeight = cam.GetComponent<CameraMovement>().aboveHeight;
    }
}
