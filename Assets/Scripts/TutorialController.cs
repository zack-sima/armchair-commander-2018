using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking; 

public class TutorialController : MonoBehaviour {
    public Sprite[] englishSprites, chineseSprites;
    public Image image;
    int index; 

    public void addIndex () {
        if (index < chineseSprites.Length - 1)
            index++;
    }
    public void subtractIndex () {
        if (index > 0)
            index--; 
    }
    void Update () {
        if (PlayerPrefs.GetString("language") == "Chinese") {
            image.sprite = chineseSprites[index];
        } else {
            image.sprite = englishSprites[index];
        }
    }
    public void changeScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
}
