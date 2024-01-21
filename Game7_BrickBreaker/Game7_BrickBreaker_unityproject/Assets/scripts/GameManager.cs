using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject block;
    public Text text;

    private int GameState;
    private GameObject bullet;      // bullet object related
    private Vector3 bulletPosOr;
    private GameObject paddle;      // paddle object related
    private Vector3 paddlePosOr;

    private StandardFunctions stdfunctions;
   

    // Start is called before the first frame update
    void Start()
    {

        // import the standard functions
        stdfunctions = new StandardFunctions();

        // start values
        GameState = 0;          // gamestate will 
        bullet = GameObject.FindGameObjectWithTag("bullet");    // the bulelt object
        bulletPosOr = bullet.transform.position;                    // the original position of the bullet

        paddle = GameObject.FindGameObjectWithTag("Player");        // get the paddle object
        paddlePosOr = paddle.transform.position;                    // get the original position of the paddle

        createBlocks();     // create the blocks
    
        // set the first instructions
        text.text = "Press D to launch";
    }

    private void createBlocks()
    {
        // spawn blocks
        Vector3 BlockSize = block.GetComponent<Renderer>().bounds.size;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                var BlockId = Instantiate(block, new Vector3(-7.0f + BlockSize.x * i, 7.0f - BlockSize.y * j, 0.0f), Quaternion.identity);
            }
        }
    }



    private void Reset()
    {
        // remove all blocks
        GameObject[] all_blocks = GameObject.FindGameObjectsWithTag("block");
        foreach (GameObject block in all_blocks)
        {
            block.GetComponent<Block>().setTodestroy();
        }

        // reset positions
        bullet.GetComponent<Bullet>().stopMovement();       // stop the bullet from moving
        bullet.transform.position = bulletPosOr;        // reset bullet position
        paddle.transform.position = paddlePosOr;        // reset paddle position
        GameState = 0;                                  // reset game state
        createBlocks();                                 // create the blocks
        text.text = "Press D to launch";                // reset instructions
    }



    // Update is called once per frame
    void Update()
    {
        if (GameState == 0)     // starting state
        {
            // Testing : make the bullet move
            if (Input.GetKey(KeyCode.W))
            {
                
                float randomDir = Random.Range(-35f, 35f);  // direction of the bullet
                bullet.GetComponent<Bullet>().setSpeed(randomDir);

                GameState = 1;  //play state
                text.text = "";
            }
        }
        else if (GameState == 1)
        {
            // check if there are any blocks left
            GameObject[] all_blocks = GameObject.FindGameObjectsWithTag("block");
            text.text = "Number of blocks left : \n" + all_blocks.Length.ToString() + "\nPress R to reset";
            if (all_blocks.Length == 0)
            {
                GameState = 3;      // set to win state
                bullet.GetComponent<Bullet>().stopMovement();
                text.text = "Victory!\nPress R to reset";
            }

            // check if bullet is beyond
            bool outside = stdfunctions.IsOutsideCameraRange(bullet.transform.position);
            if (outside){
                GameState = 2;      // set to loss state
                bullet.GetComponent<Bullet>().stopMovement();       // stop the bullet from moving
                text.text = "Game Over\nPress R to reset";
            }

            if (Input.GetKey(KeyCode.R))
            {
                Reset();
            }

        }
        else if (GameState == 2 || GameState==3)        // loss state
        {
            if (Input.GetKey(KeyCode.R))
            {
                Reset();
            }
        }
    }
}
