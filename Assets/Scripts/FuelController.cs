using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FuelController : SerializedMonoBehaviour
{
    private static FuelController _instance;
    public static FuelController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    [SerializeField] private DashboardFuelBar fuelBar;

    [SerializeField] private float _fuel_usage_animation_factor = 1.5f;
    [SerializeField] private Ship_ctrl _ship_ctrl;

    [SerializeField] internal float AmountOfFuel;

    [SerializeField] private Dictionary<Gear, float> fuelCostPerGear;

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
                float fuelDiff = (fuelCostPerGear[EngineController.Instance.CurrentGear]) +
                                (EngineController.Instance.EngineCooling ? GlobalGameplayVariables.Instance.ActiveCoolingCostPerSecond : 0);
                float newValue = AmountOfFuel - (fuelDiff * Time.deltaTime);

                if (newValue > 0){
                    _ship_ctrl.fuelUsage = (int)EngineController.Instance.CurrentGear + (EngineController.Instance.EngineCooling ? 1 : 0);
                }
                AmountOfFuel = Mathf.Clamp(newValue, 0f, GlobalGameplayVariables.Instance.FuelCapacity);
                fuelBar.Set(AmountOfFuel / GlobalGameplayVariables.Instance.FuelCapacity);
            }
        }
    }

    //should be called with the parameter from the GlobalVariablesController
    public void FuelDrop(float amount, float heatLoss)
    {
        Debug.Log("Fuel Drop");
        float newValue = AmountOfFuel - amount;
        AmountOfFuel = Mathf.Clamp(newValue, 0f, GlobalGameplayVariables.Instance.FuelCapacity);
        _ship_ctrl.fuelDrop = true;

        SoundManager.Instance.PlayOneshotound("Fuel Drop");
        EngineController.Instance.HeatLoss(heatLoss);
    }
}
