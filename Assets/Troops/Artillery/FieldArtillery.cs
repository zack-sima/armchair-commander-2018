using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldArtillery : Soldier {
    //all enemy units use grey
    [SerializeField]
    Renderer[] partsThatChangeToGrey;
    [SerializeField]
    Renderer[] partsThatChangeToDarkGrey;

    public GameObject bigExplosionPrefab;

    void Start() {
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
    protected override void animateAttack(Soldier target) {
        if (controllerScript.roundPassDelay <= 0) {
            StartCoroutine(artilleryAnimation(target));
        }
    }
    protected IEnumerator artilleryAnimation(Soldier target) {
        transform.position = new Vector3(currentTile.transform.position.x, transform.position.y, currentTile.transform.position.z);
        Vector3 originalPos = transform.position; 
        transform.Translate(Vector3.back / 6f);
        float i = 0;
        while (i < 0.8f) {
            i += Time.deltaTime;
            transform.Translate(Vector3.forward * Time.deltaTime / 4.73f);
            yield return null;
        }
        transform.position = originalPos; 
        if (target != null)
        	Instantiate(bigExplosionPrefab, target.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
    }
    void Update() {
        if (framesCounter <= 0) {
            SoldierActionsIterative();
            framesCounter = framesPerUpdate;
        } else {
            framesCounter--;
        }
    }
}
