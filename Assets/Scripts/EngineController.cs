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

    private bool EngineInShutdown = false;

    //TODO: change when mockup is replaced:
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
                //Here any additional initialization should occur:
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start(){
        HeatLevel = 0;
        OverheatLevel = GlobalGameplayVariables.Instance.MaxOverheat;
        EngineCooling = false;
    }

    //engine heat loss, adjust overheat level if necessary, add boost if necessary
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

                    // the fuel cost is handled in the fuel controller for verbosity.
                }
                else
                {
                    if (CompressorOpeningTimer > 0){
                        CompressorOpeningTimer -= Time.deltaTime;
                    }
                }
            }

            if (HeatLevel <= 20)
            {
                CurrentGear = Gear.zero;
            }
            else if (HeatLevel <= 30)
            {
                CurrentGear = Gear.first;
            }
            else if (HeatLevel <= 50)
            {
                CurrentGear = Gear.second;
            }
            else if (HeatLevel <= 70)
            {
                CurrentGear = Gear.third;
            }
            else if (HeatLevel <= 90)
            {
                CurrentGear = Gear.fourth;
            }
            else
            {
                CurrentGear = Gear.fifth;
            }

            // consider wind passive cooling:
            if (HeatLevel <= 0f)
            {
                float newOverheatLevel = OverheatLevel + GlobalGameplayVariables.Instance.PassiveCoolingCostPerSecondWhenNotHot * Time.deltaTime;
                OverheatLevel = Mathf.Clamp(newOverheatLevel, 0f, GlobalGameplayVariables.Instance.MaxOverheat);
            }
            else if (TutorialController.Instance.EnablePassiveCooling && WindController.Instance.Direction() == WindDirection.FrontWind)
            {
                float newOverheatLevel = OverheatLevel + GlobalGameplayVariables.Instance.PassiveCoolingPerSecond * Time.deltaTime;
                OverheatLevel = Mathf.Clamp(newOverheatLevel, 0f, GlobalGameplayVariables.Instance.MaxOverheat);
            }

            RepresentEngineChanges();
        }
    }

    private float CompressorOpeningTimer = 0f;
    private void RepresentEngineChanges()
    {
        overheatLevelIndicator.Set(OverheatLevel);

        //TODO: implement flickering
        if (HeatLevel > OverheatLevel){
            ship_Ctrl.alertLight = Mathf.Lerp(ship_Ctrl.alertLight, 1f, 0.1f);
        }
        else{
            ship_Ctrl.alertLight = Mathf.Lerp(ship_Ctrl.alertLight, 0f, 0.1f);
        }

        //Cooler opening:
        //TODO: consider adding a factor, so opening will not always take 1 second
        ship_Ctrl.compressS = (Mathf.Clamp(CompressorOpeningTimer, 0f, 1f) * CompressionTimeFactor);
    }

    public void PumpEngine(){
        if (!EngineInShutdown)
        {
            //if engine rises from 0 to anything more, add consume a bit more fuel:
            float newHeatLevel = HeatLevel + GlobalGameplayVariables.Instance.HeatPerPress;
            SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Pump);
            ship_Ctrl.tap = true;
            HeatLevel = Mathf.Clamp(newHeatLevel, 0f, 100f);
        }

    }

    private void ShutDownEngine()
    {
        Debug.Log("SHUT DOWN");
        EngineInShutdown = true;
        ship_Ctrl.shutDown = true;
        HeatLevel = 0f;

        //TIMER?
        FunctionTimer.Create(() => ship_Ctrl.shutDown = false, 2f);
        FunctionTimer.Create(() => EngineInShutdown = false, GlobalGameplayVariables.Instance.MaxOverheat / GlobalGameplayVariables.Instance.PassiveCoolingCostPerSecondWhenNotHot);
    }

}
