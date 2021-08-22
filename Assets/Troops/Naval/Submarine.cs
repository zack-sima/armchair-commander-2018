using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : Soldier {
    public GameObject bigExplosionPrefab; 

    void Start() {
        SoldierInit();
        updateUniformColor(); 
    }
    protected override void updateUniformColor() {
    }
    protected override void animateAttack (Soldier target) {
        if (controllerScript.roundPassDelay <= 0) {
            StartCoroutine(subAnimation(target));
        }
    }
    protected IEnumerator subAnimation (Soldier target) {
        float i = 0; 
        while (i < 0.82f) {
            i += Time.deltaTime;
            yield return null; 
        }
        if (target != null)
            Instantiate(bigExplosionPrefab, target.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
    }
    void Update() {
        if (mobility <= 0)
            moved = true; 
        if (framesCounter <= 0) {
            SoldierActionsIterative();
            framesCounter = framesPerUpdate;
        } else {
            framesCounter--;
        }
    }
}
