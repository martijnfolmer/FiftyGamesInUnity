using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Paddle : MonoBehaviour
{

    public float movementSpeed = 10f;
    public GameObject LeftWall;
    public GameObject RightWall;


    private Vector3 PaddleSize;
    private Vector2 HorLimits;

    // Start is called before the first frame update
    void Start()
    {
        // import the standard functions
        

        // get the width and height
        //var renderer = GetComponent<MeshRenderer>();
        PaddleSize = GetComponent<Renderer>().bounds.size;
        float LeftLimit = LeftWall.transform.position.x + LeftWall.GetComponent<Renderer>().bounds.size.x / 2 + PaddleSize.x/2;
        float RightlImit = RightWall.transform.position.x - RightWall.GetComponent<Renderer>().bounds.size.x / 2 - PaddleSize.x/2;
        HorLimits = new Vector2(LeftLimit, RightlImit);
       

    }


    public Vector3 GetPaddleSize()
    {
        return PaddleSize;
    }



    // movement
    void movePaddle(int dir)
    {
        var newX = transform.position.x + dir * movementSpeed * Time.deltaTime; 

        if (newX < HorLimits.x){
            newX = HorLimits.x;
        }
        else if (newX > HorLimits.y)
        {
            newX = HorLimits.y;
        }

        // moves the paddle left or right
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    // User input
    void UserInput()
    {
        // Move to the left
        if (Input.GetKey(KeyCode.D))
        {
            movePaddle(1);
        }

        // Move to the right
        if (Input.GetKey(KeyCode.A))
        {
            movePaddle(-1);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Movement and user related
        UserInput();
    }




}
