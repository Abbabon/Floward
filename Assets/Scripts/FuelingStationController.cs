using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class FuelingStationController : SerializedMonoBehaviour
{
    [SerializeField] private Transform _entryPosition;
    [SerializeField] private List<FuelingStation> _fuelingStationPrefabs;
    private FuelingStation _currentStation;

    #region Singleton Implementation

    private static FuelingStationController _instance;
    public static FuelingStationController Instance { get { return _instance; } }
    private static readonly object padlock = new object();

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
    }

    #endregion 

    public void StartFuelingProcess(int fuelStationIndex){
        ShipSpeedController.Instance.EnterFuelingMode();
        InstantiateFuelStation(fuelStationIndex);
        FunctionTimer.Create(() => SoundManager.Instance.ChangeParameter("Petrol Station", 3f), 0.3f);
    }

    private void InstantiateFuelStation(int fuelStationIndex)
    {
        _currentStation = Instantiate(_fuelingStationPrefabs[fuelStationIndex], _entryPosition.position, Quaternion.identity);
    }

    //called from the FuelController when fueling is done; call the animation
    public void FuelingDone()
    {
        SoundManager.Instance.ChangeParameter("Fuel Tank Is Full", 1f);

        SoundManager.Instance.ChangeParameter("Fuel Pump In", 0f);
        SoundManager.Instance.ChangeParameter("Fuel Tank Is Filling", 0f);
        FunctionTimer.Create(() => SoundManager.Instance.ChangeParameter("Fuel Tank Is Full", 0f), 0.3f);
        if (_currentStation != null)
        {
            _currentStation.FuelingDone();
            ShipSpeedController.Instance.ExitFuelingMode();
            SoundManager.Instance.ChangeParameter("Petrol Station", 0f);
        }
    }

    internal bool FuelingStationAvailable()
    {
        return (_currentStation != null && !_currentStation._fueledOnce);
    }
}
