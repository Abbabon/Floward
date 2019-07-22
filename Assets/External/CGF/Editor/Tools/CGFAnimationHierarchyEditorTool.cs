/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 02/05/2017
/// Author: Chloroplast Games
/// Web: http://www.chloroplastgames.com
/// Programmers:  David Cuenca Diez
/// Description: Tool that allows change the path of hierarchy from a animation from an animation clip.
///

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Local Namespace
namespace Assets.CGF.Editor.Tools
{
   
    /// \english
    /// <summary>
    /// Tool that allows change the path of hierarchy from a animation from an animation clip.
    /// </summary>
    /// \endenglish
    /// \spanish
    /// <summary>
    /// Herramienta que permite canviar la ruta de la jerarquía de las animaciones de un clip de animación.
    /// </summary>
    /// \endspanish
    public class CGFAnimationHierarchyEditorTool : EditorWindow
    {

        #region Public Variables

        #endregion


        #region Private Variables

            private enum TypeGameObject
            {

                None,

                GameObject,

                AnimationClip,

                RuntimeAnimatorController

            };

            private TypeGameObject _currentSelectionGameObject;

            private int index;

            private ArrayList _pathsKeys;

            private List<GameObject> _gameObjects;

            private Animator _animatorObject;

            private Animator _animatorObject2;

            private AnimationClip _animationClips2;

            private Hashtable _paths;

            private GameObject _null;

            private Vector2 _scrollPos = Vector2.zero;

            private RuntimeAnimatorController _myruntime;

            private Dictionary<string, string> _tempPathOverrides;

            private Dictionary<string, string> _tempPathRemoveds;

            private List<AnimationClip> _animationClips;

            private List<AnimationClip> _myanimationClips;

            private List<string> clipNames = new List<string>();

            private string _sOriginalRoot = "Root";

            private string _sNewRoot = "SomeNewObject";

            private string _sReplacementOldRoot;

            private string _sReplacementNewRoot;

            private string[] _modes = new string[] { "Path", "GameObject"};

            private int _selectedMode = 0;

            private string _replacePath;

            private GameObject _replaceGameObject;

            private string _replacementPath;

            private GameObject _replacementGameObject;

            private bool _changeAll;

        #endregion


        #region Main Methods

            [MenuItem("Window/Chloroplast Games Framework/Animation Hierarchy Editor Tool")]
            private static void ShowWindow()
            {

                EditorWindow window = EditorWindow.GetWindow(typeof (CGFAnimationHierarchyEditorTool), false, "Animation Hierarchy Editor Tool", true);

                window.minSize = new Vector2(650, 350);

            }

            public CGFAnimationHierarchyEditorTool()
            {

                _currentSelectionGameObject = new TypeGameObject();

                _currentSelectionGameObject = TypeGameObject.None;

                _animationClips = new List<AnimationClip>();

                _myanimationClips = new List<AnimationClip>();

                _tempPathOverrides = new Dictionary<string, string>();

                _tempPathRemoveds = new Dictionary<string, string>();

                _gameObjects = new List<GameObject>();

                _changeAll = false;

            }

            private void OnSelectionChange()
            {
                _animationClips.Clear();

                _myanimationClips.Clear();

                clipNames.Clear();

                _animatorObject = null;

                _myruntime = null;

                _animatorObject2 = null;

                index = 0;

                if (Selection.activeObject is AnimationClip)
                {

                    _currentSelectionGameObject = TypeGameObject.AnimationClip;

                }
                else if (Selection.activeGameObject is GameObject)
                {

                    _currentSelectionGameObject = TypeGameObject.GameObject;

                }
                else if (Selection.activeObject is RuntimeAnimatorController)
                {

                    _currentSelectionGameObject = TypeGameObject.RuntimeAnimatorController;

                }
                else
                {

                    _currentSelectionGameObject = TypeGameObject.None;

                }

                switch (_currentSelectionGameObject)
                {

                    case TypeGameObject.AnimationClip:

                        _animationClips.Add((AnimationClip) Selection.activeObject);

                        FillModel();

                        break;

                    case TypeGameObject.GameObject:

                        if (Selection.activeGameObject.GetComponent<Animator>() == null)
                        {

                            _currentSelectionGameObject = TypeGameObject.None;

                        }

                        else if (Selection.activeGameObject.GetComponent<Animator>() != null)
                        {

                            _animatorObject2 = Selection.activeGameObject.GetComponent<Animator>();

                            _animatorObject = _animatorObject2;

                            if(_animatorObject2.runtimeAnimatorController != null)
                            {

                                _myruntime = _animatorObject2.runtimeAnimatorController;

                                if (_myruntime.animationClips.Length > 0)
                                {

                                    foreach (AnimationClip i in _myruntime.animationClips)
                                    {

                                        _myanimationClips.Add(i);

                                    }

                                    foreach (AnimationClip e in _myanimationClips)
                                    {

                                        clipNames.Add(e.name);

                                    }

                                    foreach (AnimationClip o in _myanimationClips)
                                    {

                                        _animationClips.Add(o);

                                    }

                                    FillModel();

                                }

                                else 
                                {

                                    _currentSelectionGameObject = TypeGameObject.None;

                                }

                            }

                            else
                            {

                                _currentSelectionGameObject = TypeGameObject.None;

                            }
                                                                                  

                        }

                        break;

                    case TypeGameObject.RuntimeAnimatorController:

                        foreach (RuntimeAnimatorController o in Selection.objects)
                        {

                            _myruntime = o;

                        }

                        foreach (AnimationClip i in _myruntime.animationClips)
                        {

                            _myanimationClips.Add(i);

                        }

                        foreach (AnimationClip e in _myanimationClips)
                        {

                            clipNames.Add(e.name);

                        }

                        foreach (AnimationClip o in _myanimationClips)
                        {

                            _animationClips.Add(o);

                        }

                        FillModel();

                        break;

                }

                this.Repaint();

            }

