using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour {
    public string terrainName;
    public float movementCost;
    public Renderer[] changeAlphas; 
    Tile tile; 
    private void Start() {
        foreach (Collider i in Physics.OverlapBox(transform.position, new Vector3(0.1f, 6, 0.1f))) {
            if (i.GetComponent<Tile>() != null) {
                i.GetComponent<Tile>().terrainName = terrainName;
                i.GetComponent<Tile>().movementCost = movementCost * 2;
                tile = i.GetComponent<Tile>(); 
                break;
            }
        }
    }
    bool prevOccupied; 
    private void Update() {
        if (tile.isOccupied != prevOccupied) {
            prevOccupied = tile.isOccupied;
            if (prevOccupied) {
                foreach (Renderer i in changeAlphas) {
                    i.material.color = new Color(i.material.color.r, i.material.color.g, i.material.color.b, 0.3f);
                }
            } else {
                foreach (Renderer i in changeAlphas) {
                    i.material.color = new Color(i.material.color.r, i.material.color.g, i.material.color.b, 0.82f);
                }
            }
        }
    }
}
