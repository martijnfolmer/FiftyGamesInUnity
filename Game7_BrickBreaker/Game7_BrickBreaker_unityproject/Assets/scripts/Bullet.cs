using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Bullet : MonoBehaviour
{



    public GameObject leftWall;             // pass on the walls we collide with
    public GameObject rightWall;
    public GameObject topWall;
    public AudioClip BlockBreak;            // the sounds for the bullets
    public AudioClip WallBounce;
    public AudioClip PaddleBounce;
    public float speedTot = 10.0f;          // how big the speed can be
                                         
    private float speedHor = 0.0f;          // horizontal speed
    private float speedVert = 0.0f;         // vertical speed
    private bool justChanged = false;
    private AudioSource audio;


    // Public variables
    public void setSpeed(float direction)
    {
        // set the initial speed for the bullet
        float radians = Mathf.Deg2Rad * direction;
        
        speedHor = Mathf.Sin(radians) * speedTot;
        speedVert = Mathf.Cos(radians) * speedTot;
    }

    public void stopMovement()
    {
        speedHor = 0.0f;
        speedVert = 0.0f;
    }



    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    

    


    void moveBullet()
    {

        // move the bullet in a direction
        float newX = transform.position.x + speedHor * Time.deltaTime;
        float newY = transform.position.y + speedVert * Time.deltaTime;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }


    // Update is called once per frame
    void Update()
    {

        //movement
        moveBullet();

        justChanged = false;    

    }

    // On trigger Enter
    private void OnTriggerEnter2D(Collider2D collision)
    {


        // left wall
        if (collision.gameObject == leftWall || collision.gameObject == rightWall) // the left wall (bounce right) or the right wall (bound left)
        {
            if (!justChanged)
            {
                speedHor *= -1;
                justChanged = true;
                audio.PlayOneShot(WallBounce);
            }
        }
        else if (collision.gameObject == topWall)        // the top wall (bounce down)
        {
            if (!justChanged)
            {
                speedVert *= -1;
                justChanged = true;
                audio.PlayOneShot(WallBounce);
            }

        }
        else if (collision.gameObject.tag == "block")   // the blocks we can break
        {

            // bounce bullet of the blocks
            if (!justChanged)
            {
                float diffx = Mathf.Abs(speedHor);
                float diffy = Mathf.Abs(speedVert);

                if (diffx > diffy)
                {
                    speedHor *= -1;
                }
                else
                {
                    speedVert *= -1;
                }
                justChanged = true;
            }
            //destroy block
            collision.gameObject.GetComponent<Block>().setTodestroy();
            audio.PlayOneShot(BlockBreak);

        }
        else if (collision.gameObject.tag == "Player")      // paddle of player
        {
            Vector3 PaddleSize = collision.gameObject.GetComponent<Paddle>().GetPaddleSize();
            float frac = (transform.position.x - collision.gameObject.transform.position.x) / PaddleSize.x;
            float dir = frac * 55f;
            setSpeed(dir);
            audio.PlayOneShot(PaddleBounce);
        }
        

    }
}
