using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using CodeMonkey.Utils;

public class BoostController : MonoBehaviour
{
    private static BoostController _instance;
    public static BoostController Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    public float boostPercentage = 0f; //from 0 to 100 of course

    public delegate void BoostFull();
    public event BoostFull OnBoostFull;

    public delegate void BoostUsed();
    public event BoostUsed OnBoostUsed;

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

            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance.IsRunning && !ShipSpeedController.Instance.IsFueling)
        {
            //Manage boost bar color:   
            if (boostPercentage >= GlobalGameplayVariables.Instance.BoostThreshold)
            {
                if (TutorialController.Instance.EnableBoostHandle)
                {
                    DashboardManager.Instance.TurnOnBoostPullie();
                    OnBoostFull();
                }
            }

            if (ShipSpeedController.Instance.IsBoosting)
            {
                boostDurationTimer += Time.deltaTime;

                //TODO: handle super boost
                if (boostDurationTimer >= GlobalGameplayVariables.Instance.NormalBoostTime)
                {
                    DashboardManager.Instance.TurnOnDashboard();
                    ShipSpeedController.Instance.StopBoosting();
                }
            }
            else
            {
                boostDurationTimer = 0f;
            }
        }
    }

    internal void AddBoost()
    {
        float newValue = boostPercentage + ((TutorialController.Instance.InTutorial && TutorialController.Instance.currentTutorialPhase < 7 ?
                                             GlobalGameplayVariables.Instance.BoostPerSecondInOverdriveInTutorial :
                                             GlobalGameplayVariables.Instance.BoostPerSecondInOverdrive)
                                             * Time.deltaTime);
        boostPercentage = Mathf.Clamp(newValue, 0f, 100f);
    }

    internal bool IsBoostAvailable()
    {
        return (boostPercentage > GlobalGameplayVariables.Instance.BoostThreshold);
    }

    private float boostDurationTimer;

    public void BoostHandlePulled()
    {
        if (!ShipSpeedController.Instance.IsBoosting &&
            IsBoostAvailable() &&
            !EngineController.Instance.EngineInShutdown &&
            !FuelingStationController.Instance.FuelingStationAvailable() &&
            boostPercentage >= GlobalGameplayVariables.Instance.BoostThreshold)
        {

            DashboardManager.Instance.TurnOffBoostPullie();
            SoundManager.Instance.ChangeParameter("Activate Boost", 1f);
            ShipSpeedController.Instance.StartBoosting();
            OnBoostUsed?.Invoke();

            //TODO: animate emptying...
            //TODO: sound of negative feedback if the handle was used when tank not critical:
            boostPercentage = 0;
            BoostLightsController.Instance.Restart();
        }
    }
}
