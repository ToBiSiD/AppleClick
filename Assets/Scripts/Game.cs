using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public float speed;
    public float rotspeed;
    public int apple_score;
    public GameObject Apple_Jack;

    public GameObject Apple;
    public AudioSource hit;

    public bool spawn;

   
    public void Start()
    {
        spawn = false;
    }
    public void FixedUpdate()
    {

    }


    public void Update()
    {
      
    }
    public void OnMouseDown()
    { 
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.x = mousePosition.x > 2.5f ? 2.5f : mousePosition.x;
        mousePosition.x = mousePosition.x < -0.24f ? -0.24f : mousePosition.x;
        Apple_Jack.transform.position = Vector2.MoveTowards(Apple_Jack.transform.position,
            new Vector2(mousePosition.x, Apple_Jack.transform.position.y),speed*Time.deltaTime);

        hit.Play();
        spawn = true;
        gameObject.GetComponent<Animator>().SetTrigger("drag");

        Spawn();

        if (Apple_Jack.transform.position.x >= 1.365f)
        {
           
            Apple_Jack.GetComponent<Animator>().SetTrigger("left");
            Debug.Log("left");
        }

        else
        {

            Apple_Jack.GetComponent<Animator>().SetTrigger("click");
            Debug.Log("right");

        }

        apple_score = apple_score + Tap_Controller.bonus;
    }

    public void OnMouseUp()
    {
        spawn = false;
    }
     public void Spawn()
    {
        if (spawn)
        {
            Instantiate(Apple, new Vector2(Random.Range(-1.35f, 2.9f), Random.Range(2.85f, -0.15f)), Quaternion.identity);
            
        }
    }


 


}