            private void DrawGui(bool animatorActive, bool runeAnimatorControllerActive, bool animationClipActive)
            { 

                bool animations = true;

                EditorGUILayout.Space();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Selected Animator", GUILayout.Width(170));

                GUI.enabled = animatorActive;

                _animatorObject = ((Animator)EditorGUILayout.ObjectField(_animatorObject, typeof(Animator), true, GUILayout.Width(168)));

                GUI.enabled = true;

                GUILayout.FlexibleSpace();

                GUILayout.Label("Mode");

                _selectedMode = EditorGUILayout.Popup(_selectedMode,_modes);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Selected Animator Controller", GUILayout.Width(170));

                GUI.enabled = runeAnimatorControllerActive;

                _myruntime = ((RuntimeAnimatorController)EditorGUILayout.ObjectField(_myruntime, typeof(RuntimeAnimatorController), true, GUILayout.Width(168)));

                GUILayout.FlexibleSpace();

                GUI.enabled = true;

                switch (_selectedMode)
                {

                    case 0:

                        GUILayout.Label("Path");

                        _replacePath = EditorGUILayout.TextField(_replacePath);

                    break;

                    case 1:

                        GUILayout.Label("GameObject");

                        _replaceGameObject = (GameObject)EditorGUILayout.ObjectField(_replaceGameObject, typeof(GameObject), true);

                    break;

                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Selected Animation Clip", GUILayout.Width(170));

                GUI.enabled = animationClipActive;

                Rect positionRect2;

                switch (_currentSelectionGameObject)
                {
                    case TypeGameObject.AnimationClip:

                        _animationClips[0] = ((AnimationClip)EditorGUILayout.ObjectField(_animationClips[0], typeof(AnimationClip), true,GUILayout.Width(168)));

                    break;

                    case TypeGameObject.GameObject:

                        positionRect2 = new Rect(new Vector2(177, 49), new Vector2(168, 10));

                        index = EditorGUI.Popup(positionRect2, index, clipNames.ToArray());

                        _animationClips[0] = _myanimationClips[index];

                    break;

                    case TypeGameObject.RuntimeAnimatorController:

                        positionRect2 = new Rect(new Vector2(177, 49), new Vector2(168, 10));

                        index = EditorGUI.Popup(positionRect2, index, clipNames.ToArray());

                        try
                        {

                            _animationClips[0] = _myanimationClips[index];

                        }
                        catch (Exception EX_NAME)
                        {

                            Console.WriteLine(EX_NAME);

                            animations = false;

                        }

                    break;

                    case TypeGameObject.None:

                        _animationClips2 = ((AnimationClip)EditorGUILayout.ObjectField(_animationClips2, typeof(AnimationClip), true, GUILayout.Width(168)));

                        animations = false;

                    break;
                }

                GUILayout.FlexibleSpace();

                GUILayout.Label("Replacement");

                switch (_selectedMode)
                {

                    case 0:

                        _replacementPath = EditorGUILayout.TextField(_replacementPath);

                        break;

                    case 1:

                        _replacementGameObject = (GameObject)EditorGUILayout.ObjectField(_replacementGameObject, typeof(GameObject),true);

                        break;

                }

                EditorGUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Replace", GUILayout.Width(205)))
                {
                    Replace();
                }

                _gameObjects.Clear();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal("Toolbar");

                GUILayout.Label("Path");

                GUILayout.Space(-120);

                GUILayout.Label("Object");

                /*
                GUIStyle s = new GUIStyle(EditorStyles.toolbarButton);

                s.fixedWidth = 120;

                if (GUILayout.Button("Change All", s))
                {
                    _changeAll = true;
                }
                */

                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                if (_currentSelectionGameObject != TypeGameObject.None)
                {

                    if (_paths != null)
                    {
                        for (int i = 0; i < _pathsKeys.Count; i++)
                        {
                            GUICreatePathItem((string)_pathsKeys[i]);
                        }
                    }

                }

                _changeAll = false;

                if (!animations)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUI.enabled = false;

                    string pathOverride = _sNewRoot;

                    _sNewRoot = EditorGUILayout.TextField(pathOverride);

                    GameObject objFail = null;

                    GameObject newObjFail;

                    newObjFail = (GameObject)EditorGUILayout.ObjectField(objFail, typeof(GameObject), true);

                    GUILayout.Button("Change", GUILayout.Width(60));

                    GUILayout.Button("Revert", GUILayout.Width(60));

                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    GUI.enabled = true;

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.HelpBox("Please select a GameObject or Prefab with an Animator, Animation Clip or an Animator Controller.", MessageType.Info);

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                }

                FillModel();
           
            }

