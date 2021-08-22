using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

[System.Serializable]
public class CountryInfo {
    public Sprite sprite;
    public string name;
    public Material color;
    public string alliance;
    public int startingCash;
    public int startingIndustry; 
}
//in game cash, supplies, etc
[System.Serializable]
public class CountryData {
    public string name;
    public int cash;
    public int industry;
    public CountryData (string name, int startingCash, int startingIndustry) {
        this.name = name;
        cash = startingCash;
        industry = startingIndustry; 
    }
    public CountryData() { }
}
public static class Custom2D {
    public static Rect generatePointDetectionRect(Vector2 position, Rect rect) {
        Rect newRect = new Rect(new Vector2(position.x - rect.width / 2, position.y - rect.height / 2), new Vector2(rect.width, rect.height));
        return newRect;
    }
}
public class Controller : MonoBehaviour {
    [HideInInspector]
    public MapData loadedData;
    //if yes, load saved data
    [HideInInspector]
    public bool savedData;
    [HideInInspector]
    public int year = 1939;
    public GameObject map1939, map1941, map1944, map1945; 
    //Stop raycasts screen scroll etc when mouse/fingers is/are in rects; check in update
    public RectTransform[] UIRectTransforms;
    [HideInInspector]
    public Rect[] UIRects; 
    public Button buildTroopsButton;
    public Text cashDisplay;
    public Text industrialDisplay;
    public Transform buildTroopsUI, buildShipsUI, buildConstructionsUI, pauseUI, roundPassUI;
    public Text roundPassCashDisplay, roundPassIndustryDisplay, roundPassRoundDisplay; 
    public bool mouseOnUI; 

    //Soldier vars
    public Material yellowMaterial, blueMaterial, tileBlueMaterial, redMaterial, greyMaterial, darkGreyMaterial;
    public Sprite[] veterencySprites;

    public CountryInfo[] countryInfos;
    //match up with countryInfos (replace)
    public string[] alliance1939, alliance1941, alliance1944, alliance1945;
    public int[] startingCash1939, startingCash1941, startingCash1944, startingCash1945;
    public int[] startingIndustry1939, startingIndustry1941, startingIndustry1944, startingIndustry1945;

    [HideInInspector]
    public Canvas canvas; 
    //written in script with (data copied from tester) 
    [HideInInspector]
    public Vector2[] startingArmyLocations, cityLocations, terrainLocations;
    [HideInInspector]
    public int[] startingArmyTypes;
    [HideInInspector]
    public string[] startingArmyCountries; 
    public string[] cityNames;
    [HideInInspector]
    public int[] cityLevels, terrainTypes;
    [HideInInspector]
    public int[] cityIndustrials;
    [HideInInspector]
    public City[] cities;
    [HideInInspector]
    public string[] tileCountries;
    [HideInInspector]
    public MyVector3[] tilePositions;
    [HideInInspector]
    public bool[] tileIsWaters; 
    [HideInInspector]
    public Tile[] tiles;
    [HideInInspector]
    public bool onlyEurope; 

    //stores financial information about ai/player countries
    //[HideInInspector]
    public CountryData[] countries;
    //[HideInInspector]
    public CountryData playerCountry;

    public int[] buildTroopCashCosts, buildTroopIndustrialCosts, buildTroopIndustrialRequirements; 

    //[HideInInspector]
    public string playerCountryName;
    [HideInInspector]
    public int round;

    public Soldier[] aiSoldiers, playerSoldiers;

    [HideInInspector]
    public Soldier selectedSoldier;
    //[HideInInspector]
    public Tile selectedTile;

    //by round
    int playerCashProfit = 0, playerIndustryProfit = 0;

    public GameObject[] soldierPrefabs, terrainPrefabs; 
    public GameObject tilePrefab, cityPrefab, portPrefab, capitulationUIPrefab, victoryUIPrefab, defeatUIPrefab;

    [HideInInspector]
    public Transform cam;

    Vector3 mouseDownPosition; 

    //testing only
    Vector3[] positions;
    int[] numbers;

    //stop player from being able to select units when units are fighting or rounds are changing
    public bool stopPlayerActions, inUI; 

    [HideInInspector]
    public AudioManager audioManager;

