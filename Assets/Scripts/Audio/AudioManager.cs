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
    public AudioSource ambienceSource;      // Source for ambient sounds
    public AudioSource musicSource;         // Source for background music (bgm)

    
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
    }

    // TODO add more options later
    // Set the master volume of all sounds
    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("MasterVolume", volume);
    }
    
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

    // Play Ambient sound from ambient source
    public void PlayAmbienceSound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        ambienceSource.clip = c;
        ambienceSource.outputAudioMixerGroup = ambientGroup;
        ambienceSource.Play();
    }
    

    // Play music sound from the music source
    public void PlayMusicSound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        musicSource.clip = c;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.Play();
    }
    
    // Play UI sounds from the UI source
    public void PlayUISound(String clipName)
    {
        AudioClip c = _audioClipDatabase.GetAudioClip(clipName);
        audioSource.clip = c;
        audioSource.outputAudioMixerGroup = uiGroup;
        audioSource.Play();
    }
}
