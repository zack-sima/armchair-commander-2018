using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {
    //movement cost is cost of unit passing through and name is; water also different from land (non-water)
    public string terrainName;
    public bool isWater, isCity, selected;

    [HideInInspector]
    public City city;

    //country colors & water are differently tinted
    public Material defaultMaterial;

    public float movementCost; 
    public GameObject curTarget;

    //occupier is soldier on current tile
    public Soldier occupier;
    public bool isOccupied, canWalkOn, canAttack;
    public string owningCountry;

    //[HideInInspector]
    public GameObject pointTo;

    public Vector3[] soldierPath;
    public GameObject[] checkedGameObjects;
    Controller controllerScript;

    void Start() {
        controllerScript = GameObject.Find("Controller").GetComponent<Controller>();
        if (!controllerScript.savedData) {
            Vector3 rangeDetection = new Vector3(0.1f, 2, 0.1f);
            if (isWater)
                rangeDetection = new Vector3(0.7f, 2, 0.7f);
            foreach (Collider i in Physics.OverlapBox(transform.position, rangeDetection)) {
                if (i.transform.parent != null) {
                    if (i.transform.parent.name == "CountrySpawn") {
                        owningCountry = i.name;
                        defaultMaterial = controllerScript.findCountryColor(i.name);
                        break;
                    }
                }
            }
            GetComponent<Renderer>().material = defaultMaterial;
            setDefaultMaterial();
        }
        checkedGameObjects = new GameObject[100];
    }
    void Update() {
        if (isOccupied && occupier != null) {
            pointTo = null;
            selected = occupier.selected;
            if (occupier.isCity) {
                isOccupied = false;
                occupier = null;
            } else if (occupier.country != "" && owningCountry != occupier.country) {
                owningCountry = occupier.country; 
            }
        }
        //if (selected) {
        //    //construction code here
        //    if (!isOccupied) {
        //        //more
        //    }
        //}
    }
    void pushItemToArray<T>(ref T[] array, T item) {
        for (int i = 0; i < array.Length; i++) {
            if (array[i] == null) {
                array[i] = item;
                break;
            }
        }
    }
    public void setDefaultMaterial() {
        GetComponent<Renderer>().material = defaultMaterial;
        ///*testing only
        if (isWater) {
            GetComponent<Renderer>().enabled = false;
        }           
        //*/           
    }
    public bool findPathToSoldier(Tile target, float mobility, string country) {
        Vector3[] directions;
        return findPathToSoldier(target, out directions, mobility, country);
    }
    public bool findPathToSoldier(Tile target, out Vector3[] directions, float mobility, string country) {

        checkedGameObjects = new GameObject[200];
        checkedGameObjects[0] = gameObject;
        soldierPath = new Vector3[20];

        //direct
        if (Vector3.Distance(transform.position, target.transform.position) < 1.3f && (isWater || target.isOccupied && !target.occupier.isNaval)) {
            Vector3[] dir = new Vector3[2];
            dir[0] = transform.position;
            directions = dir;
            return true;
        } 
        if (isWater != target.isWater) {
            directions = new Vector3[0];
            return false;
        }
        //prevent overload crash
        int count = 0;
        while (count < mobility + 2) {
            //if still true then cut pathfinding
            bool noNewTile = true;
            count++;
            Vector3 minTilePosition = Vector3.zero;
            float shortestDistance = 100f;
            GameObject currentShortestObject = null;
            GameObject currentShortestObjectPointer = null;
            for (int k = 0; k < checkedGameObjects.Length; k++) {
                if (checkedGameObjects[k] != null) {
                    Vector3 checkBoxPosition = checkedGameObjects[k].transform.position;
                    foreach (Collider i in Physics.OverlapBox(checkBoxPosition, new Vector3(0.7f, 1f, 0.7f))) {
                        if (i != null && i.GetComponent<Tile>() != null && i.gameObject != gameObject && ((!i.GetComponent<Tile>().isOccupied && (!i.GetComponent<Tile>().isCity || i.GetComponent<Tile>().city.citySoldier.health <= 0)) || i.GetComponent<Tile>().occupier != null && controllerScript.findCountryAlliance(i.GetComponent<Tile>().occupier.country, country))) {
                            if (i.GetComponent<Tile>().isOccupied && i.GetComponent<Tile>().occupier.isNaval && !i.GetComponent<Tile>().isWater)
                                continue; 
                            if (i.GetComponent<Tile>().pointTo != null || i.GetComponent<Tile>().isWater != isWater)
                                continue;
                            if (Vector3.Distance(checkBoxPosition, transform.position) + Vector3.Distance(checkBoxPosition, target.transform.position) <= shortestDistance) {
                                //fix movement bug later
                                float mobilityCost = movementCost / 2f;
                                bool ok = true; 
                                GameObject newPointer = checkedGameObjects[k].gameObject;
                                for (int index = 0; mobilityCost <= mobility && index < 20 && newPointer != gameObject; index++) {
                                    mobilityCost += newPointer.GetComponent<Tile>().movementCost / 2f;
                                    newPointer = newPointer.GetComponent<Tile>().pointTo;
                                    //movement cost exceeds soldier's
                                    if (mobilityCost > mobility) {
                                        ok = false;
                                        break; 
                                    }
                                }
                                if (!ok)
                                    continue;
                                shortestDistance = Vector3.Distance(checkBoxPosition, transform.position) + Vector3.Distance(checkBoxPosition, target.transform.position);
                                currentShortestObject = i.gameObject;
                                currentShortestObjectPointer = checkedGameObjects[k].gameObject;
                                noNewTile = false;
                            }
                        }
                    }
                }
            }
            if (currentShortestObject != null) {
                if (currentShortestObject == target.gameObject) {
                    //found destination
                    soldierPath[0] = currentShortestObjectPointer.transform.position;
                    GameObject newPointer = currentShortestObjectPointer;

                    //prevent crash and index
                    int index = 1;
                    float mobilityCost = movementCost / 2f; 
                    for (; newPointer != gameObject && index <= 20; index++) {
                        if (index >= soldierPath.Length) {
                            directions = new Vector3[0];
                            resetPointers();
                            return false;
                        }
                        mobilityCost += newPointer.GetComponent<Tile>().movementCost / 2f; 
                        soldierPath[index] = newPointer.GetComponent<Tile>().pointTo.transform.position;
                        newPointer = newPointer.GetComponent<Tile>().pointTo;
                    }
                    directions = soldierPath;
                    resetPointers();
                    if (mobility >= mobilityCost && (target.isWater == isWater || index == 1)) {
                        return true;
                    } else {
                        return false; 
                    }
                } else {
                    currentShortestObject.GetComponent<Tile>().pointTo = currentShortestObjectPointer;
                    pushItemToArray(ref checkedGameObjects, currentShortestObject);
                }
            } else {
                break;
            }
            if (noNewTile) {
                break;
            }
        }
        Vector3[] dir1;
        resetPointers();
        if (target.owningCountry == controllerScript.playerCountryName && findCheapPathToSoldier(target, out dir1, mobility, country)) {
            directions = dir1;
            return true;
        }
        directions = null;
        return false;
    }
    void resetPointers() {
        foreach (GameObject i in checkedGameObjects) {
            if (i != null)
                i.GetComponent<Tile>().pointTo = null;
        }
    }
    bool findCheapPathToSoldier (Tile target, out Vector3[] directions, float mobility, string country) {
        //this function assumes that previous function failed, so pathfinding will be conducted
        //where only pahts with no terrain will be searched for. This means that mobilityCost 
        //will be 1 at all times to reduce lag from accessing variables. 

        checkedGameObjects = new GameObject[200];
        checkedGameObjects[0] = gameObject;
        soldierPath = new Vector3[20];

        //direct
        if (Vector3.Distance(transform.position, target.transform.position) < 1.3f) {
            Vector3[] dir = new Vector3[2];
            dir[0] = transform.position;
            directions = dir;
            return true;
        } else if (isWater != target.isWater) {
            directions = new Vector3[0];
            return false;
        }
        //prevent overload crash
        int count = 0;
        while (count < 25) {
            //if still true then cut pathfinding
            bool noNewTile = true;
            count++;
            Vector3 minTilePosition = Vector3.zero;
            float shortestDistance = 100f;
            GameObject currentShortestObject = null;
            GameObject currentShortestObjectPointer = null;
            for (int k = 0; k < checkedGameObjects.Length; k++) {
                if (checkedGameObjects[k] != null) {
                    Vector3 checkBoxPosition = checkedGameObjects[k].transform.position;
                    foreach (Collider i in Physics.OverlapBox(checkBoxPosition, new Vector3(0.7f, 1f, 0.7f))) {
                        if (i != null && i.GetComponent<Tile>() != null && i.gameObject != gameObject && (i.GetComponent<Tile>().movementCost < 2.1f || i.gameObject.GetComponent<Tile>() == target) && ((!i.GetComponent<Tile>().isOccupied && (!i.GetComponent<Tile>().isCity || i.GetComponent<Tile>().city.citySoldier.health <= 0)) || i.GetComponent<Tile>().occupier != null && controllerScript.findCountryAlliance(i.GetComponent<Tile>().occupier.country, country))) {
                            if (i.GetComponent<Tile>().pointTo != null || i.GetComponent<Tile>().isWater != isWater)
                                continue;
                            if (Vector3.Distance(checkBoxPosition, transform.position) + Vector3.Distance(checkBoxPosition, target.transform.position) <= shortestDistance) {
                                //fix movement bug later
                                float mobilityCost = movementCost / 2f;
                                bool ok = true;
                                GameObject newPointer = checkedGameObjects[k].gameObject;
                                for (int index = 0; mobilityCost <= mobility && index < 20 && newPointer != gameObject; index++) {
                                    mobilityCost++;
                                    newPointer = newPointer.GetComponent<Tile>().pointTo;
                                    //movement cost exceeds soldier's
                                    if (mobilityCost > mobility) {
                                        ok = false;
                                        break;
                                    }
                                }
                                if (!ok)
                                    continue;
                                shortestDistance = Vector3.Distance(checkBoxPosition, transform.position) + Vector3.Distance(checkBoxPosition, target.transform.position);
                                currentShortestObject = i.gameObject;
                                currentShortestObjectPointer = checkedGameObjects[k].gameObject;
                                noNewTile = false;
                            }
                        }
                    }
                }
            }
            if (currentShortestObject != null) {
                if (currentShortestObject == target.gameObject) {
                    //found destination
                    soldierPath[0] = currentShortestObjectPointer.transform.position;
                    GameObject newPointer = currentShortestObjectPointer;

                    //prevent crash and index
                    int index = 1;
                    float mobilityCost = 1;
                    for (; newPointer != gameObject && index <= 20; index++) {
                        if (index >= soldierPath.Length) {
                            directions = new Vector3[0];
                            resetPointers();
                            return false;
                        }
                        mobilityCost++;
                        soldierPath[index] = newPointer.GetComponent<Tile>().pointTo.transform.position;
                        newPointer = newPointer.GetComponent<Tile>().pointTo;
                    }
                    directions = soldierPath;
                    resetPointers();
                    if (mobility >= mobilityCost && (target.isWater == isWater || index == 1)) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    currentShortestObject.GetComponent<Tile>().pointTo = currentShortestObjectPointer;
                    pushItemToArray(ref checkedGameObjects, currentShortestObject);
                }
            } else {
                break;
            }
            if (noNewTile) {
                break;
            }
        }
        resetPointers();
        directions = null;
        return false;
    }
}