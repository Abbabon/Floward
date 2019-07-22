#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class RunEventsWindow : EditorWindow
{
    [MenuItem("Window/Invoke Environmental Events")]
    public static void ShowWindow()
    {
        GetWindow<RunEventsWindow>("Environmental Events");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create Tornado"))
        {
            EffectsController.Instance.RunTornado();
        }
    }
}

#endif