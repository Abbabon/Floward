using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;

public enum Gear
{
    zero=0,
    first=1,
    second=2,
    third=3
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

    //engine flicker:
    private float firstFlickerRange;
    private float secondFlickerRange;
    private float thirdFlickerRange;

    public delegate void OnEngineTap();
    public event OnEngineTap OnTap;

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
                //Here any additional initialization should occur:
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start(){
        HeatLevel = 0;
        OverheatLevel = GlobalGameplayVariables.Instance.MaxOverheat;
        EngineCooling = false;

        float[] keys = GlobalGameplayVariables.Instance.OverheatLampFrequency.Keys.ToArray<float>();
        firstFlickerRange = keys[0];
        secondFlickerRange = keys[1];
        thirdFlickerRange = keys[2];

    }

    //engine heat loss, adjust overheat level if necessary, add boost if necessary
    private float CompressorOpeningTimer = 0f;
    private float CompressorOverOpeningTimer = 0f;
    private void Update()
    {
        if (GameManager.Instance.IsRunning)
        {
            if (!(ShipSpeedController.Instance.IsBoosting) && !(ShipSpeedController.Instance.InStation))
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

            if (HeatLevel <= GlobalGameplayVariables.Instance.GearOneHeatThreshold)
            {
                ChangeGear(Gear.zero);
            }
            else if (HeatLevel <= GlobalGameplayVariables.Instance.GearTwoHeatThreshold)
            {
                ChangeGear(Gear.first);
            }
            else if (HeatLevel <= GlobalGameplayVariables.Instance.GearThreeHeatThreshold)
            {
                ChangeGear(Gear.second);
            }
            else
            {
                ChangeGear(Gear.third);
            }

            //ship_Ctrl.gearS = ((int)CurrentGear);

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
        }
        RepresentEngineChanges();
    }

    private void ChangeGear(Gear newGear)
    {
        if (newGear > CurrentGear)
        {
            SoundManager.Instance.ChangeParameter("Engine Power Goes Up", 1f);
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
        ship_Ctrl.overHeat = (100f - (OverheatLevel / GlobalGameplayVariables.Instance.MaxOverheat) * 100f);

        if (HeatLevel > OverheatLevel){
            float OverHeatPercentage = ((1 - OverheatLevel / GlobalGameplayVariables.Instance.MaxOverheat) * 100f);
            float frequency = GetOverheatFlickerFrequency(OverHeatPercentage);

            SoundManager.Instance.ChangeParameter("Engine Overdrive", 1f);

            if (flickerTurnOn)
            {
                ship_Ctrl.alertLight += (frequency * Time.deltaTime) / 2;
                if (ship_Ctrl.alertLight >= 0.99f){
                    flickerTurnOn = false;
                }
            }
            else
            {
                ship_Ctrl.alertLight -= (frequency * Time.deltaTime) / 2; 
                if (ship_Ctrl.alertLight <= 0.01f)
                {
                    flickerTurnOn = true;
                }
            }

            if (!ship_Ctrl.overDrive)
                ship_Ctrl.overDrive = true;
            if (!overOverheat)
            {
                overOverheat = true;
            }
        }
        else{
            ship_Ctrl.alertLight = Mathf.Lerp(ship_Ctrl.alertLight, 0f, 0.1f);
            flickerTurnOn = true;

            SoundManager.Instance.ChangeParameter("Engine Overdrive", 0f);

            if (ship_Ctrl.overDrive)
                ship_Ctrl.overDrive = false;
            if (overOverheat)
                overOverheat = false;
        }

        //Cooler opening:
        ship_Ctrl.compressS = (Mathf.Clamp(CompressorOpeningTimer, 0f, 1f) * CompressionTimeFactor);
        SoundManager.Instance.ChangeParameter("Engine Power Levels", HeatLevel / 100f);
    }

    private float GetOverheatFlickerFrequency(float overHeatPercentage)
    {
        if (overHeatPercentage < firstFlickerRange){
            return GlobalGameplayVariables.Instance.OverheatLampFrequency[firstFlickerRange];
        }
        else if (overHeatPercentage < secondFlickerRange){
            return GlobalGameplayVariables.Instance.OverheatLampFrequency[secondFlickerRange];
        }
        else {
            return GlobalGameplayVariables.Instance.OverheatLampFrequency[thirdFlickerRange];
        }
    }

    public void PumpEngine(){
        if (!EngineInShutdown)
        {
            OnTap?.Invoke();

            if (!ShipSpeedController.Instance.IsFueling && !TutorialController.Instance.Froezen())

            if (FuelController.Instance.AmountOfFuel == 0f){
                if (!TutorialController.Instance.InTutorial)
                {
                    ship_Ctrl.end = true;
                    SoundManager.Instance.ChangeParameter("Timeline Control", 0.9f);
                    GameManager.Instance.TouchEnabled = false;
                }
            }
            else
            {
                //if engine rises from 0 to anything more, add consume a bit more fuel:
                float newHeatLevel = HeatLevel + GlobalGameplayVariables.Instance.HeatPerPress;
                SoundManager.Instance.ChangeParameter("Engine Tap", 1f);
                SoundManager.Instance.ChangeParameter("Engine Tap", 0f);
                ship_Ctrl.tap = true;
                HeatLevel = Mathf.Clamp(newHeatLevel, 0f, 100f);
            }
        }

    }

    private void ShutDownEngine()
    {
        if (TutorialController.Instance.EnableEngineCrash)
        {
            EngineInShutdown = true;
            ship_Ctrl.shutDown = true;
            HeatLevel = 0f;
            SoundManager.Instance.ChangeParameter("Engine Shut Down", 1f);
            FuelController.Instance.FuelDrop(GlobalGameplayVariables.Instance.FuelDropEngineShutDown,
                                             GlobalGameplayVariables.Instance.FuelDropEngineShutDownHeatLoss);


            //TIMER?
            FunctionTimer.Create(() => ship_Ctrl.shutDown = false, 2f);
            FunctionTimer.Create(() => EngineInShutdown = false, GlobalGameplayVariables.Instance.MaxOverheat / GlobalGameplayVariables.Instance.PassiveCoolingCostPerSecondWhenNotHot);
        }
    }

}
