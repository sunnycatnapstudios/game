using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnScenePlayAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // For now just play the forest ambient
        AudioManager.Instance.CrossFadeAmbienceSound("Ambient_Forest", 3);
    }
}
