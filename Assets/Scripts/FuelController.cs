using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelController : MonoBehaviour
{
    private static FuelController _instance;
    public static FuelController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    [SerializeField] private DashboardFuelBar fuelBar;

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
        fuelCostPerGear = new Dictionary<Gear, float>
        {
            { Gear.zero,    0f },
            { Gear.first,   0.2f },
            { Gear.second,  0.6f },
            { Gear.third,   1.2f },
            { Gear.fourth,  2f },
            { Gear.fifth,   3f }
        };
    }

    private void Update()
    {
        if (GameManager.Instance.IsRunning)
        {
            if (!ShipSpeedController.Instance.IsBoosting)
            {
                float newValue = AmountOfFuel -
                                (fuelCostPerGear[EngineController.Instance.CurrentGear] * Time.deltaTime) -
                                (EngineController.Instance.EngineCooling ? GlobalGameplayVariables.Instance.EngineCoolingCostPerSecond * Time.deltaTime : 0);
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
