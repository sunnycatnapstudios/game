using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnEnemy : MonoBehaviour 
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
        if(scene.name == "Battle Scene") 
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
        if(other.gameObject.CompareTag("Player")) 
        {
            this.spawning = true;
            SceneManager.LoadScene("Battle Scene");
        }
    }
}
