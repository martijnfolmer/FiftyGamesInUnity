using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardFunctions
{
    Camera mainCamera = Camera.main;




    //private StandardFunctions stdfunctions = new StandardFunctions();

    public void printUwU()
    {
        Debug.Log("UwU");
    }


    
    // Determine if an object is outside of the view range or not
    public bool IsOutsideCameraRange(Vector3 _position)
    {
       
        // Get the viewport position of the object
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(_position);

        // Check if the object is outside the camera's viewport (not visible)
        return (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1);
    }



    // collision related
    /*
    private void OnTriggerEnter2D(Collider2D collision){}
    
    private void OnTriggerStay2D(Collider2D collision){}
    
    private void OnTriggerExit2D(Collider2D collision){}
    */


}
