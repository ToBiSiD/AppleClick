using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple_Controller : MonoBehaviour
{
    
    private float aspeed = 12f; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5.2f)
            Destroy(gameObject);
        transform.position -= new Vector3(0, aspeed * Time.deltaTime, 0);
       
    }



}
