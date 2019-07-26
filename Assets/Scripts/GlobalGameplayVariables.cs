using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameplayVariables : MonoBehaviour
{
    private static GlobalGameplayVariables _instance;
    public static GlobalGameplayVariables Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    #region Variables

    [Header("Wind")]
    [Tooltip("In Seconds")]
    public float WindChangeMin = 5f; //seconds
    [Tooltip("In Seconds")]
    public float WindChangeMax = 15f; //seconds
    [Tooltip("In Speed Units")]
    public float SpeedWindWeight = 10f;

    [Header("Fuel")]
    [Tooltip("In Liters")]
    public float FuelCapacity = 500f; //in liters

    [Header("Sails")]
    [Tooltip("In PERCENTAGE per second")]
    public float DurabilityLossPerSecond = 2f; // in percentages!

    [Header("Engine")]
    [Tooltip("In Speed Units")]
    public float SpeedEngineWeight = 20f;
    [Tooltip("In PERCENTAGE")]
    public float MaxOverheat = 90f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float HeatLossPerSecond = 30f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float OverheatLossPerSecond = 0.5f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float ActiveCoolingPerSecond = 12f; // in percentages
    [Tooltip("In Percentage per second")]
    public float ActiveCoolingCostPerSecond = 1f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float PassiveCoolingPerSecond = 2.5f; // in percentages
    [Tooltip("In Percentage per second")]
    public float PassiveCoolingCostPerSecondWhenNotHot = 10f; // in percentages
    [Tooltip("In PERCENTAGE per press")]
    public float HeatPerPress = 10f; // in percentages

    [Header("Boost")]
    [Tooltip("In PERCENTAGE")]
    public float BoostThreshold = 90f; //in percentage
    [Tooltip("In Seconds")]
    public float NormalBoostTime = 10f; //in seconds
    [Tooltip("In PERCENTAGE")]
    public float BoostPerSecondInOverdrive = 2f; //in percentage

    [Header("Clouds")]
    [Tooltip("In WorldUnits per second")]
    public float MinimumCloudSpeed;
    [Tooltip("In WorldUnits per second")]
    public float MaxCloudSpeed;

    [Header("Speed")]
    [Tooltip("In Speed Units")]
    public float MaxSpeed = 150f;
    [Tooltip("In Speed Units")]
    public float MaxSpeedWithoutBoost = 100f;

    #endregion Variables

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
        DontDestroyOnLoad(this.gameObject);
    }
}
