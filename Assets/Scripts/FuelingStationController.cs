using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelingStationController : MonoBehaviour
{
    [SerializeField] private Transform _entryPosition;
    [SerializeField] private FuelingStation _fuelingStationPrefab;
    private FuelingStation _currentStation;

    public bool Fueled;

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

    public void StartFuelingProcess(){
        ShipSpeedController.Instance.EnterFuelingMode();
        InstantiateFuelStation();
    }

    private void InstantiateFuelStation(){
        _currentStation = Instantiate(_fuelingStationPrefab, _entryPosition.position, Quaternion.identity);
    }

    //called from the FuelController when fueling is done; call the animation
    public void FuelingDone()
    {
        if (_currentStation != null)
        {
            _currentStation.FuelingDone();
            ShipSpeedController.Instance.ExitFuelingMode();
        }
    }

    internal bool FuelingStationAvailable()
    {
        return (_currentStation != null && !_currentStation._fueledOnce);
    }
}
