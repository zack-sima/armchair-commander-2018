using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageAdapter : MonoBehaviour {
    public string chineseTranslation;
    private string englishText; 
    public bool constantlyUpdate;
    void Start() {
        //attached gameObject must have a UI text script
        englishText = GetComponent<Text>().text;
        checkLanguage(); 
    }
    void checkLanguage () {
        switch (PlayerPrefs.GetString("language")) {
        case "Chinese":
            GetComponent<Text>().text = chineseTranslation;
            break;
        default:
            GetComponent<Text>().text = englishText;
            break;
        }
    }
    void Update () {
        if (constantlyUpdate) {
            checkLanguage(); 
        }
    }
}
