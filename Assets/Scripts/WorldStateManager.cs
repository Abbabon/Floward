using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    private static WorldStateManager _instance;
    public static WorldStateManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    private float windDirectionChangeTimer = 0f;
    private double nextWindDirectionChange = 0f;

    public float windDirectionChangeRate = 3f;

    System.Random RNGesus = new System.Random();

    private void Awake()
    {
        Debug.Log("AWAKE");
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("DESTROY");
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                //Here any additional initialization should occur:
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        windDirectionChangeTimer = 0;
        nextWindDirectionChange = GeneratePoissonNumber(windDirectionChangeRate) + GetNextWindChangeBuffer(WindManager.Instance.State);
    }

    private void Update()
    {
        ChangeWindDirection();
    }

    readonly int numberOfWindStates = Enum.GetValues(typeof(WindState)).Length;
    public void ChangeWindDirection()
    {
        windDirectionChangeTimer += Time.deltaTime;

        if (windDirectionChangeTimer >= nextWindDirectionChange)
        {
            WindManager.Instance.State = (WindState)RNGesus.Next(0, numberOfWindStates);
            windDirectionChangeTimer = 0;
            nextWindDirectionChange = GeneratePoissonNumber(windDirectionChangeRate) + GetNextWindChangeBuffer(WindManager.Instance.State);
            Debug.Log(String.Format("Next wind direction change in {0}", nextWindDirectionChange));
        }
    }

    private double GetNextWindChangeBuffer(WindState state)
    {
        switch (state)
        {
            case WindState.BackWind:
                return 4f;
            default:
                return 3f;
        }
    }

    private double GeneratePoissonNumber(float rate)
    {
        float res = ((float)RNGesus.Next(100) / 101.0f);
        var a = -Math.Log(1.0f - res) / rate;
        return a;
    }
}
