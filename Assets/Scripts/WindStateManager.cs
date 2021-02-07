using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WindStateManager : MonoBehaviour
{
    private static WindStateManager _instance;
    public static WindStateManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    [SerializeField] private float windDirectionChangeTimer = 0f;
    [SerializeField] private float windDirectionWatcherTimer = 0f;
    [SerializeField] private float nextWindDirectionChange = 0f;


    private bool forceChange = false;

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

    private void Start()
    {
        windDirectionChangeTimer = 0;
        RandomizeNextWindChange();
    }

    private void Update()
    {
        if (GameManager.Instance.IsRunning)
        {
            if (TutorialController.Instance.EnableWindChanges)
                ChangeWindDirection();
        }
    }

    WindDirection lastWindDirection = WindDirection.NoWind;
    public void ChangeWindDirection()
    {
        windDirectionChangeTimer += Time.deltaTime;


        WindDirection currentWindDirection = WindController.Instance.Direction();
        if (currentWindDirection == lastWindDirection)
        {
            windDirectionWatcherTimer += Time.deltaTime;
            if (windDirectionWatcherTimer > GlobalGameplayVariables.Instance.SameWindDirectionAllowdFor)
            {
                windDirectionChangeTimer = nextWindDirectionChange;
                forceChange = true;
            }
        }
        else
        {
            windDirectionWatcherTimer = 0f;
            forceChange = false;
        }

        if (windDirectionChangeTimer >= nextWindDirectionChange)
        {
            int FinalChange = 0;
            if (forceChange)
            {
                int ChangeAmount;
                int ChangeVar = UnityEngine.Random.Range(0, 100);
                int DirectionOfChange = UnityEngine.Random.Range(0, 2);

                if (ChangeVar < 60)
                {
                    ChangeAmount = 1;
                }
                else if (ChangeVar < 95)
                {
                    ChangeAmount = 2;
                }
                else
                {
                    ChangeAmount = 3;
                }
                FinalChange = ChangeAmount * (DirectionOfChange == 0 ? 1 : -1);

            }
            else
            {
                switch (currentWindDirection)
                {
                    case WindDirection.BackWind:
                        FinalChange = -3;
                        break;
                    case WindDirection.FrontWind:
                        FinalChange = 3;
                        break;
                    case WindDirection.NoWind:
                        FinalChange = 2;
                        break;
                }
            }

            WindController.Instance.ChangeState(FinalChange);
            RandomizeNextWindChange();
            windDirectionChangeTimer = 0f;
        }
    }

    private void RandomizeNextWindChange()
    {
        nextWindDirectionChange = UnityEngine.Random.Range(GlobalGameplayVariables.Instance.WindChangeMin, GlobalGameplayVariables.Instance.WindChangeMax);
    }

    internal void SetTutorialMode()
    {
        WindController.Instance.ChangeState(0);
    }
}
