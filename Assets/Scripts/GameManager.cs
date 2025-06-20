using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance {get{return instance;}}

    public float money;
    public float moneyValue;
    private float averageMoneyPerSecond = 3;
    public float moneySum;
    private float startTime;

    public TMP_Text moneyText;
    public TMP_Text averageMoneyText;
    public TMP_Text stringText;
    public TMP_Text containsText;
    public TMP_Text newMonkeyText;

    private char[] availableCharacters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    public char[] Alphabet {get{return availableCharacters;}}

    private System.Random rnd = new System.Random();
    public System.Random Random {get{return rnd;}}

    public List<GameObject> monkeyList = new List<GameObject>();

    public TextAsset dictionaryText;
    public HashSet<string> wordSet;

    private Coroutine updateText;
    public float UIupdateSpeed;

    float lastTime = 0f;

    public GameObject monkeyHolder;
    public GameObject monkeyPrefab;

    void Awake() {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        wordSet = new HashSet<string>();
        string[] lines = dictionaryText.text.Split('\n');
        
        foreach (string line in lines)
        {
            string cleanWord = line.Trim().ToLower();
            if (!string.IsNullOrEmpty(cleanWord))
            {
                wordSet.Add(cleanWord);
            }
        }

        startTime = Time.time;

        updateText = StartCoroutine(UpdateTextRepeatedlyCoroutine());

        AddNewMonkey();
    }

    // --------------------- UI ---------------------------
    public void AddValue(float value) {
        money += value;
        moneySum += value;
    }

    public float GetAverage() {
        return moneySum / (Time.time - startTime);
    }

    IEnumerator UpdateTextRepeatedlyCoroutine() {
        while (true) {
            AddValue(0f);
            averageMoneyPerSecond = GetAverage();
            moneyText.text = "£" + money.ToString("0.00");
            averageMoneyText.text = "£" + averageMoneyPerSecond.ToString("0.00") + "/s";
            yield return new WaitForSeconds(UIupdateSpeed);
        }
    }

    void StopUpdateText() {
        if (updateText != null) {
            StopCoroutine(updateText);
            updateText = null;
        }
    }

    // -------------------- Logic ----------------------
    public void AcceptMonkey(string monkeyReturn, int monkeyID) {
        List<string> monkeysWords = GetPoints(monkeyReturn);
        stringText.text = monkeyReturn;
        
        string containsString = "";
        int totalContainLength = 0;
        foreach(string i in monkeysWords) {
            containsString += i + " ";
            totalContainLength += i.Length;
        }
        containsText.text = containsString;

        if (monkeysWords.Count == 0) return;
        AddValue(moneyValue * Mathf.Pow(2, monkeysWords.Count - 1));
    }
    public List<string> GetPoints(string checkString) {
        List<string> contains = new List<string>();
        int strLen = checkString.Length;
        
        for (int i = 0; i < strLen; i++) {
            for (int j = 3; j <= Math.Min(10, strLen - i); j++) {
                string subString = checkString.Substring(i, j);
                if (wordSet.Contains(subString)) {
                    contains.Add(subString);
                }
            }
        }
        return contains;
    }

    public float GetLengthCost(int lengthLevel) {
        return Mathf.Pow(2, lengthLevel);
    }

    public float GetSpeedCost(int speedLevel) {
        return Mathf.Round(Mathf.Pow(1.5f, speedLevel) * 100) / 100;
    }

    public void BuyNewMonkey() {
        if (money < Mathf.Pow(10, monkeyList.Count)) return;

        money -= Mathf.Pow(10, monkeyList.Count);
        AddNewMonkey();

    }

    void AddNewMonkey() {
        GameObject newMonkey = Instantiate(monkeyPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        newMonkey.transform.parent = monkeyHolder.transform;
        monkeyList.Add(newMonkey);
        newMonkey.GetComponent<Monkey>().SetID(monkeyList.Count);

        newMonkeyText.text = "New Monkey: £" + Mathf.Pow(10, monkeyList.Count).ToString();
    }
}