            private void Replace()
            {

                switch (_selectedMode)
                {

                    case 0:

                        if (_paths != null)
                        {
                            for (var i = 0; i < _pathsKeys.Count; i++)
                            {
                                if (_pathsKeys[i].Equals(_replacePath))
                                {
                                    _tempPathOverrides[(string)_pathsKeys[i]] = _replacementPath;
                                }
                            }
                        }

                        break;

                    case 1:

                        if (_paths != null)
                        {
                            for (var i = 0; i < _pathsKeys.Count; i++)
                            {
                                if (_pathsKeys[i].Equals(ChildPath(_replaceGameObject)))
                                {
                                    _tempPathOverrides[(string)_pathsKeys[i]] = ChildPath(_replacementGameObject);
                                }
                            }
                        }

                    break;

                }
            }

            private void OnGUI()
            {

                switch (_currentSelectionGameObject)
                {

                    case TypeGameObject.AnimationClip:
                    
                        DrawGui(false, false, true);

                        break;

                    case TypeGameObject.GameObject:

                        DrawGui(false, false, true);
                    
                        break;

                    case TypeGameObject.RuntimeAnimatorController:

                        DrawGui(false, false, true);
                        
                        break;

                    case TypeGameObject.None:

                        DrawGui(false, false, false);
                    
                        break;

                }
            }

            private void GUICreatePathItem(string path)
            {

                string newPath = path;

                GameObject newObj;

                ArrayList properties = (ArrayList) _paths[path];

                string pathOverride = path;

                GameObject obj = null;

                if (_tempPathOverrides.ContainsKey(path))
                {

                    pathOverride = _tempPathOverrides[path];

                    obj = FindObjectInRoot(_tempPathOverrides[path]);

                }
                else
                {

                    obj = FindObjectInRoot(path);

                }

                EditorGUILayout.BeginHorizontal();

                GUIStyle s = new GUIStyle(EditorStyles.textField);

                GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

                if (obj == null) s.normal.textColor = Color.yellow;

                if (newPath != pathOverride)
                {

                    buttonStyle.fontStyle = FontStyle.Bold;

                    s.fontStyle = FontStyle.Bold;

                }

                pathOverride = EditorGUILayout.TextField(pathOverride, s);
                
                if (pathOverride != path) _tempPathOverrides[path] = pathOverride;

                Color standardColor = GUI.color;

                if (obj != null)
                {

                    GUI.color = Color.white;

                }

                if (obj == null)
                {

                    GUI.color = Color.yellow;

                    if (_currentSelectionGameObject == TypeGameObject.AnimationClip)
                    {

                        GUI.enabled = false;

                        GUI.color = Color.white;

                    }

                    else if (_currentSelectionGameObject == TypeGameObject.RuntimeAnimatorController)
                    {

                        GUI.enabled = false;

                        GUI.color = Color.white;

                    }

                }

                newObj = (GameObject) EditorGUILayout.ObjectField(obj, typeof (GameObject), true);
            
                if (obj != null)
                {
                    _gameObjects.Add(obj);
                }

                GUI.color = standardColor;

                GUI.enabled = true;

                buttonStyle.fontSize = 11;

                buttonStyle.fixedHeight = 18;

                buttonStyle.fixedWidth = 60;

                if (GUILayout.Button("Change", buttonStyle) || _changeAll)
                {

                    if (buttonStyle.fontStyle == FontStyle.Bold)
                    {
                    
                        newPath = pathOverride;

                        _tempPathRemoveds.Add(newPath,path);

                        _tempPathOverrides.Remove(path);

                    }

                }

                buttonStyle.fontStyle = _tempPathRemoveds.ContainsKey(path) ? FontStyle.Bold : FontStyle.Normal;   

                if (GUILayout.Button("Revert", buttonStyle))
                {

                    if (buttonStyle.fontStyle == FontStyle.Bold)
                    {

                        newPath = _tempPathRemoveds[path];

                        _tempPathRemoveds.Remove(path);

                    }

                }

                EditorGUILayout.EndHorizontal();

                try
                {

                    if (obj != newObj)
                    {

                        _tempPathOverrides[path] = ChildPath(newObj);

                    }

                    if (newPath != path)
                    {

                        UpdatePath(path, newPath);

                    }

                }
                catch (UnityException ex)
                {

                    Debug.LogError(ex.Message);

                }

            }

