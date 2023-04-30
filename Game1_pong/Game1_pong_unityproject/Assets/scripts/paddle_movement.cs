using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paddle_movement : MonoBehaviour
{


    float spriteWidth;      // Width and height of the sprite of the paddle
    float spriteHeight;
    float maxY;             // how far it can go up or down before moving outside of view
    float minY;

    // user input
    public float PlayermaxV;    //player max speed of paddle
    public float EnemymaxV;     //enemy max speed of paddle
    public bool PlayerControlled;

    // The original positions of the paddles
    public float orx;
    public float ory;

    //Ball object
    public GameObject ball;

    // Start is called before the first frame update
    void Start()
    {

        // Get width and height of the sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); // get the sprite renderer component
        spriteWidth = spriteRenderer.sprite.bounds.size.x * transform.localScale.x;       // width of the paddle
        spriteHeight = spriteRenderer.sprite.bounds.size.y * transform.localScale.y;      // height of the paddle


        // Get the bottom left and top right coordinates of the camera in world space
        Camera camera = Camera.main;            //get the main camera
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));


        //find the min and max y range that we can move (otherwise we would go out of bounds)
        maxY = topRight[1] - spriteHeight / 2;
        minY = bottomLeft[1] + spriteHeight / 2;

        // find the original y position
        ory = transform.position.y;
        orx = transform.position.x;

    }

    // Update is called once per frame
    void Update()
    {

        //input manager for vertical (which does not snap to true or not, but like and axis it moves gradually from 0 to 1 or from 0 to -1)
        if (PlayerControlled==true && Input.GetAxis("Vertical") != 0){

            // find the next position
            float translation = Input.GetAxis("Vertical")*PlayermaxV * Time.deltaTime;
            float nextyPosition = transform.position.y + translation;

            //make sure we don't get out of bounds
            if (nextyPosition > maxY) { nextyPosition = maxY; }
            else if (nextyPosition < minY) {  nextyPosition = minY; }

            // actually move the paddle
            transform.position = new Vector3(transform.position.x,nextyPosition,transform.position.z);

        }
        // Enemy controller
        else if (PlayerControlled == false)
        {
            float ballyPosition = ball.transform.position.y;
            float paddleyPosition = transform.position.y;
            if (ballyPosition != paddleyPosition)
            {
                // we are not at the same level as the ball, so move  towards it
                float nextyPosition = paddleyPosition + Mathf.Sign(ballyPosition - paddleyPosition) * EnemymaxV * Time.deltaTime;
                if (nextyPosition > maxY) { nextyPosition = maxY; }
                else if (nextyPosition < minY) { nextyPosition = minY; }

                //actually move the paddle
                transform.position = new Vector3(transform.position.x, nextyPosition, transform.position.z);
            }

        }



    }
}
