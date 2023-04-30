using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ball_movement : MonoBehaviour
{

    public float startV = 5.0f;
    public float acceleration = 0.1f;

    float spriteWidth;          // witdth and height of the sprite that represents the ball, used for bounding of the sides of the game
    float spriteHeight;

    float leftx;                // the corner coordinates of the rectangle of our field, used for detecting out of bounds and bouncing
    float rightx;
    float topy;
    float bottomy;

    //starting location
    public float orx = 0f;      // original location
    public float ory = 0f;

    public float curVx = 0f;    // the current horizontal speed
    public float curVy = 0f;    // the current vertical speed

    public GameObject manager;  //the manager game object.
    game_manager gm;

    public GameObject leftPaddle;   // reference the left paddle 
    public GameObject rightPaddle;  // reference the right paddle object

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
        Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));

        // For scoring and bouncing of the outside of the field
        topy = topRight[1] - spriteHeight / 2;
        bottomy = bottomLeft[1] + spriteHeight / 2;
        leftx = bottomLeft[0] - spriteWidth / 2;
        rightx = topRight[0] + spriteHeight / 2 ;

        // get the game manager script
        gm = manager.GetComponent<game_manager>();
    }


    // Accessible to others, shoots the ball away
    public void shootBall()
    {
        //random speed for the ball
        float randomDirection = Random.Range(0, 359);
        curVx = Mathf.Cos(Mathf.Deg2Rad * randomDirection) * startV;
        curVy = Mathf.Sin(Mathf.Deg2Rad * randomDirection) * startV;

        if (Mathf.Abs(curVx) < 0.5)
        {
            curVx = Mathf.Sign(curVx) * 0.5f;
        }
    }



    // Check for movement and collision of the ball, act accordingly
    void Update()
    {

        // check our next position:
        float nextX = transform.position.x + curVx*Time.deltaTime;
        float nextY = transform.position.y + curVy*Time.deltaTime;


        // bounce from the top and bottom of the screen, and down
        if (nextY >= topy)
        {  
            curVy *= -1;
        }
        if (nextY < bottomy)
        {
            curVy *= -1;
        }

        //check if we are out of bounds on the left or right
        if (nextX < leftx)
        {
            gm.IncrementScore(0, 1); // increment the score
            gm.ResetField();         // reset the game
        }
        if (nextX > rightx)
        {
            gm.IncrementScore(1, 0);  // increment the score for the other one
            gm.ResetField();          // reset the field
        }

        //update our movement
        transform.position = transform.position + new Vector3(curVx*Time.deltaTime, curVy * Time.deltaTime, 0);
    }

    // For on trigger, we need to add a box collider to both objects as well as a rigid body to one of them
    void OnTriggerEnter2D()
    {
        
        //means we collided with the paddle
        curVx *= -1;    // remove
        curVx *= (1 + acceleration);    // accelerate the ball

        // Make sure that the ball is shot at an angle relative to the position it hits the paddle with
        float curV = Mathf.Sqrt(Mathf.Pow(curVx, 2) + Mathf.Pow(curVy, 2));  // Total current speed
        Vector3 currentPosition = transform.position;
        Vector2 PaddleLoc = Vector2.zero;
        if (currentPosition.x < 0){
            PaddleLoc = new Vector2(leftPaddle.transform.position.x, leftPaddle.transform.position.y);
        }
        else
        {
            PaddleLoc = new Vector2(rightPaddle.transform.position.x, rightPaddle.transform.position.y);
        }

        float diffy = currentPosition.y - PaddleLoc.y;
        curVy = curV * diffy / 2.0f;
        curVx = Mathf.Sign(curVx) * Mathf.Sqrt(Mathf.Pow(curV, 2) - Mathf.Pow(curVy, 2));
    }

}
