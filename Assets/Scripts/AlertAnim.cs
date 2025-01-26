using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertAnim : MonoBehaviour
{
    public Enemy Enemy;
    public Animator Alert;
    public AudioSource alertAudi;
    public int playCount;
    // Start is called before the first frame update
    void Start()
    {
        Alert = GetComponent<Animator>();
        alertAudi = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Enemy.attack){
            // Alert.SetTrigger("AlertActive");
            Alert.Play("Alert");
            if (!alertAudi.isPlaying && playCount <=0 ) {alertAudi.Play(); playCount+=1;}
        }
        else{
            Alert.Play("AlertNone");
            playCount = 0;
        }
    }
}
