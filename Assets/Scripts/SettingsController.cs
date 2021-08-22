using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SettingsController : MonoBehaviour {
    public void changeLanguage (string languageName) {
        PlayerPrefs.SetString("language", languageName);
    }
    public void changeScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
}

