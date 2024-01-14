using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float movementSpeed = 0.05f;


    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        
        mainCamera = Camera.main;       // find the main camera currently
    }

    // Update is called once per frame
    void Update()
    {
        // moving the bullet
        transform.Translate(new Vector3(movementSpeed, 0, 0) * Time.deltaTime);
       
        // check if the bullet is outside of the camera
        if (IsOutsideCameraRange()) {
            Destroy(this.gameObject);           // destroy this object
        }
    }


    // Trigger collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this happens every time we collide with another body
        if (collision.gameObject.tag == "enemy")
        {

            // get the scrip of the other object and activate the destruction button.
            Asteroid asteroidScript = collision.gameObject.GetComponent<Asteroid>();
            asteroidScript.Destruction();

            // Destroy ourselves
            Destroy(this.gameObject);
        }
    }


    // check if outside of the viewport
    bool IsOutsideCameraRange()
    {
        // Get the viewport position of the object
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);


        // Check if the object is outside the camera's viewport (not visible)
        return (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1);
    }

    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
    */





}
