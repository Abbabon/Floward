﻿using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
    [SerializeField] private TouchController controller;
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.TouchEnabled && !EngineController.Instance.EngineInShutdown && !ShipSpeedController.Instance.InStation)
        {

            if (controller.Tap && TutorialController.Instance.EnableEnginePump)
            {
                EngineController.Instance.PumpEngine();
            }

            // coooool
            if (controller.SwipeDown && TutorialController.Instance.EnableCooling && !controller.TouchInZone(TouchZone.Boost) && FuelController.Instance.AmountOfFuel > 0)
            {
                EngineController.Instance.EngineCooling = true;
                SoundManager.Instance.ChangeParameter("Cooling", 1f);
            }
            else
            {
                if (EngineController.Instance.EngineCooling && !controller.CurrentlyHeld)
                {
                    EngineController.Instance.EngineCooling = false;
                    SoundManager.Instance.ChangeParameter("Cooling Stops", 1f);
                    FunctionTimer.Create(() => SoundManager.Instance.ChangeParameter("Cooling Stops", 0f), 0.3f);
                    FunctionTimer.Create(() => SoundManager.Instance.ChangeParameter("Cooling", 0f), 0.3f);
                }
            }

            // swipe sails on:
            if (controller.SwipeLeft && TutorialController.Instance.EnableSailsUp)
            {
                if (SailsController.Instance.State == SailsState.SailsDown)
                {
                    SailsController.Instance.SetState(SailsState.SailsUp);
                    SailsController.Instance.Locked = true;
                }
            }

            // swipe sails off:
            if (controller.SwipeRight && TutorialController.Instance.EnableSailsDown)
            {
                if (SailsController.Instance.State == SailsState.SailsUp)
                {
                    SailsController.Instance.SetState(SailsState.SailsDown);
                    SailsController.Instance.Locked = false;
                }
            }

            // boost handle
            if (controller.SwipeDown && controller.TouchInZone(TouchZone.Boost) && TutorialController.Instance.EnableBoostHandlePull)
            {
                BoostController.Instance.BoostHandlePulled();
            }
        }
    }
}
