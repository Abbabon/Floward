using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DashboardEngineOverheatLevelMock : MonoBehaviour {

    [SerializeField] private RectTransform overheatLevel;

    private void Awake()
    {
        overheatLevel = GetComponent<RectTransform>();
    }

    // percentage is between 0 and 100 (actually 90) - but you need to show *the opposite!* or - the percentage of overheat under 90 (factored to be between 0 and 1)
    public void Set(float percentage) {
        //float overheatScale = (100 - (percentage * 100 / GlobalGameplayVariables.Instance.MaxOverheat));
        float overheatScale = (percentage * 100 / GlobalGameplayVariables.Instance.MaxOverheat);
        overheatLevel.localScale = new Vector3(overheatScale/100f, 1f, 1f);
    }
}
