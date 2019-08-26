using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShipSpeedController : MonoBehaviour
{
    private static ShipSpeedController _instance;
    public static ShipSpeedController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    public float SpeedMultiplier;

    public bool IsBoosting = false;
    public bool IsBoostingAnimation = false; // this is set by ship_ctrl when boost animation entered critical phase
    public bool IsFueling = false;
    public bool InStation = false;
    public float TargetSpeed = 0f; // in KMph, between 0 and 100
    public float CurrentSpeed = 0f; // in KMph, between 0 and 100
    public float ShipAcceleration = 0f;

    public CinemachineVirtualCamera virtualCamera;

    public AudioSource audioSource;

    public Ship_ctrl ship_Ctrl;

    public float miles;
    public float milesThisStation;
    public TextMeshProUGUI milesDisplay;

    public float nextFuelingStation;
    public int fuelStationIndex;

    public Transform ShipTransform;

    //petrol stations UI:
    [SerializeField] public RectTransform _shipLocationUI;
    [SerializeField] public RectTransform _petrolLocationStartUI;
    [SerializeField] public RectTransform _petrolLocationUI;
    private float _petrolLocationDistances;

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
                InitializeCachedVariables();
            }
        }
    }

    private void InitializeCachedVariables()
    {
        CameraUnit = MaxCameraDistance - MinCameraDistance;
        SpeedMultiplier = 1f;
        miles = 0;
        milesThisStation = 0f;
        ShipAcceleration = GlobalGameplayVariables.Instance.AccelerationRate;
        nextFuelingStation = GlobalGameplayVariables.Instance.FuelStationsLocations.First();
        fuelStationIndex = 0;
        _petrolLocationDistances = Math.Abs(_shipLocationUI.localPosition.x) + Math.Abs(_petrolLocationStartUI.localPosition.x);
    }

    // Update is called once per frame
    void Update()
    {
		if (GameManager.Instance.IsRunning)
		{
		    ManageSpeed();
            ManageClouds();
            ManageCamera();
            ManageAnimations();
            ManageMileage();
            ManageFuelStations();
        }
    }

    /* 
     * manage ship's speed (according to engine and sail): Formula:
     * (EngineGear * EngineWeight) +- (if sails up ? ) (direction) * (power)  : 0 
    */
    private void ManageSpeed()
    {
        float windEffect = (SailsController.Instance.SailsAttached ? (int)(SailsController.Instance.State) * (int)(WindController.Instance.Direction()) *  GlobalGameplayVariables.Instance.WindStrengthToSpeed[WindController.Instance.Strength()] : 0);
        float newSpeedValue = GlobalGameplayVariables.Instance.EngineGearToSpeedWeight[EngineController.Instance.CurrentGear] + windEffect;

        TargetSpeed = InStation ? 0 : IsBoostingAnimation ? GlobalGameplayVariables.Instance.MaxSpeed : Mathf.Clamp(newSpeedValue, 0f, GlobalGameplayVariables.Instance.MaxSpeedWithoutBoost);
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, TargetSpeed, ShipAcceleration);

        //TODO: add boosting animation to speedometer or something like that
        //if (IsBoostingAnimation){
        //}
        ship_Ctrl.speed = CurrentSpeed;
    }

    //set background scrolling speed (if needed - the clouds query it on their own)
    private void ManageClouds()
    {

    }

    [SerializeField] private float MaxCameraDistance = 8.5f;
    [SerializeField] private float MinCameraDistance = 6.6f;
    private float CameraUnit;

    private void ManageCamera()
    {
        //used when we had the ortho camera
        //DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, MinCameraDistance + (CurrentSpeed / GlobalGameplayVariables.Instance.MaxSpeedWithoutBoost) * CameraUnit, 0.5f);
    }

    //TODO: change to Km, according to the specs>?
    private void ManageMileage()
    {
        //TODO: consider boosting and add mileage delta according to boost duration and boost distance
        float targetMileageDelta = 0f;

        //handle back movement:
        if (IsBoostingAnimation)
        { 
            targetMileageDelta = CurrentSpeed + (GlobalGameplayVariables.Instance.BoostAddedDistance / GlobalGameplayVariables.Instance.NormalBoostTime);
        }
        else if (WindController.Instance.Direction() == WindDirection.FrontWind && CurrentSpeed == 0){
            targetMileageDelta = GlobalGameplayVariables.Instance.BackMovementPerSecond * WindController.Instance.Strength() * -1;
        }
        else{
            targetMileageDelta = CurrentSpeed;
        }

        float newMileage = miles + targetMileageDelta * Time.deltaTime;
        miles = Mathf.Max(newMileage, 0f);
        milesThisStation = milesThisStation + targetMileageDelta * Time.deltaTime;
        milesDisplay.text = ((int)miles).ToString().PadLeft(5, '0');

        if (TutorialController.Instance.EnableFuelStations)
        {
            //manage next fueling station indicator:
            _petrolLocationUI.localPosition = new Vector2(Mathf.Clamp(_petrolLocationStartUI.localPosition.x - (milesThisStation / GlobalGameplayVariables.Instance.FuelStationsLocations[fuelStationIndex]) * _petrolLocationDistances,
                                                                        _shipLocationUI.localPosition.x, _petrolLocationStartUI.localPosition.x),
                                                            _petrolLocationUI.localPosition.y);
        }
    }

    internal void StartBoosting(){
        IsBoosting = true;
        //?
    }

    internal void StopBoosting(){
        IsBoosting = false;
        //??
    }

    internal void EnterFuelingMode(){
        IsFueling = true;
        EngineController.Instance.HeatLevel = Mathf.Min(20f, EngineController.Instance.HeatLevel);
    }

    internal void EnteringStation()
    {
        InStation = true;
        SailsController.Instance.SetState(SailsState.SailsDown);
        EngineController.Instance.HeatLevel = 0;
        TargetSpeed = 0;
        CurrentSpeed = 0;
    }

    internal void ExitFuelingMode(){
        IsFueling = false;
        InStation = false;
        nextFuelingStation = miles + GlobalGameplayVariables.Instance.FuelStationsLocations[fuelStationIndex+1];
        milesThisStation = 0;
        fuelStationIndex++;
    }

    //TODO: consider moving ot its own script
    private void ManageAnimations()
    {
        float gear = 0f;
        if (EngineController.Instance.HeatLevel > 0f)
        {
            if (EngineController.Instance.HeatLevel > EngineController.Instance.OverheatLevel)//in overheat
            {
                gear = Mathf.Clamp(EngineController.Instance.HeatLevel / 100f, 0.26f, 1f);
            }
            else
            {
                gear = Mathf.Clamp(EngineController.Instance.HeatLevel / 100f, 0.26f, 0.79f);
            }
        }

        ship_Ctrl.gearS = gear;
        ship_Ctrl.boost = IsBoosting;
    }

    private void ManageFuelStations()
    {
        if (TutorialController.Instance.EnableFuelStations && !IsFueling && !EngineController.Instance.EngineInShutdown && miles >= nextFuelingStation){
            FuelingStationController.Instance.StartFuelingProcess();
            //the next station's location is deterimined when exiting the station
         
        }   
    }
}
