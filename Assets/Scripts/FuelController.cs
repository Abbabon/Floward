using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class FuelController : MonoBehaviour
{
        private static FuelController _instance;
        public static FuelController Instance { get { return _instance; } }
        private static readonly object padlock = new object();

        [SerializeField] private DashboardFuelBar fuelBar;

        [SerializeField] private float _fuel_usage_animation_factor = 1.5f;
        [SerializeField] private Ship_ctrl _ship_ctrl;

        [SerializeField] internal float AmountOfFuel;

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

    public void Restart()
    {
        AmountOfFuel = GlobalGameplayVariables.Instance.FuelCapacity;
        fuelBar.Set(100f);
    }

    private void Start()
    {
        Restart();
    }

    private void Update()
    {
        if (GameManager.Instance.IsRunning && AmountOfFuel > 0)
        {
            if (!ShipSpeedController.Instance.IsBoosting)
            {
                float fuelDiff = 0;
                fuelDiff = (GlobalGameplayVariables.Instance.GearToFuelCostPerSecond[EngineController.Instance.CurrentGear]);

                if (fuelDiff <= 0.1f){
                    fuelDiff = GlobalGameplayVariables.Instance.BackgroundFuelConsumptionPerSecond;
                }

                float newValue = AmountOfFuel - (fuelDiff * Time.deltaTime);

                if (newValue > 0){
                    _ship_ctrl.fuelUsage = (int)EngineController.Instance.CurrentGear;
                }
                var minFuelAmount = TutorialController.Instance.InTutorial ? GlobalGameplayVariables.Instance.TutorialMinimumFuelAmount : 0f;
                AmountOfFuel = Mathf.Clamp(newValue, minFuelAmount, GlobalGameplayVariables.Instance.FuelCapacity);
            }
        }

        fuelBar.Set(AmountOfFuel / GlobalGameplayVariables.Instance.FuelCapacity);
        _ship_ctrl.lowFuel = (AmountOfFuel / GlobalGameplayVariables.Instance.FuelCapacity < GlobalGameplayVariables.Instance.LowFuelPoint);
    }

    //should be called with the parameter from the GlobalVariablesController
    public void FuelDrop(float amount, float heatLoss)
    {
        float newValue = AmountOfFuel - amount;
        AmountOfFuel = Mathf.Clamp(newValue, 0f, GlobalGameplayVariables.Instance.FuelCapacity);
        _ship_ctrl.fuelDrop = true;

        SoundManager.Instance.ChangeParameter("Fuel Drop", 1f);
        SoundManager.Instance.ChangeParameter("Fuel Drop", 0f);
        EngineController.Instance.HeatLoss(heatLoss);
    }

    public void AddFuel(){
        StartCoroutine(AddFuelCoroutine(
                        (GlobalGameplayVariables.Instance.FuelAddedInFuelStation / (GlobalGameplayVariables.Instance.FuelingDuration) * Time.deltaTime),
                        Mathf.Clamp(AmountOfFuel + GlobalGameplayVariables.Instance.FuelAddedInFuelStation, 0f, GlobalGameplayVariables.Instance.FuelCapacity)));
        SoundManager.Instance.ChangeParameter("Fuel Pump In", 1f);
        FunctionTimer.Create(() => SoundManager.Instance.ChangeParameter("Fuel Tank Is Filling", 1f), 0.5f);
    }

    private IEnumerator AddFuelCoroutine(float addedPerFrame, float endAmount)
    {
        for (float f = AmountOfFuel; f < endAmount; f += addedPerFrame)
        {
            float newValue = AmountOfFuel + addedPerFrame;
            AmountOfFuel = Mathf.Clamp(newValue, 0f, GlobalGameplayVariables.Instance.FuelCapacity);
            yield return null;
        }

        FuelingStationController.Instance.FuelingDone();
    }
}
