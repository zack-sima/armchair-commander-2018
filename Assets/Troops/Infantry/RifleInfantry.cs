using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleInfantry : Soldier {
    [SerializeField]
    Renderer[] partsThatChangeToGrey;

    private void Start() {
        SoldierInit();
        updateUniformColor(); 
    }
    protected override void updateUniformColor() {
        if (country == "Germany" || country == "Bulgaria" || country == "Hungary" || country == "Italy" || country == "Romania") {
            foreach (Renderer i in partsThatChangeToGrey) {
                i.material = controllerScript.greyMaterial;
            }
        }
    }
    private void Update() {
        if (framesCounter <= 0) {
            SoldierActionsIterative();
            framesCounter = framesPerUpdate; 
        } else {
            framesCounter--; 
        }
    }
}
