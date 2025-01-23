using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateEnemyMenuItems : MonoBehaviour 
{
    [SerializeField]
    private GameObject targetEnemyUnitPrefab;
    [SerializeField]
    private Sprite menuItemSprite;
    [SerializeField]
    private Vector2 initialPosition, itemDimensions;
    [SerializeField]
    private KillEnemy killEnemyScript;
    // Use this for initialization
    void Awake () 
    {
        GameObject enemyUnitsMenu = GameObject.Find("EnemyUnitsMenu");
        GameObject[] existingItems = GameObject.FindGameObjectsWithTag("TargetEnemyUnit");
        Debug.Log((enemyUnitsMenu == null).ToString() + " " + (existingItems == null).ToString());
        Vector2 nextPosition = new Vector2(this.initialPosition.x + (existingItems.Length * this.itemDimensions.x), this.initialPosition.y);
        GameObject targetEnemyUnit = Instantiate(this.targetEnemyUnitPrefab, enemyUnitsMenu.transform) as GameObject;
        targetEnemyUnit.name = "Target" + this.gameObject.name;
        targetEnemyUnit.transform.localPosition = nextPosition;
        targetEnemyUnit.transform.localScale = new Vector2(0.7f, 0.7f);
        targetEnemyUnit.GetComponent<Button>().onClick.AddListener(() => selectEnemyTarget());
        targetEnemyUnit.GetComponent<Image>().sprite = this.menuItemSprite;
        killEnemyScript.menuItem = targetEnemyUnit;
    }
    public void selectEnemyTarget () 
    {
    
    }
}
