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

    // TODO add more options later
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
        source.outputAudioMixerGroup = soundGroup;      // Attach unity soundmixer to this
        source.PlayOneShot(c);      // Play the clip
    }
    
	public void PlaySound(String clipName){
		PlaySound(clipName, audioSource);
	}
    
    // Play UI sounds from the UI source
    public void PlayUiSound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        audioSource.outputAudioMixerGroup = uiGroup;
        audioSource.PlayOneShot(c);
    }
    
    // Stop the audio source (Generally shouldn't be necessary)
    public void StopSound()
    {
        audioSource.Stop();
    }
    
    // Cross-fade the ambient sounds
    public void CrossFadeAmbienceSound(String clipName, float fadeTime, float maxVolume = 1, float delayBeforeCrossFade = 0)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        ambienceDoubleSource.CrossFade(c, fadeTime, maxVolume, delayBeforeCrossFade);
    }

    // Fade the ambient source to silence.
    public void CrossFadeAmbienceToZero(float fadeTime, float delayBeforeCrossFade = 0)
    {
        ambienceDoubleSource.CrossFadeToZero(fadeTime, delayBeforeCrossFade);
    }
    
    // Just play ambient sound from ambient source (shouldn't be necessary)
    public void JustPlayAmbienceSound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        ambienceDoubleSource.JustPlaySound(c);
    }
    
    // Just stop playing ambient sounds (shouldn't be necessary)
    public void JustStopAmbienceSound()
    {
        ambienceDoubleSource.JustStopSound();
    }
    
    // Cross-fade the music sounds
    public void CrossFadeMusicSound(String clipName, float fadeTime, float maxVolume = 1, float delayBeforeCrossFade = 0)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        musicDoubleSource.CrossFade(c, fadeTime, maxVolume, delayBeforeCrossFade);
    }
    
    // Fade the music source to silence.
    public void CrossFadeMusicToZero(float fadeTime, float delayBeforeCrossFade = 0)
    {
        musicDoubleSource.CrossFadeToZero(fadeTime, delayBeforeCrossFade);
    }
    
    // Just play music sound from music source (shouldn't be necessary)
    public void JustPlayMusicSound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        musicDoubleSource.JustPlaySound(c);
    }
    
    // Just stop music sounds (shouldn't be necessary)
    public void JustStopMusicSound()
    {
        musicDoubleSource.JustStopSound();
    }
}
