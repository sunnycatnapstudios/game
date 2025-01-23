using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public float speed, counter = 0;
    public bool shoot = false;
    public Vector3 DIR;
    public SpriteRenderer sprender;
    public CircleCollider2D circollider2D;

    // Start is called before the first frame update
    void Start()
    {
        sprender = GetComponent<SpriteRenderer>();
        circollider2D = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!shoot){

            sprender.enabled = false;
            circollider2D.enabled = false;

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                transform.position = new Vector3(Input.GetAxisRaw("Horizontal")*0.8f, 0.0f, 0.0f) + target.position;
                DIR = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, 0.0f);
            }
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                transform.position = new Vector3(0f, Input.GetAxisRaw("Vertical")*0.8f, 0f) + target.position;
                DIR = new Vector3(0.0f, Input.GetAxisRaw("Vertical"), 0.0f);
            }
        }

        if (Input.GetMouseButtonUp(1) && !shoot){
            shoot = true;
            sprender.enabled = true;
            circollider2D.enabled = true;
        }
        if (shoot){
            transform.position += DIR*Time.deltaTime*speed;
            if (counter < 1.5){
                counter += Time.deltaTime;
            }
        }
        if (counter >= 1.5){
            shoot = false;
            counter = 0;
            transform.position = new Vector3 (0.0f, .8f, 0.0f) + target.position;
        }
    }
}
