﻿using System.Collections;
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
        if (GameManager.Instance.IsRunning)
        {
            //Manage boost bar color: 
            if (boostPercentage >= GlobalGameplayVariables.Instance.BoostThreshold)
            {
                DashboardManager.Instance.TurnOnBoostPullie();
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
        float newValue = boostPercentage + (GlobalGameplayVariables.Instance.BoostPerSecondInOverdrive * Time.deltaTime);
        boostPercentage = Mathf.Clamp(newValue, 0f, 100f);
    }

    internal bool IsBoostAvailable()
    {
        return (boostPercentage > GlobalGameplayVariables.Instance.BoostThreshold);
    }

    private float boostDurationTimer;

    public void BoostHandlePulled()
    {
        if (!ShipSpeedController.Instance.IsBoosting && IsBoostAvailable())
        {
            if (boostPercentage >= GlobalGameplayVariables.Instance.BoostThreshold)
            {
                //TODO: make this an additive thing like tweening
                FunctionTimer.Create(() => StartCoroutine(CameraShake.Instance.Shake(1f, 0.02f)), 0f);
                FunctionTimer.Create(() => StartCoroutine(CameraShake.Instance.Shake(1.6f, 0.4f)), 1f);
                FunctionTimer.Create(() => StartCoroutine(CameraShake.Instance.Shake(3.4f, 0.08f)), 2.6f);
                FunctionTimer.Create(() => StartCoroutine(CameraShake.Instance.Shake(5f, 0.04f)), 6f);
                
                //Handheld.Vibrate();

                //DashboardManager.Instance.TurnOffDashboard();
                DashboardManager.Instance.TurnOffBoostPullie();
                SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Boost);
                ShipSpeedController.Instance.StartBoosting();
            }

            //TODO: animate emptying...
            //TODO: sound of negative feedback if the handle was used when tank not critical:
            boostPercentage = 0;
        }
    }
}
