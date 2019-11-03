using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    [SerializeField] private float MusicVolume = 1f;
    [SerializeField] private float SFXVolume = 1f;

    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider EffectsVolumeSlider;

    [SerializeField] private List<string> parameters;

    private Transform _emittingLocation;

    [FMODUnity.EventRef]
    public string _RadioEventName;
    FMOD.Studio.EventInstance _radioEventInstance;

    // Start is called before the first frame update
    void Awake()
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
                _emittingLocation = GetComponent<Transform>();
                LoadSoundPrefs();
                _radioEventInstance = FMODUnity.RuntimeManager.CreateInstance(_RadioEventName);
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void Play()
    {
        FMOD.Studio.PLAYBACK_STATE playbackState;
        _radioEventInstance.getPlaybackState(out playbackState);
        bool isPlaying = playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED;
        if (!isPlaying)
            _radioEventInstance.start();
    }

    private void LoadSoundPrefs()
    {
        MusicVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 1f;
        SFXVolume = PlayerPrefs.HasKey("SoundEffectsVolume") ? PlayerPrefs.GetFloat("SoundEffectsVolume") : 1f;
    }

    private void Start()
    {
        EffectsVolumeSlider.value = SFXVolume;
        MusicVolumeSlider.value = MusicVolume;
    }

    #region SoundEffects

    public void ChangeParameter(string name, float value){
        _radioEventInstance.setParameterValue(name, value);
    }

    public void SetAllParameters(float value)
    {
        parameters.ForEach(p => _radioEventInstance.setParameterValue(p, value));
    }

    #endregion

    #region Menu 
    //handle value change from UI

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        ChangeParameter("Music Volume", 1 - value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetEffectsVolume(float value)
    {
        SFXVolume = value;
        ChangeParameter("SFX Volume", 1 - value);
        PlayerPrefs.SetFloat("SoundEffectsVolume", value);
    }

    #endregion
}
