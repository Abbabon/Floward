using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public enum Gear
{
    zero=0,
    first=1,
    second=2,
    third=3,
    fourth=4,
    fifth=5
}

//this controller gets pumping from the user, adds to the engine, and 
public class EngineController : MonoBehaviour
{
    private static EngineController _instance;
    public static EngineController Instance { get { return _instance; } }

    private static readonly object padlock = new object();

    public Gear CurrentGear;
    public float HeatLevel; // between 0 and 100, starting at 0
    public float OverheatLevel; // between 0 and 100, starting 90

    public bool EngineCooling; // set on/off by the Dashboard Cooling Switch
    public float CompressionTimeFactor = 1.6f;

    public bool EngineInShutdown = false;

    [FMODUnity.EventRef]
    public string _EngineSoundEventName;
    FMOD.Studio.EventInstance _engineSoundInstance;

    public DashboardEngineOverheatLevelMock overheatLevelIndicator;

    public Ship_ctrl ship_Ctrl;

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
                _engineSoundInstance = FMODUnity.RuntimeManager.CreateInstance(_EngineSoundEventName);
                //Here any additional initialization should occur:
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start(){
        HeatLevel = 0;
        OverheatLevel = GlobalGameplayVariables.Instance.MaxOverheat;
        EngineCooling = false;
        _engineSoundInstance.setParameterValue("Engine Level", 0);
        _engineSoundInstance.start();
    }

    //engine heat loss, adjust overheat level if necessary, add boost if necessary
    private float CompressorOpeningTimer = 0f;
    private float CompressorOverOpeningTimer = 0f;
    private void Update()
    {
        if (GameManager.Instance.IsRunning)
        {
            if (!(ShipSpeedController.Instance.IsBoosting))
            {
                float newHeatLevel = HeatLevel - GlobalGameplayVariables.Instance.HeatLossPerSecond * Time.deltaTime;
                HeatLevel = Mathf.Clamp(newHeatLevel, 0f, 100f);

                if (HeatLevel >= OverheatLevel)
                {
                    float newOverheatLevel = OverheatLevel - GlobalGameplayVariables.Instance.OverheatLossPerSecond * Time.deltaTime;
                    OverheatLevel = Mathf.Clamp(newOverheatLevel, 0f, 100f);

                    if (HeatLevel > 1){
                        BoostController.Instance.AddBoost();
                    }

                    if (OverheatLevel == 0)
                    {
                        ShutDownEngine();
                    }
                }

                if (EngineCooling)
                {
                    float newOverheatLevel = OverheatLevel + GlobalGameplayVariables.Instance.ActiveCoolingPerSecond * Time.deltaTime;
                    OverheatLevel = Mathf.Clamp(newOverheatLevel, 0f, GlobalGameplayVariables.Instance.MaxOverheat);

                    if (CompressorOpeningTimer < 1) {
                        CompressorOpeningTimer += Time.deltaTime;
                    }

                    if (OverheatLevel == GlobalGameplayVariables.Instance.MaxOverheat){
                        CompressorOverOpeningTimer += Time.deltaTime;

                        if (CompressorOverOpeningTimer > GlobalGameplayVariables.Instance.ActiveOverCoolingTimer)
                        {
                            FuelController.Instance.FuelDrop(GlobalGameplayVariables.Instance.FuelDropCoolingWhenCooledDown,
                                                             GlobalGameplayVariables.Instance.FuelDropCoolingWhenCooledDownHeatLoss);
                            CompressorOverOpeningTimer = 0f;
                        }
                    }

                    // the fuel cost is handled in the fuel controller for verbosity.
                }
                else
                {
                    if (CompressorOpeningTimer > 0){
                        CompressorOpeningTimer -= Time.deltaTime;
                    }
                    CompressorOverOpeningTimer = 0f;
                }
            }

            if (HeatLevel <= 20)
            {
                ChangeGear(Gear.zero);
            }
            else if (HeatLevel <= 30)
            {
                ChangeGear(Gear.first);
            }
            else if (HeatLevel <= 50)
            {
                ChangeGear(Gear.second);
            }
            else if (HeatLevel <= 70)
            {
                ChangeGear(Gear.third);
            }
            else if (HeatLevel <= 90)
            {
                ChangeGear(Gear.fourth);
            }
            else
            {
                ChangeGear(Gear.fifth);
            }

            // consider wind passive cooling:
            if (HeatLevel <= 0f)
            {
                float newOverheatLevel = OverheatLevel + GlobalGameplayVariables.Instance.PassiveCoolingCostPerSecondWhenNotHot * Time.deltaTime;
                OverheatLevel = Mathf.Clamp(newOverheatLevel, 0f, GlobalGameplayVariables.Instance.MaxOverheat);
            }
            else if (TutorialController.Instance.EnablePassiveCooling)
            {
                float newOverheatLevel = OverheatLevel +
                    (WindController.Instance.Direction() == WindDirection.FrontWind ? GlobalGameplayVariables.Instance.PassiveCoolingPerSecondFrontWind:
                                                                                     GlobalGameplayVariables.Instance.PassiveCoolingPerSecond)
                                                                                     * Time.deltaTime;
                OverheatLevel = Mathf.Clamp(newOverheatLevel, 0f, GlobalGameplayVariables.Instance.MaxOverheat);
            }

            RepresentEngineChanges();
        }
    }

