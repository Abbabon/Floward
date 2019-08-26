using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public bool TouchEnabled { get; internal set; }
    public bool IsRunning { get; internal set; }

    private static readonly object padlock = new object();

    [SerializeField] private TouchController touchController;

    [SerializeField] private CanvasGroup MenuCanvas;
    [SerializeField] private CanvasGroup PauseCanvas;
    [SerializeField] private CanvasGroup TutorialCanvas;

    [SerializeField] public Animator ShipAnimator;
    [SerializeField] private Animator faderAnimator;
    [SerializeField] private GameObject faderCanvas;

    private void Awake()
    {
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                //Here any additional initialization should occur:
                faderCanvas.SetActive(true);
                
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Screen.SetResolution(750, 1334, false);

        IsRunning = TouchEnabled = false;
        PauseCanvas.gameObject.SetActive(false);
    }

    //TODO: enable, somehow, to get here from restart and have the fact we pressed 'start' persist.
    public void StartGame()
    {
        DashboardManager.Instance.TurnOnDashboard();
        IsRunning = TouchEnabled = true;
        StartCoroutine(FadeTo(0f, 1f));
        SoundManager.Instance.DisableRadioMuffle();
        SoundManager.Instance.PlayOneshotound("Menu Buttons Click");

        //TODO: persist tutorial done / not done!
        //TutorialController.Instance.StartTutorial();
        MenuCanvas.gameObject.SetActive(false);
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = MenuCanvas.alpha;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            MenuCanvas.alpha = Mathf.Lerp(alpha, aValue, t);
            yield return null;
        }
    }

    public void PauseGame()
    {
        if (IsRunning)
        {
            SoundManager.Instance.PlayOneshotound("Pause Button");
            IsRunning = TouchEnabled = false;
            ShipAnimator.speed = 0;

            PauseCanvas.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.Instance.PlayOneshotound("Menu Buttons Click");
            touchController.Reset();
            IsRunning = TouchEnabled = true;
            ShipAnimator.speed = 1;

            PauseCanvas.gameObject.SetActive(false);
        }
    }

    public void SkipButton()
    {
        //TODO: implement skip
        TutorialController.Instance.SkipTutorial();
    }

    public void Restart()
    {
        PauseGame();
        ResetAllParameters();
    }

    public void StartTutorial()
    {
        PauseGame();
        TutorialController.Instance.StartTutorial();
    }

    internal void ResetAllParameters()
    {
        ShipSpeedController.Instance.TargetSpeed = 0;
        ShipSpeedController.Instance.CurrentSpeed = 0;
        ShipSpeedController.Instance.miles = 0;
        ShipSpeedController.Instance.milesThisStation = 0f;
        ShipSpeedController.Instance.nextFuelingStation = GlobalGameplayVariables.Instance.FuelStationsLocations.First();
        ShipSpeedController.Instance.fuelStationIndex = 0;
        ShipSpeedController.Instance._petrolLocationUI.localPosition = ShipSpeedController.Instance._petrolLocationStartUI.localPosition;
        EngineController.Instance.EngineCooling = false;
        EngineController.Instance.HeatLevel = 0;
        EngineController.Instance.OverheatLevel = GlobalGameplayVariables.Instance.MaxOverheat;
        SailsController.Instance.SetState(SailsState.SailsDown);
        SailsController.Instance.SailsDurability = 100f;
        BoostController.Instance.boostPercentage = 0;
        FuelController.Instance.Restart();
        WindController.Instance.State = 0;

        //TODO: reset post processing as well.
    }

    public void StartGameOverSequence()
    {
        //TODO: save hiscore to player prefs (https://docs.unity3d.com/ScriptReference/PlayerPrefs.html)
        //TODO: OR - use https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html to encrypt it so its unexploitable

        PlayerPrefs.SetInt("current_score", (int)ShipSpeedController.Instance.miles);

        faderAnimator.SetBool("Fade", true);
        FunctionTimer.Create(() => FlowardSceneManager.Instance.LoadFloawardScene(FlowardScene.Score), 1f);
    }
}
