using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Soldier {
    //all enemy units use grey
    [SerializeField]
    Renderer[] partsThatChangeToGrey;
    [SerializeField]
    Renderer[] partsThatChangeToDarkGrey;
    Transform turret;

    public GameObject bigExplosionPrefab; 

	void Start() {
        turret = transform.GetChild(0).GetChild(0); 
        SoldierInit();
        updateUniformColor(); 
	}
    protected override void updateUniformColor() {
        if (country == "Germany" || country == "Bulgaria" || country == "Hungary" || country == "Italy" || country == "Romania") {
            foreach (Renderer i in partsThatChangeToGrey) {
                i.material = controllerScript.greyMaterial;
            }
            foreach (Renderer i in partsThatChangeToDarkGrey) {
                i.material = controllerScript.darkGreyMaterial;
            }
        }
    }
    protected override void animateAttack (Soldier target) {
        if (controllerScript.roundPassDelay <= 0) {
            StartCoroutine(tankAnimation(target));
        }
    }
    protected IEnumerator tankAnimation (Soldier target) {
        Vector3 originalPos = turret.position;
        turret.Translate(Vector3.forward / 6f);
        float i = 0; 
        while (i < 0.9f) {
            i += Time.deltaTime;
            turret.Translate(Vector3.back * Time.deltaTime / 5.3f);
            yield return null; 
        }
        if (target != null)
            Instantiate(bigExplosionPrefab, target.transform.position, Quaternion.identity);
        turret.position = originalPos; 
        yield return new WaitForSeconds(0.1f);
    }
	void Update() {
        if (mobility == 0)
            moved = true; 
        if (framesCounter <= 0) {
            SoldierActionsIterative();
            framesCounter = framesPerUpdate;
        } else {
            framesCounter--;
        }
    }
}