    public string translateCountryName (string englishName, string language) {
        print(englishName);
        if (language == "Chinese") { 
            if (englishName == "Germany") {
                return "德国"; 
            }
            if (englishName == "Russia") {
                return "苏联";
            }
            if (englishName == "USA") {
                return "美国";
            }
            if (englishName == "France") {
                return "法国";
            }
            if (englishName == "Netherlands")
                return "荷兰";
            if (englishName == "Britain") {
                return "英国";
            }
            if (englishName == "Italy") {
                return "意大利";
            }
            if (englishName == "Poland") {
                return "波兰";
            }
            if (englishName == "Spain") {
                return "西班牙";
            }
            if (englishName == "Finland") {
                return "芬兰";
            }
            if (englishName == "Yugoslavia") {
                return "南斯拉夫";
            }
            if (englishName == "Romania") {
                return "罗马尼亚";
            }
            if (englishName == "Hungary") {
                return "匈牙利";
            }
            if (englishName == "Bulgaria") {
                return "保加利亚";
            }
            if (englishName == "Greece") {
                return "希腊";
            }
            if (englishName == "Norway") {
                return "挪威";
            }
            if (englishName == "Luxembourg") {
                return "卢森堡";
            }
        }
        return "";
    }
    public void lowerRoundPassUI () {
        roundPassUI.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        inUI = true;
        if (selectedSoldier != null) {
            selectedSoldier.selected = false;
            selectedSoldier.returnTiles();
        }
    }
    public void returnRoundPassUI () {
        roundPassUI.position = new Vector3(Screen.width / 2f, Screen.height * 4f, 0f);
        inUI = false;
    }
    public void pauseGame() {
        pauseUI.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        inUI = true;
        if (selectedSoldier != null) {
            selectedSoldier.selected = false;
            selectedSoldier.returnTiles();
        }
    }
    public void resumeGame() {
        pauseUI.position = new Vector3(Screen.width / 2f, Screen.height * 4f, 0f);
        inUI = false;
    }
    public void changeScene(int index) {
        if (roundPassDelay <= 0) {
            tilePositions = new MyVector3[tiles.Length];
            tileCountries = new string[tiles.Length];
            tileIsWaters = new bool[tiles.Length];
            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i] != null) {
                    tilePositions[i] = new MyVector3(tiles[i].transform.position);
                    tileCountries[i] = tiles[i].owningCountry;
                    tileIsWaters[i] = tiles[i].isWater;
                }
            }
            BinarySaveSystem.SaveMap(this);
            PlayerPrefs.SetString("save_" + year.ToString(), "true");
            SceneManager.LoadScene(index);
        }
    }
    public void buildTroopsButtonEnable() {
        //activiate port menu instead
        if (selectedTile.isCity && selectedTile.city.isPort) {
            buildShipsButtonEnable();
            return; 
        } 
        //activate forts menu
        if (!selectedTile.isCity) {
            buildFortsButtonEnable();
            return; 
        }
        buildTroopsUI.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        inUI = true;
        if (selectedSoldier != null) {
            selectedSoldier.selected = false;
            selectedSoldier.returnTiles();
        }
    }
    public void buildShipsButtonEnable () {
        buildShipsUI.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        inUI = true;
        if (selectedSoldier != null) {
            selectedSoldier.selected = false;
            selectedSoldier.returnTiles();
        }
    }
    public void buildFortsButtonEnable() {
        buildConstructionsUI.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        inUI = true;
        if (selectedSoldier != null) {
            selectedSoldier.selected = false;
            selectedSoldier.returnTiles();
        }
    }
    public void buildTroopsButtonDisable() {
        buildTroopsUI.position = new Vector3(Screen.width / 2f, Screen.height * 2f, 0f);
        buildShipsUI.position = new Vector3(Screen.width / 2f, Screen.height * 2f, 0f);
        buildConstructionsUI.position = new Vector3(Screen.width / 2f, Screen.height * 2f, 0f);
        inUI = false; 
    }
    public void buildTroops(int id) {
        if (selectedTile != null && !selectedTile.isOccupied && playerCountry.cash >= buildTroopCashCosts[id - 1] && playerCountry.industry >= buildTroopIndustrialCosts[id - 1] && (!selectedTile.isCity || selectedTile.city.cityIndustrial >= buildTroopIndustrialRequirements[id - 1])) {
            GameObject insItem = Instantiate(soldierPrefabs[id - 1], new Vector3(selectedTile.transform.position.x, 1, selectedTile.transform.position.z), Quaternion.identity);
            insItem.GetComponent<Soldier>().moved = true;
            insItem.GetComponent<Soldier>().attacked = true;
            playerCountry.cash -= buildTroopCashCosts[id - 1];
            playerCountry.industry -= buildTroopIndustrialCosts[id - 1];
            buildTroopsButtonDisable();
        }
        return; 
    }
    public Soldier aiBuildTroops(int id, Tile spawnTile, CountryData country) {
        if (spawnTile != null && !spawnTile.isOccupied && country.cash >= buildTroopCashCosts[id - 1] && country.industry >= buildTroopIndustrialCosts[id - 1] && spawnTile.city.cityIndustrial >= buildTroopIndustrialRequirements[id - 1]) {
            GameObject insItem = Instantiate(soldierPrefabs[id - 1], new Vector3(spawnTile.transform.position.x, 1, spawnTile.transform.position.z), Quaternion.identity);
            insItem.GetComponent<Soldier>().moved = true;
            insItem.GetComponent<Soldier>().attacked = true;
            country.cash -= buildTroopCashCosts[id - 1];
            country.industry -= buildTroopIndustrialCosts[id - 1];
            return insItem.GetComponent<Soldier>(); 
        }
        return null; 
    }
    public bool pushItemToArray<T>(ref T[] array, T item) {
        for (int i = 0; i < array.Length; i++) {
            if (array[i] == null) {
                array[i] = item;
                return true; 
            }
        }
        return false; 
    }
    public bool findNeutrality (string country) {
        string alliance = "";
        foreach (CountryInfo i in countryInfos) {
            if (i.name == country) {
                alliance = i.alliance;
            }
        }
        if (alliance == "")
            return true;
        else 
            return false;
    }
    public bool findCountryAlliance (string country1, string country2) {
        string alliance1 = "";
        string alliance2 = ""; 
        foreach (CountryInfo i in countryInfos) { 
            if (i.name == country1) {
                alliance1 = i.alliance; 
            }
            if (i.name == country2) {
                alliance2 = i.alliance; 
            }
        }
        if (alliance1 == "" || alliance2 == "")
            return true;
        if (alliance1.Equals(alliance2)) {
            return true; 
        } else 
            return false; 
    }
    public CountryData findCountryData (string countryName) {
        foreach (CountryData i in countries) {
            if (i.name == countryName) {
                return i; 
            }
        }
        return null;
    }
    //reflects array status
    public int findTroopID (string name) {
        switch (name) {
        case "assaultInfantry":
            return 0;
        case "tank":
            return 1;
        case "fieldArtillery":
            return 2;
        case "turret":
            return 3;
        case "MGTeam":
            return 4;
        case "heavyTank":
            return 5;
        case "ATGun":
            return 6;
        case "battleship":
            return 7;
        case "submarine":
            return 8;
        case "superHeavyTank":
            return 9;
        default:
            return 0;
        }
    }
    //flag for each country by string
    public Sprite findCountryFlag (string countryName) {
        foreach (CountryInfo i in countryInfos) { 
            if (i.name == countryName) {
                return i.sprite; 
            }
        }
        return null; 
    }
    //tile color for each country by string
    public Material findCountryColor (string countryName) {
        foreach (CountryInfo i in countryInfos) { 
            if (i.name == countryName) {
                return i.color; 
            }
        }
        return null; 
    }
    void Awake() {
        round = 1;
        year = PlayerPrefs.GetInt("year");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        if (year == 1939) {
            map1939.name = "CountrySpawn";
            for (int i = 0; i < countryInfos.Length; i++) {
                countryInfos[i].alliance = alliance1939[i];
                countryInfos[i].startingCash = startingCash1939[i];
                countryInfos[i].startingIndustry = startingIndustry1939[i];
            }
        } else if (year == 1941) {
            map1941.name = "CountrySpawn";
            for (int i = 0; i < countryInfos.Length; i++) {
                countryInfos[i].alliance = alliance1941[i];
                countryInfos[i].startingCash = startingCash1941[i];
                countryInfos[i].startingIndustry = startingIndustry1941[i];
            }
        } else if (year == 1944) { 
            map1944.name = "CountrySpawn";
            for (int i = 0; i < countryInfos.Length; i++) {
                countryInfos[i].alliance = alliance1944[i];
                countryInfos[i].startingCash = startingCash1944[i];
                countryInfos[i].startingIndustry = startingIndustry1944[i];
            }
        } else if (year == 1945) {
            map1945.name = "CountrySpawn";
            for (int i = 0; i < countryInfos.Length; i++) {
                countryInfos[i].alliance = alliance1945[i];
                countryInfos[i].startingCash = startingCash1945[i];
                countryInfos[i].startingIndustry = startingIndustry1945[i];
            }
        }

        if (PlayerPrefs.GetString("save_" + year.ToString()).Equals("true")) {
            loadedData = BinarySaveSystem.LoadMap();
            round = loadedData.round; 
            savedData = true;
        }
        if (savedData) {
            playerCountryName = loadedData.playerCountry.name;
            onlyEurope = loadedData.onlyEurope;
            print(onlyEurope); 
        } else {
            playerCountryName = PlayerPrefs.GetString("playerCountry");
            if (PlayerPrefs.GetInt("onlyEurope") == 1) {
                onlyEurope = true;
            } else {
                onlyEurope = false;
            }
        }
        //remake rects
        UIRects = new Rect[UIRectTransforms.Length];
        for (int i = 0; i < UIRectTransforms.Length; i++) {
            UIRects[i] = Custom2D.generatePointDetectionRect(new Vector2(UIRectTransforms[i].position.x, UIRectTransforms[i].position.y), UIRectTransforms[i].rect);
        }
        audioManager = GetComponent<AudioManager>();

        //for testing only
        positions = new Vector3[1000];
        numbers = new int[1000];

        aiSoldiers = new Soldier[1000];
        playerSoldiers = new Soldier[300];
        countries = new CountryData[countryInfos.Length - 1];
        if (!savedData) {
            for (int i = 0; i < countryInfos.Length; i++) {
                if (countryInfos[i].name != playerCountryName)
                    pushItemToArray(ref countries, new CountryData(countryInfos[i].name, countryInfos[i].startingCash, countryInfos[i].startingIndustry));
                else {
                    playerCountry = new CountryData(countryInfos[i].name, countryInfos[i].startingCash, countryInfos[i].startingIndustry);
                }
            }
        } else {
            countries = loadedData.countryDatas;
            playerCountry = loadedData.playerCountry;
        }

        if (onlyEurope) {
            cityLocations = new Vector2[] {
            //cities with names
            new Vector2(-48f, -6f), new Vector2(-54f, -5f), new Vector2(-55f, -7.5f), new Vector2(-52f, -11f), new Vector2(-48f, -12f), new Vector2(-45f, -9.5f), new Vector2(-42f, -3f), new Vector2(-61f, -10.5f), new Vector2(-60f, -15f), new Vector2(-58f, -17f), new Vector2(-64f, -17f), new Vector2(-65f, -13.5f), new Vector2(-59f, -6.5f), new Vector2(-57f, -4.5f), new Vector2(-66f, -3f), new Vector2(-69f, -1.5f), new Vector2(-70f, 3f), new Vector2(-73f, -1.5f), new Vector2(-52f, 6f), new Vector2(-46f, 4f), new Vector2(-40f, 7f), new Vector2(-52f, -1f),
            new Vector2(-38f, 0f), new Vector2(-29f, 4.5f), new Vector2(-30f, -2f), new Vector2(-29f, -8.5f), new Vector2(-25f, -14.5f), new Vector2(-13f, -11.5f), new Vector2(-38f, -6f), new Vector2(-37f, -9.5f), new Vector2(-22f, 1f), new Vector2(-14f, 5f), new Vector2(-7f, -4.5f), new Vector2(-1f, 2.5f), new Vector2(1f, -7.5f),
            new Vector2(-55f, -12.5f), new Vector2(-53f, -14.5f), new Vector2(-51f, -17.5f), new Vector2(-54f, -19f), new Vector2(-48f, -26f), new Vector2(-47f, -21.5f), new Vector2(-43f, -16.5f), new Vector2(-37f, -18.5f),
            new Vector2(-32f, -20f), new Vector2(-38f, -26f), new Vector2(-37f, -14.5f), new Vector2(-42f, -12f), new Vector2(-68f, -21f), new Vector2(-62f, -20f), new Vector2(-74f, -19f), new Vector2(-70f, -26f), new Vector2(-75f, -24.5f), new Vector2(-57f, -8.5f),

            //cities without names 
            new Vector2(-67f, -10.5f), new Vector2(-62f, -13f), new Vector2(-61f, -22.5f), new Vector2(-51f, -7.5f), new Vector2(-43f, 13.5f), new Vector2(-39f, 17.5f), new Vector2(-37f, 14.5f), new Vector2(-20f, -7f),
            new Vector2(-31f, 11.5f), new Vector2(-34f, -12f), new Vector2(-33f, -15.5f), new Vector2(-41f, -19.5f), new Vector2(-34f, -19f), new Vector2(-39f, -22.5f), new Vector2(-54f, -22f), new Vector2(-76f, -2f), new Vector2(-69f, -3.5f),
            new Vector2(-43f, -4.5f),

            //ports
            new Vector2(-71f, -3.5f), new Vector2(-75f, -4.5f), new Vector2(-65f, -1.5f), new Vector2(-65f, -8.5f), new Vector2(-66f, -14f), new Vector2(-64f, -23f), new Vector2(-50f, -21f), new Vector2(-42f, -22f), new Vector2(-27f, -15.5f),
            new Vector2(-53f, 3.5f), new Vector2(-58f, 5f), new Vector2(-55f, -2.5f),
        };
            if (PlayerPrefs.GetString("language") == "Chinese") {
                cityNames = new string[] {
                "柏林", "汉堡", "科隆", "慕尼黑", "维也纳", "布拉格", "柯尼斯堡", "巴黎", "里昂", "马赛",
                "图卢兹", "波尔多", "布鲁塞尔", "阿姆斯特丹", "伦敦", "曼彻斯特", "格拉斯哥", "都柏林", "奥斯陆", "斯德哥尔摩",
                "赫尔辛基", "哥本哈根", "里加", "列宁格勒", "明斯克", "基辅", "顿涅茨克", "斯大林格勒", "华沙", "克拉科夫",
                "斯莫林斯克", "莫斯科", "萨拉托夫", "叶卡捷琳堡", "阿斯塔纳", "苏黎世", "米兰", "罗马", "阿雅克肖", "巴勒莫",
                "那不勒斯", "贝尔格莱德", "索菲亚", "伊斯坦布尔", "雅典", "布加勒斯特", "布达佩斯", "马德里", "巴萨罗那", "科鲁纳",
                "塞维利亚", "利斯博亚", "卢森堡市", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "", "", "", "",

                //ports
                "", "", "", "", "", "", "", "", "", "", "", ""
            };
            } else {
                cityNames = new string[] {
                "Berlin", "Hamburg", "Cologne", "Munich", "Vienna", "Prague", "Konigsburg", "Paris", "Lyon", "Marseilles",
                "Toulouse", "Bordeaux", "Brussels", "Amsterdam", "London", "Manchester", "Glasgow", "Dublin", "Oslo", "Stockholm",
                "Helsinki", "Copenhagon", "Riga", "Liningrad", "Minsk", "Kiev", "Donetsk", "Stalingrad", "Warsaw", "Krakow",
                "Smolensk", "Moscow", "Saratov", "Yekaterinburg", "Astana", "Zurich", "Milan", "Rome", "Ajaccio", "Palermo",
                "Naples", "Belgrade", "Sofia", "Istanbul", "Athens", "Bucharest", "Budapest", "Madrid", "Barcelona", "A Coruna",
                "Sevilla", "Lisboa", "Luxembourg City", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "", "", "", "",

                //ports
                "", "", "", "", "", "", "", "", "", "", "", ""
            };
            }
            terrainLocations = new Vector2[] {
            new Vector2(-35f, 9.5f), new Vector2(-65f, -3.5f), new Vector2(-66f, -4f), new Vector2(-68f, -4f), new Vector2(-68f, 0f), new Vector2(-69f, 0.5f), new Vector2(-70f, 1f), new Vector2(-70f, 2f), new Vector2(-68f, -2f), new Vector2(-68f, -3f), new Vector2(-67f, -2.5f), new Vector2(-74f, -1f), new Vector2(-74f, -2f),
            new Vector2(-75f, -2.5f), new Vector2(-75f, -1.5f), new Vector2(-76f, -4f), new Vector2(-73f, -0.5f), new Vector2(-74f, -3f), new Vector2(-71f, 3.5f), new Vector2(-69f, 2.5f), new Vector2(-75f, -21.5f), new Vector2(-73f, -18.5f), new Vector2(-72f, -18f), new Vector2(-73f, -20.5f), new Vector2(-75f, -20.5f),
            new Vector2(-74f, -22f), new Vector2(-73f, -22.5f), new Vector2(-74f, -24f), new Vector2(-67f, -20.5f), new Vector2(-68f, -20f), new Vector2(-70f, -23f), new Vector2(-69f, -23.5f), new Vector2(-67f, -23.5f), new Vector2(-68f, -23f), new Vector2(-72f, -25f), new Vector2(-68f, -26f), new Vector2(-71f, -26.5f),
            new Vector2(-65f, -20.5f), new Vector2(-66f, -19f), new Vector2(-70f, -20f), new Vector2(-66f, -22f), new Vector2(-63f, -16.5f), new Vector2(-63f, -15.5f), new Vector2(-62f, -15f), new Vector2(-61f, -16.5f), new Vector2(-63f, -17.5f), new Vector2(-60f, -13f), new Vector2(-60f, -11f), new Vector2(-63f, -11.5f),
            new Vector2(-65f, -11.5f), new Vector2(-60f, -10f), new Vector2(-62f, -9f), new Vector2(-59f, -14.5f), new Vector2(-59f, -15.5f), new Vector2(-58f, -15f), new Vector2(-57f, -14.5f), new Vector2(-56f, -14f), new Vector2(-57f, -15.5f), new Vector2(-56f, -15f), new Vector2(-55f, -14.5f), new Vector2(-55f, -13.5f),
            new Vector2(-54f, -14f), new Vector2(-54f, -13f), new Vector2(-53f, -13.5f), new Vector2(-52f, -14f), new Vector2(-52f, -13f), new Vector2(-53f, -12.5f), new Vector2(-51f, -13.5f), new Vector2(-50f, -14f), new Vector2(-51f, -12.5f), new Vector2(-50f, -13f), new Vector2(-49f, -13.5f), new Vector2(-52f, -17f),
            new Vector2(-52f, -18f), new Vector2(-50f, -19f), new Vector2(-54f, -16f), new Vector2(-48f, -21f), new Vector2(-46f, -23f), new Vector2(-47f, -25.5f), new Vector2(-47f, -5.5f), new Vector2(-46f, -6f), new Vector2(-42f, -8f), new Vector2(-44f, -5f), new Vector2(-49f, -9.5f), new Vector2(-41f, -1.5f), new Vector2(-40f, -2f),
            new Vector2(-39f, -2.5f), new Vector2(-46f, -10f), new Vector2(-46f, -9f), new Vector2(-44f, -8f), new Vector2(-48f, -8f), new Vector2(-51f, -5.5f), new Vector2(-53f, -7.5f), new Vector2(-55f, -9.5f), new Vector2(-52f, -9f), new Vector2(-57f, -10.5f), new Vector2(-59f, -8.5f), new Vector2(-56f, -6f), new Vector2(-50f, -4f),
            new Vector2(-58f, -12f), new Vector2(-50f, -7f), new Vector2(-54f, -8f), new Vector2(-53f, -5.5f), new Vector2(-53f, -2.5f), new Vector2(-54f, 0f), new Vector2(-45f, -3.5f), new Vector2(-45f, -14.5f), new Vector2(-41f, -16.5f),
            new Vector2(-42f, -15f), new Vector2(-43f, -18.5f), new Vector2(-39f, -19.5f), new Vector2(-41f, -18.5f), new Vector2(-40f, -21f), new Vector2(-40f, -22f), new Vector2(-39f, -24.5f), new Vector2(-39f, -26.5f), new Vector2(-36f, -21f), new Vector2(-36f, -19f), new Vector2(-33f, -18.5f),
            new Vector2(-38f, -18f), new Vector2(-39f, -17.5f), new Vector2(-33f, -20.5f), new Vector2(-36f, -18f), new Vector2(-43f, -12.5f), new Vector2(-42f, -13f), new Vector2(-41f, -12.5f), new Vector2(-40f, -13f), new Vector2(-41f, -13.5f), new Vector2(-44f, -13f), new Vector2(-43f, -13.5f), new Vector2(-40f, -14f), new Vector2(-38f, -13f),
            new Vector2(-36f, -12f), new Vector2(-35f, -14.5f), new Vector2(-36f, -14f), new Vector2(-36f, -16f), new Vector2(-33f, -13.5f), new Vector2(-34f, -13f), new Vector2(-31f, -13.5f), new Vector2(-34f, -16f), new Vector2(-32f, -15f), new Vector2(-33f, -11.5f),
            new Vector2(-35f, -10.5f), new Vector2(-40f, -9f), new Vector2(-41f, -7.5f), new Vector2(-42f, -5f), new Vector2(-43f, -5.5f), new Vector2(-40f, -4f), new Vector2(-40f, -6f), new Vector2(-37f, -7.5f), new Vector2(-38f, -9f), new Vector2(-37f, -5.5f), new Vector2(-36f, -7f), new Vector2(-35f, -3.5f), new Vector2(-35f, -2.5f),
            new Vector2(-37f, -3.5f), new Vector2(-38f, -4f), new Vector2(-36f, -9f), new Vector2(-47f, -13.5f), new Vector2(-43f, -9.5f), new Vector2(-53f, -10.5f), new Vector2(-50f, -11f),
            new Vector2(-54f, 5f), new Vector2(-56f, 6f), new Vector2(-53f, 7.5f), new Vector2(-52f, 10f), new Vector2(-49f, 12.5f), new Vector2(-51f, 10.5f), new Vector2(-47f, 15.5f), new Vector2(-43f, 17.5f), new Vector2(-41f, 16.5f), new Vector2(-37f, 17.5f), new Vector2(-38f, 17f), new Vector2(-48f, 4f), new Vector2(-48f, 3f), new Vector2(-49f, 2.5f), new Vector2(-48f, 6f), new Vector2(-46f, 8f), new Vector2(-51f, 4.5f), new Vector2(-48f, 10f), new Vector2(-45f, 10.5f), new Vector2(-46f, 13f), new Vector2(-44f, 13f), new Vector2(-41f, 14.5f), new Vector2(-44f, 15f), new Vector2(-45f, 14.5f), new Vector2(-45f, 16.5f), new Vector2(-44f, 17f), 
            new Vector2(-48f, 1f), new Vector2(-51f, 3.5f), new Vector2(-46f, 6f), new Vector2(-50f, 7f), new Vector2(-38f, 8f), new Vector2(-36f, 7f), new Vector2(-40f, 8f), new Vector2(-39f, 6.5f), new Vector2(-34f, 8f), new Vector2(-37f, 9.5f), new Vector2(-38f, 11f), new Vector2(-36f, 12f), new Vector2(-38f, 13f), new Vector2(-36f, 15f), new Vector2(-38f, 16f), new Vector2(-34f, 11f), new Vector2(-36f, 9f), new Vector2(-36f, 13f), new Vector2(-33f, 7.5f), new Vector2(-34f, 7f), new Vector2(-31f, -2.5f), new Vector2(-30f, -3f), new Vector2(-30f, -4f), new Vector2(-32f, -1f), new Vector2(-30f, 1f), new Vector2(-35f, 1.5f), new Vector2(-32f, 2f), new Vector2(-37f, -0.5f), new Vector2(-38f, 2f), new Vector2(-30f, 4f), new Vector2(-27f, 0.5f), new Vector2(-28f, -2f), new Vector2(-28f, 3f), new Vector2(-25f, 1.5f), new Vector2(-26f, -4f), new Vector2(-32f, -6f),
             new Vector2(-32f, -4f), new Vector2(-31f, -9.5f), new Vector2(-33f, -8.5f), new Vector2(-30f, -10f), new Vector2(-31f, -8.5f), new Vector2(-27f, -7.5f), new Vector2(-28f, -9f), new Vector2(-25f, -9.5f), new Vector2(-29f, -5.5f), new Vector2(-27f, -11.5f), new Vector2(-25f, -12.5f), new Vector2(-23f, -1.5f), new Vector2(-22f, -1f), new Vector2(-17f, -0.5f), new Vector2(-16f, -1f), new Vector2(-16f, 0f), new Vector2(-16f, 1f), new Vector2(-18f, 4f), new Vector2(-21f, 5.5f), new Vector2(-16f, 8f), new Vector2(-14f, -5f), new Vector2(-14f, -6f), new Vector2(-13f, -6.5f), new Vector2(-18f, -11f), new Vector2(-18f, -10f), new Vector2(-17f, -9.5f), new Vector2(-15f, -14.5f), new Vector2(-15f, -15.5f), new Vector2(-19f, -14.5f), new Vector2(-15f, -12.5f), new Vector2(-22f, -11f), new Vector2(-17f, -7.5f), new Vector2(-14f, -10f), new Vector2(-19f, -3.5f), 
             new Vector2(-23f, -5.5f), new Vector2(-24f, 4f), new Vector2(-28f, 7f), new Vector2(-24f, 8f), new Vector2(-26f, 6f), new Vector2(-20f, 10f), new Vector2(-28f, 9f), new Vector2(-24f, 10f), new Vector2(-20f, 1f), new Vector2(-22f, -8f), new Vector2(-21f, -7.5f), new Vector2(-21f, -4.5f), new Vector2(-13f, -18.5f), new Vector2(-12f, -16f), new Vector2(-17f, -16.5f), new Vector2(-15f, -17.5f), new Vector2(-13f, -16.5f), new Vector2(-12f, -14f), new Vector2(-11f, -19.5f), new Vector2(-14f, -20f), new Vector2(-9f, -10.5f), new Vector2(-4f, -6f), new Vector2(-9f, -6.5f), new Vector2(-8f, -7f), new Vector2(-7f, -7.5f), new Vector2(-9f, -5.5f), new Vector2(-9f, -4.5f), new Vector2(-10f, -4f), new Vector2(-8f, 2f), new Vector2(-7f, 2.5f), new Vector2(-7f, 3.5f), new Vector2(-7f, 4.5f), new Vector2(-7f, 5.5f), new Vector2(-6f, 6f), new Vector2(-12f, 2f), 
             new Vector2(-11f, 6.5f), new Vector2(-9f, 9.5f), new Vector2(-14f, 10f), new Vector2(-12f, -1f), new Vector2(-14f, -3f), new Vector2(-17f, -4.5f), new Vector2(-31f, 10.5f), new Vector2(-33f, 15.5f), new Vector2(-32f, 15f), new Vector2(-32f, 12f), new Vector2(-11f, -8.5f), new Vector2(-4f, -11f), new Vector2(-6f, -11f), new Vector2(-1f, -4.5f), new Vector2(-3f, -9.5f), new Vector2(-1f, -12.5f), new Vector2(-2f, -16f), new Vector2(1f, -18.5f), new Vector2(1f, -15.5f), new Vector2(4f, -16f), new Vector2(-2f, -18f), new Vector2(3f, -11.5f), new Vector2(3f, -6.5f), new Vector2(0f, -8f), new Vector2(1f, -10.5f), new Vector2(2f, -4f), new Vector2(-2f, -2f), new Vector2(-5f, -2.5f), new Vector2(-8f, -1f), new Vector2(-4f, 1f), new Vector2(0f, 0f), new Vector2(-3f, 3.5f), new Vector2(2f, 5f), new Vector2(3f, 1.5f), new Vector2(-2f, 6f), new Vector2(0f, 5f), 
             new Vector2(1f, 7.5f), new Vector2(-3f, 9.5f), new Vector2(-1f, 9.5f), new Vector2(2f, 11f), new Vector2(-7f, 12.5f), new Vector2(-12f, 11f), new Vector2(-16f, 12f), new Vector2(-18f, 7f), new Vector2(-25f, -1.5f), new Vector2(-25f, -6.5f), new Vector2(-22f, -13f), new Vector2(-17f, -13.5f), new Vector2(-20f, -11f), new Vector2(-11f, 3.5f),
            new Vector2(-58f, -8f), new Vector2(-58f, -7f), new Vector2(-57f, -6.5f), new Vector2(-58f, -6f), new Vector2(-57f, -7.5f), new Vector2(-60f, -8f), new Vector2(-59f, -9.5f), new Vector2(-60f, -9f), new Vector2(-58f, -9f), new Vector2(-59f, -7.5f), new Vector2(-60f, -7f),
        };
            terrainTypes = new int[] {
            1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0,
            0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0,
            1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        };
            if (year == 1939) {
                startingArmyLocations = new Vector2[] {
                    ///*
                    //Germany
                    new Vector2(-55f, -12.5f), new Vector2(-56f, -6f), new Vector2(-57f, -7.5f), new Vector2(-46f, -5f), new Vector2(-44f, -8f), new Vector2(-46f, -7f), new Vector2(-45f, -7.5f), new Vector2(-55f, -7.5f), new Vector2(-55f, -8.5f), new Vector2(-55f, -4.5f), new Vector2(-47f, -3.5f), new Vector2(-43f, -9.5f), new Vector2(-56f, -4f), new Vector2(-54f, -11f), new Vector2(-41f, -3.5f), new Vector2(-42f, -3f), new Vector2(-41f, -2.5f), new Vector2(-47f, -11.5f), new Vector2(-48f, -12f), new Vector2(-50f, -10f), new Vector2(-52f, -4f), new Vector2(-53f, -3.5f), new Vector2(-54f, -5f), new Vector2(-54f, -9f), new Vector2(-51f, -7.5f), new Vector2(-52f, -11f), new Vector2(-50f, -12f), new Vector2(-45f, -9.5f), new Vector2(-41f, -9.5f), new Vector2(-45f, -3.5f), new Vector2(-48f, -6f), new Vector2(-50f, -9f), new Vector2(-48f, -9f), new Vector2(-50f, -4f), 
                    //Poland
                    new Vector2(-44f, -5f), new Vector2(-43f, -6.5f), new Vector2(-42f, -8f), new Vector2(-38f, -6f), new Vector2(-37f, -9.5f), new Vector2(-40f, -7f), new Vector2(-39f, -8.5f), new Vector2(-35f, -3.5f), new Vector2(-36f, -7f), new Vector2(-37f, -3.5f), new Vector2(-39f, -4.5f), 
                    //France
                    new Vector2(-57f, -9.5f), new Vector2(-56f, -10f), new Vector2(-56f, -11f), new Vector2(-56f, -12f), new Vector2(-57f, -10.5f), new Vector2(-57f, -11.5f), new Vector2(-57f, -12.5f), new Vector2(-58f, -9f), new Vector2(-58f, -10f), new Vector2(-57f, -13.5f), new Vector2(-58f, -17f), new Vector2(-64f, -17f), new Vector2(-60f, -15f), new Vector2(-61f, -10.5f), new Vector2(-65f, -13.5f), new Vector2(-61f, -8.5f), new Vector2(-60f, -9f), new Vector2(-59f, -11.5f), new Vector2(-63f, -15.5f), new Vector2(-60f, -17f), new Vector2(-62f, -13f), new Vector2(-64f, -11f), new Vector2(-57f, -15.5f), new Vector2(-57f, -16.5f), new Vector2(-62f, -18f), new Vector2(-59f, -6.5f), new Vector2(-57f, -4.5f), 
                    //Italy
                    new Vector2(-41f, 7.5f), new Vector2(-55f, -14.5f), new Vector2(-55f, -15.5f), new Vector2(-53f, -14.5f), new Vector2(-51f, -17.5f), new Vector2(-54f, -19f), new Vector2(-47f, -21.5f), new Vector2(-49f, -18.5f), new Vector2(-50f, -20f), new Vector2(-45f, -23.5f), new Vector2(-46f, -20f), new Vector2(-50f, -14f), new Vector2(-48f, -26f), new Vector2(-52f, -16f), new Vector2(-49f, -19.5f), new Vector2(-54f, -22f), 
                    //Russia
                    new Vector2(-38f, 0f), new Vector2(-29f, 4.5f), new Vector2(-30f, -2f), new Vector2(-29f, -8.5f), new Vector2(-25f, -14.5f), new Vector2(-22f, 1f), new Vector2(-14f, 5f), new Vector2(-13f, -11.5f), new Vector2(-33f, -7.5f), new Vector2(-33f, -3.5f), new Vector2(-37f, -1.5f), new Vector2(-31f, -10.5f), new Vector2(-31f, -5.5f), new Vector2(-28f, -12f), new Vector2(-27f, -5.5f), new Vector2(-34f, 1f), new Vector2(-23f, -11.5f), new Vector2(-32f, 5f), new Vector2(-31f, 8.5f), new Vector2(-33f, 10.5f), new Vector2(-33f, 13.5f), new Vector2(-34f, 16f), new Vector2(-31f, 11.5f), new Vector2(-32f, 15f), 
                    //Scandanavia
                    new Vector2(-53f, -0.5f), new Vector2(-52f, -1f), new Vector2(-56f, 5f), new Vector2(-52f, 6f), new Vector2(-54f, 7f), new Vector2(-51f, 10.5f), new Vector2(-46f, 4f), new Vector2(-48f, 1f), new Vector2(-50f, 3f), new Vector2(-48f, 6f), new Vector2(-50f, 1f), new Vector2(-44f, 12f), new Vector2(-46f, 10f), new Vector2(-54f, 4f), new Vector2(-41f, 14.5f), new Vector2(-38f, 18f), new Vector2(-41f, 17.5f), new Vector2(-37f, 18.5f), new Vector2(-36f, 10f), new Vector2(-40f, 7f), new Vector2(-38f, 8f), new Vector2(-38f, 7f), new Vector2(-37f, 7.5f), new Vector2(-39f, 9.5f), new Vector2(-41f, 7.5f), new Vector2(-37f, 11.5f), new Vector2(-36f, 15f), new Vector2(-35f, 8.5f), new Vector2(-33f, 7.5f), new Vector2(-35f, 6.5f), new Vector2(-33f, 9.5f), new Vector2(-34f, 11f), new Vector2(-35f, 12.5f), new Vector2(-39f, 14.5f), new Vector2(-38f, 15f), 
                    //Britain
                    new Vector2(-73f, -1.5f), new Vector2(-70f, 3f), new Vector2(-69f, -1.5f), new Vector2(-66f, -3f), new Vector2(-68f, -3f), new Vector2(-67f, -0.5f), new Vector2(-69f, 1.5f), new Vector2(-75f, -2.5f), new Vector2(-72f, 4f), new Vector2(-76f, -4f), new Vector2(-70f, -4f), new Vector2(-65f, -4.5f), new Vector2(-66f, -1f), 
                    //Baltics
                    new Vector2(-46f, -14f), new Vector2(-47f, -14.5f), new Vector2(-44f, -15f), new Vector2(-45f, -16.5f), new Vector2(-43f, -16.5f), new Vector2(-42f, -15f), new Vector2(-43f, -18.5f), new Vector2(-41f, -17.5f), new Vector2(-46f, -15f), new Vector2(-40f, -20f), new Vector2(-37f, -18.5f), new Vector2(-34f, -19f), new Vector2(-36f, -20f), new Vector2(-32f, -18f), new Vector2(-38f, -17f), new Vector2(-35f, -17.5f), new Vector2(-38f, -26f), new Vector2(-39f, -23.5f), new Vector2(-35f, -20.5f), new Vector2(-39f, -21.5f), new Vector2(-32f, -20f), new Vector2(-37f, -14.5f), new Vector2(-34f, -16f), new Vector2(-39f, -14.5f), new Vector2(-32f, -15f), new Vector2(-35f, -14.5f), new Vector2(-33f, -11.5f), new Vector2(-31f, -13.5f), new Vector2(-38f, -12f), new Vector2(-42f, -12f), new Vector2(-44f, -12f), new Vector2(-39f, -10.5f), 
                    //Spain
                    new Vector2(-62f, -20f), new Vector2(-68f, -21f), new Vector2(-74f, -19f), new Vector2(-75f, -24.5f), new Vector2(-70f, -26f), new Vector2(-61f, -22.5f), new Vector2(-65f, -19.5f), new Vector2(-67f, -18.5f), new Vector2(-63f, -19.5f), new Vector2(-69f, -23.5f), new Vector2(-72f, -21f), new Vector2(-65f, -21.5f), new Vector2(-57f, -8.5f),
                    //Navy
                    new Vector2(-55f, -2.5f), new Vector2(-56f, -3f), new Vector2(-57f, -3.5f), new Vector2(-58f, -4f), new Vector2(-56f, -2f), new Vector2(-57f, -2.5f), new Vector2(-55f, -1.5f), new Vector2(-66f, -9f), new Vector2(-64f, -8f), new Vector2(-68f, -6f), new Vector2(-67f, -6.5f), new Vector2(-63f, -3.5f), new Vector2(-63f, -2.5f), new Vector2(-63f, -0.5f), new Vector2(-63f, 0.5f), new Vector2(-72f, -4f), new Vector2(-73f, -5.5f), new Vector2(-72f, -8f), new Vector2(-71f, -8.5f), new Vector2(-68f, -7f), 
                    //*/
                };
                startingArmyTypes = new int[] {
                    //Germanya
                    2, 5, 5, 5, 5, 4, 4, 4, 4, 4,
                    1, 1, 1, 2, 2, 1, 4, 6, 2, 6,
                    1, 1, 10, 0, 10, 0, 4, 4, 1, 1,
                    1, 0, 10, 2, 
                    //Poland
                    10, 10, 10, 2, 6, 1, 1, 0, 10, 4, 6, 
                    //France
                    3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 1, 4, 1, 1, 6, 6, 6, 2, 10, 10, 10, 3, 2, 2, 2,
                    //Italy
                    0, 1, 2, 5, 0, 2, 6, 4, 4, 3, 3, 1, 1, 2, 0, 1,
                    //Russia
                    1, 2, 4, 2, 4, 0, 5, 2, 5, 5, 3, 3, 3, 10, 10, 10, 0, 10, 10, 10, 0, 10, 10, 10, 2, 
                    //Scandanavia
                    2, 2, 1, 0, 10, 10, 1, 0, 0, 2, 3, 1, 0, 3, 3, 1, 0, 2, 1, 1, 10, 10, 10, 10, 0, 10, 10, 2, 10, 10, 1, 10, 10, 10, 10,
                    //Britain
                    2, 1, 1, 5, 6, 6, 4, 4, 0, 0, 3, 3, 3, 
                    //Balkans
                    4, 4, 0, 1, 2, 6, 6, 6, 6, 0, 1, 2, 6, 0, 0, 0, 2, 0, 0, 4, 1, 1, 1, 2, 6, 0, 0, 0, 0, 2, 0, 0, 
                    //Spain
                    1, 1, 2, 10, 10, 0, 2, 1, 3, 0, 4, 4, 
                    //Navy
                    7, 8, 8, 8, 8, 8, 8, 8, 8, 7,
                    7, 7, 7, 7, 7, 7, 7, 7, 8, 8,
                };
                startingArmyCountries = new string[] {
                    "Germany", "Germany", "Germany", "Germany", "Germany", "Germany", "Germany", "France", "France", "Britain",
                    "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain",
                };
                cityLevels = new int[] {
                    //cities with names
                    5, 4, 3, 3, 3, 4, 2, 5, 4, 3,
                    2, 3, 4, 4, 5, 4, 3, 4, 4, 4,
                    5, 4, 3, 4, 3, 3, 2, 3, 5, 4,
                    3, 5, 2, 2, 1, 4, 3, 5, 1, 2,
                    3, 4, 4, 4, 5, 5, 5, 5, 4, 2,
                    2, 2, 3, 

                    //cities without names
                    1, 2, 1, 3, 1, 1, 1,
                    1, 1, 2, 2, 3, 2, 1, 1, 2, 1, 2,

                    //ports
                    2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 2
                };
                cityIndustrials = new int[] {
                    //cities with names
                    3, 2, 2, 2, 1, 1, 2, 3, 2, 1,
                    1, 1, 1, 1, 3, 2, 1, 1, 1, 1,
                    1, 1, 1, 2, 2, 2, 1, 1, 2, 1,
                    1, 3, 1, 1, 0, 1, 1, 2, 0, 0,
                    1, 1, 1, 1, 1, 1, 1, 2, 1, 1,
                    1, 1, 1,

                    //cities without names
                    0, 0, 0, 0, 1, 0, 0,
                    0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 

                    //ports
                    3, 3, 2, 2, 1, 1, 2, 1, 2, 1, 1, 2
                };
            } else if (year == 1941) {
                cityLevels = new int[] {
                //cities with names
                5, 3, 3, 3, 2, 3, 2, 4, 3, 2,
                1, 1, 2, 2, 5, 4, 4, 4, 3, 4,
                4, 3, 3, 4, 3, 3, 2, 4, 3, 2,
                3, 5, 3, 3, 2, 4, 3, 5, 1, 2,
                3, 3, 4, 4, 3, 5, 5, 5, 4, 2,
                2, 2, 2, 

                //cities without names
                1, 2, 1, 2, 1, 1, 1,
                1, 1, 2, 2, 2, 2, 1, 1, 2, 1, 1,

                //ports
                2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 2
            };
                cityIndustrials = new int[] {
                //cities with names
                3, 2, 3, 2, 2, 1, 1, 1, 0, 0,
                0, 0, 0, 0, 3, 3, 2, 2, 1, 1,
                2, 1, 2, 3, 2, 2, 2, 3, 1, 0,
                2, 3, 3, 3, 2, 1, 1, 3, 0, 0,
                1, 0, 2, 1, 0, 2, 2, 2, 1, 1,
                1, 1, 1,

                //cities without names
                0, 0, 0, 0, 1, 0, 0,
                0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0,

                //ports
                3, 3, 2, 2, 1, 1, 2, 1, 2, 1, 1, 2
            };
                //army depends on map type
                startingArmyLocations = new Vector2[] {
                ///*
                //Germany
                new Vector2(-35f, -1.5f), new Vector2(-37f, -2.5f), new Vector2(-39f, -2.5f), new Vector2(-35f, -3.5f), new Vector2(-35f, -5.5f), new Vector2(-37f, -4.5f), new Vector2(-36f, -3f), new Vector2(-34f, -5f), new Vector2(-34f, -3f), new Vector2(-35f, -7.5f),
                new Vector2(-36f, -6f), new Vector2(-37f, -3.5f), new Vector2(-38f, -3f), new Vector2(-36f, -4f), new Vector2(-36f, -5f), new Vector2(-35f, -4.5f), new Vector2(-35f, -2.5f), new Vector2(-36f, -2f), new Vector2(-35f, -6.5f), new Vector2(-36f, -7f),
                new Vector2(-35f, -8.5f), new Vector2(-38f, -6f), new Vector2(-37f, -9.5f), new Vector2(-42f, -3f), new Vector2(-45f, -9.5f), new Vector2(-48f, -6f), new Vector2(-51f, -7.5f), new Vector2(-54f, -5f), new Vector2(-55f, -7.5f), new Vector2(-57f, -8.5f),
                new Vector2(-48f, -12f), new Vector2(-38f, -26f), new Vector2(-61f, -10.5f), new Vector2(-60f, -15f), new Vector2(-64f, -17f), new Vector2(-52f, -1f), new Vector2(-52f, 6f), new Vector2(-43f, -16.5f), 
                //Italy
                new Vector2(-41f, 7.5f), new Vector2(-55f, -14.5f), new Vector2(-55f, -15.5f), new Vector2(-53f, -14.5f), new Vector2(-51f, -17.5f), new Vector2(-54f, -19f), new Vector2(-47f, -21.5f), new Vector2(-49f, -18.5f), new Vector2(-50f, -20f), new Vector2(-45f, -23.5f), new Vector2(-46f, -20f), new Vector2(-50f, -14f), new Vector2(-48f, -26f), new Vector2(-52f, -16f), new Vector2(-49f, -19.5f), new Vector2(-54f, -22f), 
                //Russia
                new Vector2(-38f, 0f), new Vector2(-36f, 1f), new Vector2(-33f, 0.5f), new Vector2(-32f, -3f), new Vector2(-32f, -1f), new Vector2(-32f, 2f), new Vector2(-35f, 2.5f), new Vector2(-40f, 1f), new Vector2(-30f, -5f), new Vector2(-31f, -6.5f), new Vector2(-32f, -5f), new Vector2(-33f, -7.5f), new Vector2(-30f, -2f), new Vector2(-29f, -8.5f), new Vector2(-31f, -8.5f), new Vector2(-28f, -6f), new Vector2(-29f, 1.5f), new Vector2(-26f, -2f), new Vector2(-25f, -8.5f), new Vector2(-29f, -11.5f), new Vector2(-22f, 1f), new Vector2(-29f, 4.5f), new Vector2(-31f, 11.5f), new Vector2(-33f, 15.5f), new Vector2(-31f, 15.5f), new Vector2(-32f, 14f), new Vector2(-32f, 12f), new Vector2(-31f, 5.5f), new Vector2(-30f, -10f), new Vector2(-23f, 0.5f), new Vector2(-14f, 5f), new Vector2(-19f, 5.5f), new Vector2(-24f, 7f), new Vector2(-18f, -3f), new Vector2(-20f, -7f), new Vector2(-23f, -4.5f), new Vector2(-25f, -14.5f), new Vector2(-22f, -11f), 
                //Scandanavia
                new Vector2(-46f, 4f), new Vector2(-43f, 13.5f), new Vector2(-40f, 7f), new Vector2(-37f, 14.5f), new Vector2(-37f, 9.5f), new Vector2(-36f, 8f), new Vector2(-34f, 8f), new Vector2(-36f, 12f), new Vector2(-35f, 9.5f), new Vector2(-36f, 13f), new Vector2(-38f, 8f), new Vector2(-38f, 10f), 
                //Britain
                new Vector2(-73f, -1.5f), new Vector2(-70f, 3f), new Vector2(-69f, -1.5f), new Vector2(-66f, -3f), new Vector2(-68f, -3f), new Vector2(-67f, -0.5f), new Vector2(-69f, 1.5f), new Vector2(-75f, -2.5f), new Vector2(-72f, 4f), new Vector2(-76f, -4f), new Vector2(-70f, -4f), new Vector2(-65f, -4.5f), new Vector2(-66f, -1f), 
                //Balkans
                new Vector2(-42f, -12f), new Vector2(-41f, -11.5f), new Vector2(-41f, -12.5f), new Vector2(-44f, -12f), new Vector2(-37f, -14.5f), new Vector2(-34f, -12f), new Vector2(-32f, -13f), new Vector2(-34f, -10f), new Vector2(-33f, -11.5f), new Vector2(-32f, -12f), new Vector2(-31f, -12.5f), new Vector2(-31f, -13.5f), new Vector2(-33f, -12.5f), new Vector2(-33f, -15.5f), new Vector2(-37f, -18.5f), new Vector2(-34f, -19f), new Vector2(-33f, -18.5f), new Vector2(-32f, -20f), 
                //Spain
                new Vector2(-62f, -20f), new Vector2(-68f, -21f), new Vector2(-74f, -19f), new Vector2(-75f, -24.5f), new Vector2(-70f, -26f), new Vector2(-61f, -22.5f), new Vector2(-65f, -19.5f), new Vector2(-67f, -18.5f), new Vector2(-63f, -19.5f), new Vector2(-69f, -23.5f), new Vector2(-72f, -21f), new Vector2(-65f, -21.5f),
                //Navy
                new Vector2(-64f, -8f), new Vector2(-63f, -7.5f), new Vector2(-62f, -7f), new Vector2(-61f, -6.5f), new Vector2(-60f, -6f), new Vector2(-59f, -5.5f), new Vector2(-59f, -4.5f), new Vector2(-58f, -4f),
                new Vector2(-66f, -6f), new Vector2(-67f, -5.5f), new Vector2(-67f, -6.5f), new Vector2(-69f, -7.5f), new Vector2(-70f, -7f), new Vector2(-70f, -8f), new Vector2(-72f, -8f), new Vector2(-63f, -3.5f), new Vector2(-63f, -2.5f), new Vector2(-62f, -3f), new Vector2(-62f, -4f), new Vector2(-73f, -4.5f), new Vector2(-73f, -5.5f), new Vector2(-63f, 0.5f), new Vector2(-62f, 1f), 
                //*/
            };
                startingArmyTypes = new int[] {
                //Germanya
                1, 1, 1, 0, 0, 5, 5, 5, 5, 5,
                5, 5, 5, 5, 1, 1, 2, 2, 1, 1,
                1, 1, 1, 1, 0, 0, 10, 0, 10, 0,
                0, 10, 2, 0, 1, 0, 10, 0, 

                //Italy
                0, 1, 2, 5, 0, 2, 6, 4, 4, 3, 3, 1, 1, 2, 0, 1,
                //Russia
                1, 1, 1, 1, 10, 10, 10, 10, 10, 0,
                1, 1, 5, 1, 2, 2, 2, 0, 10, 0,
                2, 1, 1, 0, 10, 0, 3, 3, 3, 3,
                5, 0, 10, 10, 0, 10, 10, 0, 
                //Scandanavia
                1, 0, 4, 4, 4, 4, 1, 1, 3, 3, 2, 0, 
                //Britain
                2, 1, 1, 5, 6, 6, 4, 4, 0, 0, 3, 3, 3, 
                //Baltics
                1, 1, 1, 2, 1, 1, 1, 1, 0, 0, 0, 4, 4, 1, 1, 4, 2, 2, 
                //Spain
                1, 1, 2, 0, 0, 0, 2, 1, 3, 0, 4, 4,
                //Navy
                8, 8, 8, 8, 8, 8, 8, 8,
                7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 7, 7, 7, 8,
            };
                startingArmyCountries = new string[] {
                "Germany", "Germany", "Germany", "Germany", "Germany", "Germany", "Germany", "Germany",
                "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain",
            };
            } else if (year == 1944) {
                cityLevels = new int[] {
                //cities with names
                5, 3, 3, 3, 2, 3, 2, 4, 3, 2,
                1, 1, 2, 2, 5, 4, 4, 4, 3, 4,
                5, 3, 3, 4, 3, 3, 3, 4, 3, 2,
                4, 5, 3, 3, 2, 4, 3, 5, 1, 2,
                3, 3, 4, 4, 3, 5, 5, 5, 4, 2,
                2, 2, 2, 

                //cities without names
                1, 2, 1, 2, 1, 1, 1,
                1, 1, 2, 2, 2, 2, 1, 1, 2, 1, 1, 

                //ports
                2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 2
            };
                cityIndustrials = new int[] {
                //cities with names
                4, 3, 3, 3, 2, 1, 1, 2, 0, 0,
                0, 0, 0, 0, 3, 3, 2, 2, 1, 1,
                2, 1, 2, 3, 3, 3, 3, 3, 1, 0,
                3, 4, 3, 3, 2, 1, 1, 3, 0, 0,
                3, 0, 2, 1, 0, 2, 2, 2, 1, 1,
                1, 1, 1,

                //cities without names
                0, 0, 0, 0, 1, 0, 0,
                0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0,

                //ports
                3, 3, 2, 2, 1, 1, 2, 1, 2, 1, 1, 2
            };
                startingArmyLocations = new Vector2[] {
                ///*
                //Germany
                new Vector2(-30f, -2f),new Vector2(-31f, 2.5f), new Vector2(-33f, 3.5f), new Vector2(-30f, 0f), new Vector2(-31f, -3.5f), new Vector2(-30f, -1f), new Vector2(-33f, -5.5f), new Vector2(-35f, -7.5f), new Vector2(-36f, -10f), new Vector2(-33f, 0.5f), new Vector2(-37f, 0.5f), new Vector2(-36f, -2f), new Vector2(-38f, -5f), new Vector2(-68f, -10f), new Vector2(-65f, -9.5f), new Vector2(-63f, -8.5f), new Vector2(-61f, -7.5f), new Vector2(-62f, -11f), new Vector2(-68f, -11f), new Vector2(-59f, -17.5f), new Vector2(-61f, -18.5f), new Vector2(-56f, -17f), new Vector2(-38f, 0f), new Vector2(-34f, -1f), new Vector2(-33f, -2.5f), new Vector2(-34f, -4f), new Vector2(-34f, 2f), new Vector2(-35f, 3.5f), new Vector2(-35f, 0.5f), new Vector2(-32f, 1f), new Vector2(-33f, -1.5f), new Vector2(-35f, -5.5f), new Vector2(-48f, -6f), new Vector2(-55f, -7.5f), new Vector2(-61f, -10.5f), 
                new Vector2(-67f, -10.5f), new Vector2(-59f, -6.5f), new Vector2(-57f, -4.5f), new Vector2(-54f, -5f), new Vector2(-38f, -26f), new Vector2(-39f, -22.5f), new Vector2(-41f, -19.5f), new Vector2(-43f, -16.5f), new Vector2(-64f, -17f), new Vector2(-60f, -15f), new Vector2(-38f, -6f), 
                //Italy
                new Vector2(-41f, 7.5f), new Vector2(-55f, -14.5f), new Vector2(-55f, -15.5f), new Vector2(-53f, -14.5f), new Vector2(-51f, -17.5f), new Vector2(-54f, -19f), new Vector2(-47f, -21.5f), new Vector2(-49f, -18.5f), new Vector2(-50f, -20f), new Vector2(-45f, -23.5f), new Vector2(-46f, -20f), new Vector2(-50f, -14f), new Vector2(-48f, -26f), new Vector2(-52f, -16f), new Vector2(-49f, -19.5f), new Vector2(-54f, -22f), 
                //Russia
                new Vector2(-33f, -6.5f), new Vector2(-32f, -6f), new Vector2(-32f, -5f), new Vector2(-31f, -4.5f), new Vector2(-30f, -4f), new Vector2(-30f, -3f), new Vector2(-29f, -2.5f), new Vector2(-29f, -1.5f), new Vector2(-29f, 1.5f), new Vector2(-30f, 2f), new Vector2(-30f, 3f), new Vector2(-31f, 3.5f), new Vector2(-32f, 4f), new Vector2(-28f, 4f), new Vector2(-30f, 5f), new Vector2(-27f, 1.5f), new Vector2(-27f, -0.5f), new Vector2(-28f, -4f), new Vector2(-30f, -6f), new Vector2(-32f, -8f), new Vector2(-28f, 1f), new Vector2(-29f, -3.5f), new Vector2(-34f, -8f), new Vector2(-33f, -7.5f), new Vector2(-30f, -8f), new Vector2(-28f, -6f),
                new Vector2(-27f, -2.5f), new Vector2(-26f, 3f), new Vector2(-25f, 0.5f), new Vector2(-25f, -2.5f), new Vector2(-26f, -5f), new Vector2(-29f, -9.5f), new Vector2(-32f, -10f), new Vector2(-31f, -11.5f), new Vector2(-29f, -12.5f), new Vector2(-31f, 9.5f), new Vector2(-33f, 12.5f), new Vector2(-34f, 16f), new Vector2(-32f, 6f), new Vector2(-31f, 11.5f), new Vector2(-31f, -5.5f), new Vector2(-30f, -5f), new Vector2(-32f, -7f), new Vector2(-31f, -6.5f), new Vector2(-29f, -4.5f), new Vector2(-33f, -9.5f), new Vector2(-31f, -10.5f), new Vector2(-30f, -12f), new Vector2(-30f, -13f), new Vector2(-30f, -11f), new Vector2(-29f, 0.5f), new Vector2(-28f, 0f), new Vector2(-28f, -1f), new Vector2(-31f, 4.5f), new Vector2(-32f, 5f), new Vector2(-29f, 2.5f), new Vector2(-28f, 2f), new Vector2(-28f, -3f), new Vector2(-29f, 4.5f), new Vector2(-29f, 3.5f), new Vector2(-34f, -7f), new Vector2(-34f, -9f), new Vector2(-32f, -9f), 
                //Scandanavia
                new Vector2(-46f, 4f), new Vector2(-43f, 13.5f), new Vector2(-40f, 7f), new Vector2(-37f, 14.5f), new Vector2(-37f, 9.5f), new Vector2(-36f, 8f), new Vector2(-34f, 8f), new Vector2(-36f, 12f), new Vector2(-35f, 9.5f), new Vector2(-36f, 13f), new Vector2(-38f, 8f), new Vector2(-38f, 10f), 
                //Britain
                new Vector2(-73f, -1.5f), new Vector2(-70f, 3f), new Vector2(-69f, -1.5f), new Vector2(-66f, -3f), new Vector2(-68f, -3f), new Vector2(-67f, -0.5f), new Vector2(-69f, 1.5f), new Vector2(-75f, -2.5f), new Vector2(-72f, 4f), new Vector2(-76f, -4f), new Vector2(-70f, -4f), new Vector2(-65f, -4.5f), new Vector2(-66f, -1f), 
                //Balkans
                new Vector2(-42f, -12f), new Vector2(-41f, -11.5f), new Vector2(-41f, -12.5f), new Vector2(-44f, -12f), new Vector2(-37f, -14.5f), new Vector2(-34f, -12f), new Vector2(-32f, -13f), new Vector2(-34f, -10f), new Vector2(-33f, -11.5f), new Vector2(-32f, -12f), new Vector2(-31f, -12.5f), new Vector2(-31f, -13.5f), new Vector2(-33f, -12.5f), new Vector2(-33f, -15.5f), new Vector2(-37f, -18.5f), new Vector2(-34f, -19f), new Vector2(-33f, -18.5f), new Vector2(-32f, -20f), 
                //Spain
                new Vector2(-62f, -20f), new Vector2(-68f, -21f), new Vector2(-74f, -19f), new Vector2(-75f, -24.5f), new Vector2(-70f, -26f), new Vector2(-61f, -22.5f), new Vector2(-65f, -19.5f), new Vector2(-67f, -18.5f), new Vector2(-63f, -19.5f), new Vector2(-69f, -23.5f), new Vector2(-72f, -21f), new Vector2(-65f, -21.5f),
                //USA
                new Vector2(-66f, -9f), new Vector2(-65f, -8.5f), new Vector2(-64f, -8f), new Vector2(-63f, -7.5f), new Vector2(-63f, -6.5f), new Vector2(-64f, -7f), new Vector2(-65f, -7.5f), new Vector2(-66f, -8f), new Vector2(-67f, -8.5f), new Vector2(-67f, -7.5f), new Vector2(-66f, -7f), new Vector2(-65f, -6.5f), new Vector2(-67f, -9.5f), new Vector2(-60f, -19f), new Vector2(-59f, -18.5f), new Vector2(-58f, -19f), new Vector2(-57f, -19.5f), new Vector2(-56f, -19f), new Vector2(-59f, -19.5f), new Vector2(-58f, -20f), new Vector2(-57f, -20.5f), new Vector2(-56f, -20f), new Vector2(-57f, -18.5f), new Vector2(-56f, -18f),
                new Vector2(-62f, -6f), new Vector2(-61f, -5.5f), new Vector2(-62f, -5f), new Vector2(-63f, -5.5f), new Vector2(-64f, -6f), new Vector2(-68f, -9f), new Vector2(-69f, -9.5f), new Vector2(-69f, -8.5f), new Vector2(-68f, -8f), new Vector2(-60f, -20f), new Vector2(-55f, -17.5f), new Vector2(-55f, -18.5f), new Vector2(-61f, -20.5f), 
                //Britain ships
                new Vector2(-67f, -5.5f), new Vector2(-68f, -6f), new Vector2(-63f, -2.5f), new Vector2(-63f, -0.5f), new Vector2(-62f, -3f), new Vector2(-63f, -3.5f), new Vector2(-74f, -5f), new Vector2(-52f, -21f), new Vector2(-51f, -21.5f), 
                //German ships
                new Vector2(-59f, -4.5f), new Vector2(-58f, -4f), new Vector2(-57f, -3.5f), new Vector2(-56f, -3f), new Vector2(-68f, -12f), new Vector2(-67f, -12.5f), new Vector2(-69f, -11.5f), 
                //*/
            };
                startingArmyTypes = new int[] {
                //Germanya
                1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 5, 1, 1, 1, 1, 2, 2, 2, 1, 2, 4, 0, 1, 2, 0, 10, 0, 10, 0, 10, 0, 5, 
                //Italy
                0, 1, 2, 5, 0, 2, 6, 4, 4, 3, 3, 1, 1, 2, 0, 1,
                //Russia
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 5, 5, 4, 1, 1, 1, 1, 4, 4, 4, 4, 4, 2, 2, 2, 1, 3, 3, 3, 0,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 4, 1, 0, 0, 
                //Scandanavia
                1, 0, 4, 4, 4, 4, 1, 1, 3, 3, 2, 0, 
                //Britain
                2, 5, 5, 5, 5, 5, 4, 4, 4, 4, 3, 3, 3, 
                //Baltics
                1, 1, 1, 2, 1, 1, 1, 1, 0, 0, 0, 4, 4, 1, 1, 4, 2, 2, 
                //Spain
                1, 1, 2, 0, 10, 0, 2, 1, 3, 0, 4, 4,
                //USA
                1, 1, 1, 1, 7, 2, 2, 2, 5, 5,
                5, 5, 5, 1, 1, 1, 1, 2, 2, 2,
                4, 4, 7, 7, 7, 7, 7, 7, 7, 7,
                7, 7, 7, 7, 7, 7, 7,
                //Britain ships
                7, 7, 7, 7, 7, 7, 7, 7, 7,
                //German shipsm
                8, 8, 8, 8, 8, 8, 8,
            };
                startingArmyCountries = new string[] {
                "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA",  "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA",
                "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA",  "USA", "USA", "USA", "USA", "USA", "USA", "USA", "Britain", "Britain", "Britain",
                "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Germany", "Germany", "Germany", "Germany", "Germany", "Germany", "Germany",
            };
            } else if (year == 1945) {
                cityLevels = new int[] {
                    //cities with names
                    5, 4, 4, 4, 3, 4, 2, 5, 4, 3,
                    2, 2, 3, 3, 5, 4, 4, 4, 3, 5,
                    5, 4, 3, 4, 3, 3, 3, 4, 3, 2,
                    3, 5, 2, 2, 2, 5, 4, 4, 1, 1,
                    2, 3, 3, 5, 4, 3, 3, 5, 4, 2,
                    2, 2, 3, 

                    //cities without names
                    1, 2, 1, 2, 1, 1, 1,
                    1, 1, 2, 2, 2, 2, 1, 1, 2, 1, 1, 

                    //ports
                    2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 2
                };
                cityIndustrials = new int[] {
                    //cities with names
                    4, 4, 4, 4, 3, 3, 2, 3, 2, 1,
                    0, 1, 1, 2, 3, 3, 2, 2, 2, 2,
                    2, 2, 3, 4, 3, 3, 3, 4, 2, 1,
                    3, 4, 2, 2, 2, 2, 3, 2, 0, 0,
                    2, 1, 1, 2, 3, 1, 0, 2, 2, 2,
                    1, 1, 1,

                    //cities without names
                    0, 0, 0, 0, 1, 0, 0,
                    0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0,

                    //ports
                    3, 3, 2, 2, 1, 1, 2, 1, 2, 1, 1, 2
                };
                startingArmyLocations = new Vector2[] {
                    ///*
                    //Germany
                    new Vector2(-32f, 10f), new Vector2(-54f, -5f), new Vector2(-48f, -6f), new Vector2(-55f, -7.5f), new Vector2(-45f, -9.5f), new Vector2(-45f, -7.5f), new Vector2(-44f, -6f), new Vector2(-46f, -4f), new Vector2(-43f, -8.5f), new Vector2(-42f, -6f), new Vector2(-41f, -4.5f), new Vector2(-44f, -3f), new Vector2(-43f, -7.5f), new Vector2(-45f, -5.5f), new Vector2(-41f, -8.5f), new Vector2(-40f, -7f), new Vector2(-41f, -5.5f), new Vector2(-41f, -6.5f), new Vector2(-40f, -5f), new Vector2(-41f, -3.5f), new Vector2(-40f, -3f), new Vector2(-40f, -4f), new Vector2(-40f, -6f), new Vector2(-40f, -9f), new Vector2(-41f, -9.5f), new Vector2(-43f, -10.5f), new Vector2(-45f, -10.5f), new Vector2(-46f, -11f), new Vector2(-47f, -12.5f), new Vector2(-48f, -13f), new Vector2(-57f, -6.5f), new Vector2(-57f, -7.5f), new Vector2(-56f, -8f), new Vector2(-56f, -9f), new Vector2(-55f, -9.5f), new Vector2(-55f, -10.5f), new Vector2(-57f, -5.5f), new Vector2(-58f, -6f),
                    new Vector2(-58f, -5f), new Vector2(-56f, -6f), new Vector2(-56f, -5f), new Vector2(-57f, -4.5f), new Vector2(-56f, -7f), new Vector2(-53f, -8.5f), new Vector2(-53f, -9.5f), new Vector2(-51f, -8.5f), new Vector2(-50f, -9f), new Vector2(-49f, -7.5f), new Vector2(-51f, -7.5f), new Vector2(-48f, -12f), new Vector2(-52f, -11f), new Vector2(-52f, -1f), new Vector2(-54f, -2f), new Vector2(-54f, -3f), new Vector2(-52f, -4f), new Vector2(-50f, -3f), new Vector2(-50f, -5f), new Vector2(-56f, 4f), new Vector2(-55f, 5.5f), new Vector2(-54f, 7f), new Vector2(-56f, 7f), new Vector2(-52f, 6f), new Vector2(-39f, 17.5f), 
                    //Italy
                    new Vector2(-53f, -14.5f), new Vector2(-54f, -15f), new Vector2(-52f, -15f), new Vector2(-53f, -15.5f), new Vector2(-51f, -14.5f), new Vector2(-55f, -15.5f), new Vector2(-55f, -14.5f), new Vector2(-54f, -14f), new Vector2(-53f, -13.5f), new Vector2(-52f, -14f), new Vector2(-51f, -13.5f), new Vector2(-50f, -14f), new Vector2(-55f, -13.5f), new Vector2(-54f, -16f), new Vector2(-51f, -15.5f), 
                    //Russia
                    new Vector2(-39f, -3.5f), new Vector2(-39f, -4.5f), new Vector2(-39f, -5.5f), new Vector2(-39f, -6.5f), new Vector2(-38f, -6f), new Vector2(-38f, -5f), new Vector2(-38f, -4f), new Vector2(-38f, -3f), new Vector2(-39f, -2.5f), new Vector2(-38f, -7f), new Vector2(-39f, -7.5f), new Vector2(-39f, -8.5f), new Vector2(-39f, -9.5f), new Vector2(-39f, -10.5f), new Vector2(-40f, -11f), new Vector2(-48f, -14f), new Vector2(-47f, -13.5f), new Vector2(-46f, -13f), new Vector2(-45f, -12.5f), new Vector2(-45f, -11.5f), new Vector2(-44f, -11f), new Vector2(-44f, -12f), new Vector2(-43f, -11.5f), new Vector2(-47f, -14.5f), new Vector2(-46f, -14f), new Vector2(-45f, -13.5f), new Vector2(-48f, -15f), new Vector2(-47f, -15.5f), new Vector2(-46f, -15f), new Vector2(-42f, -11f), new Vector2(-41f, -10.5f), new Vector2(-41f, -11.5f), new Vector2(-45f, -14.5f), new Vector2(-43f, -16.5f), new Vector2(-37f, -14.5f), new Vector2(-37f, -18.5f), new Vector2(-41f, -19.5f), 
                    new Vector2(-40f, -17f), new Vector2(-35f, -17.5f), new Vector2(-34f, -19f), new Vector2(-39f, -20.5f), new Vector2(-37f, -20.5f), new Vector2(-37f, -9.5f), new Vector2(-30f, -2f), new Vector2(-38f, 0f), new Vector2(-39f, -0.5f), new Vector2(-29f, 4.5f), new Vector2(-22f, 1f),
                    new Vector2(-14f, 5f), new Vector2(-29f, -8.5f), new Vector2(-34f, -12f), new Vector2(-33f, -15.5f), new Vector2(-39f, -11.5f), new Vector2(-40f, -12f), new Vector2(-44f, -13f), new Vector2(-43f, -12.5f), new Vector2(-42f, -12f), 
                    //Britain
                    new Vector2(-66f, -3f), new Vector2(-69f, -1.5f), new Vector2(-69f, -3.5f), new Vector2(-73f, -1.5f), new Vector2(-67f, -2.5f), new Vector2(-67f, -3.5f), new Vector2(-69f, -4.5f), new Vector2(-70f, 3f), new Vector2(-76f, -2f), new Vector2(-75f, -2.5f), new Vector2(-68f, 0f), new Vector2(-69f, 0.5f), new Vector2(-38f, -26f), new Vector2(-39f, -23.5f), new Vector2(-39f, -22.5f), new Vector2(-38f, -22f), new Vector2(-40f, -23f), new Vector2(-40f, -22f), new Vector2(-37f, -21.5f), new Vector2(-35f, -20.5f), new Vector2(-40f, -25f), new Vector2(-39f, -25.5f), new Vector2(-38f, -25f), 
                    //Neutrals
                    new Vector2(-40f, 7f), new Vector2(-37f, 8.5f), new Vector2(-36f, 11f), new Vector2(-36f, 10f), new Vector2(-37f, 13.5f), new Vector2(-37f, 14.5f), new Vector2(-35f, 7.5f), new Vector2(-34f, 8f), new Vector2(-34f, 7f), new Vector2(-55f, -12.5f), new Vector2(-68f, -21f), new Vector2(-62f, -20f), new Vector2(-74f, -19f), new Vector2(-70f, -26f), new Vector2(-67f, -19.5f), new Vector2(-65f, -20.5f), new Vector2(-66f, -18f), new Vector2(-64f, -19f), new Vector2(-62f, -19f), new Vector2(-61f, -22.5f), new Vector2(-31f, -16.5f), new Vector2(-46f, 4f), new Vector2(-43f, 13.5f), new Vector2(-49f, 5.5f), new Vector2(-48f, 8f), new Vector2(-50f, 4f), new Vector2(-47f, 10.5f), new Vector2(-46f, 12f), new Vector2(-47f, 8.5f), 
                    //USA
                    new Vector2(-59f, -6.5f), new Vector2(-58f, -7f), new Vector2(-58f, -8f), new Vector2(-58f, -9f), new Vector2(-57f, -8.5f), new Vector2(-59f, -7.5f), new Vector2(-60f, -7f), new Vector2(-60f, -8f), new Vector2(-59f, -8.5f), new Vector2(-57f, -9.5f), new Vector2(-61f, -7.5f), new Vector2(-59f, -10.5f), new Vector2(-61f, -10.5f), new Vector2(-62f, -10f), new Vector2(-57f, -11.5f), new Vector2(-58f, -12f), new Vector2(-58f, -13f), new Vector2(-60f, -15f), new Vector2(-62f, -13f), new Vector2(-65f, -13.5f), new Vector2(-67f, -10.5f), new Vector2(-64f, -17f), new Vector2(-64f, -15f), new Vector2(-62f, -14f), new Vector2(-58f, -17f), new Vector2(-51f, -17.5f), new Vector2(-51f, -18.5f), new Vector2(-50f, -18f), new Vector2(-47f, -21.5f), new Vector2(-54f, -19f), new Vector2(-54f, -22f), new Vector2(-44f, -22f), new Vector2(-45f, -23.5f), new Vector2(-64f, -12f), 
                    //USA ships
                    new Vector2(-60f, -5f), new Vector2(-59f, -3.5f), new Vector2(-57f, -2.5f), new Vector2(-57f, -0.5f), new Vector2(-57f, -1.5f), new Vector2(-58f, -3f), new Vector2(-58f, -2f), new Vector2(-55f, -18.5f), new Vector2(-56f, -18f), new Vector2(-55f, -17.5f), new Vector2(-54f, -17f), new Vector2(-54f, -18f), new Vector2(-42f, -25f), new Vector2(-43f, -24.5f), new Vector2(-43f, -25.5f), new Vector2(-42f, -26f), new Vector2(-43f, -26.5f), new Vector2(-42f, -27f), 
                    //Britain ships
                    new Vector2(-36f, -23f), new Vector2(-35f, -22.5f), new Vector2(-35f, -23.5f), new Vector2(-36f, -24f), new Vector2(-34f, -23f), new Vector2(-34f, -24f), new Vector2(-31f, -22.5f), new Vector2(-30f, -22f), new Vector2(-61f, -3.5f), new Vector2(-61f, -1.5f), new Vector2(-60f, 0f), new Vector2(-60f, -1f), new Vector2(-63f, -2.5f), new Vector2(-63f, 0.5f), new Vector2(-63f, -4.5f), new Vector2(-64f, -6f), new Vector2(-66f, -7f), new Vector2(-68f, -7f), new Vector2(-70f, -8f), new Vector2(-71f, -3.5f), new Vector2(-75f, -4.5f), new Vector2(-65f, -1.5f), 
                    //German ships
                    new Vector2(-55f, -2.5f), new Vector2(-56f, -3f), new Vector2(-57f, -3.5f), new Vector2(-58f, -4f), new Vector2(-55f, -1.5f), new Vector2(-55f, -0.5f), 
                    //Soviet ships
                    new Vector2(-27f, -15.5f), new Vector2(-28f, -15f), new Vector2(-27f, -16.5f), new Vector2(-26f, -17f), new Vector2(-28f, -16f), new Vector2(-27f, -17.5f), new Vector2(-26f, -18f), new Vector2(-25f, -16.5f), new Vector2(-25f, -17.5f), 
                    //*/
                };
                startingArmyTypes = new int[] {
                    ///*
                    //Germanya
                    9, 9, 9, 6, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 5, 5, 5, 5, 5, 1, 1, 4, 4, 4, 4, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0,
                    //Italy
                    5, 5, 5, 2, 2, 2, 0, 0, 0, 4, 4, 4, 4, 3, 3, 
                    //Russia
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 2, 2, 2, 2, 2, 2, 2, 9, 1, 0, 0, 0, 6, 6, 1, 3, 3, 1, 2, 2, 2, 5, 1, 1, 1, 0, 0, 1, 1, 1, 1, 2, 
                    //Britain
                    5, 5, 5, 5, 2, 2, 2, 1, 1, 1, 1, 1, 5, 2, 2, 1, 1, 3, 3, 3, 2, 5, 5, 
                    //Neutrals
                    1, 1, 10, 10, 10, 0, 10, 10, 0, 5, 1, 2, 2, 1, 1, 1, 3, 3, 3, 2, 1, 1, 0, 0, 2, 2, 2, 1, 1,
                    //USA
                    5, 5, 5, 5, 5, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 1, 1, 9, 1, 0, 6, 4, 4, 2, 5, 2, 2, 1, 2, 2, 1, 1, 6, 
                    //USA Ships
                    7, 7, 7, 7, 8, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 
                    //Britain ships
                    7, 7, 7, 7, 8, 8, 8, 8, 7, 7, 7, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 
                    //German ships
                    8, 8, 8, 8, 8, 8, 
                    //Soviet ships
                    7, 7, 8, 8, 7, 8, 8, 8, 8, 
                    //*/
                };
                startingArmyCountries = new string[] {
                    "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA",  "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA", "USA",
                    "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain",  "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "Britain",
                    "Germany", "Germany", "Germany", "Germany", "Germany", "Germany",
                    "Russia", "Russia", "Russia", "Russia", "Russia", "Russia", "Russia", "Russia", "Russia",
                };
            }
            cities = new City[cityNames.Length];
        } else {
            //here
            cityLocations = new Vector2[] {
                new Vector2(-29f, 6.5f), new Vector2(-33f, 7.5f), new Vector2(-38f, 6f), new Vector2(-44f, 7f), new Vector2(-36f, 17f), new Vector2(-30f, 18f), new Vector2(-29f, 20.5f), new Vector2(-36f, -4f), new Vector2(-51f, 13.5f), new Vector2(-59f, 6.5f), 
                new Vector2(-63f, 16.5f), new Vector2(-45f, 22.5f), new Vector2(-57f, 23.5f), new Vector2(-27f, 15.5f), new Vector2(-25f, 12.5f), new Vector2(-26f, -1f), new Vector2(-20f, 8f), new Vector2(-17f, 10.5f), new Vector2(-14f, 11f), new Vector2(-16f, 18f), 
                new Vector2(-28f, 27f), new Vector2(-38f, 37f), new Vector2(-48f, 30f), new Vector2(-71f, 25.5f), new Vector2(-73f, 19.5f), new Vector2(-68f, 4f), new Vector2(-69f, -6.5f), new Vector2(-56f, -2f), new Vector2(-49f, -7.5f), new Vector2(-44f, -11f), 
                new Vector2(-40f, -12f), new Vector2(-42f, -4f), new Vector2(-43f, -19.5f), new Vector2(-41f, -22.5f), new Vector2(-39f, -31.5f), new Vector2(-4f, -33f), new Vector2(-23f, -14.5f), new Vector2(-23f, -42.5f), new Vector2(-2f, -44f), new Vector2(-45f, -1.5f), 
                new Vector2(-39f, 1.5f), new Vector2(-35f, 2.5f), new Vector2(-34f, 12f), new Vector2(2f, -57f),

                new Vector2(-31f, 0.5f), new Vector2(-34f, 24f), new Vector2(-40f, 15f), new Vector2(-44f, 14f), new Vector2(-49f, 5.5f), new Vector2(-74f, 7f), new Vector2(-65f, -1.5f), new Vector2(-52f, 3f), new Vector2(-49f, 1.5f), new Vector2(-45f, -23.5f), 
                new Vector2(-30f, -25f), new Vector2(-23f, -26.5f), new Vector2(-11f, -29.5f), new Vector2(-36f, -6f), new Vector2(-70f, 39f), new Vector2(-11f, 29.5f), new Vector2(-14f, 13f), new Vector2(-20f, 10f), new Vector2(-14f, -38f), new Vector2(-17f, -47.5f), 
                new Vector2(-7f, -50.5f), new Vector2(3f, -49.5f), new Vector2(-30f, -47f), new Vector2(-62f, -18f), new Vector2(-65f, -14.5f), new Vector2(-59f, 33.5f), new Vector2(-19f, 39.5f), new Vector2(-25f, -8.5f), new Vector2(-21f, -16.5f),

                //ports
                new Vector2(-35f, -4.5f), new Vector2(-28f, 7f), new Vector2(-14f, 10f), new Vector2(-21f, 9.5f), new Vector2(-15f, 16.5f), new Vector2(-26f, 12f), new Vector2(-27f, -2.5f), new Vector2(-63f, -14.5f), new Vector2(-68f, -13f), new Vector2(-51f, -6.5f), 
                new Vector2(-46f, -14f), new Vector2(-40f, -22f), new Vector2(-43f, -23.5f), new Vector2(-30f, -20f), new Vector2(-23f, -27.5f), new Vector2(-24f, -15f), new Vector2(-21f, -14.5f), new Vector2(-4f, -35f), new Vector2(-11f, -28.5f), new Vector2(-32f, -32f), 
                new Vector2(-21f, -40.5f), new Vector2(0f, -44f), new Vector2(4f, -57f), new Vector2(-13f, 29.5f), new Vector2(-28f, 32f), new Vector2(-21f, 21.5f), new Vector2(-16f, 20f),
            };
            print(cityLocations.Length);
            if (PlayerPrefs.GetString("language") == "Chinese") {
                cityNames = new string[] {
                    "上海", "南京", "武汉", "重庆", "北平", "沈阳", "长春", "香港", "兰州", "拉萨",
                    "乌鲁木齐", "乌兰巴托", "乌里雅苏台城", "平壤", "首尔", "台北", "长崎", "横滨", "东京", "札幌",
                    "哈巴罗夫斯克", "雅库茨克", "赤塔", "新西伯利亚", "阿斯塔纳", "新德里", "孟买", "加尔各答", "仰光", "曼谷",
                    "金边", "河内", "吉隆坡", "新加坡", "雅加达", "魔尔斯贝港", "曼尼拉", "达尔文", "布里斯班", "昆明",
                    "长沙", "南昌", "济南", "悉尼",

                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "", "",

                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "",
                };
            } else {
                cityNames = new string[] {
                    "Shanghai", "Nanking", "Wuhan", "Chongking", "Peking", "Shenyang", "Changchun", "Hong Kong", "Lanzhou", "Lhasa",
                    "Urumqi", "Ulan Bator", "Uliastai", "Pyongyang", "Seoul", "Taipei", "Nagasaki", "Yokohama", "Tokyo", "Sapporo",
                    "Khabarovsk", "Yakutsk", "Chita", "Novosibirsk", "Astana", "New Delhi", "Mumbai", "Kolkota", "Yangon", "Bangkok",
                    "Phnom Penh", "Hanoi", "Kuala Lumpur", "Singapore", "Jakarta", "Port Moresby", "Manila", "Darwin", "Brisbane", "Kunming",
                    "Changsha", "Nanchang", "Jinan", "Sydney",

                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "", "",

                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "",
                };
            }
            terrainLocations = new Vector2[] {
                new Vector2(-47f, 1.5f), new Vector2(-46f, 1f), new Vector2(-45f, 1.5f), new Vector2(-44f, 1f), new Vector2(-45f, 0.5f), new Vector2(-44f, 0f), new Vector2(-43f, -0.5f), new Vector2(-46f, 2f), new Vector2(-47f, 2.5f), new Vector2(-35f, -1.5f), new Vector2(-34f, -1f), 
                new Vector2(-37f, 0.5f), new Vector2(-30f, 5f), new Vector2(-31f, 4.5f), new Vector2(-42f, 4f), new Vector2(-42f, 17f), new Vector2(-43f, 17.5f), new Vector2(-43f, 16.5f), new Vector2(-41f, 17.5f), new Vector2(-41f, 16.5f), new Vector2(-42f, 18f), new Vector2(-44f, 17f), 
                new Vector2(-54f, 16f), new Vector2(-54f, 15f), new Vector2(-55f, 15.5f), new Vector2(-55f, 14.5f), new Vector2(-62f, 18f), new Vector2(-61f, 18.5f), new Vector2(-65f, 17.5f), new Vector2(-66f, 17f), new Vector2(-70f, 11f), new Vector2(-69f, 10.5f), new Vector2(-68f, 10f), 
                new Vector2(-68f, 9f), new Vector2(-69f, 9.5f), new Vector2(-67f, 8.5f), new Vector2(-68f, 8f), new Vector2(-67f, 7.5f), new Vector2(-66f, 8f), new Vector2(-66f, 7f), new Vector2(-65f, 7.5f), new Vector2(-65f, 6.5f), new Vector2(-64f, 7f), new Vector2(-70f, 10f), new Vector2(-70f, 9f),
                new Vector2(-69f, 8.5f), new Vector2(-68f, 7f), new Vector2(-67f, 6.5f), new Vector2(-66f, 6f), new Vector2(-65f, 5.5f), new Vector2(-64f, 6f), new Vector2(-63f, 5.5f), new Vector2(-62f, 5f), new Vector2(-63f, 6.5f), new Vector2(-62f, 6f), new Vector2(-64f, 5f), new Vector2(-63f, 4.5f), 
                new Vector2(-50f, 9f), new Vector2(-49f, 9.5f), new Vector2(-54f, 10f), new Vector2(-46f, 11f), new Vector2(-76f, 6f), new Vector2(-75f, 5.5f), new Vector2(-75f, 4.5f), new Vector2(-74f, 4f), new Vector2(-74f, 3f), new Vector2(-73f, 2.5f), new Vector2(-74f, 2f), new Vector2(-73f, 1.5f), 
                new Vector2(-72f, 1f), new Vector2(-75f, 3.5f), new Vector2(-72f, 0f), new Vector2(-71f, -0.5f), new Vector2(-67f, -5.5f), new Vector2(-68f, -5f), new Vector2(-68f, -4f), new Vector2(-67f, -6.5f), new Vector2(-66f, -7f), new Vector2(-66f, -8f), new Vector2(-66f, -9f), new Vector2(-25f, 26.5f), new Vector2(-25f, 25.5f), new Vector2(-24f, 25f), new Vector2(-24f, 24f), new Vector2(-36f, 29f), new Vector2(-35f, 29.5f), new Vector2(-36f, 28f), new Vector2(-39f, 30.5f), new Vector2(-40f, 30f), new Vector2(-41f, 29.5f), new Vector2(-41f, 28.5f), new Vector2(-40f, 29f), new Vector2(-41f, 27.5f), new Vector2(-42f, 35f), new Vector2(-43f, 35.5f), 
                new Vector2(-50f, 29f), new Vector2(-49f, 28.5f), new Vector2(-48f, 28f), new Vector2(-48f, 27f), new Vector2(-48f, 26f), new Vector2(-48f, 25f), new Vector2(-47f, 24.5f), new Vector2(-47f, 23.5f), new Vector2(-48f, 24f), new Vector2(-47f, 25.5f), new Vector2(-60f, 31f), new Vector2(-60f, 30f), new Vector2(-59f, 29.5f), new Vector2(-59f, 28.5f), new Vector2(-58f, 37f), new Vector2(-59f, 37.5f), new Vector2(-60f, 38f), new Vector2(-61f, 38.5f), new Vector2(-62f, 38f), new Vector2(-61f, 37.5f), new Vector2(-63f, 38.5f), new Vector2(-62f, 39f), new Vector2(-63f, 39.5f), new Vector2(-64f, 43f), new Vector2(-63f, 42.5f), new Vector2(-62f, 42f), 
                new Vector2(-63f, 41.5f), new Vector2(-62f, 41f), new Vector2(-62f, 40f), new Vector2(-63f, 40.5f), new Vector2(-67f, 25.5f), new Vector2(-66f, 26f), new Vector2(-65f, 26.5f), new Vector2(-64f, 27f), new Vector2(-63f, 27.5f), new Vector2(-64f, 28f), new Vector2(-71f, 14.5f), new Vector2(-70f, 15f), new Vector2(-71f, 15.5f), new Vector2(-62f, -4f), new Vector2(-61f, -3.5f), new Vector2(-50f, -4f), new Vector2(-49f, -3.5f), new Vector2(-43f, -11.5f), new Vector2(-42f, -11f), new Vector2(-40f, -9f), new Vector2(-39f, -9.5f), new Vector2(-38f, -14f), new Vector2(-37f, -13.5f), new Vector2(-43f, -27.5f), new Vector2(-43f, -26.5f), new Vector2(-31f, -23.5f), 
                new Vector2(-30f, -23f), new Vector2(-30f, -24f), new Vector2(-24f, -12f), new Vector2(-24f, -13f), new Vector2(-23f, -12.5f), new Vector2(-27f, -1.5f), new Vector2(-16f, 11f), new Vector2(-15f, 11.5f), new Vector2(-15f, 18.5f), new Vector2(-37f, 23.5f), new Vector2(-36f, 24f), new Vector2(-28f, 19f), new Vector2(-27f, 19.5f), new Vector2(-37f, 16.5f), new Vector2(-38f, 16f), new Vector2(-20f, -43f), new Vector2(-19f, -42.5f), new Vector2(-19f, -43.5f), new Vector2(-19f, -44.5f), new Vector2(-27f, -48.5f), new Vector2(-26f, -48f), new Vector2(-25f, -48.5f), new Vector2(-21f, -50.5f), new Vector2(-20f, -50f), new Vector2(-15f, -49.5f), new Vector2(-14f, -50f), 
                new Vector2(-5f, -51.5f), new Vector2(-4f, -52f), new Vector2(-7f, -57.5f), new Vector2(-6f, -58f), new Vector2(0f, -56f), new Vector2(1f, -55.5f), new Vector2(-1f, -52.5f), new Vector2(1f, -51.5f), new Vector2(-3f, -45.5f), new Vector2(-3f, -46.5f), new Vector2(-2f, -47f), new Vector2(-10f, -46f), new Vector2(-11f, -45.5f), new Vector2(-10f, -31f), new Vector2(-9f, -30.5f), new Vector2(-8f, -30f), new Vector2(-8f, -31f), new Vector2(-7f, -31.5f), new Vector2(-7f, -30.5f), new Vector2(-7f, -29.5f), new Vector2(-6f, -32f), new Vector2(-6f, -31f), new Vector2(-5f, -31.5f), new Vector2(-31f, -24.5f), new Vector2(-32f, -25f), new Vector2(-32f, -24f), new Vector2(-33f, -25.5f), 
                new Vector2(-32f, -26f), new Vector2(-31f, -25.5f), new Vector2(-25f, -26.5f), new Vector2(-24f, -27f),

                new Vector2(-41f, 1.5f), new Vector2(-42f, 2f), new Vector2(-41f, 2.5f), new Vector2(-39f, 3.5f), new Vector2(-38f, 4f), new Vector2(-39f, 4.5f), new Vector2(-36f, 4f), new Vector2(-35f, 3.5f), new Vector2(-33f, 2.5f), new Vector2(-32f, 2f), new Vector2(-34f, 1f), new Vector2(-39f, -0.5f), new Vector2(-37f, 1.5f), new Vector2(-40f, 8f), new Vector2(-41f, 8.5f), new Vector2(-44f, 10f), new Vector2(-46f, 8f), new Vector2(-42f, 6f), new Vector2(-36f, -1f), new Vector2(-36f, -2f), new Vector2(-35f, -2.5f), new Vector2(-38f, -3f), new Vector2(-39f, -2.5f), new Vector2(-40f, -3f), new Vector2(-39f, -3.5f), new Vector2(-42f, -6f), new Vector2(-43f, -5.5f), new Vector2(-45f, -4.5f), 
                new Vector2(-44f, -7f), new Vector2(-43f, -7.5f), new Vector2(-44f, -9f), new Vector2(-45f, -8.5f), new Vector2(-41f, -10.5f), new Vector2(-39f, -10.5f), new Vector2(-40f, -8f), new Vector2(-40f, -6f), new Vector2(-41f, -6.5f), new Vector2(-38f, -12f), new Vector2(-39f, -12.5f), new Vector2(-41f, -13.5f), new Vector2(-45f, -12.5f), new Vector2(-45f, -13.5f), new Vector2(-44f, -17f), new Vector2(-45f, -16.5f), new Vector2(-45f, -15.5f), new Vector2(-43f, -18.5f), new Vector2(-42f, -21f), new Vector2(-42f, -20f), new Vector2(-47f, -21.5f), new Vector2(-48f, -20f), new Vector2(-45f, -24.5f), new Vector2(-44f, -24f), new Vector2(-44f, -25f), new Vector2(-41f, -28.5f), new Vector2(-42f, -28f), 
                new Vector2(-42f, -29f), new Vector2(-40f, -29f), new Vector2(-41f, -29.5f), new Vector2(-37f, -32.5f), new Vector2(-40f, -30f), new Vector2(-35f, -26.5f), new Vector2(-33f, -27.5f), new Vector2(-31f, -27.5f), new Vector2(-30f, -27f), new Vector2(-29f, -25.5f), new Vector2(-29f, -23.5f), new Vector2(-29f, -21.5f), new Vector2(-30f, -22f), new Vector2(-33f, -26.5f), new Vector2(-33f, -23.5f), new Vector2(-33f, -32.5f), new Vector2(-25f, -27.5f), new Vector2(-26f, -27f), new Vector2(-24f, -26f), new Vector2(-22f, -16f), new Vector2(-23f, -16.5f), new Vector2(-22f, -15f), new Vector2(-21f, -17.5f), new Vector2(-25f, -10.5f), new Vector2(-25f, -9.5f), new Vector2(-25f, -11.5f), new Vector2(-33f, 5.5f), 
                new Vector2(-31f, 6.5f), new Vector2(-35f, 8.5f), new Vector2(-34f, 9f), new Vector2(-36f, 11f), new Vector2(-36f, 12f), new Vector2(-40f, 11f), new Vector2(-39f, 11.5f), new Vector2(-39f, 10.5f), new Vector2(-41f, 12.5f), new Vector2(-43f, 13.5f), new Vector2(-41f, 14.5f), new Vector2(-38f, 14f), new Vector2(-36f, 15f), new Vector2(-39f, 17.5f), new Vector2(-36f, 20f), new Vector2(-33f, 19.5f), new Vector2(-33f, 18.5f), new Vector2(-34f, 22f), new Vector2(-32f, 22f), new Vector2(-35f, 25.5f), new Vector2(-30f, 21f), new Vector2(-31f, 20.5f), new Vector2(-28f, 18f), new Vector2(-29f, 16.5f), new Vector2(-28f, 17f), new Vector2(-27f, 21.5f), new Vector2(-31f, 18.5f), new Vector2(-38f, 19f), 
                new Vector2(-40f, 19f), new Vector2(-39f, 18.5f), new Vector2(-35f, 17.5f), new Vector2(-46f, 14f), new Vector2(-49f, 15.5f), new Vector2(-48f, 13f), new Vector2(-48f, 12f), new Vector2(-49f, 6.5f), new Vector2(-52f, 8f), new Vector2(-52f, 11f), new Vector2(-53f, 10.5f), new Vector2(-53f, 8.5f), new Vector2(-55f, 6.5f), new Vector2(-57f, 7.5f), new Vector2(-58f, 10f), new Vector2(-58f, 6f), new Vector2(-61f, 8.5f), new Vector2(-62f, 11f), new Vector2(-64f, 10f), new Vector2(-62f, 9f), new Vector2(-60f, 10f), new Vector2(-60f, 12f), new Vector2(-57f, 12.5f), new Vector2(-65f, 13.5f), new Vector2(-64f, 20f), new Vector2(-59f, 17.5f), new Vector2(-46f, 4f), new Vector2(-45f, 3.5f), new Vector2(-44f, 2f), 
                new Vector2(-43f, 1.5f), new Vector2(-44f, -2f), new Vector2(-46f, -2f), new Vector2(-47f, -1.5f), new Vector2(-49f, -2.5f), new Vector2(-51f, -1.5f), new Vector2(-50f, 0f), new Vector2(-50f, 1f), new Vector2(-49f, -0.5f), new Vector2(-53f, 1.5f), new Vector2(-59f, -0.5f), new Vector2(-60f, -1f), new Vector2(-67f, 1.5f), new Vector2(-66f, 2f), new Vector2(-72f, 6f), new Vector2(-71f, 6.5f), new Vector2(-74f, 8f), new Vector2(-70f, 3f), new Vector2(-65f, -2.5f), new Vector2(-65f, -12.5f), new Vector2(-66f, -13f), new Vector2(-67f, -10.5f), new Vector2(-63f, -6.5f), new Vector2(-55f, 20.5f), new Vector2(-53f, 22.5f), new Vector2(-49f, 20.5f), new Vector2(-60f, 23f), new Vector2(-62f, 24f), 
                new Vector2(-44f, 22f), new Vector2(-44f, 29f), new Vector2(-36f, 31f), new Vector2(-31f, 26.5f), new Vector2(-28f, 25f), new Vector2(-29f, 25.5f), new Vector2(-33f, 30.5f), new Vector2(-34f, 34f), new Vector2(-34f, 33f), new Vector2(-38f, 34f), new Vector2(-39f, 33.5f), new Vector2(-43f, 32.5f), new Vector2(-31f, 36.5f), new Vector2(-30f, 36f), new Vector2(-34f, 39f), new Vector2(-35f, 39.5f), new Vector2(-37f, 40.5f), new Vector2(-41f, 39.5f), new Vector2(-42f, 40f), new Vector2(-46f, 39f), new Vector2(-28f, 40f), new Vector2(-23f, 38.5f), new Vector2(-18f, 39f), new Vector2(-12f, 37f), new Vector2(-11f, 31.5f), new Vector2(-12f, 31f), new Vector2(-11f, 30.5f), new Vector2(-12f, 32f), 
                new Vector2(-54f, 36f), new Vector2(-58f, 35f), new Vector2(-55f, 41.5f), new Vector2(-66f, 31f), new Vector2(-71f, 28.5f), new Vector2(-71f, 38.5f), new Vector2(-72f, 34f), new Vector2(-66f, 35f), new Vector2(-76f, 26f), new Vector2(-76f, 33f), new Vector2(-73f, 22.5f), new Vector2(-76f, 15f), new Vector2(-73f, 17.5f), new Vector2(-15f, 12.5f), new Vector2(-14f, 12f), new Vector2(-19f, 10.5f), new Vector2(-16f, 19f), new Vector2(-15f, 14.5f), new Vector2(-14f, 14f), new Vector2(-30f, 1f), new Vector2(-31f, -0.5f), new Vector2(-32f, -1f), new Vector2(-25f, -7.5f), new Vector2(-23f, -13.5f), new Vector2(-23f, -15.5f), new Vector2(-22f, -18f), new Vector2(-34f, -25f), new Vector2(-17f, -43.5f), 
                new Vector2(-16f, -43f), new Vector2(-22f, -43f), new Vector2(-16f, -40f), new Vector2(-13f, -41.5f), new Vector2(-12f, -41f), new Vector2(-10f, -43f), new Vector2(-9f, -42.5f), new Vector2(-11f, -42.5f), new Vector2(-8f, -32f), new Vector2(-7f, -32.5f), new Vector2(-12f, -29f), new Vector2(-13f, -28.5f), new Vector2(-10f, -29f), new Vector2(-13f, -29.5f), new Vector2(-5f, -30.5f), new Vector2(-3f, -31.5f), new Vector2(-2f, -33f), new Vector2(-1f, -33.5f), new Vector2(-5f, -33.5f), new Vector2(-7f, -33.5f), new Vector2(-6f, -33f), new Vector2(-4f, -32f), new Vector2(-5f, -43.5f), new Vector2(-6f, -45f), new Vector2(-8f, -45f), new Vector2(-5f, -47.5f), new Vector2(-2f, -56f), new Vector2(1f, -54.5f),
                new Vector2(0f, -54f), new Vector2(3f, -53.5f), new Vector2(1f, -57.5f), new Vector2(2f, -58f), new Vector2(-22f, -46f), new Vector2(-28f, -48f), new Vector2(-25f, -47.5f), new Vector2(-26f, -51f), new Vector2(-25f, -55.5f), new Vector2(-28f, -55f), new Vector2(-18f, -53f), new Vector2(-9f, -52.5f), new Vector2(-3f, -58.5f), new Vector2(-12f, -54f), new Vector2(-1f, -64.5f),
            };
            terrainTypes = new int[] {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,

                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 
                1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            };
            if (year == 1939) {
                startingArmyLocations = new Vector2[] {
                    new Vector2(-35f, 2.5f), new Vector2(-38f, 6f), new Vector2(-39f, 1.5f), new Vector2(-31f, 0.5f), new Vector2(-32f, 1f), new Vector2(-30f, 2f), new Vector2(-31f, 2.5f), new Vector2(-37f, 5.5f), new Vector2(-38f, 8f), new Vector2(-38f, 9f), new Vector2(-39f, 9.5f), new Vector2(-41f, 11.5f), 
                    new Vector2(-43f, 12.5f), new Vector2(-44f, 14f), new Vector2(-44f, 15f), new Vector2(-42f, 13f), new Vector2(-42f, 12f), new Vector2(-42f, 10f), new Vector2(-41f, 9.5f), new Vector2(-40f, 8f), new Vector2(-40f, 7f), new Vector2(-37f, 3.5f), new Vector2(-44f, 7f), new Vector2(-46f, 13f), 
                    new Vector2(-36f, -6f), new Vector2(-45f, -1.5f), new Vector2(-42f, 0f), new Vector2(-46f, 3f), new Vector2(-46f, 6f), new Vector2(-48f, 9f), new Vector2(-49f, 5.5f), new Vector2(-47f, 6.5f), new Vector2(-45f, 8.5f), new Vector2(-43f, 5.5f), new Vector2(-40f, 4f), new Vector2(-51f, 13.5f), 
                    new Vector2(-48f, 15f), new Vector2(-49f, 14.5f), new Vector2(-46f, 15f), new Vector2(-34f, 3f), new Vector2(-37f, -0.5f), new Vector2(-36f, 0f), new Vector2(-34f, 0f), new Vector2(-43f, 7.5f), new Vector2(-37f, 6.5f), new Vector2(-63f, 16.5f), new Vector2(-62f, 16f), new Vector2(-59f, 6.5f),
                    new Vector2(-58f, 7f), new Vector2(-52f, 13f), new Vector2(-50f, 11f), new Vector2(-48f, 12f), new Vector2(-43f, 2.5f), new Vector2(-56f, 12f), new Vector2(-58f, 16f), new Vector2(-54f, 7f), new Vector2(-51f, 8.5f),

                    new Vector2(-40f, 14f), new Vector2(-35f, 11.5f), new Vector2(-36f, 16f), new Vector2(-34f, 7f), new Vector2(-30f, 6f), new Vector2(-33f, 9.5f), new Vector2(-32f, 5f), new Vector2(-29f, 21.5f), new Vector2(-34f, 25f), new Vector2(-34f, 21f), new Vector2(-29f, 18.5f), new Vector2(-42f, 15f), 
                    new Vector2(-40f, 13f), new Vector2(-38f, 11f), new Vector2(-36f, 8f), new Vector2(-34f, 4f), new Vector2(-33f, 4.5f), new Vector2(-34f, 5f), new Vector2(-35f, 8.5f), new Vector2(-36f, 9f), new Vector2(-39f, 12.5f), new Vector2(-37f, 12.5f), new Vector2(-38f, 14f), new Vector2(-40f, 17f),
                    new Vector2(-29f, 3.5f), new Vector2(-32f, 7f), new Vector2(-33f, 7.5f), new Vector2(-29f, 6.5f), new Vector2(-34f, 12f), new Vector2(-36f, 14f), new Vector2(-36f, 17f), new Vector2(-40f, 15f), new Vector2(-29f, 20.5f), new Vector2(-30f, 18f), new Vector2(-27f, 15.5f), new Vector2(-14f, 11f), 
                    new Vector2(-17f, 10.5f), new Vector2(-20f, 10f), new Vector2(-25f, 12.5f),
                    new Vector2(-33f, 24.5f), new Vector2(-32f, 24f), new Vector2(-31f, 23.5f), new Vector2(-30f, 24f), new Vector2(-29f, 23.5f), new Vector2(-33f, 25.5f), new Vector2(-34f, 26f), new Vector2(-34f, 24f), new Vector2(-37f, 26.5f), new Vector2(-38f, 26f), new Vector2(-38f, 27f), new Vector2(-41f, 19.5f), 
                    new Vector2(-40f, 20f), new Vector2(-37f, 19.5f), new Vector2(-35f, 19.5f), new Vector2(-31f, 20.5f), new Vector2(-26f, 23f), new Vector2(-26f, 22f), new Vector2(-27f, 20.5f), new Vector2(-33f, 17.5f),
                    new Vector2(-30f, 23f), new Vector2(-35f, 26.5f), new Vector2(-35f, 25.5f), new Vector2(-36f, 26f), new Vector2(-29f, 22.5f), new Vector2(-38f, 24f), new Vector2(-37f, 20.5f), new Vector2(-43f, 16.5f),

                    new Vector2(-34f, 27f), new Vector2(-33f, 26.5f), new Vector2(-32f, 26f), new Vector2(-32f, 25f), new Vector2(-31f, 24.5f), new Vector2(-30f, 25f), new Vector2(-31f, 25.5f), new Vector2(-32f, 27f), new Vector2(-33f, 27.5f), new Vector2(-31f, 26.5f), new Vector2(-30f, 26f), new Vector2(-29f, 24.5f), 
                    new Vector2(-29f, 25.5f), new Vector2(-27f, 24.5f), new Vector2(-26f, 25f), new Vector2(-26f, 24f), new Vector2(-25f, 20.5f), new Vector2(-40f, 26f), new Vector2(-40f, 27f), new Vector2(-41f, 26.5f), new Vector2(-38f, 29f), new Vector2(-37f, 29.5f), new Vector2(-31f, 28.5f), new Vector2(-34f, 29f), 
                    new Vector2(-42f, 31f), new Vector2(-38f, 33f), new Vector2(-33f, 32.5f), new Vector2(-38f, 37f), new Vector2(-48f, 30f), new Vector2(-46f, 33f), new Vector2(-45f, 28.5f), new Vector2(-43f, 34.5f), new Vector2(-35f, 35.5f), new Vector2(-28f, 37f), new Vector2(-19f, 39.5f), new Vector2(-11f, 29.5f), 
                    new Vector2(-59f, 33.5f), new Vector2(-55f, 29.5f), new Vector2(-71f, 25.5f), new Vector2(-73f, 19.5f), new Vector2(-28f, 27f), new Vector2(-45f, 22.5f), new Vector2(-46f, 21f), new Vector2(-57f, 23.5f), new Vector2(-55f, 22.5f), new Vector2(-47f, 19.5f), new Vector2(-50f, 21f), new Vector2(-52f, 19f),

                    new Vector2(-52f, 3f), new Vector2(-55f, 1.5f), new Vector2(-61f, 2.5f), new Vector2(-56f, -2f), new Vector2(-65f, -1.5f), new Vector2(-68f, 4f), new Vector2(-69f, -6.5f), new Vector2(-74f, 7f), new Vector2(-72f, 9f), new Vector2(-71f, 2.5f), new Vector2(-64f, -5f), new Vector2(-60f, -2f), new Vector2(-65f, -14.5f), 
                    new Vector2(-65f, -9.5f), new Vector2(-69f, -0.5f), new Vector2(-49f, -7.5f), new Vector2(-49f, 1.5f), new Vector2(-50f, -1f), new Vector2(-48f, -3f), new Vector2(-51f, -2.5f), new Vector2(-49f, -5.5f), new Vector2(-48f, -5f), new Vector2(-44f, -11f), new Vector2(-43f, -9.5f), new Vector2(-46f, -8f), 
                    new Vector2(-44f, -9f), new Vector2(-45f, -14.5f), new Vector2(-45f, -17.5f), new Vector2(-44f, -18f), new Vector2(-43f, -19.5f), new Vector2(-41f, -22.5f), new Vector2(-45f, -23.5f), new Vector2(-39f, -31.5f), new Vector2(-30f, -25f), new Vector2(-34f, -25f), new Vector2(-30f, -22f), new Vector2(-23f, -26.5f), 
                    new Vector2(-23f, -14.5f), new Vector2(-23f, -13.5f), new Vector2(-25f, -8.5f), new Vector2(-21f, -16.5f), new Vector2(-34f, -32f), new Vector2(-33f, -32.5f), new Vector2(-42f, -28f), new Vector2(-11f, -29.5f),
                    new Vector2(-42f, -4f), new Vector2(-40f, -12f), new Vector2(-38f, -11f), new Vector2(-41f, -4.5f), new Vector2(-36f, -4f), new Vector2(-4f, -33f), new Vector2(-14f, -38f), new Vector2(-23f, -42.5f), new Vector2(-2f, -44f), new Vector2(3f, -49.5f), new Vector2(2f, -57f), new Vector2(-30f, -47f), new Vector2(-24f, -45f), new Vector2(-16f, -41f), new Vector2(-11f, -43.5f), new Vector2(-19f, -43.5f), new Vector2(-4f, -41f), new Vector2(-7f, -50.5f), new Vector2(-17f, -47.5f),
                
                    //ships
                    new Vector2(-28f, 7f), new Vector2(-26f, 12f), new Vector2(-14f, 10f), new Vector2(-17f, 13.5f), new Vector2(-28f, -3f), new Vector2(-27f, -2.5f), new Vector2(-28f, 32f), new Vector2(-21f, 21.5f), new Vector2(-13f, 29.5f), 
                };
                startingArmyTypes = new int[] {
                    2, 0, 0, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 0, 1, 2, 0, 2, 0, 0, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 0, 0, 2, 3, 3, 0, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,

                    3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0, 0, 10, 2, 0, 0, 10, 1, 0, 0, 0, 10, 0, 1, 0, 0,
                    2, 2, 1, 0, 0, 0, 10, 2, 2, 1, 0, 1, 0, 1, 0, 2, 2, 0, 10, 10, 4, 3, 3, 3, 3, 4, 4, 3,

                    1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 10, 10, 10, 10, 1, 10, 0, 0, 2, 0, 10, 1, 10, 1, 1, 2, 2, 10, 10, 10, 1, 2, 0, 0,

                    0, 10, 10, 0, 1, 1, 1, 0, 10, 10, 10, 10, 10, 0, 0, 1, 1, 2, 2, 0, 4, 4, 2, 0, 0, 0, 0, 10, 10, 1, 1, 2, 2, 2, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 4, 1, 2, 2, 0, 10, 10, 10, 10, 0, 0, 0,

                    //ships
                    7, 7, 7, 7, 7, 7, 7, 7, 8,
                };
                startingArmyCountries = new string[] {
                    "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Russia", "Russia", "Russia"
                };
                cityLevels = new int[] {
                    3, 4, 3, 5, 4, 2, 2, 3, 1, 1,
                    2, 3, 1, 2, 2, 2, 3, 3, 5, 2,
                    2, 2, 2, 3, 3, 5, 3, 2, 2, 4,
                    2, 2, 2, 3, 3, 1, 3, 4, 5, 3,
                    3, 2, 2, 4,

                    1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 
                    2, 1, 2, 1, 2, 2, 2, 2, 1, 1, 
                    1, 2, 2, 1, 2, 2, 1, 1, 1,

                    1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 2, 1, 1,
                };
                cityIndustrials = new int[] {
                    1, 2, 1, 2, 2, 2, 2, 1, 0, 0,
                    0, 1, 0, 1, 1, 1, 2, 2, 2, 1,
                    2, 2, 2, 2, 3, 1, 0, 0, 0, 0,
                    0, 2, 1, 2, 2, 0, 3, 1, 2, 1,
                    0, 1, 1, 2,

                    0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 
                    1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 
                    0, 1, 1, 0, 1, 1, 0, 2, 0,

                    1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 2, 1, 1,
                };
            } else if (year == 1941) {
                startingArmyLocations = new Vector2[] {
                    new Vector2(-63f, 16.5f), new Vector2(-59f, 6.5f), new Vector2(-51f, 13.5f), new Vector2(-49f, 5.5f), new Vector2(-44f, 7f), new Vector2(-39f, 1.5f), new Vector2(-35f, 2.5f), new Vector2(-40f, 5f), new Vector2(-39f, 4.5f), new Vector2(-41f, 7.5f), new Vector2(-43f, 9.5f), new Vector2(-43f, 10.5f), new Vector2(-38f, 2f), new Vector2(-43f, 6.5f), new Vector2(-43f, 7.5f), new Vector2(-44f, 6f), new Vector2(-32f, 1f), new Vector2(-44f, 14f), new Vector2(-44f, 13f), new Vector2(-45f, 13.5f), new Vector2(-44f, 10f), new Vector2(-36f, 2f), new Vector2(-35f, 1.5f), new Vector2(-42f, 4f), new Vector2(-41f, 3.5f), new Vector2(-36f, -1f), new Vector2(-35f, -1.5f), new Vector2(-45f, -1.5f), new Vector2(-45f, 15.5f), new Vector2(-47f, 14.5f), new Vector2(-48f, 11f), new Vector2(-47f, 6.5f), 
                    new Vector2(-46f, 4f), new Vector2(-46f, 9f), new Vector2(-42f, 0f), new Vector2(-41f, -1.5f), new Vector2(-41f, -2.5f), new Vector2(-42f, -2f), new Vector2(-43f, -2.5f), new Vector2(-48f, 2f), new Vector2(-48f, 3f), new Vector2(-49f, 3.5f), new Vector2(-50f, 4f), new Vector2(-38f, 6f), new Vector2(-33f, 7.5f), new Vector2(-40f, 15f), new Vector2(-36f, 5f), new Vector2(-34f, 4f), new Vector2(-32f, 4f), new Vector2(-36f, 4f), new Vector2(-35f, 4.5f), new Vector2(-31f, 2.5f), new Vector2(-30f, 3f), new Vector2(-29f, 3.5f), new Vector2(-29f, 2.5f), new Vector2(-39f, 7.5f), new Vector2(-40f, 9f), new Vector2(-41f, 10.5f), new Vector2(-42f, 12f), new Vector2(-38f, 9f), new Vector2(-37f, 9.5f), new Vector2(-37f, 6.5f), new Vector2(-34f, 7f), new Vector2(-37f, 4.5f), new Vector2(-30f, 6f), 
                    new Vector2(-42f, 15f), new Vector2(-42f, 14f), new Vector2(-38f, 27f), new Vector2(-37f, 27.5f), new Vector2(-35f, 25.5f), new Vector2(-32f, 24f), new Vector2(-30f, 23f), new Vector2(-27f, 22.5f), new Vector2(-26f, 21f), new Vector2(-26f, 19f), new Vector2(-36f, 17f), new Vector2(-34f, 24f), new Vector2(-36f, 21f), new Vector2(-35f, 21.5f), new Vector2(-30f, 20f), new Vector2(-29f, 20.5f), new Vector2(-33f, 19.5f), new Vector2(-32f, 20f), new Vector2(-35f, 18.5f), new Vector2(-34f, 18f), new Vector2(-39f, 17.5f), new Vector2(-31f, 17.5f), new Vector2(-30f, 18f), new Vector2(-27f, 15.5f), new Vector2(-25f, 12.5f), new Vector2(-25f, 13.5f), new Vector2(-27f, 17.5f), new Vector2(-38f, 13f), new Vector2(-34f, 12f), new Vector2(-38f, 11f), new Vector2(-31f, 0.5f), new Vector2(-35f, 6.5f), 
                    new Vector2(-32f, 5f), new Vector2(-30f, 4f), new Vector2(-41f, 11.5f), new Vector2(-35f, 7.5f), new Vector2(-24f, 20f), new Vector2(-28f, 24f), new Vector2(-33f, 26.5f), new Vector2(-40f, 25f), new Vector2(-28f, 27f), new Vector2(-33f, 28.5f), new Vector2(-42f, 26f), new Vector2(-39f, 29.5f), new Vector2(-38f, 30f), new Vector2(-41f, 28.5f), new Vector2(-35f, 30.5f), new Vector2(-31f, 25.5f), new Vector2(-26f, 24f), new Vector2(-25f, 23.5f), new Vector2(-30f, 27f), new Vector2(-45f, 22.5f), new Vector2(-57f, 23.5f), new Vector2(-51f, 21.5f), new Vector2(-48f, 19f), new Vector2(-46f, 20f), new Vector2(-53f, 20.5f), new Vector2(-48f, 22f), new Vector2(-48f, 30f), new Vector2(-38f, 37f), new Vector2(-19f, 39.5f), new Vector2(-11f, 29.5f),

                    new Vector2(-42f, -4f), new Vector2(-40f, -12f), new Vector2(-42f, -5f), new Vector2(-39f, -11.5f), new Vector2(-44f, -11f), new Vector2(-43f, -10.5f), new Vector2(-46f, -8f), new Vector2(-46f, -7f), new Vector2(-42f, -10f), new Vector2(-44f, -9f), new Vector2(-45f, -14.5f), new Vector2(-44f, -18f), new Vector2(-44f, -17f), new Vector2(-45f, -17.5f), new Vector2(-49f, -7.5f), new Vector2(-49f, 1.5f), new Vector2(-49f, -5.5f), new Vector2(-51f, -2.5f), new Vector2(-49f, -1.5f), new Vector2(-51f, 0.5f), new Vector2(-47f, -4.5f), new Vector2(-23f, -14.5f), new Vector2(-25f, -8.5f), new Vector2(-21f, -16.5f), new Vector2(-25f, -10.5f), new Vector2(-43f, -19.5f), new Vector2(-41f, -22.5f), new Vector2(-30f, -25f), new Vector2(-23f, -26.5f), new Vector2(-39f, -31.5f), new Vector2(-45f, -23.5f), 
                    new Vector2(-11f, -29.5f), new Vector2(-8f, -30f), new Vector2(-34f, -25f), new Vector2(-52f, 3f), new Vector2(-56f, -2f), new Vector2(-65f, -1.5f), new Vector2(-68f, 4f), new Vector2(-69f, -6.5f), new Vector2(-65f, -14.5f), new Vector2(-74f, 7f), new Vector2(-54f, -1f), new Vector2(-53f, 0.5f), new Vector2(-52f, 2f), new Vector2(-5f, -31.5f), new Vector2(-4f, -33f), new Vector2(-6f, -34f), new Vector2(-23f, -42.5f), new Vector2(-14f, -38f), new Vector2(-2f, -44f), new Vector2(3f, -49.5f), new Vector2(2f, -57f), new Vector2(-7f, -50.5f), new Vector2(-17f, -47.5f), new Vector2(-15f, -41.5f), new Vector2(-9f, -43.5f), new Vector2(-30f, -47f), new Vector2(-25f, -46.5f), new Vector2(-24f, -46f), new Vector2(-31f, -23.5f), new Vector2(-29f, -22.5f), new Vector2(-25f, -26.5f),
                
                    //ships
                    new Vector2(-39f, -6.5f), new Vector2(-38f, -7f), new Vector2(-38f, -8f), new Vector2(-37f, -8.5f), new Vector2(-37f, -9.5f), new Vector2(-36f, -10f), new Vector2(-36f, -11f), new Vector2(-35f, -11.5f), new Vector2(-35f, -12.5f), new Vector2(-35f, -10.5f), new Vector2(-36f, -9f), new Vector2(-37f, -7.5f), new Vector2(-26f, -8f), new Vector2(-27f, -9.5f), new Vector2(-23f, -7.5f), new Vector2(-27f, -6.5f), new Vector2(-28f, -7f), new Vector2(-26f, -6f), new Vector2(-28f, -8f), new Vector2(-33f, -18.5f), new Vector2(-32f, -18f), new Vector2(-30f, -17f), new Vector2(-27f, -17.5f), new Vector2(-36f, -18f), new Vector2(-35f, -14.5f), new Vector2(-31f, -12.5f), new Vector2(-30f, -12f), new Vector2(-27f, -2.5f), new Vector2(-14f, 10f), new Vector2(-26f, 12f), new Vector2(-28f, 7f), new Vector2(-21f, -7.5f), new Vector2(-21f, -8.5f), new Vector2(-20f, -9f), new Vector2(-20f, -10f), new Vector2(-20f, -11f), new Vector2(-29f, -10.5f), new Vector2(-28f, -13f), new Vector2(-35f, -20.5f), new Vector2(-34f, -19f), new Vector2(-32f, -16f), new Vector2(-34f, -17f), new Vector2(-29f, -16.5f), new Vector2(-30f, -15f), new Vector2(-28f, -16f), new Vector2(-34f, -15f), new Vector2(-25f, -15.5f), new Vector2(-24f, -17f), new Vector2(-22f, -11f),
                    new Vector2(-30f, -20f), new Vector2(-34f, -22f), new Vector2(-40f, -22f), new Vector2(-27f, -19.5f), new Vector2(-43f, -23.5f), new Vector2(-41f, -19.5f), new Vector2(-24f, -15f), new Vector2(-22f, -13f), new Vector2(-23f, -11.5f), new Vector2(-26f, -11f), new Vector2(-22f, -19f), new Vector2(-20f, -16f),
                };
                startingArmyTypes = new int[] {
                    0, 10, 10, 10, 5, 2, 1, 2, 0, 0, 0, 0, 3, 3, 3, 3, 3, 0, 2, 10, 10, 10, 10, 10, 10, 0, 0, 2, 0, 10, 10, 10, 10, 10, 10, 1, 1, 1, 1, 1, 1, 1, 1, 5, 5, 5, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0, 4, 4, 3, 3, 3, 
                    3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 1, 1, 0, 0, 10, 10, 10, 10, 10, 2, 10, 10, 4, 4, 4, 4, 0, 2, 1, 1, 1, 1, 1, 1, 5, 5, 2, 2, 2, 2, 0, 
                    0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 10, 10, 10, 2, 2, 1, 1,

                    0, 0, 1, 2, 2, 1, 1, 0, 0, 0, 0, 0, 5, 1, 2, 0, 1, 0, 0, 0, 2, 1, 2, 2, 0, 1, 0, 2, 0, 1, 0, 0, 0, 0, 10, 10, 10, 0, 0, 10, 10, 3, 3, 3, 1, 0, 0, 2, 0, 6, 6, 0, 0, 0, 10, 10, 0, 0, 10, 2, 2, 2,

                    //ships
                    0, 1, 1, 1, 0, 0, 0, 2, 2, 7,
                    7, 7, 7, 7, 7, 1, 1, 1, 1, 7, 
                    7, 7, 7, 7, 7, 8, 8, 7, 7, 7, 
                    7, 1, 1, 2, 2, 2, 2, 2, 7, 1, 
                    1, 1, 6, 6, 4, 4, 4, 4, 7,

                    7, 7, 7, 7, 8, 8, 7, 7, 7, 7, 7, 7,
                };
                startingArmyCountries = new string[] { 
                    "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan",
                    "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan",
                    "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan",
                    "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan",
                    "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", "Japan", 

                    "Britain", "Britain", "Britain", "Britain", "Britain", "Britain", "USA", "USA", "USA", "USA", "USA", "USA"
                };
                cityLevels = new int[] {
                    4, 4, 3, 5, 4, 3, 3, 3, 2, 1,
                    2, 3, 1, 3, 3, 3, 3, 4, 5, 3,
                    2, 2, 2, 3, 3, 5, 3, 2, 2, 5,
                    2, 3, 2, 3, 3, 2, 3, 4, 5, 4,
                    4, 2, 3, 4,

                    1, 1, 2, 2, 1, 1, 2, 2, 1, 1,
                    2, 1, 2, 1, 2, 2, 2, 2, 1, 1,
                    1, 2, 2, 1, 2, 2, 1, 1, 1,

                    1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 2, 1, 1,
                };
                cityIndustrials = new int[] {
                    1, 2, 1, 2, 2, 2, 2, 1, 0, 0,
                    0, 1, 0, 1, 1, 1, 2, 2, 2, 1,
                    2, 2, 2, 2, 3, 1, 0, 0, 0, 0,
                    1, 2, 1, 2, 2, 0, 3, 1, 2, 2,
                    2, 1, 1, 2,

                    0, 0, 1, 1, 0, 0, 1, 1, 0, 0,
                    1, 0, 1, 0, 1, 1, 1, 1, 0, 0,
                    0, 1, 1, 0, 1, 1, 0, 2, 0,

                    1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 2, 1, 1,
                };
            } else if (year == 1944) {
                cityLevels = new int[] {};
                cityIndustrials = new int[] {};
                startingArmyLocations = new Vector2[] {};
                startingArmyTypes = new int[] {};
                startingArmyCountries = new string[] { };
            } else if (year == 1945) {
                cityLevels = new int[] {};
                cityIndustrials = new int[] {};
                startingArmyLocations = new Vector2[] {};
                startingArmyTypes = new int[] {};
                startingArmyCountries = new string[] {};
            }
            cities = new City[cityNames.Length];
        }

        float iMin = 0, iMax = 0, jMin = 0, jMax = 0;
        if (onlyEurope) {
            tiles = new Tile[7000];
            iMin = -80;
            iMax = 7;
            jMin = -30;
            jMax = 25;
        } else {
            tiles = new Tile[30000];
            iMin = -91; 
            iMax = 18;
            jMin = -67;
            jMax = 57;
        }
        if (!savedData) {
            for (float i = iMin; i < iMax; i++) {
                for (float j = jMin; j < jMax; j++) {
                    float j1 = j;
                    if (Mathf.Abs((int)i % 2) == 1) {
                        j1 += 0.5f;
                    }
                    bool canSpawn1 = false;
                    bool canSpawn2 = false;
                    bool canSpawn3 = false;
                    bool canSpawn4 = false;
                    foreach (Collider i1 in Physics.OverlapBox(new Vector3(i + 0.2f, 0.4f, j1 + 0.2f), new Vector3(0.04f, 2f, 0.04f))) {
                        if (i1.name == "Map") {
                            canSpawn1 = true;
                            break;
                        }
                    }
                    foreach (Collider i1 in Physics.OverlapBox(new Vector3(i + 0.2f, 0.4f, j1 - 0.2f), new Vector3(0.04f, 2f, 0.04f))) {
                        if (i1.name == "Map") {
                            canSpawn2 = true;
                            break;
                        }
                    }
                    foreach (Collider i1 in Physics.OverlapBox(new Vector3(i - 0.2f, 0.4f, j1 + 0.2f), new Vector3(0.04f, 2f, 0.04f))) {
                        if (i1.name == "Map") {
                            canSpawn3 = true;
                            break;
                        }
                    }
                    foreach (Collider i1 in Physics.OverlapBox(new Vector3(i - 0.2f, 0.4f, j1 - 0.2f), new Vector3(0.04f, 2f, 0.04f))) {
                        if (i1.name == "Map") {
                            canSpawn4 = true;
                            break;
                        }
                    }
                    if (canSpawn1 && canSpawn2 && canSpawn3 && canSpawn4) {
                        GameObject insTile = Instantiate(tilePrefab, new Vector3(i, 0.5f, j1), Quaternion.identity);
                        insTile.GetComponent<Tile>().terrainName = "plains";
                        pushItemToArray(ref tiles, insTile.GetComponent<Tile>());
                    } else {
                        //water tile
                        GameObject insTile = Instantiate(tilePrefab, new Vector3(i, 0.5f, j1), Quaternion.identity);
                        insTile.GetComponent<Tile>().defaultMaterial = tileBlueMaterial;
                        insTile.GetComponent<Tile>().terrainName = "water";
                        insTile.GetComponent<Tile>().isWater = true;
                        pushItemToArray(ref tiles, insTile.GetComponent<Tile>());
                    }
                }
            }
        } else {
            for (int i = 0; i < loadedData.tilePositions.Length; i++) {
                if (loadedData.tilePositions[i] != null) {
                    GameObject insTile = Instantiate(tilePrefab, loadedData.tilePositions[i].toVector3(), Quaternion.identity);
                    insTile.GetComponent<Tile>().isWater = loadedData.tileIsWaters[i];
                    insTile.GetComponent<Tile>().owningCountry = loadedData.tileCountries[i];
                    insTile.GetComponent<Tile>().defaultMaterial = findCountryColor(insTile.GetComponent<Tile>().owningCountry);
                    insTile.GetComponent<Tile>().GetComponent<Renderer>().material = insTile.GetComponent<Tile>().defaultMaterial;
                    insTile.GetComponent<Tile>().setDefaultMaterial();
                    pushItemToArray(ref tiles, insTile.GetComponent<Tile>());
                }
            }
        }
        //spawn terrains
        for (int i = 0; i < terrainLocations.Length; i++) {
            Instantiate(terrainPrefabs[terrainTypes[i]], new Vector3(terrainLocations[i].x, 0, terrainLocations[i].y), Quaternion.identity);
        }

        //spawn cities
        for (int i = 0; i < cityLocations.Length; i++) {
            string s = "";
            switch (cityLevels[i]) {
            case 1:
                s = "I";
                break;
            case 2:
                s = "II";
                break;
            case 3:
                s = "III";
                break;
            case 4:
                s = "IV";
                break;
            case 5:
                s = "V";
                break;
            default:
                s = "";
                break;
            }
            GameObject insItem = null; 
            bool foundTile = false; 
            foreach (Collider j in Physics.OverlapBox(new Vector3(cityLocations[i].x, 1, cityLocations[i].y), new Vector3(0.1f, 6f, 0.1f))) { 
                if (j.GetComponent<Tile>() != null) {
                    if (j.GetComponent<Tile>().isWater) 
                        insItem = Instantiate(portPrefab, new Vector3(cityLocations[i].x, 1, cityLocations[i].y), portPrefab.transform.rotation);
                    else
                        insItem = Instantiate(cityPrefab, new Vector3(cityLocations[i].x, 1, cityLocations[i].y), Quaternion.identity);
                    foundTile = true; 
                    break;
                }
            }
            if (!foundTile)
                continue; 


            if (cityIndustrials[i] > 0) {
                string j = "";
                switch (cityIndustrials[i]) {
                case 1:
                    j = "I";
                    break;
                case 2:
                    j = "II";
                    break;
                case 3:
                    j = "III";
                    break;
                case 4:
                    j = "IV";
                    break;
                case 5:
                    j = "V";
                    break;
                default:
                    j = "";
                    break;
                }
                insItem.GetComponent<City>().cityIndustrialText.text = j;
            } else {
                insItem.GetComponent<City>().industrialIcon.enabled = false;
                insItem.GetComponent<City>().cityIndustrialText.enabled = false; 
            }
            insItem.GetComponent<City>().cityText.text = cityNames[i];
            insItem.GetComponent<City>().cityLevelText.text = s;
            insItem.GetComponent<City>().cityLevel = cityLevels[i];
            insItem.GetComponent<City>().cityIndustrial = cityIndustrials[i];
            if (savedData)
                insItem.GetComponent<City>().citySoldier.health = loadedData.cityHealths[i]; 
            pushItemToArray(ref cities, insItem.GetComponent<City>());
        }
        if (!savedData) {
            //spawn soldiers without save
            int z = 0, n = 0;
            if (startingArmyLocations.Length > 0) {
                foreach (Vector2 i in startingArmyLocations) {
                    GameObject insItem = Instantiate(soldierPrefabs[startingArmyTypes[z]], new Vector3(i.x, 1, i.y), Quaternion.identity);
                    foreach (Collider j in Physics.OverlapBox(insItem.transform.position, new Vector3(0.1f, 2f, 0.1f))) {
                        if (j.GetComponent<Tile>() != null) {
                            if (j.GetComponent<Tile>().isWater) {
                                insItem.GetComponent<Soldier>().country = startingArmyCountries[n];
                                n++;
                            }
                            switch (year) {
                            case 1939:
                                insItem.GetComponent<Soldier>().experience = 260;
                                break;
                            case 1941:
                                insItem.GetComponent<Soldier>().experience = 460;
                                break;
                            case 1944:
                                insItem.GetComponent<Soldier>().experience = 720;
                                break;
                            case 1945:
                                insItem.GetComponent<Soldier>().experience = 1100;
                                break;
                            }
                                 
                            break;
                        }
                    }
                    z++;
                }
            }
        } else { 
            //spawn soldiers from saved data
            for (int i = 0; i < loadedData.playerSoldiers.Length; i++) {
                if (loadedData.playerSoldiers[i] != null) {
                    GameObject insItem = Instantiate(soldierPrefabs[findTroopID(loadedData.playerSoldiers[i].troopName)], loadedData.playerSoldiers[i].position.toVector3(), Quaternion.identity);
                    insItem.GetComponent<Soldier>().health = loadedData.playerSoldiers[i].health;
                    insItem.GetComponent<Soldier>().country = loadedData.playerSoldiers[i].country;
                    insItem.GetComponent<Soldier>().moved = loadedData.playerSoldiers[i].moved;
                    insItem.GetComponent<Soldier>().attacked = loadedData.playerSoldiers[i].attacked;
                    insItem.GetComponent<Soldier>().experience = loadedData.playerSoldiers[i].experience; 
                }
            }
            for (int i = 0; i < loadedData.aiSoldiers.Length; i++) {
                if (loadedData.aiSoldiers[i] != null) {
                    GameObject insItem = Instantiate(soldierPrefabs[findTroopID(loadedData.aiSoldiers[i].troopName)], loadedData.aiSoldiers[i].position.toVector3(), Quaternion.identity);
                    insItem.GetComponent<Soldier>().health = loadedData.aiSoldiers[i].health;
                    insItem.GetComponent<Soldier>().country = loadedData.aiSoldiers[i].country;
                    insItem.GetComponent<Soldier>().moved = loadedData.aiSoldiers[i].moved;
                    insItem.GetComponent<Soldier>().attacked = loadedData.aiSoldiers[i].attacked;
                    insItem.GetComponent<Soldier>().experience = loadedData.aiSoldiers[i].experience;
                }
            }
        }
    }

    private void Start() {
        cam = GameObject.Find("Main Camera").transform;
    }

    //*/
    bool firstFrame = false; 
    public float roundPassDelay; 

    void Update() {
        if (!cam.GetComponent<Camera>().enabled)
            cam = GameObject.Find("Main Camera").transform;
        if (!firstFrame) {
            firstFrame = true; 
        }
        cashDisplay.text = playerCountry.cash.ToString();
        industrialDisplay.text = playerCountry.industry.ToString(); 
        for (int i = 0; i < UIRectTransforms.Length; i++) {
            UIRects[i] = Custom2D.generatePointDetectionRect(new Vector2(UIRectTransforms[i].position.x, UIRectTransforms[i].position.y), UIRectTransforms[i].rect);
        }
        mouseOnUI = false;
        for (int i = 0; i < UIRects.Length; i++) { 
            if (UIRects[i].Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) && UIRectTransforms[i].GetComponent<Image>().enabled) {
                mouseOnUI = true;
                break; 
            }
        }

        if (roundPassDelay > 0) {
            roundPassDelay -= Time.deltaTime; 
            if (roundPassDelay <= 0) {
                //ai purchase units
                //prioritize empty cities with enemies nearby
                foreach (City i in cities) {
                    if (i != null) {
                        if (!i.country.Equals(playerCountryName) && !i.tile.isOccupied && findCountryData(i.country) != null && findCountryData(i.country).cash >= 60f) {
                            foreach (Collider j in Physics.OverlapSphere(new Vector3(i.transform.position.x, 0, i.transform.position.z), 6.2f)) {
                                if (j.GetComponent<Soldier>() != null && !findCountryAlliance(j.GetComponent<Soldier>().country, i.country)) {
                                    //buy troops at random chance; keep redrawing randoms after choosing soldier too expensive to purchase
                                    bool buildSuccessful = false;
                                    //did not use while so that program will not die if stuck

                                    for (int z = 0; !buildSuccessful && z < 10; z++) {
                                        int rand;
                                        if (i.isPort) {
                                            rand = Random.Range(8, 9);
                                        } else {
                                            rand = Random.Range(1, 11);
                                            if (rand == 4)
                                                rand = 2;
                                            if (rand == 8 || rand == 9)
                                                rand = 10;
                                        }
                                        i.tile.occupier = aiBuildTroops(rand, i.tile, findCountryData(i.country));
                                        if (i.tile.occupier != null) {
                                            buildSuccessful = true;
                                            i.tile.isOccupied = true;
                                        }
                                    }
                                    //for (int z = 0; !buildSuccessful && z < 10; z++) {
                                    //    int rand = Random.Range(1, 7);
                                    //    if (rand == 4)
                                    //        rand = 2;
                                    //    i.tile.occupier = aiBuildTroops(rand, i.tile, findCountryData(i.country));
                                    //    if (i.tile.occupier != null) {
                                    //        buildSuccessful = true;
                                    //        i.tile.isOccupied = true;
                                    //    }
                                    //}
                                    break;
                                }
                            }
                        }
                    }
                }
                foreach (City i in cities) {
                    if (i != null) {
                        if (!i.country.Equals(playerCountryName) && !i.tile.isOccupied && findCountryData(i.country).cash >= 300f && findCountryData(i.country).industry >= 100f) {
                            bool buildSuccessful = false;
                            for (int j = 0; !buildSuccessful && j < 10; j++) {
                                int rand;
                                if (i.isPort) {
                                    rand = Random.Range(8, 9);
                                } else {
                                    rand = Random.Range(1, 10);
                                    if (rand == 4)
                                        rand = 2;
                                    if (rand == 8 || rand == 9)
                                        rand = 10;
                                }
                                i.tile.occupier = aiBuildTroops(rand, i.tile, findCountryData(i.country));
                                if (i.tile.occupier != null) {
                                    buildSuccessful = true;
                                    i.tile.isOccupied = true;
                                }
                            }
                            //for (int j = 0; !buildSuccessful && j < 100; j++) {
                            //    int rand = Random.Range(2, 7);
                            //    if (rand == 4)
                            //        rand = 2;
                            //    i.tile.occupier = aiBuildTroops(rand, i.tile, findCountryData(i.country));
                            //    if (i.tile.occupier != null) {
                            //        buildSuccessful = true;
                            //        i.tile.isOccupied = true;
                            //    }
                            //}
                        }
                    }
                }

                //set soldiers' status
                foreach (Soldier i in playerSoldiers) {
                    if (i != null) {
                        i.attacked = false;
                        i.moved = false;
                    }
                }
                stopPlayerActions = false;

                //drop down UI
                lowerRoundPassUI();
                roundPassCashDisplay.text = "+" + playerCashProfit.ToString();
                roundPassIndustryDisplay.text = "+" + playerIndustryProfit.ToString();
                if (PlayerPrefs.GetString("language") == "Chinese")
                    roundPassRoundDisplay.text = "第" + round.ToString() + "回合";
                else
                    roundPassRoundDisplay.text = "Round " + round.ToString();

                //saves game every time round passes
                tilePositions = new MyVector3[tiles.Length];
                tileCountries = new string[tiles.Length];
                tileIsWaters = new bool[tiles.Length];
                for (int i = 0; i < tiles.Length; i++) {
                    if (tiles[i] != null) {
                        tilePositions[i] = new MyVector3(tiles[i].transform.position);
                        tileCountries[i] = tiles[i].owningCountry;
                        tileIsWaters[i] = tiles[i].isWater;
                    }
                }
                BinarySaveSystem.SaveMap(this);
                PlayerPrefs.SetString("save_" + year.ToString(), "true");
            }
        } else {
            //testing only
            if (Input.GetKeyDown(KeyCode.Q)) {
                string s = ""; 
                foreach (Vector3 i in positions) { 
                    if (i != Vector3.zero) {
                        s += "new Vector2(" + i.x + "f, " + i.z + "f), "; 
                    }
                }
                print(s);
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                string s = "";
                foreach (int i in numbers) {
                    if (i != 0) {
                        s += (i - 1) + ", "; 
                    }
                }
                print(s);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { 
                for (int i = 0; i < numbers.Length; i++) { 
                    if (numbers[i] == 0) {
                        numbers[i] = 1;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 2;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 3;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 4;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 5;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha6)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 6;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha7)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 7;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha8)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 8;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha9)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 9;
                        break;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                for (int i = 0; i < numbers.Length; i++) {
                    if (numbers[i] == 0) {
                        numbers[i] = 11;
                        break;
                    }
                }
            }
            //testing up to here*/

            if (Input.GetMouseButtonDown(0)) {
                mouseDownPosition = Input.mousePosition; 
            }
            if (Input.GetMouseButtonUp(0) && Vector3.Distance(mouseDownPosition, Input.mousePosition) < 8f && !stopPlayerActions && !mouseOnUI && !inUI && !cam.GetComponent<CameraMovement>().aboveHeight) {
               
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                if (Physics.Raycast(ray, out hit, 1000f)) {
                    print(hit.collider.name);
                    /*//testing only
                    if (hit.collider.GetComponent<Tile>() != null) {
                        for (int a = 0; a < positions.Length; a++) {
                            if (positions[a] == Vector3.zero) {
                                positions[a] = hit.collider.GetComponent<Tile>().transform.position;
                                break;
                            }
                        }

                        hit.collider.GetComponent<Tile>().GetComponent<Renderer>().material = null;
                        hit.collider.GetComponent<Tile>().GetComponent<Renderer>().enabled = false; 
                    }
                    //testing only*/

                    if (hit.collider.GetComponent<Tile>() != null && selectedTile != null && hit.collider.gameObject == selectedTile.gameObject) {
                        //do nothing
                    } else if (hit.collider.GetComponent<Soldier>() != null && hit.collider.GetComponent<Soldier>().country == playerCountryName) {
                        if (selectedSoldier != null) {
                            selectedSoldier.selected = false;
                            selectedSoldier.returnTiles();
                        }
                        selectedSoldier = hit.collider.GetComponent<Soldier>();
                        selectedSoldier.selected = true;
                        if (selectedTile != null) {
                            selectedTile.selected = false;
                            selectedTile.GetComponent<Tile>().GetComponent<Renderer>().material = selectedTile.defaultMaterial;
                        }
                        selectedTile = selectedSoldier.currentTile;
                        selectedSoldier.currentTile.selected = true; 
                   } else if (hit.collider.GetComponent<Soldier>() != null && hit.collider.GetComponent<Soldier>().currentTile != null && hit.collider.GetComponent<Soldier>().currentTile.canAttack) {
                        //soldier should be selected to activate parameters
                        selectedSoldier.attack(hit.collider.GetComponent<Soldier>());
                        deSelectSoldier();
                    } else if (hit.collider.GetComponent<Tile>() != null && hit.collider.GetComponent<Tile>().canWalkOn && selectedSoldier != null && selectedSoldier.currentTile != hit.collider.GetComponent<Tile>()) {
                        selectedSoldier.moveSoldier(hit.collider.GetComponent<Tile>(), false);
                        setBuildTroopsButton(false);
                        selectedTile = null; 
                    } else if (hit.collider.GetComponent<Tile>() != null && (!hit.collider.GetComponent<Tile>().isWater || hit.collider.GetComponent<Tile>().isCity)) {
                        if (selectedSoldier != null) 
                            deSelectSoldier();
                        hit.collider.GetComponent<Tile>().selected = true;
                        hit.collider.GetComponent<Tile>().GetComponent<Renderer>().material = blueMaterial;
                        if (selectedTile != null) {
                            selectedTile.selected = false;
                            selectedTile.GetComponent<Tile>().GetComponent<Renderer>().material = selectedTile.defaultMaterial;
                        }
                        selectedTile = hit.collider.GetComponent<Tile>();
                    } else if (hit.collider.GetComponent<Tile>() != null && hit.collider.GetComponent<Tile>().isWater) {
                        if (selectedSoldier != null) 
                            deSelectSoldier();
                        if (selectedTile != null && !selectedTile.isOccupied) {
                            selectedTile.selected = false;
                            selectedTile.GetComponent<Tile>().GetComponent<Renderer>().material = selectedTile.defaultMaterial;
                            selectedTile = null;
                        }
                    }
                }
                if (selectedTile != null) {
                    //city troop construction
                    if (!selectedTile.isOccupied && selectedTile.isCity && selectedTile.owningCountry == playerCountryName) {
                        setBuildTroopsButton(true);
                    } else if (!selectedTile.isOccupied && !selectedTile.isCity && selectedTile.owningCountry == playerCountryName && !selectedTile.isWater) {
                        setBuildTroopsButton(true);
                    } else
                        setBuildTroopsButton(false);
                } else {
                    setBuildTroopsButton(false);
                }
            }
            if (selectedTile != null && selectedTile.isOccupied || selectedTile == null || selectedTile.owningCountry != playerCountryName) {
                setBuildTroopsButton(false);
            }

            //pass round
            if (Input.GetKeyDown(KeyCode.Return)) {
                passRound(true); 
            }
        }
    }

    public void passRound (bool cycleSoldiers) {
        if (roundPassDelay <= 0) {
            round++;
            buildTroopsButtonDisable();
            if (selectedTile != null)
                selectedTile.setDefaultMaterial();
            selectedTile = null;
            setBuildTroopsButton(false);
            roundPassDelay = 2f;
            deSelectSoldier();
            if (cycleSoldiers)
                StartCoroutine(soldierCycle());

            //add money to countries, health to occupied cities & inhabitants
            playerCashProfit = 0;
            playerIndustryProfit = 0; 
            foreach (City i in cities) {
                if (i != null) {
                    if (i.tile.isOccupied && i.citySoldier.health < i.maxHealth) {
                        i.citySoldier.health += 20;
                        if (i.citySoldier.health > i.maxHealth)
                            i.citySoldier.health = i.maxHealth;
                        i.tile.occupier.health += i.cityLevel * 4f;
                        if (i.tile.occupier.health > i.tile.occupier.maxHealth)
                            i.tile.occupier.health = i.tile.occupier.maxHealth; 
                    }
                    foreach (CountryData j in countries) {
                        if (j.name.Equals(i.country)) {
                            if (cycleSoldiers) {
                                if (findCountryAlliance(playerCountryName, i.country)) {
                                    j.cash += i.cityLevel * (int)10.0f;
                                    j.industry += i.cityIndustrial * (int)7.0f;
                                } else {
                                    j.cash += i.cityLevel * (int)15.0f;
                                    j.industry += i.cityIndustrial * (int)10.0f;
                                }
                            }
                            break;
                        }
                    }
                    if (i.country.Equals(playerCountryName) && cycleSoldiers) {
                        playerCountry.cash += i.cityLevel * (int)15.0f;
                        playerCountry.industry += i.cityIndustrial * (int)10.0f;
                        playerCashProfit += i.cityLevel * (int)15.0f;
                        playerIndustryProfit += i.cityIndustrial * (int)10.0f;
                    }
                }
            }
        }
    }
    void setBuildTroopsButton (bool toggle) {
        if (toggle) {
            buildTroopsButton.enabled = true;
            buildTroopsButton.GetComponent<Image>().enabled = true;
            buildTroopsButton.transform.GetChild(0).GetComponent<Text>().enabled = true;
        } else {
            buildTroopsButton.enabled = false;
            buildTroopsButton.GetComponent<Image>().enabled = false;
            buildTroopsButton.transform.GetChild(0).GetComponent<Text>().enabled = false;
        }
    }
    IEnumerator soldierCycle () {
        foreach (Soldier i in aiSoldiers) {
            if (i != null) {
                i.aiMovement();
                roundPassDelay = 2f; 
            }
            yield return new WaitForSeconds(0.02f); 
        }
        yield return new WaitForSeconds(0.1f); 
    }
    void deSelectSoldier() {
        if (selectedSoldier != null) {
            selectedSoldier.selected = false;
            selectedSoldier.returnTiles();
            selectedSoldier = null;
        }
    }
}
