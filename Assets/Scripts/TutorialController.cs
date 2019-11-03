using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CodeMonkey.Utils;
using CodeMonkey.MonoBehaviours;
using System;

public class TutorialController : MonoBehaviour
{
    private static TutorialController _instance;
    public static TutorialController Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    [SerializeField] private TutorialTextBox tutorialTextBox;
    [SerializeField] private TutorialMarker tutorialMarker;
    [SerializeField] private List<TutorialPhase> tutorialPhases;
    [SerializeField] private List<FunctionTimer> phaseTimers;
    [SerializeField] private float numberOfTutorialPhases;
    
    [SerializeField] public int currentTutorialPhase = 0;
    [SerializeField] private bool inTutorial = false;

    [SerializeField] private CanvasGroup tutorialCanvasGroup;
    [SerializeField] private RectTransform pauseButton;
    [SerializeField] private RectTransform skipButton;

    [SerializeField] private bool OnBoostFullTutorialPassed;
    [SerializeField] private bool OnStrongWindTutorialPassed;

	[SerializeField] private Ship_ctrl _ship_ctrl;

    private bool StepCondition1Met;
    private bool StepCondition2Met;
    private bool StepCondition3Met;
    private bool StepCondition4Met;
    private bool StepCondition5Met;

    public bool InTutorial { get { return inTutorial; } }

    //freezes
    [SerializeField] public bool IsFreezingTap;
    [SerializeField] public bool IsFreezingSail;
    [SerializeField] public bool IsFreezingCooling;
    [SerializeField] public bool IsFreezingBoost;

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
                SetAllFeatures(true);

                //TODO: consider registering these only when the tutorial is active
                OnBoostFullTutorialPassed = (PlayerPrefs.GetInt("BoostTutorialPassed") == 77);
                BoostController.Instance.OnBoostFull += OnBoostFullTutorial;
                OnStrongWindTutorialPassed = (PlayerPrefs.GetInt("StrongWindTutorialPassed") == 77);
                WindController.Instance.OnWindChange += OnStrongWindTutorial;
                SailsController.Instance.OnSailsChange += OnStrongWindTutorial;

