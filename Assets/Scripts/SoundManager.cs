using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
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
    [SerializeField] private float MuffledMusicVolume = 0.2f;

    [SerializeField] private float SFXVolume = 1f;

    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider EffectsVolumeSlider;

    private Transform _emittingLocation;

    [EventRef(MigrateTo="<fieldname>")]
    public string _RadioMockEventName;
    FMOD.Studio.EventInstance _radioMockEventInstance;

    private FMOD.Studio.Bus SFXBus;
    private FMOD.Studio.Bus RadioBus;

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
                //LoadSoundEffects();
                LoadSoundPrefs();
                _radioMockEventInstance = FMODUnity.RuntimeManager.CreateInstance(_RadioMockEventName);

                SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
                RadioBus = FMODUnity.RuntimeManager.GetBus("bus:/Radio");
            }
        }

        //ontDestroyOnLoad(this.gameObject);
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
        SFXBus.setVolume(SFXVolume);
        RadioBus.setVolume(Mathf.Min(MusicVolume, MuffledMusicVolume));

        PlayRadioMusic();
    }

    #region SoundEffects

    public void PlayOneshotound(string EventName)
    {
        FMODUnity.RuntimeManager.PlayOneShot(String.Format("event:/{0}", EventName), _emittingLocation.position);
    }

    #endregion

    #region Radio

    private void PlayRadioMusic()
    {
        _radioMockEventInstance.start();
    }

    public void DisableRadioMuffle()
    {
        RadioBus.setVolume(MusicVolume);
    }

    #endregion Radio

    #region Menu 
    //handle value change from UI

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        RadioBus.setVolume(MusicVolume);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetEffectsVolume(float value)
    {
        SFXVolume = value;
        SFXBus.setVolume(SFXVolume);
        PlayerPrefs.SetFloat("SoundEffectsVolume", value);
    }

    #endregion
}
