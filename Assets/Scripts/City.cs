using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {
    //assigned when spawned by controller
    [HideInInspector]
    public Tile tile;
    public bool isPort; 
    public Text cityText, cityLevelText, cityIndustrialText;
    public int cityLevel, cityIndustrial; 
    public Image flag, industrialIcon;
    public string country;
    public Renderer[] walls;
    bool prevOccupied;
    public Material opaqueMat, translucentMat;
    public RectTransform healthBar;
    [HideInInspector]
    public int maxHealth;
    public Soldier citySoldier; 

    Controller controllerScript;

    public void changeCountry(string country) {
        flag.sprite = controllerScript.findCountryFlag(country);
        this.country = country;
    }
    void Start() {
        controllerScript = GameObject.Find("Controller").GetComponent<Controller>();
        maxHealth = cityLevel * 80; 

        //prevent bug lagging game out
        if (maxHealth == 0)
            maxHealth = 1;
        if (!controllerScript.savedData) {
            citySoldier.health = maxHealth;
        }
        citySoldier.maxHealth = maxHealth;
        citySoldier.isCity = true;
        foreach (Collider i in Physics.OverlapBox(transform.position, new Vector3(0.1f, 6, 0.1f))) {
            if (i.GetComponent<Tile>() != null) {
                tile = i.GetComponent<Tile>();
                break;
            }
        }
        tile.isCity = true;
        tile.city = this; 
        changeCountry(tile.owningCountry);
        if (isPort)
            setPortMaterials(true);
        else
            setWallMaterial(opaqueMat);
        Update(); 
    }
    void setPortMaterials (bool opaque) {
        foreach (Renderer i in walls) {
            if (opaque)
                i.enabled = true;
            else
                i.enabled = false; 
        }
    }
    void setWallMaterial (Material material) {
        walls[0].GetComponent<Renderer>().material = material; 
    }

    void Update() {
        ///*Testing
        if (Input.GetKeyDown(KeyCode.O))
            Destroy(citySoldier.gameObject);
        //Testing*/
        if (!controllerScript.findCountryAlliance(country, controllerScript.playerCountryName))
            healthBar.GetComponent<Image>().color = Color.red;
        else if (controllerScript.findNeutrality(country))
            healthBar.GetComponent<Image>().color = new Color(0.78f, 0.76f, 0.78f);
        else
            healthBar.GetComponent<Image>().color = new Color(0, 0.85f, 0.05f);
        healthBar.localScale = new Vector3((float)citySoldier.health / (float)citySoldier.maxHealth, healthBar.localScale.y, healthBar.localScale.z);
        if (tile.owningCountry != country) {
            changeCountry(tile.owningCountry);
        }
        if (tile.isOccupied && !prevOccupied) {
            flag.sprite = tile.occupier.countryFlagImage.sprite;
            country = tile.occupier.country;
            if (isPort)
                setPortMaterials(false);
            else
                setWallMaterial(translucentMat);
        } else if (!tile.isOccupied && prevOccupied) {
            if (isPort)
                setPortMaterials(true);
            else
                setWallMaterial(opaqueMat);
        }
        prevOccupied = tile.isOccupied;
    }
}
