using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    public GameObject menuItem;

    private void OnDestroy()
    {
        Destroy(menuItem);
    }
}
