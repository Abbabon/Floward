using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
    [SerializeField] private TouchController controller;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.TouchEnabled)
        {
            // pump engine:

            if (Input.GetMouseButtonDown(0))
            {

            }

            //if (controller.Tap && controller.TouchInZone(TouchZone.Engine) && TutorialController.Instance.EnableEnginePump)
            if (controller.Tap && TutorialController.Instance.EnableEnginePump)
            {
                EngineController.Instance.PumpEngine();
                SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Pump);
            }

            // coooool

            //if (controller.SwipeDown && controller.TouchInZone(TouchZone.Engine) && TutorialController.Instance.EnableCooling)
            if (controller.SwipeDown && TutorialController.Instance.EnableCooling)
            {
                Debug.Log("Cooling");
                EngineController.Instance.EngineCooling = true;
                SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Cooler, true);
            }
            else
            {
                if (EngineController.Instance.EngineCooling && !controller.CurrentlyHeld)
                {
                    EngineController.Instance.EngineCooling = false;
                    Debug.Log("Releasing Cooling");
                }
            }

            // swipe sails on:

            //if (controller.SwipeLeft && controller.TouchInZone(TouchZone.Sails) && TutorialController.Instance.EnableSailsUp)
            if (controller.SwipeLeft && TutorialController.Instance.EnableSailsUp)
            {
                if (SailsController.Instance.State == SailsState.SailsDown)
                {
                    SailsController.Instance.SetState(SailsState.SailsUp);
                    SailsController.Instance.Locked = true;
                    SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Sails, true);
                }
            }

            // swipe sails off:

            //if (controller.SwipeRight && controller.TouchInZone(TouchZone.Sails) && TutorialController.Instance.EnableSailsDown)
            if (controller.SwipeRight && TutorialController.Instance.EnableSailsDown)
            {
                if (SailsController.Instance.State == SailsState.SailsUp)
                {
                    SailsController.Instance.SetState(SailsState.SailsDown);
                    SailsController.Instance.Locked = false;
                }
            }

            // boost handle

            if (controller.SwipeDown && controller.TouchInZone(TouchZone.Boost) && TutorialController.Instance.EnableBoost)
            //if (controller.SwipeDown && controller.TouchInZone(TouchZone.Boost) && TutorialController.Instance.EnableBoost)
            {
                Debug.Log("Pulled!");
                BoostController.Instance.BoostHandlePulled();
            }
        }




    }
}
