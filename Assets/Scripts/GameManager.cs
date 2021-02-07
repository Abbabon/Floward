using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using Sirenix.OdinInspector;
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

    [SerializeField] private GameObject PauseFrame;
    [SerializeField] private GameObject CreditsFrame;

    [SerializeField] public Animator ShipAnimator;
    [SerializeField] private Animator faderAnimator;
    [SerializeField] private GameObject faderCanvas;

    [SerializeField] private GameObject headphonesObject;
    [SerializeField] private GameObject splashObject;

    [SerializeField] private WorldManager _worldManager;

    private bool _hasUsedEngine;
    private bool _hasTraveledMinimumDistance;

    private const float MINIMUM_DISTANCE_TO_MOVE_IN = 50f;

    private void Update()
    {
        HandleTapToStartMessage();
        HandleMoveInAnimation();
    }

    private void HandleMoveInAnimation()
    {
        if (_hasTraveledMinimumDistance)
        {
            return;
        }
        if (ShipSpeedController.Instance.miles > MINIMUM_DISTANCE_TO_MOVE_IN)
        {
            _worldManager.MoveIn();
            _hasTraveledMinimumDistance = true;
        }
    }

    private void HandleTapToStartMessage()
    {
        if (_hasUsedEngine)
        {
            return;
        }

        var finishedTutorial = PlayerPrefs.GetInt("TutorialCompleted") != 0;

        if (EngineController.Instance.HeatLevel > 12 &&
            finishedTutorial)
        {
            TutorialController.Instance.HideTapToStartMessage();
            DashboardManager.Instance.TurnOnDashboard();
            _hasUsedEngine = true;
        }
    }

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
                
                // some game-wide settings:
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private List<float> timelineValues = new List<float>() { 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f };
    private void Start()
    {
        //TODO: REMOVE! when not for phones anyway

        //Screen.SetResolution(750, 1334, false);

        ResetAllParameters();
        SoundManager.Instance.Play();

        if (SessionManager.Instance.FinishedOneRun)
        {
            splashObject.SetActive(false);
            headphonesObject.SetActive(false);
            _worldManager.Skip();
        }

        IsRunning = TouchEnabled = false;
        PauseCanvas.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        IsRunning = TouchEnabled = true;
        //StartCoroutine(FadeTo(0f, 1f));
        //MenuCanvas.interactable = false;
        var finishedTutorial = PlayerPrefs.GetInt("TutorialCompleted") != 0;
        if (finishedTutorial)
        {
            TutorialController.Instance.ShowTapToStartMessage();
        }
        else
        {
            InitializeGame();
        }
    }

    public void OnTappedStart()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (SessionManager.Instance.FinishedOneRun)
        {
            float timeline = timelineValues[UnityEngine.Random.Range(0, timelineValues.Count)];
            SoundManager.Instance.ChangeParameter("Timeline Control", timeline);
        }
        else
        {
            //TODO: persist tutorial done / not done!
            if (PlayerPrefs.GetInt("TutorialCompleted") == 0) // DIDNT FINISH TUTORIAL
            { 
                TutorialController.Instance.StartTutorial(true);
            }
            else
            {                                            // FINISHED TUTORIAL
                SoundManager.Instance.ChangeParameter("Timeline Control", 0.1f);
                FunctionTimer.Create(() => SoundManager.Instance.ChangeParameter("Timeline Control", 1f), 1f);
            }
        }
    }

    private bool _openedSky = false;
    public void OpenSky() {
        if (!_openedSky)
        {
            _worldManager.OpenSky();
            _openedSky = true;
        }
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
            SoundManager.Instance.ChangeParameter("Pause", 1f);
            IsRunning = TouchEnabled = false;
            ShipAnimator.speed = 0;
            CreditsFrame.SetActive(false);
            PauseCanvas.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.Instance.ChangeParameter("Pause", 0f);
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
        _openedSky = false;
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
        ShipSpeedController.Instance.fuelStationIndex = TutorialController.Instance.InTutorial ? 0 : 1;
        ShipSpeedController.Instance._petrolLocationUI.localPosition = ShipSpeedController.Instance._petrolLocationStartUI.localPosition;
        PlantsController.Instance.ResetState();
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
        SessionManager.Instance.FinishedOneRun = true;


        PlantsController.Instance.Serialize();

        faderAnimator.SetBool("Fade", true);
        FunctionTimer.Create(() => FlowardSceneManager.Instance.LoadFloawardScene(FlowardScene.Score), 5f);
    }

    [Button]
    private void ClearPlayerPrefs(){
        PlayerPrefs.DeleteAll();
    }

    [Button]
    private void PassedTutorial(){
        PlayerPrefs.SetInt("TutorialCompleted", 1);
    }

    [Button]
    private void FillBoost(){
        BoostController.Instance.boostPercentage = 100f;
    }

    [Button]
    private void EndRun(){
        GameManager.Instance.StartGameOverSequence();
    }

    public void Credits()
    {
        PauseFrame.SetActive(false);
        CreditsFrame.SetActive(true);
        SoundManager.Instance.ChangeParameter("Menu Buttons Click", 1f);
        SoundManager.Instance.ChangeParameter("Menu Buttons Click", 0f);
    }

    public void CreditsBack()
    {
        PauseFrame.SetActive(true);
        CreditsFrame.SetActive(false);
        SoundManager.Instance.ChangeParameter("Menu Buttons Click", 1f);
        SoundManager.Instance.ChangeParameter("Menu Buttons Click", 0f);
    }

    //reuse and reuse to infinity and beyond
    public void CreditsAmit(){
        Application.OpenURL("https://twitter.com/abbabon/");
    }

    public void CreditsYonatan(){
        Application.OpenURL("https://www.facebook.com/Hayonatan");
    }

    public void CreditsHadar()
    {
        Application.OpenURL("https://www.facebook.com/hadar.weinbergerbardavid");
    }

    public void CreditsMor()
    {
        Application.OpenURL("https://www.facebook.com/mor.sedero");
    }

    public void CreditsStav()
    {
        Application.OpenURL("https://www.facebook.com/stavstein");
    }
}
