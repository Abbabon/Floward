using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneScript : MonoBehaviour
{
    public TextMeshProUGUI milesDisplay;
    public CanvasGroup retryPanel;
    public GameObject faderCanvas;
    public Animator faderAnimator;


    void Awake()
    {
        faderCanvas.SetActive(true);
    }

    // Fade In
    // Display The mileage
    // Fade in 'Try Again' button
    void Start()
    {
        milesDisplay.text = PlayerPrefs.GetInt("current_score").ToString().PadLeft(7,'0');
        FunctionTimer.Create(() => retryPanel.GetComponent<FadeInOut>().FadeIn(), 3f);
    }

    public void Restart()
    {
        //fade out
        faderAnimator.SetBool("Fade", true);

        //restart
        FunctionTimer.Create(() => FlowardSceneManager.Instance.LoadFloawardScene(FlowardScene.Gameplay), 1f);
    }
}
 