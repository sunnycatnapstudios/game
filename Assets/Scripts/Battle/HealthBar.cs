using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform bar;
    // Start is called before the first frame update
    private void Awake()
    {
        bar = transform.Find("Bar");
    }

    public void SetBarSize(float sizeNormalized)
    {
        bar.localScale = new Vector3(bar.localScale.x * sizeNormalized, bar.localScale.y, 1);
    }

    public void SetActive()
    {
        this.gameObject.SetActive(true);
    }
    
    public void SetInactive()
    {
        this.gameObject.SetActive(false);
    }
}
