using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTextBox : MonoBehaviour
{
    [SerializeField] private TutorialPhase currentPhase;
    [SerializeField] private TextMeshProUGUI TMP;

    public void SetTutorialMessage(TutorialPhase newMessage)
    {
        currentPhase = newMessage;
        //escape characters (for example \n) are serialized as actual characters (\\n) and that's why we need an excluded character. 
        TMP.text = newMessage.Text.Replace('$', '\n');
    }
    
}
