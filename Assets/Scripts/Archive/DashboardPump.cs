using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashboardPump : MonoBehaviour
{
    public void StartEngine(){
        EngineController.Instance.PumpEngine();
        SoundManager.Instance.PlaySoundEffect(SoundManager.SoundEffect.Dashboard_Pump);
    }
}
