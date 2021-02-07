using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneScript : SerializedMonoBehaviour
{
    public TextMeshProUGUI milesDisplay;
    public CanvasGroup retryPanel;
    public GameObject faderCanvas;
    public Animator faderAnimator;
    public List<PlantSpot> plantsSpots;
    public TextMeshProUGUI[] milesDisplayDigits;

    void Awake()
    {
        faderCanvas.SetActive(true);
    }

    // Fade In
    // Display The mileage
    // Fade in 'Try Again' button
    void Start()
    {
        int todisplay = PlayerPrefs.GetInt("current_score");

        for (int i = 0; i < milesDisplayDigits.Length; i++)
        {
            int digit = todisplay % 10;
            milesDisplayDigits[i].text = $"{digit}";
            todisplay /= 10;
        }

        FunctionTimer.Create(() => retryPanel.GetComponent<FadeInOut>().FadeIn(), 3f);

        //handle plants:
        foreach (var plantSpot in plantsSpots){
            plantSpot.gameObject.SetActive(PlayerPrefs.GetInt(String.Format("Plant{0}", plantSpot.GetComponent<Plant>().PlantId)) == 1);
        }
    }

    public void Restart()
    {
        //fade out
        faderAnimator.SetBool("Fade", true);

        //restart
        FunctionTimer.Create(() => FlowardSceneManager.Instance.LoadFloawardScene(FlowardScene.Gameplay), 1f);
    }
}
 