using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    //if inEurasia is true the country can play both Europe and Asia maps (e.g. Soviet Union)
    [HideInInspector]
    public bool isPacific, isEurasia; 
    public string[] availableCountries1939, availableCountries1941, availableCountries1944, availableCountries1945;
    //current input
    public Dropdown countryInput;
    public Dropdown countryInput1939, countryInput1941, countryInput1944, countryInput1945;
    public Dropdown yearInput;
    //transform input to actually read string (e.g. Chinese to string value)
    public string[] countryOptions1939, countryOptions1941, countryOptions1944, countryOptions1945, yearOptions;
    [HideInInspector]
    public string[] selectedCountryOptions; 
    //original input position
    Vector3 inputPosition; 
    public Image continueButton, europeButton, pacificButton; 
    bool savedData;
    int year; 

    private void Awake() {
        //translate manually for countries
        if (PlayerPrefs.GetString("language") == "Chinese") {
            countryInput1939.options[0].text = "德国";
            countryInput1939.options[1].text = "苏联";
            countryInput1939.options[2].text = "法国";
            countryInput1939.options[3].text = "英国";
            countryInput1939.options[4].text = "意大利";
            countryInput1939.options[5].text = "波兰";
            countryInput1939.options[6].text = "西班牙";
            countryInput1939.options[7].text = "芬兰";
            countryInput1939.options[8].text = "南斯拉夫";
            countryInput1939.options[9].text = "罗马尼亚";
            countryInput1939.options[10].text = "匈牙利";
            countryInput1939.options[11].text = "保加利亚";
            countryInput1939.options[12].text = "希腊";
            countryInput1939.options[13].text = "挪威";
            countryInput1939.options[14].text = "卢森堡";
            countryInput1939.options[15].text = "荷兰";
            countryInput1939.options[16].text = "中华民国";
            countryInput1939.options[17].text = "日本";
            countryInput1939.options[17].text = "蒙古";

            countryInput1941.options[0].text = "德国";
            countryInput1941.options[1].text = "苏联";
            countryInput1941.options[2].text = "英国";
            countryInput1941.options[3].text = "意大利";
            countryInput1941.options[4].text = "西班牙";
            countryInput1941.options[5].text = "芬兰";
            countryInput1941.options[6].text = "罗马尼亚";
            countryInput1941.options[7].text = "匈牙利";
            countryInput1941.options[8].text = "保加利亚";
            countryInput1941.options[9].text = "法国";
            countryInput1941.options[10].text = "中华民国";
            countryInput1941.options[11].text = "日本";
            countryInput1941.options[12].text = "美国";
            countryInput1941.options[13].text = "澳大利亚";
            countryInput1941.options[14].text = "印度";

            countryInput1944.options[0].text = "德国";
            countryInput1944.options[1].text = "苏联";
            countryInput1944.options[2].text = "美国";
            countryInput1944.options[3].text = "英国";
            countryInput1944.options[4].text = "意大利";
            countryInput1944.options[5].text = "西班牙";
            countryInput1944.options[6].text = "芬兰";
            countryInput1944.options[7].text = "罗马尼亚";
            countryInput1944.options[8].text = "匈牙利";
            countryInput1944.options[9].text = "保加利亚";

            countryInput1945.options[0].text = "德国";
            countryInput1945.options[1].text = "苏联";
            countryInput1945.options[2].text = "美国";
            countryInput1945.options[3].text = "英国";
            countryInput1945.options[4].text = "意大利";
        }
        inputPosition = countryInput1939.transform.position;
        switch (PlayerPrefs.GetInt("year")) {
        case 1939:
            yearInput.value = 0;
            break;
        case 1941:
            yearInput.value = 1;
            break;
        case 1944:
            yearInput.value = 2;
            break;
        case 1945:
            yearInput.value = 3;
            break;
        }
    }
    private void Update() {
        year = int.Parse(yearOptions[yearInput.value]); 
        switch (year) {
        case 1939:
            countryInput = countryInput1939;
            selectedCountryOptions = countryOptions1939;
            countryInput1939.transform.position = inputPosition;
            countryInput1941.transform.position = new Vector3(0, 3800, 0);
            countryInput1944.transform.position = new Vector3(0, 3800, 0);
            countryInput1945.transform.position = new Vector3(0, 3800, 0);

            //15 is the index for asian countries 
            if (countryInput1939.value > 15) {
                isPacific = true;
            } else
                isPacific = false;
            if (countryInput1939.value == 1)
                isEurasia = true;
            else
                isEurasia = false; 
            break; 
        case 1941:
            countryInput = countryInput1941;
            selectedCountryOptions = countryOptions1941;
            countryInput1941.transform.position = inputPosition;
            countryInput1939.transform.position = new Vector3(0, 3800, 0);
            countryInput1944.transform.position = new Vector3(0, 3800, 0);
            countryInput1945.transform.position = new Vector3(0, 3800, 0);

            //8 is the index for asian countries 
            if (countryInput1941.value > 8) {
                isPacific = true;
            } else
                isPacific = false;
            if (countryInput1941.value == 2)
                isEurasia = true;
            else
                isEurasia = false;
            break;
        case 1944:
            countryInput = countryInput1944;
            selectedCountryOptions = countryOptions1944;
            countryInput1944.transform.position = inputPosition;
            countryInput1941.transform.position = new Vector3(0, 3800, 0);
            countryInput1939.transform.position = new Vector3(0, 3800, 0);
            countryInput1945.transform.position = new Vector3(0, 3800, 0);
            break;
        case 1945:
            countryInput = countryInput1945;
            selectedCountryOptions = countryOptions1945;
            countryInput1945.transform.position = inputPosition;
            countryInput1941.transform.position = new Vector3(0, 3800, 0);
            countryInput1939.transform.position = new Vector3(0, 3800, 0);
            countryInput1944.transform.position = new Vector3(0, 3800, 0);
            break;
        }
        if (PlayerPrefs.GetString("save_" + year.ToString()) == "true") {
            savedData = true;
        } else
            savedData = false; 
        if (!savedData) {
            continueButton.GetComponent<Button>().enabled = false; 
            continueButton.color = new Color(1, 1, 1, 0.3f);
            continueButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 0.3f);
        } else {
            continueButton.GetComponent<Button>().enabled = true;
            continueButton.color = new Color(1f, 1, 1f, 1f);
            continueButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1f, 1f, 1f);
        }
        if (isEurasia) {
            europeButton.GetComponent<Button>().enabled = true;
            europeButton.color = new Color(1f, 1, 1f, 1f);
            europeButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1f, 1f, 1f);
            pacificButton.GetComponent<Button>().enabled = true;
            pacificButton.color = new Color(1f, 1, 1f, 1f);
            pacificButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1f, 1f, 1f);
        } else if (isPacific) {
            europeButton.GetComponent<Button>().enabled = false;
            europeButton.color = new Color(1f, 1, 1f, 0.3f);
            europeButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1f, 1f, 0.3f);
            pacificButton.GetComponent<Button>().enabled = true;
            pacificButton.color = new Color(1f, 1, 1f, 1f);
            pacificButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1f, 1f, 1f);
        } else {
            europeButton.GetComponent<Button>().enabled = true;
            europeButton.color = new Color(1f, 1, 1f, 1f);
            europeButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1f, 1f, 1f);
            pacificButton.GetComponent<Button>().enabled = true;
            pacificButton.color = new Color(1f, 1, 1f, 0.3f);
            pacificButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1f, 1f, 0.3f);
        }
    }
    public void loadScene (int index) {
        SceneManager.LoadScene(index);
    }
    public void loadGame (bool newGame) {
        PlayerPrefs.SetInt("year", year);
        if (newGame) {
            PlayerPrefs.SetString("playerCountry", selectedCountryOptions[countryInput.value]);
            PlayerPrefs.SetString("save_" + year.ToString(), "");
            PlayerPrefs.SetInt("onlyEurope_" + year.ToString(), 1);
            PlayerPrefs.SetInt("onlyEurope", 1);
            SceneManager.LoadScene(1);
        } else if (!savedData) {
            return;
        } else {
            if (PlayerPrefs.GetInt("onlyEurope_" + year.ToString()) == 1) {
                SceneManager.LoadScene(1);
            } else {
                SceneManager.LoadScene(4);
            }
        }

    }
    public void loadGameWholeMap () {
        PlayerPrefs.SetInt("year", year);
        PlayerPrefs.SetInt("onlyEurope", 0);
        PlayerPrefs.SetString("playerCountry", selectedCountryOptions[countryInput.value]);
        PlayerPrefs.SetInt("onlyEurope_" + year.ToString(), 0);
        PlayerPrefs.SetString("save_" + year.ToString(), "");
        SceneManager.LoadScene(4);
    }
}
