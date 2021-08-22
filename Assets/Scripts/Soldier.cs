using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Soldier : MonoBehaviour {
    Material yellowMaterial, blueMaterial, redMaterial, greyMaterial;
    public GameObject damageDisplayPrefab; 

    //renderer turned on when in water
    public Renderer landingCraft; 

    //frames between every time iterative function is used
    public float framesPerUpdate;
    [HideInInspector]
    public float framesCounter;
    public bool isCity;

    [HideInInspector]
    public Controller controllerScript; 
    public RectTransform healthBar; 

    //attackDelay in seconds to show animation before health is lost
    public float health, maxHealth, attackDelay;
    [HideInInspector]
    public float experience;

    //calculate from experience
    [HideInInspector]
    public int veterency;

    public string troopType, troopName;
    public bool isNaval; 

    //percentage attack/defense bonus against specific troops based on attack
    [Range(0, 100)]
    public float infantryBonus, infantryDefenseBonus, armoredBonus, armoredDefenseBonus, artilleryBonus, artilleryDefenseBonus, fortressBonus, fortressDefenseBonus, navalBonus, navalDefenseBonus, mountainBonus, mountainDefenseBonus, forestBonus, forestDefenseBonus; 

    public Image countryFlagImage, veterencyImage;

    [Range(0, 20)] 
    public float mobility;
    [Range(1, 100)]
    public int damage;
    [Range(0, 10)]
    public int range;
    [Range(0, 100)]
    public int avoidRetaliationChance; 
    public string country; 
    public Tile currentTile;
    public bool selected, moved, attacked, attackable;

    //[HideInInspector]
    public int indexInArray; 

    //only use findPathToSoldier when previously not selected
    [HideInInspector]
    public bool prevSelected = false; 

    public void updateVeterency () {
        if (!isCity) {
            //5 levels in total (0-5)
            for (int i = 1; i < 6; i++) {
                if (120 * ((i * i + i) / 2) > experience) {
                    veterency = i - 1;
                    break;
                } else {
                    veterency = i;
                }
            }
            if (veterency <= 0) {
                veterencyImage.enabled = false;
            } else {
                veterencyImage.enabled = true;
                veterencyImage.sprite = controllerScript.veterencySprites[veterency - 1];
            }
        }
    }
    protected void SoldierInit() {
        controllerScript = GameObject.Find("Controller").GetComponent<Controller>();
        yellowMaterial = controllerScript.yellowMaterial;
        blueMaterial = controllerScript.blueMaterial;
        redMaterial = controllerScript.redMaterial;
        greyMaterial = controllerScript.greyMaterial;
        transform.Rotate(0, 90, 0);
        foreach (Collider i in Physics.OverlapBox(transform.position, new Vector3(0.1f, 2f, 0.1f))) {
            if (i.GetComponent<Tile>() != null) {
                i.GetComponent<Tile>().isOccupied = true;
                i.GetComponent<Tile>().occupier = this;
                currentTile = i.GetComponent<Tile>();
                if ((!controllerScript.savedData || isCity) && currentTile.owningCountry != "" && !currentTile.isWater) {
                    country = currentTile.owningCountry;
                }
                if (currentTile.owningCountry == "")
                    currentTile.owningCountry = country; 
                break;
            }
        }
        countryFlagImage.sprite = controllerScript.findCountryFlag(country);
        if (!isCity) { 
            if (country != controllerScript.playerCountryName) {
                if (country != "") {
                    if (!controllerScript.findCountryAlliance(country, controllerScript.playerCountryName))
                        healthBar.GetComponent<Image>().color = Color.red;
                    if (controllerScript.findNeutrality(country))
                        healthBar.GetComponent<Image>().color = new Color(0.78f, 0.76f, 0.78f);
                    for (int i = 0; i < controllerScript.aiSoldiers.Length; i++) {
                        if (controllerScript.aiSoldiers[i] == null) {
                            controllerScript.aiSoldiers[i] = this;
                            indexInArray = i;
                            break;
                        }
                    }
                }
            } else {
                for (int i = 0; i < controllerScript.playerSoldiers.Length; i++) {
                    if (controllerScript.playerSoldiers[i] == null) {
                        controllerScript.playerSoldiers[i] = this;
                        indexInArray = i;
                        break;
                    }
                }
            }
        }
        if (!isNaval && !isCity) {
            if (currentTile.isWater) {
                landingCraft.enabled = true;
            } else {
                landingCraft.enabled = false;
            }
        }
        updateVeterency(); 
    }
    //use via Update() in child classes
    protected void SoldierActionsIterative() {
        if (country == "") {
            country = currentTile.owningCountry;
            countryFlagImage.sprite = controllerScript.findCountryFlag(country);
            if (!controllerScript.findCountryAlliance(country, controllerScript.playerCountryName))
                healthBar.GetComponent<Image>().color = Color.red;
            if (controllerScript.findNeutrality(country))
                healthBar.GetComponent<Image>().color = new Color(0.78f, 0.76f, 0.78f);
            updateUniformColor();
            if (!isCity) {
                if (country == controllerScript.playerCountryName) {
                    for (int i = 0; i < controllerScript.playerSoldiers.Length; i++) {
                        if (controllerScript.playerSoldiers[i] == null) {
                            controllerScript.playerSoldiers[i] = this;
                            indexInArray = i;
                            break;
                        }
                    }
                } else {
                    for (int i = 0; i < controllerScript.aiSoldiers.Length; i++) {
                        if (controllerScript.aiSoldiers[i] == null) {
                            controllerScript.aiSoldiers[i] = this;
                            indexInArray = i;
                            break;
                        }
                    }
                }
            }
        }
        if (isCity) {
            country = currentTile.owningCountry;
            if (country.Equals(controllerScript.playerCountryName) || (currentTile.isCity && (currentTile.isOccupied || currentTile.city.citySoldier.health <= 0))) {
                GetComponent<BoxCollider>().enabled = false;
            } else {
                GetComponent<BoxCollider>().enabled = true;
            }
            return;
        } 
        if (currentTile.isCity) {
            countryFlagImage.enabled = false;
        } else {
            countryFlagImage.enabled = true;
        }
        healthBar.localScale = new Vector3((float)health / (float)maxHealth, healthBar.localScale.y, healthBar.localScale.z);
        if (country == controllerScript.playerCountryName) {
            attackable = false;
            foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0f, transform.position.z), range)) {
                if (i.GetComponent<Tile>() != null && (troopName != "submarine" || i.GetComponent<Tile>().isWater) && (i.GetComponent<Tile>().isOccupied && !controllerScript.findCountryAlliance(i.GetComponent<Tile>().owningCountry, country) || (i.GetComponent<Tile>().city != null && i.GetComponent<Tile>().city.citySoldier.health > 0 && !controllerScript.findCountryAlliance(i.GetComponent<Tile>().owningCountry, country)))) {
                    attackable = true;
                }
            }
            if (selected) {
                currentTile.GetComponent<Renderer>().material = blueMaterial;
                if (!moved && selected != prevSelected) {
                    //light up boxes for player
                    //only used for statement to prevent application from completely dying if stuck
                    //searchedPositions actually gets cleared 
                    //Vector3[] searchedPositions = new Vector3[1000];
                    //Vector3[] prevSearchedPositions = new Vector3[1000];
                    //prevSearchedPositions[0] = transform.position; 
                    //for (int c = 0; c < 1000; c++, prevSearchedPositions = searchedPositions, searchedPositions = new Vector3[1000]) {
                    //    for (int k = 0; k < prevSearchedPositions.Length; k++) {
                    //        if (prevSearchedPositions[k] == Vector3.zero)
                    //            break;
                    //        foreach (Collider i in Physics.OverlapSphere(prevSearchedPositions[k], 0.6f)) {
                    //            if (i.GetComponent<Tile>() != null && i.transform.position != prevSearchedPositions[k] && !i.GetComponent<Tile>().isOccupied && (!i.GetComponent<Tile>().isCity || controllerScript.findCountryAlliance(country, i.GetComponent<Tile>().owningCountry) || i.GetComponent<Tile>().city.citySoldier.health <= 0)) {
                    //                if (!i.GetComponent<Tile>().canWalkOn && i.GetComponent<Tile>().findPathToSoldier(currentTile, mobility, country)) {
                    //                    i.GetComponent<Renderer>().enabled = true;
                    //                    i.GetComponent<Renderer>().material = yellowMaterial;
                    //                    i.GetComponent<Tile>().canWalkOn = true;
                    //                    //add to array
                    //                    for (int j = 0; j < searchedPositions.Length; j++) { 
                    //                        if (searchedPositions[j] == Vector3.zero) {
                    //                            searchedPositions[j] = i.transform.position;
                    //                            break; 
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //    if (searchedPositions[0] == Vector3.zero)
                    //        break; 
                    //}
                    StartCoroutine(pathFindTiles());
                }
                if (!attacked) {
                    foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0f, transform.position.z), range)) {
                        if (i.GetComponent<Tile>() != null && (troopName != "submarine" || i.GetComponent<Tile>().isWater) && (i.GetComponent<Tile>().isOccupied && !controllerScript.findCountryAlliance(i.GetComponent<Tile>().occupier.country, country) || (i.GetComponent<Tile>().isCity && i.GetComponent<Tile>().city.citySoldier.health > 0 && !controllerScript.findCountryAlliance(i.GetComponent<Tile>().owningCountry, country)))) {
                            i.GetComponent<Renderer>().enabled = true;
                            i.GetComponent<Renderer>().material = redMaterial;
                            i.GetComponent<Tile>().canAttack = true;
                        }
                    }
                }
            } else if ((!moved || !attacked && attackable) && currentTile.GetComponent<Renderer>().material != greyMaterial && controllerScript.roundPassDelay <= 0f) {
                currentTile.GetComponent<Renderer>().enabled = true;
                currentTile.GetComponent<Renderer>().material = greyMaterial;
                currentTile.canWalkOn = false;
            } else if (currentTile.GetComponent<Renderer>().enabled) {
                currentTile.setDefaultMaterial(); 
                currentTile.canWalkOn = false;
            }
        }
        prevSelected = selected; 
    }
    IEnumerator pathFindTiles () {
        //will run for x frames
        for (float k = 1; k <= mobility; k++) {
            foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0f, transform.position.z), k)) {
                if (i.GetComponent<Tile>() != null && !i.GetComponent<Tile>().canWalkOn && !i.GetComponent<Tile>().isOccupied && (!i.GetComponent<Tile>().isCity || controllerScript.findCountryAlliance(country, i.GetComponent<Tile>().owningCountry) || i.GetComponent<Tile>().city.citySoldier.health <= 0)) {
                    if (i.GetComponent<Tile>().findPathToSoldier(currentTile, mobility, country)) {
                        i.GetComponent<Renderer>().enabled = true;
                        i.GetComponent<Renderer>().material = yellowMaterial;
                        i.GetComponent<Tile>().canWalkOn = true;
                    }
                }
            }
            controllerScript.stopPlayerActions = true; 
            yield return null; 
            
        }
        controllerScript.stopPlayerActions = false; 
        yield return new WaitForSeconds(0); 
    }
    public void returnTiles () {
        float n = mobility;
        if (range > mobility)
            n = range; 
        foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z), n)) {
            if (i.GetComponent<Tile>() != null) {
                i.GetComponent<Tile>().setDefaultMaterial();
                i.GetComponent<Tile>().canWalkOn = false;
                i.GetComponent<Tile>().canAttack = false;
            }    
        } 
    }
    protected void returnRedTiles () {
        foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0f, transform.position.z), range + mobility)) {
            if (i.GetComponent<Tile>() != null && i.GetComponent<Tile>().canAttack) {
                i.GetComponent<Tile>().setDefaultMaterial();
                i.GetComponent<Tile>().canAttack = false;
            }
        }
    }
    public void moveSoldier (Tile target, bool aiAttack) {
        if (target.isCity && target.city.citySoldier.health > 0 && target.owningCountry != country)
            return;
        Vector3[] directions;
        if (target.findPathToSoldier(currentTile, out directions, mobility, country)) {
            if (target.isCity && target.owningCountry != country) {
                //search country to see if it is defeatedbool defeated = true; 
                bool defeated = true; 
                foreach (City i in controllerScript.cities) {
                    if (i != null && i.country == target.owningCountry && i != target.city) {
                        defeated = false;
                        break; 
                    }
                }
                //declear country defeated
                if (defeated) {
                    if (target.owningCountry == controllerScript.playerCountryName) {
                        GameObject insItem = Instantiate(controllerScript.defeatUIPrefab, controllerScript.canvas.transform);
                        controllerScript.cam.GetComponent<CameraMovement>().enabled = false;
                    } else {
                        //check for overall victory
                        bool victory = true;
                        foreach (City i in controllerScript.cities) {
                            if (i != target.city && !controllerScript.findCountryAlliance(i.country, country)) {
                                victory = false;
                                break;
                            }
                        }
                        if (victory) {
                            GameObject insItem = Instantiate(controllerScript.victoryUIPrefab, controllerScript.canvas.transform);
                            controllerScript.cam.GetComponent<CameraMovement>().enabled = false;
                        }
                        if (country == controllerScript.playerCountryName && !victory) {
                            if (controllerScript.selectedSoldier != null) {
                                controllerScript.selectedSoldier.selected = false;
                                controllerScript.selectedSoldier.returnTiles();
                            }
                            GameObject insItem = Instantiate(controllerScript.capitulationUIPrefab, controllerScript.canvas.transform);
                            if (PlayerPrefs.GetString("language") == "Chinese")
                                insItem.transform.GetChild(0).GetComponent<Text>().text = "你打败了" + controllerScript.translateCountryName(target.owningCountry, "Chinese");
                            else
                                insItem.transform.GetChild(0).GetComponent<Text>().text = "You've capitulated " + target.owningCountry;
                        }
                        //despawn soldiers
                        foreach (Soldier i in controllerScript.aiSoldiers) {
                            if (i != null && i.country == target.owningCountry) {
                                Destroy(i.gameObject);
                            }
                        }
                        //change tiles
                        foreach (Tile i in controllerScript.tiles) {
                            if (i != null && i.owningCountry == target.owningCountry && i != target) {
                                i.defaultMaterial = controllerScript.findCountryColor(country);
                                i.owningCountry = country;
                                i.setDefaultMaterial();
                            }
                        }
                    }
                }
            }
            StartCoroutine(moveSchedule(directions, aiAttack));
            moved = true;
            target.occupier = this;
            target.isOccupied = true;
            currentTile.isOccupied = false;
            returnTiles();
            currentTile = target;
        }
    }
    protected IEnumerator moveSchedule (Vector3[] points, bool aiAttack) {
        if (country.Equals(controllerScript.playerCountryName))
            controllerScript.stopPlayerActions = true; 
        float originalY = transform.position.y;
        Vector3 originalPos = transform.position;
        float interpolateIndex = 0;
        int index = 0;
        while (index < points.Length && points[index] != Vector3.zero) {
            if (controllerScript.findCountryAlliance(country, controllerScript.playerCountryName)) {
                returnRedTiles();
            }
            transform.LookAt(new Vector3(points[index].x, 1, points[index].z));
            if (interpolateIndex < 1) {
                transform.position = Vector3.Lerp(originalPos, points[index], interpolateIndex);
                transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
                if (controllerScript.roundPassDelay > 0) {
                    interpolateIndex += 0.8f;
                } else
                    interpolateIndex += Time.deltaTime * 6.2f;
                if (interpolateIndex > 1)
                    interpolateIndex = 1;
            } else {
                originalPos = transform.position; 
                interpolateIndex = 0;
                index++; 
            }
            foreach (Collider i in Physics.OverlapBox(transform.position, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity)) { 
                if (i.GetComponent<Tile>() != null && !i.GetComponent<Tile>().isOccupied && (!i.GetComponent<Tile>().isCity/* || i.GetComponent<Tile>().city.citySoldier.health <= 0*/)) {
                    i.GetComponent<Tile>().defaultMaterial = controllerScript.findCountryColor(country);
                    i.GetComponent<Tile>().owningCountry = country; 
                    i.GetComponent<Tile>().setDefaultMaterial(); 
                }
            }
            yield return null;
        }
        transform.position = new Vector3(points[index - 1].x, originalY, points[index - 1].z);
        if (!isNaval && !isCity) {
            if (currentTile.isWater) {
                landingCraft.enabled = true;
            } else {
                landingCraft.enabled = false;
            }
        }
        returnRedTiles(); 
        if (aiAttack) {
            aiFindTarget();
        }
        currentTile.defaultMaterial = controllerScript.findCountryColor(country);
        currentTile.owningCountry = country;
        currentTile.setDefaultMaterial();
        if (country.Equals(controllerScript.playerCountryName))
            controllerScript.stopPlayerActions = false;
        yield return new WaitForSeconds(0.1f);
    }
    //auto attacker
    protected bool aiFindTarget () {
        foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z), range)) {
            if (i.GetComponent<Tile>() != null && (troopName != "submarine" || i.GetComponent<Tile>().isWater) && (i.GetComponent<Tile>().isOccupied || i.GetComponent<Tile>().isCity && i.GetComponent<Tile>().city.citySoldier.health > 0) && !controllerScript.findCountryAlliance(i.GetComponent<Tile>().owningCountry, country)) {
                if (Vector3.Distance(i.transform.position, currentTile.transform.position) <= range + 0.5f) {
                    if (i.GetComponent<Tile>().isCity && !i.GetComponent<Tile>().isOccupied) {
                        attack(i.GetComponent<Tile>().city.citySoldier);
                    } else 
                        attack(i.GetComponent<Tile>().occupier);
                    return true;
                }
            }
        }
        return false; 
    }
    //put in child classes
    protected void animateMove() { }


    public void attack(Soldier target) {
        attacked = true; 
        if (controllerScript.roundPassDelay <= 0f)
            controllerScript.audioManager.Play(troopName, 1);
        transform.LookAt(new Vector3(target.transform.position.x, 1, target.transform.position.z));
        animateAttack(target); 

        int noRetaliation = Random.Range(0, 100);
        if (noRetaliation >= avoidRetaliationChance && !target.isCity && (target.troopName != "submarine" || currentTile.isWater))
            StartCoroutine(doDamage(true, target, calculateDamage(target)));
        else
            StartCoroutine(doDamage(false, target, calculateDamage(target)));
    }
    protected float calculateDamage(Soldier target) {
        float dmg = Random.Range(damage * 0.75f, (damage * 1.25f) * (0.5f + health / maxHealth / 2));
        //attack bonuses
        if (target.troopType.Equals("infantry")) {
            dmg *= 1f + infantryBonus / 100f;
        } else if (target.troopType.Equals("armored")) {
            dmg *= 1f + armoredBonus / 100f;
        } else if (target.troopType.Equals("artillery")) {
            dmg *= 1f + artilleryBonus / 100f;
        } else if (target.troopType.Equals("fortress")) {
            dmg *= 1f + fortressBonus / 100f;
        } else if (target.troopType.Equals("naval")) {
            dmg *= 1f + navalBonus / 100f;
        }
        if (troopType.Equals("infantry")) {
            dmg *= 1f - target.infantryDefenseBonus / 100f;
        } else if (troopType.Equals("armored")) {
            dmg *= 1f - target.armoredDefenseBonus / 100f;
        } else if (troopType.Equals("artillery")) {
            dmg *= 1f - target.artilleryDefenseBonus / 100f;
        } else if (troopType.Equals("fortress")) {
            dmg *= 1f - target.fortressDefenseBonus / 100f;
        } else if (troopType.Equals("naval")) {
            dmg *= 1f - target.navalDefenseBonus / 100f;
        }
        if (currentTile.terrainName.Equals("forest")) {
            dmg *= 1f + forestBonus / 100f;
        } else if (currentTile.terrainName.Equals("mountains")) {
            dmg *= 1f + mountainBonus / 100f;
        }
        if (target.currentTile.terrainName.Equals("forest")) {
            dmg *= 1f - target.forestDefenseBonus / 100f;
        } else if (target.currentTile.terrainName.Equals("mountains")) {
            dmg *= 1f - target.mountainDefenseBonus / 100f;
        }
        //less damage when in water
        if (currentTile.isWater && !isNaval)
            dmg *= 0.71f; 
        //more damage to landing crafts
        if (target.currentTile.isWater && !target.isNaval)
            dmg *= 1.61f;
        if (!target.isCity && target.currentTile.isCity && target.currentTile.city.citySoldier.health > 0)
            dmg *= 0.63f;
        if (target.isCity && !target.currentTile.isOccupied)
            dmg *= 1.2f;
        //multiply damage by veterency and enemy veterency (more veterence = more attack and defense)
        dmg *= 1f + (veterency - target.veterency) / 10.8f; 
        return dmg; 
    }
    //use for damage countdown
    protected IEnumerator doDamage (bool canRetaliate, Soldier target, float dmg) {
        if (country.Equals(controllerScript.playerCountryName)) 
            controllerScript.stopPlayerActions = true;
        float i = 0;
        if (controllerScript.roundPassDelay > 0)
            i = attackDelay - 0.01f; 
        bool triggered = false; 
        while (i < attackDelay) {
            i += Time.deltaTime;
            if (canRetaliate && !triggered && i > 0.3f) {
                if (Vector3.Distance(currentTile.transform.position, target.currentTile.transform.position) < target.range + 0.3f && target.health - dmg > 0f) {
                    target.retaliateAttack(this);
                }
                triggered = true; 
            }
            yield return null; 
        }
        target.health -= dmg;

        //update experience and veterency
        experience += dmg;
        updateVeterency(); 

        if (controllerScript.roundPassDelay <= 0) {
            GameObject insItem = Instantiate(damageDisplayPrefab, target.transform.position, Quaternion.identity);
            insItem.transform.GetChild(0).GetComponent<Text>().text = "-" + (int)dmg;
            if (!target.isCity && target.currentTile.isCity && target.currentTile.city.citySoldier.health > 0) {
                insItem = Instantiate(damageDisplayPrefab, new Vector3(target.transform.position.x, target.transform.position.y - 0.6f, target.transform.position.z), Quaternion.identity);
                insItem.transform.GetChild(0).GetComponent<Text>().text = "-" + (int)(dmg * 1.16f);
                insItem.GetComponent<DamageDisplay>().destroyTimer += 0.62f; 
            }
        }
        if (!target.isCity && target.currentTile.isCity) {
            target.currentTile.city.citySoldier.health -= dmg * 1.16f;
            if (target.currentTile.city.citySoldier.health < 0)
                target.currentTile.city.citySoldier.health = 0;
        }
        if (target.health <= 0) {
            if (target.isCity) {
                target.health = 0;
                if (troopType == "armored") {
                    attacked = false;
                    if (country != controllerScript.playerCountryName && controllerScript.roundPassDelay > 0)
                        aiFindTarget(); 
                } else
                    moved = true; 
            } else {
                target.currentTile.setDefaultMaterial();
                target.currentTile.isOccupied = false;
                if (target != null) {
                    if (target.country.Equals(controllerScript.playerCountryName)) {
                        controllerScript.playerSoldiers[target.indexInArray] = null;
                    } else
                        controllerScript.aiSoldiers[target.indexInArray] = null;
                    Destroy(target.gameObject);
                    if (troopType == "armored") {
                        attacked = false;
                        if (country != controllerScript.playerCountryName && controllerScript.roundPassDelay > 0)
                            aiFindTarget();
                    } else
                        moved = true; 
                }
            }
        } else { 
            moved = true;
        }
        if (country.Equals(controllerScript.playerCountryName))
            controllerScript.stopPlayerActions = false; 
        yield return new WaitForSeconds(0.1f);
    }

    //replace locally
    protected virtual void animateAttack(Soldier target) { }

    protected virtual void updateUniformColor () { }

    public void retaliateAttack(Soldier target) {
        if (controllerScript.roundPassDelay <= 0f)
            controllerScript.audioManager.Play(troopName, 1);
        transform.LookAt(new Vector3(target.transform.position.x, 1, target.transform.position.z));
        animateAttack(target); 
        StartCoroutine(doDamage(false, target, calculateDamage(target)));
    }
    public void aiMovement() {
        //check attackable
        if (aiFindTarget())
            return;
        if (mobility <= 0)
            return;
        //stop if city is under siege (enemy nearby)
        if (currentTile.isCity) {
            foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z), 8f)) {
                if (i.GetComponent<Soldier>() != null && Vector3.Distance(i.transform.position, transform.position) < i.GetComponent<Soldier>().mobility + 0.5f && !controllerScript.findCountryAlliance(country, i.GetComponent<Soldier>().country) && currentTile.city.citySoldier.health < currentTile.city.citySoldier.maxHealth / 2f)
                    return; 
            }
        }
        //search for empty city (priority over attacking) 
        foreach (Collider i in Physics.OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z), mobility)) {
            if (i.GetComponent<Tile>() != null && i.GetComponent<Tile>().isCity && !controllerScript.findCountryAlliance(i.GetComponent<Tile>().city.country, country) && !i.GetComponent<Tile>().isOccupied && i.GetComponent<Tile>().city.citySoldier.health <= 0) {
                moveSoldier(i.GetComponent<Tile>(), false);
                return; 
            }
        }
        //attack city (with health left)
        foreach (City i in controllerScript.cities) {
            if (i != null && !controllerScript.findCountryAlliance(country, i.country)) {
                if (Vector3.Distance(i.tile.transform.position, currentTile.transform.position) <= mobility + range + 0.1f) {
                    foreach (Collider j in Physics.OverlapBox(new Vector3(i.transform.position.x, 0, i.transform.position.z), new Vector3(range, 2, range))) {
                        if (j.GetComponent<Tile>() != null && !j.GetComponent<Tile>().isOccupied && j.GetComponent<Tile>() != i.tile) {

                            if (j.GetComponent<Tile>().findPathToSoldier(currentTile, mobility, country)) {
                                moveSoldier(j.GetComponent<Tile>(), true);
                                return;
                            }
                        }
                    }
                }
            }
        }
        if (!controllerScript.findCountryAlliance(controllerScript.playerCountryName, country)) {
            foreach (Soldier i in controllerScript.playerSoldiers) {
                if (i != null) {
                    if (Vector3.Distance(i.currentTile.transform.position, currentTile.transform.position) <= mobility + range + 0.1f) {
                        foreach (Collider j in Physics.OverlapBox(new Vector3(i.transform.position.x, 0, i.transform.position.z), new Vector3(range, 2, range))) {
                            if (j.GetComponent<Tile>() != null && !j.GetComponent<Tile>().isOccupied) {
                                if (j.GetComponent<Tile>().findPathToSoldier(currentTile, mobility, country)) {
                                    moveSoldier(j.GetComponent<Tile>(), true);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        foreach (Soldier i in controllerScript.aiSoldiers) {
            if (i != null && !controllerScript.findCountryAlliance(country, i.country)) {
                if (Vector3.Distance(i.currentTile.transform.position, currentTile.transform.position) <= mobility + range + 0.1f) {
                    foreach (Collider j in Physics.OverlapBox(new Vector3(i.transform.position.x, 0, i.transform.position.z), new Vector3(range, 2, range))) {
                        if (j.GetComponent<Tile>() != null && !j.GetComponent<Tile>().isOccupied) {
                            if (j.GetComponent<Tile>().findPathToSoldier(currentTile, mobility, country)) {
                                moveSoldier(j.GetComponent<Tile>(), true);
                                return;
                            }
                        }
                    }
                }
            }
        }

        //below: move units that cannot yet attack
        float shortestDistance = 60f;
        Vector3 currentShortestPosition = Vector3.zero; 
        if (!controllerScript.findCountryAlliance(controllerScript.playerCountryName, country)) {
            foreach (Soldier i in controllerScript.playerSoldiers) {
                if (i != null) {
                    if (Vector3.Distance(i.transform.position, transform.position) < shortestDistance) {
                        shortestDistance = Vector3.Distance(i.transform.position, transform.position);
                        currentShortestPosition = i.transform.position;
                    }
                }
            }
        }
        foreach (Soldier i in controllerScript.aiSoldiers) {
            if (i != null && !controllerScript.findCountryAlliance(country, i.country)) {
                if (Vector3.Distance(i.transform.position, transform.position) < shortestDistance) {
                    shortestDistance = Vector3.Distance(i.transform.position, transform.position);
                    currentShortestPosition = i.transform.position;
                }
            }
        }
        foreach (City i in controllerScript.cities) {
            if (i != null && !controllerScript.findCountryAlliance(country, i.country)) {
                if (Vector3.Distance(i.transform.position, transform.position) < shortestDistance) {
                    shortestDistance = Vector3.Distance(i.transform.position, transform.position);
                    currentShortestPosition = i.transform.position;
                }
            }
        }

        if (currentShortestPosition == Vector3.zero) {
            return; 
        }
        shortestDistance = Vector3.Distance(transform.position, currentShortestPosition);
        Tile shortestTile = null; 
        foreach (Collider i in Physics.OverlapBox(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(mobility, 2, mobility))) {
            if (i.GetComponent<Tile>() != null && !i.GetComponent<Tile>().isOccupied && i.GetComponent<Tile>().findPathToSoldier(currentTile, mobility, country) && Vector3.Distance(i.transform.position, currentShortestPosition) < shortestDistance) {
                shortestTile = i.GetComponent<Tile>();
                shortestDistance = Vector3.Distance(i.transform.position, currentShortestPosition);
            }
        }
        if (shortestTile != null)
            moveSoldier(shortestTile, false);
    }
}