            private void OnInspectorUpdate()
            {

                this.Repaint();

            }

            private void FillModel()
            {

                _paths = new Hashtable();

                _pathsKeys = new ArrayList();

                try
                {

                    FillModelWithCurves(AnimationUtility.GetCurveBindings(_animationClips[0]));

                    FillModelWithCurves(AnimationUtility.GetObjectReferenceCurveBindings(_animationClips[0]));

                }
                catch (Exception e)
                {

                    Console.WriteLine(e);

                }

            }

            private void FillModelWithCurves(EditorCurveBinding[] curves)
            {

                foreach (EditorCurveBinding curveData in curves)
                {

                    string key = curveData.path;

                    if (_paths.ContainsKey(key))
                    {

                        ((ArrayList) _paths[key]).Add(curveData);

                    }
                    else
                    {

                        ArrayList newProperties = new ArrayList();

                        newProperties.Add(curveData);

                        _paths.Add(key, newProperties);

                        _pathsKeys.Add(key);

                    }

                }

            }

            private void UpdatePath(string oldPath, string newPath)
            {

                if (_paths[newPath] != null)
                {

                    Debug.Log("Path alredy exist!");

                }

                AssetDatabase.StartAssetEditing();

                AnimationClip animationClip = _animationClips[0];

                Undo.RecordObject(animationClip, "Animation Hierarchy Change");

                for (int iCurrentPath = 0; iCurrentPath < _pathsKeys.Count; iCurrentPath++)
                {

                    string path = _pathsKeys[iCurrentPath] as string;

                    ArrayList curves = (ArrayList) _paths[path];

                    for (int i = 0; i < curves.Count; i++)
                    {

                        EditorCurveBinding binding = (EditorCurveBinding) curves[i];

                        AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, binding);

                        ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(animationClip, binding);

                        if (curve != null) AnimationUtility.SetEditorCurve(animationClip, binding, null);

                        else AnimationUtility.SetObjectReferenceCurve(animationClip, binding, null);

                        if (path == oldPath) binding.path = newPath;

                        if (curve != null) AnimationUtility.SetEditorCurve(animationClip, binding, curve);

                        else AnimationUtility.SetObjectReferenceCurve(animationClip, binding, objectReferenceCurve);

                        float fChunk = 1f/_animationClips.Count;

                        float fProgress = (animationClip.length*fChunk) + fChunk*((float) iCurrentPath/(float) _pathsKeys.Count);

                        EditorUtility.DisplayProgressBar("Animation Hierarchy Progress", "How far along the animation editing has progressed.", fProgress);

                    }

                }

                AssetDatabase.StopAssetEditing();

                EditorUtility.ClearProgressBar();

                FillModel();

                this.Repaint();

            }

            private GameObject FindObjectInRoot(string path)
            {

                if (_animatorObject == null)
                {

                    return null;

                }

                Transform child = _animatorObject.transform.Find(path);

                if (child != null)
                {

                    return child.gameObject;

                }

                else
                {

                    return null;

                }
            }

            private string ChildPath(GameObject obj, bool sep = false)
            {

                if (_animatorObject == null)
                {

                    throw new UnityException("Please assign Referenced Animator (Root) first!");

                }

                if (obj == _animatorObject.gameObject)
                {

                    return "";

                }

                else
                {

                    if (obj.transform.parent == null)
                    {

                        throw new UnityException("Object must belong to " + _animatorObject.ToString() + "!");

                    }

                    else
                    {

                        return ChildPath(obj.transform.parent.gameObject, true) + obj.name + (sep ? "/" : "");

                    }

                }

            }

        #endregion


        #region Utility Methods

        #endregion


        #region Utility Events

        #endregion

    }

}