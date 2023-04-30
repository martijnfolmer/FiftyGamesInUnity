using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playingField : MonoBehaviour
{
    public int WidthField;
    public int HeightField;

    public Transform SnakeHeadPrefab;
    public Transform SnakeBodyPrefab;
    public Transform ApplePrefab;

    Vector2[,] Field;                   // type, direction
    Vector2[,] FieldCoordinates;        // coordinate x , coordinate y for each grid
    
    // Game objects we manipulate
    Vector2 snakeLoc;                   // grid x and grid y of the snake head
    Transform snakeHead;                // the location of the snakehead
    Vector2 snakeDirection = new Vector2(1.0f, 0.0f);   // x and y grid direction
    Transform apple;
    List<Transform> segments;           // List with the transforms of all the body segments

    Vector2 topLeftCoor = new Vector2 (-7f, -4.0f); //Top left coordinates of the field
    Vector2 grid_size;                              // how big each sepperate grid is

    //time steps (how often we try to move the snake
    private float time = 0.0f;
    public float timeBetweenSteps = 0.1f;

    // Manipulation of the state of the game
    int State = 0;      // 0 = pre-game, 1 = playing the game,  2 = game over


    // Start is called before the first frame update
    void Start()
    {
        // Create the field
        Field = new Vector2[WidthField, HeightField];
        FieldCoordinates = new Vector2[WidthField, HeightField];

        // Segments which we want
        segments = new List<Transform>();

        //initialize the head and the apple (singleton)
        snakeHead = Instantiate(SnakeHeadPrefab);
        apple = Instantiate(ApplePrefab);
        

        // Actual size of the grid
        grid_size = new Vector2 ( 14.0f / WidthField, 8.0f / HeightField);

        // Find the coordinates of the grids
        findCoordinatesGrids(topLeftCoor[0], topLeftCoor[1], grid_size[0], grid_size[1]);  

        // reset the field, which initializes the first level
        ResetField();
    }




    // SNAKE MANIPULATION

    // Try to move the snake to a different position on the board
    void MoveSnake(int diffx, int diffy)
    {
        Vector2 newLoc = new Vector2(snakeLoc[0] + diffx, snakeLoc[1] + diffy);
        // set newLoc so we move from one side of the arena to the next
        if (newLoc[0] < 0) { newLoc[0] += WidthField; }
        if (newLoc[1] < 0) { newLoc[1] += HeightField; }
        if (newLoc[0] >= WidthField) { newLoc[0] -= WidthField; }
        if (newLoc[1] >= HeightField) { newLoc[1] -= HeightField; }

        Vector2 newLocVal = Field[(int)newLoc[0], (int)newLoc[1]];

        if (newLocVal[0] == 0)
        {
            // if segements are > 1, delete first, append last
            if (segments.Count > 0 )
            {
                Destroy(segments[0].gameObject);   // Delete the segment
                segments.RemoveAt(0);   // remove the non-existant segment from the list
                Grow(snakeLoc);         // Create a segment at the players head location
            }

            // move head there
            var newXYHead = gridCoordinatesToXYcoordinates(newLoc);
            snakeHead.position = new Vector2(newXYHead[0], newXYHead[1]);
            snakeLoc = newLoc;
        }
        else if (newLocVal[0] == 2)
        {
            State = 2;      // move onto game over

        }
        else if (newLocVal[0] == 3)
        {
            //Grow at snakeloc
            Grow(snakeLoc);

            // move head there
            var newXYHead = gridCoordinatesToXYcoordinates(newLoc);
            snakeHead.position = new Vector2(newXYHead[0], newXYHead[1]);
            snakeLoc = newLoc;

            // move apple to a new unoccupied location
            MoveApple();
        }

        SetField(); //Set all values in the field
    }

    // Grow the snake when eating an apple and or when moving the head
    void Grow(Vector2 GridLocation)
    {
        Transform Segment = Instantiate(SnakeBodyPrefab);

        // Set the coordinates of the segment
        var xyCoor = gridCoordinatesToXYcoordinates(GridLocation);
        Segment.position = xyCoor;

        segments.Add(Segment);     // add the segments to the body list
    }

    // Make sure that the snake can only rotates +- 90 degrees from its current position
    bool checkIfCanRotate(float angGoal)
    {
        float curAngle = Field[(int)snakeLoc[0], (int)snakeLoc[1]][1];
        if (Mathf.Abs(Mathf.DeltaAngle(curAngle, angGoal)) <= 90.0f)
        {
            return true;
        }
        return false;
    }



    // SWITCHING BETWEEN XY COORDINATES AND GRID COORDINATES AND BACK

    // transform XY position coordinates to grid coordinates
    Vector2 gridCoordinatesToXYcoordinates(Vector2 position)
    {
        return FieldCoordinates[(int)position[0], (int)position[1]];
    }

    // transform Gridcoordinates to XY position coordinates
    Vector2 XYcoordinatesTogridCoordinates(Vector2 position)
    {
        var i =(int)((position[0] - topLeftCoor[0]) / grid_size[0] - 0.5f);
        var j =(int)((position[1] - topLeftCoor[1]) / grid_size[1] - 0.5f);
        Vector2 gridCoordinates = new Vector2(i, j);

        return gridCoordinates;
    }

    //Find all of the coordinates and store them in a grid, so we know what location to move the snake to when we do a step
    void findCoordinatesGrids(float topx1, float topy1, float grid_w, float grid_h)
    {
        // Find the coordinates of all the grids
        for (int i = 0; i < WidthField; i++)
        {
            for (int j = 0; j < HeightField; j++)
            {
                FieldCoordinates[i, j] = new Vector2 (topx1 + (i+0.5f) * grid_w, topy1 + (j+0.5f) * grid_h);
            }
        }
    }





    // FIELD MANIPULATION

    // Reset the field if we start again.
    void ResetField()
    {

        // reset snake location to middle of field
        snakeLoc = new Vector2(Mathf.RoundToInt(WidthField / 2), Mathf.RoundToInt(HeightField / 2));

        // Clear the body
        for(int i = 0; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
        segments = new List<Transform>();


        // Set the location of the snake head
        Vector2 snakeHeadXY = gridCoordinatesToXYcoordinates(snakeLoc);
        snakeHead.position = new Vector3(snakeHeadXY[0], snakeHeadXY[1]);

        //Set the field
        SetField();

        // Move the apple to an unoccupied space
        MoveApple();

        // set field, but with apple as well
        SetField();

    }

    // Finding new locations and placement of the apple
    void MoveApple()
    {
        Vector2 newAppleLocGrid = findRandomLocationApple();
        Vector2 newAppleLocXY = gridCoordinatesToXYcoordinates(newAppleLocGrid);
        apple.position = newAppleLocXY; //change the location
    }

    //Find a random location for the apple (which can only be placed somewhere we can't hit it.
    Vector2 findRandomLocationApple()
    {
        int xloc = 0;
        int yloc = 0;
        while (true)
        {
            xloc = (int)Random.Range(0, WidthField - 1);
            yloc = (int)Random.Range(0, HeightField - 1);
            var FieldVal = Field[xloc, yloc];
            if (FieldVal[0] == 0)
            {
                break;
            }

        }

        return new Vector2(xloc, yloc);
    }

    // Reset the field grid and store all new values in it
    void SetField()
    {

        //reset the field
        for (int i = 0; i < WidthField; i++)
        {
            for (int j = 0; j < HeightField; j++)
            {
                Field[i, j] = new Vector2(0.0f, 0.0f);  // type and rotation direction.  Type = 0 nothing, 1=head, 2=body, 3=apple
            }
        }


        //Set head value in field
        Field[(int)snakeLoc[0], (int)snakeLoc[1]] = new Vector2(1.0f, snakeHead.transform.eulerAngles.z);

        //Set apple value in field
        Vector2 appleGridLoc = XYcoordinatesTogridCoordinates(new Vector2(apple.position[0], apple.position[1]));
        Field[(int)appleGridLoc[0], (int)appleGridLoc[1]] = new Vector2(3.0f, 0.0f);

        //set all the bodies
        for(int i = 0; i < segments.Count; i++)
        {
            var loc = XYcoordinatesTogridCoordinates(new Vector2(segments[i].position[0], segments[i].position[1]));
            Field[(int)loc[0], (int)loc[1]] = new Vector2(2.0f, 0.0f);
        }
    }




    // Player input
    void Update()
    {

        if (State == 0) // Pre-game state
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                State = 1;  //move onto the playing phase
            }
        }
        else if (State == 1) //playing the game state
        {

            // execute time step (which means moving the snake)
            time += Time.deltaTime;
            if (time >= timeBetweenSteps)
            {
                time = time - timeBetweenSteps;
                // Try to move the actual snake
                MoveSnake((int)snakeDirection[0], (int)snakeDirection[1]);
            }

            // WASD, set direction of snake
            if (Input.GetKeyDown(KeyCode.A) && checkIfCanRotate(180f))
            {
                snakeDirection = new Vector2(-1.0f, 0.0f);
                snakeHead.eulerAngles = new Vector3(snakeHead.rotation.eulerAngles.x, snakeHead.rotation.eulerAngles.y, 180f);
            }
            if (Input.GetKeyDown(KeyCode.D) && checkIfCanRotate(0f))
            {
                snakeDirection = new Vector2(1.0f, 0.0f);
                snakeHead.eulerAngles = new Vector3(snakeHead.rotation.eulerAngles.x, snakeHead.rotation.eulerAngles.y, 0f);

            }
            if (Input.GetKeyDown(KeyCode.W) && checkIfCanRotate(90f))
            {
                snakeDirection = new Vector2(0.0f, 1.0f);
                snakeHead.eulerAngles = new Vector3(snakeHead.rotation.eulerAngles.x, snakeHead.rotation.eulerAngles.y, 90f);
            }
            if (Input.GetKeyDown(KeyCode.S) && checkIfCanRotate(270f))
            {
                snakeDirection = new Vector2(0.0f, -1.0f);
                snakeHead.eulerAngles = new Vector3(snakeHead.rotation.eulerAngles.x, snakeHead.rotation.eulerAngles.y, 270f);
            }
        }
        else if (State == 2) // game over, you can reset
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResetField();
                State = 0;
            }



        }
        

    }
}
