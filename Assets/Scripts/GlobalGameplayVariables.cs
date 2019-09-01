using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GlobalGameplayVariables : SerializedMonoBehaviour
{
    private static GlobalGameplayVariables _instance;
    public static GlobalGameplayVariables Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    #region Variables

    [Header("ReadOnly")]
    [Tooltip("Should I Calculate these values?")]
    public bool CalculateReadOnlyValues = false;
    public float GearOneLitersPerMeter; 
    public float GearTwoLitersPerMeter; 
    public float GearThreeLitersPerMeter; 
    
    [Header("Wind")]
    [Tooltip("In Seconds")]
    public float WindChangeMin = 5f; //seconds
    [Tooltip("In Seconds")]
    public float WindChangeMax = 15f; //second
    [Tooltip("In Seconds")]
    public float SameWindDirectionAllowdFor = 30f;

    [Header("Fuel")]
    [Tooltip("In Liters")]
    public float FuelCapacity = 500f; //in liters
    [Tooltip("In Percentage")]
    public float BackgroundFuelConsumptionPerSecond = 1f;
    [Tooltip("Gear -> Fuel cost per second (*liters*)")]
    public Dictionary<Gear, float> GearToFuelCostPerSecond;
    [Tooltip("In <DistanceUnits>")]
    public List<float> FuelStationsLocations;
    [Tooltip("In Liters")]
    public float FuelAddedInFuelStation = 50f;
    [Tooltip("In Seconds")]
    public float FuelingDuration = 5f;
    [Tooltip("In PERCENTAGE")]
    public float LowFuelPoint = 0.2f;

    [Header("Fuel Drops")]
    [Tooltip("In Liters")]
    public float FuelDropEngineShutDown = 30f;
    [Tooltip("In Percentage")]
    public float FuelDropEngineShutDownHeatLoss = 30f;
    [Tooltip("In PERCENTAGE per second")]
    public float ActiveOverCoolingTimer = 2f; // in percentages
    [Tooltip("In Liters")]
    public float FuelDropCoolingWhenCooledDown = 30f;
    [Tooltip("In Percentage")]
    public float FuelDropCoolingWhenCooledDownHeatLoss = 30f;
    [Tooltip("In Liters")]
    public float FuelDropOpenSails = 30f;
    [Tooltip("In Percentage")]
    public float FuelDropOpenSailsHeatLoss = 30f;

    [Header("Sails")]
    [Tooltip("In PERCENTAGE per second")]
    public float DurabilityLossPerSecond = 2f; // in percentages!

    [Header("Engine")]
    [Tooltip("In PERCENTAGE")]
    public float MaxOverheat = 90f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float HeatLossPerSecond = 30f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float OverheatLossPerSecond = 0.5f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float ActiveCoolingPerSecond = 12f; // in percentages
    //[Tooltip("In Percentage per second")]
    //public float ActiveCoolingCostPerSecond = 1f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float PassiveCoolingPerSecond = 2.5f; // in percentages
    [Tooltip("In PERCENTAGE per second")]
    public float PassiveCoolingPerSecondFrontWind = 5f; // in percentages
    [Tooltip("In Percentage per second")]
    public float PassiveCoolingCostPerSecondWhenNotHot = 10f; // in percentages
    [Tooltip("In PERCENTAGE per press")]
    public float HeatPerPress = 10f; // in percentages
    [Tooltip("In PERCENTAGES (out of a 100)")]
    public float GearOneHeatThreshold = 20f; // in percentages
    [Tooltip("In PERCENTAGES (out of a 100)")]
    public float GearTwoHeatThreshold = 50f; // in percentages
    [Tooltip("In PERCENTAGES (out of a 100)")]
    public float GearThreeHeatThreshold = 80f; // in percentages

    [Header("Engine - Visuals")]
    [Tooltip("Overheat Percentage -> # of flickers per second")]
    public Dictionary<float, float> OverheatLampFrequency;

    [Header("Boost")]
    [Tooltip("In PERCENTAGE")]
    public float BoostThreshold = 90f; //in percentage
    [Tooltip("In Seconds")]
    public float NormalBoostTime = 10f; //in seconds
    [Tooltip("In PERCENTAGE")]
    public float BoostPerSecondInOverdrive = 2f; //in percentage
    [Tooltip("In PERCENTAGE")]
    public float BoostPerSecondInOverdriveInTutorial = 12f; //in percentage
    [Tooltip("In miles; will be added over time in addition to speed 100")]
    public float BoostAddedDistance = 2000f; //in percentage

    [Header("Clouds")]
    [Tooltip("In WorldUnits per second")]
    public float MinimumCloudSpeed;
    [Tooltip("In WorldUnits per second")]
    public float MaxCloudSpeed;

    [Header("Speed")]
    [Tooltip("int -> speed units")]
    public Dictionary<int, float> WindStrengthToSpeed;
    [Tooltip("In Speed Units")]
    public float MaxSpeed = 200f;
    [Tooltip("In Speed Units")]
    public float MaxSpeedWithoutBoost = 100f;
    [Tooltip("In Speed Units [distance/second]")]
    public float BackMovementPerSecond = 5f;
    [Tooltip("Gear -> Speed Units")]
    public Dictionary<Gear, float> EngineGearToSpeedWeight;
    [Tooltip("In seconds fraction")]
    public float AccelerationRate = 0.1f; //parameter used for Math.Lerp of the CurrentSpeed to TargetSpeed;

    [Header("Mileage")]
    [Tooltip("distance units")]
    public float DistanceToOpenSky = 800f;

    [Header("Touch")]
    [Tooltip("In pixels")]
    public float SwipeMagnitude = 75f;

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

    private void Update()
    {
        if (CalculateReadOnlyValues)
        {
            GearOneLitersPerMeter = GearToFuelCostPerSecond[Gear.first] / EngineGearToSpeedWeight[Gear.first];
            GearTwoLitersPerMeter = GearToFuelCostPerSecond[Gear.second] / EngineGearToSpeedWeight[Gear.second];
            GearThreeLitersPerMeter = GearToFuelCostPerSecond[Gear.third] / EngineGearToSpeedWeight[Gear.third];
        }   
    }
}
