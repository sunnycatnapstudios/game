using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Persistant Object to remember player party
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        //this.gameObject.SetActive (false);
    }
    
    // Activate if we're in battle scene
    private void OnSceneLoaded (Scene scene, LoadSceneMode mode) 
    {
        this.gameObject.SetActive(scene.name == "Battle Scene");
    }
}