                SailsController.Instance.OnSailsChange += UnFreezeSceneSails;
                EngineController.Instance.OnTap += UnFreezeSceneTap;
                BoostController.Instance.OnBoostUsed += UnFreezeSceneBoost;
            }
        }
    }

    private void Start()
    {
        tutorialTextBox.GetComponent<CanvasGroup>().alpha = 0;
        tutorialMarker.GetComponent<CanvasGroup>().alpha = 0;
        skipButton.gameObject.SetActive(false);
    }

    #region FeatureFlags

    public bool EnableBoost;
    public bool EnableBoostHandle;
    public bool EnableBoostHandlePull;
    public bool EnableEnginePump;
    public bool EnableEngineCrash;
    public bool EnableCooling;
    public bool EnablePassiveCooling;
    public bool EnableSailsUp;
    public bool EnableSailsDown;
    public bool EnableWindChanges;
    public bool EnableFuelStations;

    public void SetAllFeatures(bool setting)
    {
        EnableFuelStations = EnableBoost = EnableBoostHandle = EnableBoostHandlePull = EnableEngineCrash = EnableEnginePump = EnablePassiveCooling = EnableCooling = EnableSailsUp = EnableSailsDown = EnableWindChanges = setting;
    }

    public void ShowTapToStartMessage()
    {
        tutorialTextBox.GetComponent<FadeInOut>().FadeIn();
        tutorialTextBox.SetTutorialMessage(tutorialPhases.First(x => x.PhaseID == 0));
    }

    public void HideTapToStartMessage()
    {
        tutorialTextBox.GetComponent<FadeInOut>().FadeOut();
    }

    #endregion


    public void StartTutorial(bool firstTime = false)
    {
        currentTutorialPhase = 0;
        inTutorial = true;
        pauseButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(!firstTime);

        SoundManager.Instance.ChangeParameter("Timeline Control", 0.1f);

        PlayerPrefs.SetInt("BoostTutorialPassed", 0);
        OnBoostFullTutorialPassed = false;
        PlayerPrefs.SetInt("StrongWindTutorialPassed", 0);
        OnStrongWindTutorialPassed = false;

        WindStateManager.Instance.SetTutorialMode();
        GameManager.Instance.ResetAllParameters();

        DashboardManager.Instance.TurnOffDashboard();
        DashboardManager.Instance.TurnOffFuelStationIndicator();

        SetAllFeatures(false);
        ExecuteTutorialPhase(currentTutorialPhase);
    }

    public void AdvanceTutorialPhase()
    {
        TurnOffTextBoxAndMarker();
        currentTutorialPhase++;
        //Debug.Log(String.Format("Tutorial Phase: {0}", currentTutorialPhase));

        FunctionTimer.StopAllTimersWithName(currentTutorialPhase.ToString());
        StepCondition1Met = false;
        StepCondition2Met = false;
        StepCondition3Met = false;
        StepCondition4Met = false;
        StepCondition5Met = false;
        UnFreezeSceneBoost();
        UnFreezeSceneCooling();
        UnFreezeSceneSails();
        UnFreezeSceneTap();

        foreach (var highlight in (Highlightables[])Enum.GetValues(typeof(Highlightables))){
            HighlightShipPart(highlight, false);
        }

        if (currentTutorialPhase >= numberOfTutorialPhases)
        { //ending tutorial
            EndTutorial();
            PlayerPrefs.SetInt("TutorialCompleted", 1);
        }
        else
        {
            ExecuteTutorialPhase(currentTutorialPhase);
        }
    }

    private void EndTutorial()
    {
        SetAllFeatures(true);

        TurnOffTextBoxAndMarker();
        UnFreezeSceneBoost();
        UnFreezeSceneSails();
        UnFreezeSceneTap();
        UnFreezeSceneCooling();

        SoundManager.Instance.ChangeParameter("Timeline Control", 1f);

        inTutorial = false;
        pauseButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(false);
    }

    public void SkipTutorial()
    {
        EndTutorial();
    }

    public void ExecuteTutorialPhase(int phaseID)
    {
        if (tutorialPhases.Any(x => x.PhaseID == phaseID))
        {
            //TODO: this could be more elegant and executed via the Scriptable object but TBH I don't think it's worth it to invest the time right now.
            switch (phaseID)
            {
                case 0:
                    StepOneStart();
                    break;
                case 1:
                    StepTwoStart();
                    break;
                case 2:
                    ThirdStepStart();
                    break;
                case 3:
                    FourthStepStart();
                    break;
                case 4:
                    FifthStepStart();
                    break;
                case 5:
                    SixthStepStart();
                    break;
                case 6:
                    SeventhStepStart();
                    break;
                case 7:
                    EighthStepStart();
                    break;
                case 8:
                    StepNineStart();
                    break;
                case 9:
                    StepTenStart();
                    break;
                case 10:
                    StepElevenStart();
                    break;
                case 11:
                    StepTwelveStart();
                    break;
                case 12:
                    StepThirteenStart();
                    break;
                case 13:
                    StepFourteenStart();
                    break;
                default:
                    break;
            }

        }

    }

    public void Update()
    {
        if (inTutorial)
        {
            if (IsFreezingTap)
                freezeDuration += Time.deltaTime;

            switch (currentTutorialPhase)
            {
                case 0:
                    StepOneUpdate();
                    break;
                case 1:
                    StepTwoUpdate();
                    break;
                case 2:
                    ThirdStepUpdate();
                    break;
                case 3:
                    FourthStepUpdate();
                    break;
                case 4:
                    FifthStepUpdate();
                    break;
                case 5:
                    SixthStepUpdate();
                    break;
                case 6:
                    SeventhStepUpdate();
                    break;
                case 7:
                    EigthStepUpdate();
                    break;
                case 8:
                    StepNineUpdate();
                    break;
                case 9:
                    StepTenUpdate();
                    break;
                case 10:
                    StepElevenUpdate();
                    break;
                case 11:
                    StepTwelveUpdate();
                    break;
                case 12:
                    StepThirteenUpdate();
                    break;
                case 13:
                    StepFourteenUpdate();
                    break;
                default:
                    break;
            }
        }
    }


    #region Tutorial Phases

    private void StepOneStart()
    {
        ShowTutorialTextBox();
        ShowTutorialMarker(TutorialMarker.MarkerAnimation.Tap);
        FunctionTimer.Create(() => EnableEnginePump = true, 0.5f, currentTutorialPhase.ToString());
    }

    private void StepOneUpdate()
    {
        if (!StepCondition1Met && EngineController.Instance.HeatLevel > 0)
        {
            StepCondition1Met = true;
            FunctionTimer.Create(AdvanceTutorialPhase, 0.5f, currentTutorialPhase.ToString());
        }
    }

    internal bool Froezen()
    {
        return IsFreezingTap || IsFreezingSail || IsFreezingCooling || IsFreezingBoost;
    }

    float stepTwoTimer;
    float heatWatch;
    private void StepTwoStart()
    {
        stepTwoTimer = 0f;
        heatWatch = EngineController.Instance.HeatLevel;
        DashboardManager.Instance.TurnOnDashboard();
    }

    private void StepTwoUpdate()
    {
        if (!StepCondition1Met)
        {
            heatWatch = EngineController.Instance.HeatLevel;
            if (EngineController.Instance.HeatLevel <= heatWatch){
                stepTwoTimer += Time.deltaTime;
            }
            else{
                stepTwoTimer = 0f;
            }

            if (stepTwoTimer > 4f || (int)EngineController.Instance.CurrentGear > 1)
            {
                ShowTutorialTextBox();
                ShowTutorialMarker(TutorialMarker.MarkerAnimation.Tap);
                FreezeSceneTap();
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met)
        {
            if (EngineController.Instance.HeatLevel >= GlobalGameplayVariables.Instance.MaxOverheat)
            {
                StepCondition2Met = true;
                FunctionTimer.Create(AdvanceTutorialPhase, 0.5f, currentTutorialPhase.ToString());
            }
        }
    }

    //this step is just a message!
    private void ThirdStepStart(){
        HighlightShipPart(Highlightables.Engine, true);
        FunctionTimer.Create(ShowTutorialTextBox, 1f, currentTutorialPhase.ToString());
        FunctionTimer.Create(FreezeSceneTap, 1f, currentTutorialPhase.ToString());
    }

    private void ThirdStepUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingTap) //waiting for freeze
            {
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met && !IsFreezingTap)
        {
            StepCondition2Met = true;
            FunctionTimer.Create(AdvanceTutorialPhase, 0.5f, currentTutorialPhase.ToString());
        }
    }

    private void FourthStepStart()
    {
        //go on go on
    }

    private void FourthStepUpdate()
    {
        if (!StepCondition1Met)
        {
            if (BoostController.Instance.boostPercentage > 30)
            {
                HighlightShipPart(Highlightables.Boost, true);
                ShowTutorialTextBox();
                FunctionTimer.Create(FreezeSceneTap, 1f, currentTutorialPhase.ToString());
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met)
        {
            if (IsFreezingTap) //waiting for freeze
            {
                StepCondition2Met = true;
            }
        }
        else if (!StepCondition3Met && !IsFreezingTap)
        {
            StepCondition3Met = true;
            FunctionTimer.Create(AdvanceTutorialPhase, 0.5f, currentTutorialPhase.ToString());
        }
    }

    float fifthStepTimer;
    private void FifthStepStart()
    {
        fifthStepTimer = 0f;
        EnableCooling = true;
        FunctionTimer.Create(ShowTutorialTextBox, 0.5f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => ShowTutorialMarker(TutorialMarker.MarkerAnimation.SwipeDown), 0.5f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => HighlightShipPart(Highlightables.Engine, true), 0.5f, currentTutorialPhase.ToString());
        FunctionTimer.Create(FreezeSceneCooling, 1f, currentTutorialPhase.ToString());
    }

    private void FifthStepUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingCooling) //waiting for freeze
            {
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met && EngineController.Instance.EngineCooling)
        {
            UnFreezeSceneCooling();
            StepCondition2Met = true;
        }
        else if (!StepCondition3Met)
        {
            if (EngineController.Instance.HeatLevel > EngineController.Instance.OverheatLevel)
            {
                if (!IsFreezingCooling) //dont count while frozen
                    fifthStepTimer += Time.deltaTime;

                if (fifthStepTimer > 2.5f) {
                    //change text
                    ShowTutorialTextBox(97);
                    FreezeSceneCooling();
                    StepCondition3Met = true;
                }
            }
            else if (EngineController.Instance.HeatLevel < EngineController.Instance.OverheatLevel) {
                StepCondition3Met = true;
            }
        }
        else if (!StepCondition4Met)
        {
            if (EngineController.Instance.EngineCooling){
                UnFreezeSceneCooling();
            }
            if (EngineController.Instance.OverheatLevel >= GlobalGameplayVariables.Instance.MaxOverheat)
            {
                StepCondition4Met = true;
                AdvanceTutorialPhase();
            }
        }
    }

    private void SixthStepStart()
    {
        ShowTutorialTextBox();
        HighlightShipPart(Highlightables.Boost, true);
        FunctionTimer.Create(FreezeSceneTap, 2f, currentTutorialPhase.ToString());
    }

    private void SixthStepUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingTap) //waiting for freeze
            {
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met && !IsFreezingTap)
        {
            StepCondition2Met = true;
        }
        else if(!StepCondition3Met || !StepCondition4Met)
        {
            if (!StepCondition3Met)
            {
                if (EngineController.Instance.OverheatLevel < 5)
                {
                    ShowTutorialTextBox(96);
                    FreezeSceneCooling();
                    StepCondition3Met = true;
                }
            }

            if (!StepCondition4Met)
            {
                if (EngineController.Instance.HeatLevel < EngineController.Instance.OverheatLevel)
                {
                    ShowTutorialTextBox(95);
                    FreezeSceneTap();
                    StepCondition4Met = true;
                }
            }

            if (BoostController.Instance.IsBoostAvailable())
            {
                StepCondition3Met = StepCondition4Met = true;
            }
        }
        else{
            StepCondition4Met = true;
            AdvanceTutorialPhase();
        }
    }

    private void SeventhStepStart()
    {
        FunctionTimer.Create(ShowTutorialTextBox, 0.5f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => EnableBoostHandle = true, 0.5f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => EnableBoostHandlePull = true, 2f, currentTutorialPhase.ToString());
        FunctionTimer.Create(FreezeSceneBoost, 2f, currentTutorialPhase.ToString());
    }

    private void SeventhStepUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingBoost) //waiting for freeze
            {
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met && !IsFreezingBoost)
        {
            StepCondition2Met = true;
            tutorialTextBox.GetComponent<FadeInOut>().FadeOut();
            FunctionTimer.Create(GameManager.Instance.OpenSky, 5f, currentTutorialPhase.ToString());
            FunctionTimer.Create(() => SoundManager.Instance.ChangeParameter("Timeline Control", 1f), 5f, currentTutorialPhase.ToString());
            FunctionTimer.Create(AdvanceTutorialPhase, GlobalGameplayVariables.Instance.NormalBoostTime + 5f, currentTutorialPhase.ToString());
        }
    }

    private void EighthStepStart()
    {
        FunctionTimer.Create(AdvanceTutorialPhase, 8f, currentTutorialPhase.ToString());
    }

    private void EigthStepUpdate()
    {
        //nothing to look at here....
    }

    //this step is just a message!
    private void StepNineStart()
    {
        FunctionTimer.Create(ShowTutorialTextBox, 0.5f, currentTutorialPhase.ToString());
        FunctionTimer.Create(FreezeSceneTap, 0.5f, currentTutorialPhase.ToString());
    }

    private void StepNineUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingTap){
                //waiting for freeze
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met)
        {
            if (!IsFreezingTap){
                StepCondition2Met = true;
                FunctionTimer.Create(AdvanceTutorialPhase, 1f, currentTutorialPhase.ToString());
            }
        }
    }

    //this step is just a message!
    private void StepTenStart()
    {
        WindController.Instance.ChangeState(2);
        HighlightShipPart(Highlightables.Flag, true);
        FunctionTimer.Create(ShowTutorialTextBox, 5f, currentTutorialPhase.ToString());
        FunctionTimer.Create(FreezeSceneTap, 5f, currentTutorialPhase.ToString());
    }

    private void StepTenUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingTap) //waiting for freeze
            {
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met)
        {
            if (!IsFreezingTap){
                HighlightShipPart(Highlightables.Flag, false);
                ShowTutorialMarker(TutorialMarker.MarkerAnimation.SwipeLeft);
                ShowTutorialTextBox(94);
                FreezeSceneSails();
                EnableSailsUp = true;
                StepCondition2Met = true;
            }
        }
        else if (!StepCondition3Met)
        {
            if (!IsFreezingSail)
            {
                ShowTutorialMarker(TutorialMarker.MarkerAnimation.SwipeLeft);
                StepCondition3Met = true;
                AdvanceTutorialPhase();
            }
        }
    }

    private void StepElevenStart()
    {
        FunctionTimer.Create(() => WindController.Instance.ChangeState(-4), 3f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => HighlightShipPart(Highlightables.Flag, true), 3f, currentTutorialPhase.ToString());

        FunctionTimer.Create(FreezeSceneSails, 7f, currentTutorialPhase.ToString());
        FunctionTimer.Create(ShowTutorialTextBox, 7f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => EnableSailsDown = true, 7f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => ShowTutorialMarker(TutorialMarker.MarkerAnimation.SwipeRight), 7f, currentTutorialPhase.ToString());
    }

    private void StepElevenUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingSail) //waiting for freeze
            {
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met)
        {
            if (!IsFreezingSail)
            {
                HighlightShipPart(Highlightables.Flag, false);
                StepCondition2Met = true;
                AdvanceTutorialPhase();
            }
        }
    }

    private void StepTwelveStart()
    {
        EnablePassiveCooling = true;
        EnableWindChanges = true;
        FunctionTimer.Create(ShowTutorialTextBox, 4f, currentTutorialPhase.ToString());
        FunctionTimer.Create(FreezeSceneTap, 4f, currentTutorialPhase.ToString());
        stepTweleveTimer = 0f;
    }

    float stepTweleveTimer;
    float stepTweleveTimeout = 7f;
    private void StepTwelveUpdate()
    {
        if (!StepCondition1Met)
        {
            if (IsFreezingTap){ //waiting for freeze
                StepCondition1Met = true;
            }
        }
        else if (!StepCondition2Met && !IsFreezingTap)
        {
            tutorialTextBox.GetComponent<FadeInOut>().FadeOut();
            if ((int)EngineController.Instance.CurrentGear > 1)
            {
                stepTweleveTimer += Time.deltaTime;
                if (stepTweleveTimer > stepTweleveTimeout)
                {
                    stepTweleveTimer = 0f;
                    ShowTutorialTextBox();
                    FreezeSceneTap();
                }
            }
            else{
                StepCondition2Met = true;
                tutorialTextBox.GetComponent<FadeInOut>().FadeOut();
                AdvanceTutorialPhase();
            }
        }
    }

    private void StepThirteenStart()
    {
        EnableFuelStations = true;
        DashboardManager.Instance.TurnOnFuelStationIndicator();
        ShipSpeedController.Instance.milesThisStation = 0;
        ShipSpeedController.Instance.nextFuelingStation = ShipSpeedController.Instance.miles + (GlobalGameplayVariables.Instance.FuelStationsLocations.First() / 5);
    }

    private void StepThirteenUpdate()
    {
        if (!StepCondition1Met)
        {
            if (FuelController.Instance.AmountOfFuel >= GlobalGameplayVariables.Instance.FuelCapacity - 10f)
            {
                StepCondition1Met = true;
                ShowTutorialTextBox();
                FreezeSceneTap();
            }
        }
        else if (!StepCondition2Met && !IsFreezingTap)
        {
            StepCondition2Met = true;
            AdvanceTutorialPhase();
        }
    }

    private void StepFourteenStart()
    {
        ShowTutorialTextBox();
        FreezeSceneTap();
    }

    private void StepFourteenUpdate()
    {
        if (!StepCondition1Met && !IsFreezingTap)
        {
            StepCondition1Met = true;
            AdvanceTutorialPhase();
        }
    }

    #endregion

    #region Tutorial Phase Helpers

    #region Freezes

    float freezeDuration;
    private void FreezeSceneTap()
    {
        if (!IsFreezingTap)
        {
            freezeDuration = 0f;
            IsFreezingTap = true;
            GameManager.Instance.IsRunning = false;
            GameManager.Instance.ShipAnimator.speed = 0;
        }
    }

    float freezeBuffer = 1f;
    private void UnFreezeSceneTap()
    {
        if (IsFreezingTap)
        {
            if (freezeDuration > freezeBuffer)
            {
                IsFreezingTap = false;
                GameManager.Instance.IsRunning = true;
                GameManager.Instance.ShipAnimator.speed = 1;
            }
        } 
    }
    private void FreezeSceneBoost()
    {
        if (!IsFreezingBoost)
        {
            IsFreezingBoost = true;
            GameManager.Instance.IsRunning = false;
            GameManager.Instance.ShipAnimator.speed = 0;
        }
    }

    private void UnFreezeSceneBoost()
    {
        if (IsFreezingBoost)
        {
            IsFreezingBoost = false;
            GameManager.Instance.IsRunning = true;
            GameManager.Instance.ShipAnimator.speed = 1;
        }
    }

    private void FreezeSceneSails()
    {
        if (!IsFreezingSail)
        {
            IsFreezingSail = true;
            GameManager.Instance.IsRunning = false;
            GameManager.Instance.ShipAnimator.speed = 0;
        }
    }

    private void UnFreezeSceneSails()
    {
        if (IsFreezingSail)
        {
            IsFreezingSail = false;
            GameManager.Instance.IsRunning = true;
            GameManager.Instance.ShipAnimator.speed = 1;
        }
    }

    private void FreezeSceneCooling()
    {
        if (!IsFreezingCooling)
        {
            IsFreezingCooling = true;
            GameManager.Instance.IsRunning = false;
            GameManager.Instance.ShipAnimator.speed = 0;
        }
    }

    private void UnFreezeSceneCooling()
    {
        if (IsFreezingCooling)
        {
            IsFreezingCooling = false;
            GameManager.Instance.IsRunning = true;
            GameManager.Instance.ShipAnimator.speed = 1;
        }
    }

    #endregion Freezes

    private void ShowTutorialTextBox()
    {
        SoundManager.Instance.ChangeParameter("Tutorial Message", 0.5f);
        tutorialTextBox.SetTutorialMessage(tutorialPhases.First(x => x.PhaseID == currentTutorialPhase));
        tutorialTextBox.GetComponent<FadeInOut>().FadeIn();
    }

    private void ShowTutorialTextBox(int specificPhaseID)
    {
        tutorialTextBox.SetTutorialMessage(tutorialPhases.First(x => x.PhaseID == specificPhaseID));
        tutorialTextBox.GetComponent<FadeInOut>().FadeIn();
    }

    //TODO: fade in
    private void ShowTutorialMarker(TutorialMarker.MarkerAnimation markerAnimation)
    {
        tutorialMarker.GetComponent<FadeInOut>().FadeIn();
        tutorialMarker.Animate(markerAnimation);
    }

    private void TurnOffTextBoxAndMarker()
    {
        SoundManager.Instance.ChangeParameter("Tutorial Message", 1f);
        tutorialTextBox.GetComponent<FadeInOut>().FadeOut();
        tutorialMarker.GetComponent<FadeInOut>().FadeOut();
    }

    private enum Highlightables
    {
        Flag, Engine, Fire, Boost, Tank
    }

    private void HighlightShipPart(Highlightables part, bool state)
    {
        switch (part)
        {
            case Highlightables.Boost:
                _ship_ctrl.hlBoost = state;
                break;
            case Highlightables.Engine:
                _ship_ctrl.hlEngine = state;
                break;
            case Highlightables.Fire:
                _ship_ctrl.hlFire = state;
                break;
            case Highlightables.Flag:
                _ship_ctrl.hlFlag = state;
                break;
            case Highlightables.Tank:
                _ship_ctrl.hlTank = state;
                break;
            default:
                break;
        }
    }

    #endregion

    #region ConditionalTutorials

    //no conditional tutorials.... for now.

    private void OnBoostFullTutorial()
    {
        if (!OnBoostFullTutorialPassed && !inTutorial)
        {
            //ShowTutorialTextBox(99);
            //FunctionTimer.Create(() => TurnOffTextBoxAndMarker(), 4f);

            OnBoostFullTutorialPassed = true;
            PlayerPrefs.SetInt("BoostTutorialPassed", 77);
        }
    }

    private void OnStrongWindTutorial()
    {
        if (!OnStrongWindTutorialPassed && !inTutorial)
        {
            if (WindController.Instance.State == 3 && SailsController.Instance.State == SailsState.SailsUp)
            {
                //ShowTutorialTextBox(98);
                //FunctionTimer.Create(() => TurnOffTextBoxAndMarker(), 4f);

                OnStrongWindTutorialPassed = true;
                PlayerPrefs.SetInt("StrongWindTutorialPassed", 77);
            }
        }
    }

    #endregion
}
