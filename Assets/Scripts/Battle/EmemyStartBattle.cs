using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmemyStartBattle : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyEncounterPrefab;
    private bool spawning = false;
    void Start () 
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded (Scene scene, LoadSceneMode mode) 
    {
        if(scene.name == "Battle") 
        {
            if(this.spawning) 
            {
                Instantiate(enemyEncounterPrefab);
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter2D (Collider2D other) 
    {
        Debug.Log("ENTERED WITH" + other.gameObject.name);
        if(other.gameObject.CompareTag("Player")) 
        {
            this.spawning = true;
            SceneManager.LoadScene("Battle Scene");
        }
    }
}
