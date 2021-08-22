using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestruction : MonoBehaviour {

    public float countdown;

    void Start() {

    }

    void Update() {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
            Destroy(gameObject);
    }
}
