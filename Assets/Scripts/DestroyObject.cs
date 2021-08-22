using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class DestroyObject : MonoBehaviour {
    public void destroyObject(GameObject target) {
        Destroy(target.gameObject);
    }
    public void finishGame() {
        PlayerPrefs.SetString("save_" + GameObject.Find("Controller").GetComponent<Controller>().year.ToString(), "");
        SceneManager.LoadScene(0);
    }
}
