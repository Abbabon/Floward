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

    [SerializeField] private int currentTutorialPhase = 0;
    [SerializeField] private bool inTutorial = false;

    [SerializeField] private CanvasGroup tutorialCanvasGroup;
	[SerializeField] private RectTransform pauseButton;

	public bool InTutorial { get { return inTutorial; } }

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

            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        tutorialTextBox.GetComponent<CanvasGroup>().alpha = 0;
        tutorialMarker.GetComponent<CanvasGroup>().alpha = 0;
    }

    #region FeatureFlags

    public bool EnableBoost;
    public bool EnableBoostHandle;
    public bool EnableEnginePump;
    public bool EnableCooling;
    public bool EnablePassiveCooling;
    public bool EnableSailsUp;
    public bool EnableSailsDown;
    public bool EnableWindChanges;

    public void SetAllFeatures(bool setting)
    {
        EnableBoost = EnableBoostHandle = EnableEnginePump = EnablePassiveCooling = EnableCooling = EnableSailsUp = EnableSailsDown = EnableWindChanges = setting;
    }

    #endregion


    public void StartTutorial()
    {
        currentTutorialPhase = 0;
        inTutorial = true;
        tutorialCanvasGroup.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);

        WorldStateManager.Instance.SetTutorialMode();
        GameManager.Instance.ResetAllParameters();

        SetAllFeatures(false);
        ExecuteTutorialPhase(currentTutorialPhase);
    }

    public void AdvanceTutorialPhase()
    {
        TurnOffTextBoxAndMarker();
        currentTutorialPhase++;
        FunctionTimer.StopAllTimersWithName(currentTutorialPhase.ToString());
        PhaseEndConditionMet = false;

        if (currentTutorialPhase >= tutorialPhases.Count){
            SetAllFeatures(true);
            inTutorial = false;
            tutorialCanvasGroup.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
        else{
            ExecuteTutorialPhase(currentTutorialPhase);
        }
    }

    public void ExecuteTutorialPhase(int phaseID)
    {
        if (tutorialPhases.Any(x => x.PhaseID == phaseID))
        {
            //TODO: this could be more elegant and executed via the Scriptable object but TBH I don't think it's worth it to invest the time right now.
            switch (phaseID)
            {
                case 0:
                    ZeroPhaseStart();
                    break;
                case 1:
                    FirstPhaseStart();
                    break;
                case 2:
                    SecondPhaseStart();
                    break;
                case 3:
                    ThirdPhaseStart();
                    break;
                case 4:
                    FourthPhaseStart();
                    break;
                case 5:
                    FifthPhaseStart();
                    break;
                case 6:
                    SixthPhaseStart();
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
		    switch (currentTutorialPhase)
		    {
			    case 0:
				    ZeroPhaseUpdate();
				    break;
                case 1:
                    FirstPhaseUpdate();
                    break;
                case 2:
                    SecondPhaseUpdate();
                    break;
                case 3:
                    //ThirdPhaseUpdate(); //no update needed!
                    break;
                case 4:
                    FourthPhaseUpdate();
                    break;
                case 5:
                    FifthPhaseUpdate();
                    break;
                case 6:
                    //SixthPhaseUpdate(); //no upate needed!
                    break;
                default:
				    break;
		    }
        }
	}


    #region Tutorial Phases

    private bool PhaseEndConditionMet;

    private void ZeroPhaseStart()
    {
        ShowTutorialTextBox();
        ShowTutorialMarker(TutorialMarker.MarkerAnimation.Tap);
        FunctionTimer.Create(() => EnableEnginePump = true, 0.5f, currentTutorialPhase.ToString());
    }

	private void ZeroPhaseUpdate()
	{
        if (!PhaseEndConditionMet && EngineController.Instance.HeatLevel > 0)
        {
            PhaseEndConditionMet = true;
            FunctionTimer.Create(AdvanceTutorialPhase, 0.5f, currentTutorialPhase.ToString());
        }
	}

    private void FirstPhaseStart()
    {
        FunctionTimer.Create(ShowTutorialTextBox, 4f, currentTutorialPhase.ToString());
    }

    private void FirstPhaseUpdate()
    {
        if (!PhaseEndConditionMet && EngineController.Instance.HeatLevel > 90)
        {
            PhaseEndConditionMet = true;
            FunctionTimer.Create(AdvanceTutorialPhase, 3f, currentTutorialPhase.ToString());
        }
    }

    private void SecondPhaseStart()
    {
        FunctionTimer.Create(ShowTutorialTextBox, 1f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => ShowTutorialMarker(TutorialMarker.MarkerAnimation.SwipeDown), 1f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => EnableCooling = EnablePassiveCooling = true, 1f, currentTutorialPhase.ToString());
    }

    private void SecondPhaseUpdate()
    {

        if (!PhaseEndConditionMet && EngineController.Instance.EngineCooling)
        {
            if (EngineController.Instance.OverheatLevel == GlobalGameplayVariables.Instance.MaxOverheat)
            {
                PhaseEndConditionMet = true;
                FunctionTimer.Create(AdvanceTutorialPhase, 0.5f, currentTutorialPhase.ToString());
            }
        }
    }

    private void ThirdPhaseStart()
    {
        FunctionTimer.Create(ShowTutorialTextBox, 1f, currentTutorialPhase.ToString());
        //FunctionTimer.Create(() => tutorialFuelTankMarker.GetComponent<FadeInOut>().FadeIn(), 1f, currentTutorialPhase.ToString());
        //FunctionTimer.Create(() => tutorialFuelTankMarker.GetComponent<FadeInOut>().FadeOut(), 7f, currentTutorialPhase.ToString());
        FunctionTimer.Create(AdvanceTutorialPhase, 8f, currentTutorialPhase.ToString());
    }

    private void FourthPhaseStart()
    {
        //going to 2 wind speed
        FunctionTimer.Create(() => WindController.Instance.ChangeState(2), 0.5f, currentTutorialPhase.ToString());;
        FunctionTimer.Create(() => EnableSailsUp = true, 3f, currentTutorialPhase.ToString());
        FunctionTimer.Create(ShowTutorialTextBox, 3f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => ShowTutorialMarker(TutorialMarker.MarkerAnimation.SwipeLeft), 3f, currentTutorialPhase.ToString());
    }

    private void FourthPhaseUpdate()
    {
        if (!PhaseEndConditionMet && SailsController.Instance.State == SailsState.SailsUp)
        {
            PhaseEndConditionMet = true;
            FunctionTimer.Create(AdvanceTutorialPhase, 9f, currentTutorialPhase.ToString());
        }
    }

    private void FifthPhaseStart()
    {
        //going to -1 wind
        FunctionTimer.Create(() => WindController.Instance.ChangeState(-3), 0.5f, currentTutorialPhase.ToString()); ;
        FunctionTimer.Create(() => EnableSailsDown = true, 3f, currentTutorialPhase.ToString());
        FunctionTimer.Create(ShowTutorialTextBox, 3f, currentTutorialPhase.ToString());
        FunctionTimer.Create(() => ShowTutorialMarker(TutorialMarker.MarkerAnimation.SwipeRight), 3f, currentTutorialPhase.ToString());
    }

    private void FifthPhaseUpdate()
    {
        if (!PhaseEndConditionMet && SailsController.Instance.State == SailsState.SailsDown)
        {
            PhaseEndConditionMet = true;
            FunctionTimer.Create(AdvanceTutorialPhase, 3f, currentTutorialPhase.ToString());
        }
    }

    private void SixthPhaseStart()
    {
        //going to 0 wind speed
        FunctionTimer.Create(() => WindController.Instance.ChangeState(1), 0.5f, currentTutorialPhase.ToString()); ;
        FunctionTimer.Create(ShowTutorialTextBox, 3f, currentTutorialPhase.ToString());
        FunctionTimer.Create(AdvanceTutorialPhase, 8f, currentTutorialPhase.ToString());
    }

    #endregion

    #region Tutorial Phase Helpers

    //TODO: fade in
    private void ShowTutorialTextBox()
    {
        tutorialTextBox.SetTutorialMessage(tutorialPhases.First(x => x.PhaseID == currentTutorialPhase));
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
        tutorialTextBox.GetComponent<FadeInOut>().FadeOut();
        tutorialMarker.GetComponent<FadeInOut>().FadeOut();
    }

    #endregion
}
