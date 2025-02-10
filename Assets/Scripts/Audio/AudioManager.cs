using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _audioManager;
    public static AudioManager Instance{get{return _audioManager;}}
    private AudioClipDatabase _audioClipDatabase;
    
    [Header("Audio Sources")]               // Audio Source game objects within the scene 
    public AudioSource audioSource;         // Source for sound effects (sfx), including the UI
    public DoubleAudioSource ambienceDoubleSource;      // Double source for ambient sounds
    public DoubleAudioSource musicDoubleSource;         // Double source for music sounds
    
    [Header("Audio Mixers")]                // Unity mixers and groups. Used for controlling volumes 
    public AudioMixer mixer;
    public AudioMixerGroup ambientGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup soundGroup;
    public AudioMixerGroup uiGroup;

    private void Awake()
    {
        if (_audioManager == null)
        {
            _audioManager = this;
        } else if (_audioManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);        
        
        // Fetch the audio database
        _audioClipDatabase = ScriptableObject.CreateInstance<AudioClipDatabase>();
        _audioClipDatabase.PopulateDatabase();
        
        // Set the groups to control by mixer later
        ambienceDoubleSource.SetMixerGroup(ambientGroup);
        musicDoubleSource.SetMixerGroup(musicGroup);
    }

    // Set the master volume of all sounds
    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("MasterVolume", volume);
    }
    // TODO add more options later
    
    // Play sound from an audio source. Defaults to audioSource if no alternative source is provided
    public void PlaySound(String clipName, AudioSource source) //= audioSource)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);    // Fetch the clip from database
        source.clip = c;    // Set the source to play the clip
        source.outputAudioMixerGroup = soundGroup;      // Attach unity soundmixer to this
        source.Play();
    }
    
	public void PlaySound(String clipName){
		PlaySound(clipName, audioSource);
	}
    
    // Play UI sounds from the UI source
    public void PlayUISound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        audioSource.clip = c;
        audioSource.outputAudioMixerGroup = uiGroup;
        audioSource.Play();
    }
    
    // TODO Phase these functions out with cross fades
    // Play Ambient sound from ambient source
    // TODO Use Cross fades instead
    public void PlayAmbienceSound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        ambienceDoubleSource.JustPlaySound(c);
    }
    // Play music sound from the music source
    // TODO Use Cross fades instead
    public void PlayMusicSound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        musicDoubleSource.JustPlaySound(c);
    }
    
    // Cross-fade the ambient sounds
    public void CrossFadeAmbienceSound(String clipName, float fadeTime, float maxVolume = 1, float delayBeforeCrossFade = 0)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        ambienceDoubleSource.CrossFade(c, fadeTime, maxVolume, delayBeforeCrossFade);
    }
    
    // Cross-fade the music sounds
    public void CrossFadeMusicSound(String clipName, float fadeTime, float maxVolume = 1, float delayBeforeCrossFade = 0)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        musicDoubleSource.CrossFade(c, fadeTime, maxVolume, delayBeforeCrossFade);
    }
}
