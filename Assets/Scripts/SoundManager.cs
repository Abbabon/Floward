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

    [Space(10)]
    private AudioSource currentSoundEffectsAudioSource;
    private AudioSource currentRadioAudioSource;

    [SerializeField] private float StartingMusicVolume = 1f;
    [SerializeField] private float MuffledMusicVolume = 0.2f;
    [SerializeField] private float CurrentMusicVolume = 1f;

    [SerializeField] private float StartingEffectsVolume = 1f;
    [SerializeField] private float CurrentEffectsVolume = 1f;

    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider EffectsVolumeSlider;

    // Start is called before the first frame update
    void Awake()
    {
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("DESTROY");
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                LoadSoundEffects();
            }
        }

        //ontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        //TODO: set volumes according to the level set by the used
        //TODO: set sliders according to the levels previously set

        MusicVolumeSlider.value = StartingMusicVolume;
        EffectsVolumeSlider.value = StartingEffectsVolume;

        RegisterRadioAudioSource(GetComponent<AudioSource>());
        PlayRadioMusic();
    }

    public void RegisterSoundEffectsAudioSource(AudioSource audioSource){
        currentSoundEffectsAudioSource = audioSource;
    }

    public void RegisterRadioAudioSource(AudioSource audioSource){
        currentRadioAudioSource = audioSource;
        currentRadioAudioSource.volume = MuffledMusicVolume;
    }

    #region SoundEffects

    public enum SoundEffect
    {
        Boat_FuelFillingUp,
        Boat_GreenLightOff,
        Boat_GreenLightOn,
        Boat_PotentialLoading,

        Dashboard_Boost,
        Dashboard_Sails,
        Dashboard_Pump,
        Dashboard_PotentialHandle,
        Dashboard_Cooler,

        WindChange,
        WavingFlag,
    }

    private Dictionary<SoundEffect, AudioClip> soundEffects;
    private void LoadSoundEffects()
    {
        soundEffects = new Dictionary<SoundEffect, AudioClip>();
        foreach (SoundEffect soundEffect in (SoundEffect[])Enum.GetValues(typeof(SoundEffect)))
        {
            soundEffects.Add(soundEffect, Resources.Load<AudioClip>(String.Format("Effects/{0}", soundEffect)));
        }
    }

    public void PlaySoundEffect(SoundEffect soundEffect, bool cancelIfNotPlaying=false)
    {
        //C# doesnt have unless :(((
        if (!(currentSoundEffectsAudioSource.isPlaying && cancelIfNotPlaying))
            currentSoundEffectsAudioSource.PlayOneShot(soundEffects[soundEffect]);
    }

    #endregion

    #region Radio

    public void PlayRadioMusic()
    {
        if (currentRadioAudioSource != null)
        {
            //TODO: change here according to changes in the radio system
            currentRadioAudioSource.Play();
        }
    }

    public void DisableRadioMuffle()
    {
        currentRadioAudioSource.volume = CurrentMusicVolume;
    }

    #endregion Radio

    #region Menu 
    //handle value change from UI

    public void SetMusicVolume(float value)
    {
        if (currentRadioAudioSource != null)
        {
            CurrentMusicVolume = value;
            currentRadioAudioSource.volume = value;
        }
    }

    public void SetEffectsVolume(float value)
    {
        if (currentSoundEffectsAudioSource != null)
        {
            CurrentEffectsVolume = value;
            currentSoundEffectsAudioSource.volume = value;
        }
        
    }

    #endregion
}