    private void ChangeGear(Gear newGear)
    {
        if (newGear > CurrentGear)
        {
            SoundManager.Instance.PlayOneshotound("Engine Power Goes Up");
        }
        CurrentGear = newGear;
    }

    //called only from fuel controller - probably means a refactor is needed here...
    internal void HeatLoss(float heatLoss)
    {
        float newHeatLevel = HeatLevel - heatLoss;
        HeatLevel = Mathf.Clamp(newHeatLevel, 0f, 100f);
    }

    internal bool IsWorking()
    {
        return HeatLevel > 0;
    }



    bool flickerTurnOn = true;
    bool overOverheat = false;
    private void RepresentEngineChanges()
    {
        overheatLevelIndicator.Set(OverheatLevel);
        ship_Ctrl.overHeat = (100 - (OverheatLevel / GlobalGameplayVariables.Instance.MaxOverheat) * 100);

        if (HeatLevel > OverheatLevel){
            float OverHeatPercentage = 1 - (OverheatLevel / GlobalGameplayVariables.Instance.MaxOverheat);

            if (flickerTurnOn)
            {
                ship_Ctrl.alertLight = Mathf.Lerp(ship_Ctrl.alertLight, 1f, 0.4f * OverHeatPercentage);
                if (ship_Ctrl.alertLight >= 0.99f){
                    flickerTurnOn = false;
                }
            }
            else
            {
                ship_Ctrl.alertLight = Mathf.Lerp(ship_Ctrl.alertLight, 0f, 0.4f * OverHeatPercentage);
                if (ship_Ctrl.alertLight <= 0.01f)
                {
                    flickerTurnOn = true;
                }
            }

            if (!ship_Ctrl.overDrive)
                ship_Ctrl.overDrive = true;
            if (!overOverheat)
            {
                SoundManager.Instance.PlayOneshotound("Engine Overdrive");
                overOverheat = true;
            }
        }
        else{
            ship_Ctrl.alertLight = Mathf.Lerp(ship_Ctrl.alertLight, 0f, 0.1f);
            flickerTurnOn = true;

            if (ship_Ctrl.overDrive)
                ship_Ctrl.overDrive = false;
            if (overOverheat)
                overOverheat = false;
        }

        //Cooler opening:
        //TODO: consider adding a factor, so opening will not always take 1 second
        ship_Ctrl.compressS = (Mathf.Clamp(CompressorOpeningTimer, 0f, 1f) * CompressionTimeFactor);
        _engineSoundInstance.setParameterValue("Engine Level", HeatLevel / 100f);
    }

    public void PumpEngine(){
        if (!EngineInShutdown)
        {
            if (FuelController.Instance.AmountOfFuel == 0f){
                GameManager.Instance.StartGameOverSequence();
            }
            else
            {
                //if engine rises from 0 to anything more, add consume a bit more fuel:
                float newHeatLevel = HeatLevel + GlobalGameplayVariables.Instance.HeatPerPress;
                SoundManager.Instance.PlayOneshotound("Engine Tap");
                ship_Ctrl.tap = true;
                HeatLevel = Mathf.Clamp(newHeatLevel, 0f, 100f);
            }
        }

    }

    private void ShutDownEngine()
    {
        EngineInShutdown = true;
        ship_Ctrl.shutDown = true;
        HeatLevel = 0f;
        SoundManager.Instance.PlayOneshotound("Engine Shut Down");
        FuelController.Instance.FuelDrop(GlobalGameplayVariables.Instance.FuelDropEngineShutDown,
                                         GlobalGameplayVariables.Instance.FuelDropEngineShutDownHeatLoss);


        //TIMER?
        FunctionTimer.Create(() => ship_Ctrl.shutDown = false, 2f);
        FunctionTimer.Create(() => EngineInShutdown = false, GlobalGameplayVariables.Instance.MaxOverheat / GlobalGameplayVariables.Instance.PassiveCoolingCostPerSecondWhenNotHot);
    }

}
