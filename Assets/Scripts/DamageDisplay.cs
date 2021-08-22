using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DamageDisplay : MonoBehaviour {
    public float destroyTimer;
    float startTime;

    void Start() {
        startTime = destroyTimer;
        transform.Translate(0, 0, -0.3f);
    }
    void Update() {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
            Destroy(gameObject);
        transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * 0.7f, transform.position.z); 
        transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, destroyTimer / startTime + 0.3f);
    }
}
