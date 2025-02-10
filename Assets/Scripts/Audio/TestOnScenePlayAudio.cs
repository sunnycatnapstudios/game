using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnScenePlayAudio : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        // For now just play the forest ambient
        AudioManager.Instance.CrossFadeAmbienceSound("Ambient_Forest", 3);
        //yield return DualAudioTest();
        yield break;
    }
    
    IEnumerator DualAudioTest()
    {
        yield return new WaitForSeconds(5);
        AudioManager.Instance.CrossFadeAmbienceSound("Music_JustSynth", 10);
    }
}
