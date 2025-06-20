using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System;

public class Monkey : MonoBehaviour
{
    private int id = 1;
    private int lengthStart = 5;
    private float speedStart = 2.5f;

    public int maxLength;
    int remaining;

    public float generateSpeed;

    private Coroutine generationCoroutine;

    private int lengthUpgrade;
    private int speedUpgrade;

    public TMP_Text timeText;
    public TMP_Text lengthText;
    public TMP_Text infoText;
    public TMP_Text stringText;

    public string generatingString;

    // --------------- Starting Loop ------------------

    void Start() {
        Invoke(nameof(StartGenerating), 0.01f);

        maxLength = lengthStart + lengthUpgrade;
        generateSpeed = speedStart * Mathf.Pow(0.8f, speedUpgrade);
        
        UpdateUI();
    }

    void StartGenerating() {
        generationCoroutine = StartCoroutine(GenerateRepeatedlyCoroutine());
    }

    IEnumerator GenerateRepeatedlyCoroutine() {
        while (true) {
            if (generatingString.Length != maxLength) {
                GenerateCharacter(GameManager.Instance.Random, GameManager.Instance.Alphabet);
            }
            else {
                GameManager.Instance.AcceptMonkey(generatingString, id);
                generatingString = "";
            }
            yield return new WaitForSeconds(generateSpeed / maxLength);
        }
    }

    void StopGenerating() {
        if (generationCoroutine != null) {
            StopCoroutine(generationCoroutine);
            generationCoroutine = null;
        }
    }

    public void SetID (int newID) {
        id = newID;
    }

    // --------------------- Logic ------------------------
    public void GenerateCharacter(System.Random rnd, char[] availableCharacters) {
            generatingString += availableCharacters[rnd.Next(0, availableCharacters.Length)];
            UpdateStringText();
    }

    // --------------------- Upgrades ----------------------
    public void UpgradeLength() {
        if (GameManager.Instance.money < GameManager.Instance.GetLengthCost(lengthUpgrade)) return;

        GameManager.Instance.money -= GameManager.Instance.GetLengthCost(lengthUpgrade);
        lengthUpgrade++;
        maxLength++;
        UpdateUI();
    }

    public void UpgradeSpeed() {
        if (GameManager.Instance.money < GameManager.Instance.GetSpeedCost(speedUpgrade)) return;

        GameManager.Instance.money -= GameManager.Instance.GetSpeedCost(speedUpgrade);
        speedUpgrade++;
        generateSpeed *= 0.8f;
        UpdateUI();
    }

    // ---------------------- UI -------------------------
    void UpdateUI() {
        infoText.text = "Monkey: " + id.ToString() + "\nSpeed: " + (1f / generateSpeed).ToString("0.00") + " w/s\n" + "Length: " + maxLength.ToString(); 
        timeText.text = "Speed: " + (speedUpgrade + 1).ToString() + "\n£" + GameManager.Instance.GetSpeedCost(speedUpgrade).ToString("0.00");
        lengthText.text = "Length: " + (lengthUpgrade + 1).ToString() + "\n£" + GameManager.Instance.GetLengthCost(lengthUpgrade).ToString("0.00");
    }

    void UpdateStringText() {
        stringText.text = generatingString;
    }
}
