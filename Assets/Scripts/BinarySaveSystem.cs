using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySaveSystem {
    public static void SaveMap(Controller controller) {
        Debug.Log("useSave");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/save_" + PlayerPrefs.GetInt("year").ToString() + ".sav", FileMode.Create);

        MapData data = new MapData(controller);

        bf.Serialize(stream, data);
        stream.Close();
    }
    //add integer parameter for different maps
    public static MapData LoadMap() {
        Debug.Log("useLoad");
        if (File.Exists(Application.persistentDataPath + "/save_" + PlayerPrefs.GetInt("year").ToString() + ".sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/save_" + PlayerPrefs.GetInt("year").ToString() + ".sav", FileMode.Open);
            MapData data = bf.Deserialize(stream) as MapData;
            stream.Close();
            return data;
        } else {
            return null;
        }
    }
}
[System.Serializable]
public class MyVector3 {
    float x;
    float y;
    float z;
    public MyVector3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public MyVector3(Vector3 position) {
        x = position.x;
        y = position.y;
        z = position.z;
    }
    public Vector3 toVector3() {
        return new Vector3(x, y, z);
    }
    public Quaternion toRotation() {
        return Quaternion.Euler(toVector3());
    }
}
[System.Serializable]
public class StorageSoldier {
    public MyVector3 position;
    public float health, experience;
    public string troopName, country;
    public bool moved, attacked; 
    public StorageSoldier(Soldier soldier) {
        position = new MyVector3(soldier.transform.position);
        health = soldier.health;
        troopName = soldier.troopName;
        country = soldier.country;
        moved = soldier.moved;
        attacked = soldier.attacked;
        experience = soldier.experience; 
    }
}
[System.Serializable]
public class MapData {
    public bool onlyEurope; 
    public int round; 
    public CountryData[] countryDatas;
    public CountryData playerCountry;
    public string[] tileCountries;
    public MyVector3[] tilePositions;
    public bool[] tileIsWaters; 
    //save & load according to spawn order
    public float[] cityHealths;
    public StorageSoldier[] playerSoldiers, aiSoldiers;
    public MapData(Controller controller) {
        round = controller.round; 
        countryDatas = controller.countries;
        tileCountries = controller.tileCountries;
        tilePositions = controller.tilePositions; 
        playerCountry = controller.playerCountry;
        tileIsWaters = controller.tileIsWaters;
        onlyEurope = controller.onlyEurope; 
        //save cities' health
        cityHealths = new float[controller.cities.Length];
        for (int i = 0; i < cityHealths.Length; i++) {
            if (controller.cities[i] != null)
                cityHealths[i] = controller.cities[i].citySoldier.health;
        }
        //save soldiers' information
        playerSoldiers = new StorageSoldier[controller.playerSoldiers.Length];
        aiSoldiers = new StorageSoldier[controller.aiSoldiers.Length];
        for (int i = 0; i < controller.playerSoldiers.Length; i++) {
            if (controller.playerSoldiers[i] != null) {
                playerSoldiers[i] = new StorageSoldier(controller.playerSoldiers[i]);
            }
        }
        for (int i = 0; i < controller.aiSoldiers.Length; i++) {
            if (controller.aiSoldiers[i] != null) {
                aiSoldiers[i] = new StorageSoldier(controller.aiSoldiers[i]);
            }
        }
    }
}