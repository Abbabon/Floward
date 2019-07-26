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

    [SerializeField] private float AmountOfFuel;

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
        if (GameManager.Instance.IsRunning)
        {
            if (!ShipSpeedController.Instance.IsBoosting)
            {
                float fuelDiff = (fuelCostPerGear[EngineController.Instance.CurrentGear]) +
                                (EngineController.Instance.EngineCooling ? GlobalGameplayVariables.Instance.ActiveCoolingCostPerSecond : 0);
                float newValue = AmountOfFuel - (fuelDiff * Time.deltaTime);

                _ship_ctrl.fuelUsage = (int)EngineController.Instance.CurrentGear + (EngineController.Instance.EngineCooling ? 1 : 0);

                AmountOfFuel = Mathf.Clamp(newValue, 0f,GlobalGameplayVariables.Instance.FuelCapacity );

                fuelBar.Set(AmountOfFuel / GlobalGameplayVariables.Instance.FuelCapacity);

                if (AmountOfFuel < 0.1f)
                {
                    //TODO: Trigger ending sequence!
                    GameManager.Instance.StartGameOverSequence();
                }
            }
        }
    }
}
