using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class TetrisGame : MonoBehaviour
{

    public int widthField = 10;         // width of our field
    public int heightField = 20;        // height of our field
    public GameObject BlockSprite;      // the sprite which shows a single block
    public float period = 0.5f;         // how much time between moving the block down
    public Text scoreTxt;


    private float nextActionTime = 0.0f;        // the timer we use to determine if we should drop the block one go
    private Camera cam;                         // the world camera
    private Vector2 bottomLeftWorldCoordinates; // the top left coordinates of the field we play on
    private Vector2 topRightWorldCoordinates;   // the bottom right coordinates of the field we play on
    private float widthPerZ;                    // how wide each individual cell should be, based on the z distance of the camera to the field
    private float heightPerZ;                   // how highe each individual cell should be, based on the z distance of the camera to the field
    private GameObject[,] FieldSprites;         // an array containing all of the sprites in the field
    private Vector2[,] allTetrominoShapes = new Vector2[7, 4];      // The tetromino shapes that can spawn
    private Tetromino CurrentTetromino = new Tetromino();           // The Tetromino we are currently controlling
    private Color32[,] field;                                       // The field on which we play, and the colors we should draw it.
    private Color32 InertColor = new Color32(100, 100, 100, 255);   // the color if the block is no longer being actively controlled
    private Color32 BgColor = new Color32(255, 255, 255, 255);      // background color (when there is no block on it)
    Stack<int> ClearLines = new Stack<int>();                       // all the lines that need to be cleared from the field
    private int Score = 0;                                          // The score
    private int State = 0;                                          // 0 = wait to start, 1 = playering, 2 = end state


    // Start is called before the first frame update
    void Start()
    {
        // Get all of the tetromino shapes that we use in this game of tetris ( representations of the shape of each tetromino)
        GetAllTetrominoShapes(); 

        // Instantiate the field array
        field = new Color32[widthField, heightField];           // initialize our field
        FieldSprites = new GameObject[widthField, heightField]; // where we store all of the field sprites that we can change the color of
        clearField();                                           // change all backgrounds back

        cam = Camera.main;      // find our camera
        
        // Get the corner coordinates based on 
        bottomLeftWorldCoordinates = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.transform.position.z));
        topRightWorldCoordinates = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.transform.position.z));

        widthPerZ = (topRightWorldCoordinates.x - bottomLeftWorldCoordinates.x)/ cam.transform.position.z;  // new z value = desired width / (this value)
        heightPerZ = (topRightWorldCoordinates.y - bottomLeftWorldCoordinates.y)/ cam.transform.position.z; // new z value = desired height / (this value)

        // instantiate our field objects (which is each of the cells)
        CreateField();  

        // Set the field
        CurrentTetromino.SetWidthAndHeightField(widthField, heightField);
        CurrentTetromino.SetField(field);

        // Testing : getting random
        GetRandomTetromino();   // get random tetromino and set it to a location on our map
        CurrentTetromino.setGlobalCoordinates(widthField, heightField); // Set globalcoordinates of tetromino

        // Render field
        SetFieldColors();

        // Set score
        increaseScore(0);
    }


    void CreateField()
    {
        // Calculate the bottom left of the field, based on the size of our blocks and the height/weidth
        float blockWidth = BlockSprite.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        float blockHeight = BlockSprite.GetComponent<SpriteRenderer>().sprite.bounds.size.y;

        // Get the width and height of the field
        float fieldWidth = blockWidth * (2+field.GetLength(0));
        float fieldHeight = blockHeight * (2+field.GetLength(1));

        // find out the top left coordinates of the field
        float xleftTop = -(field.GetLength(0) * blockWidth)/2;
        float yleftTop = -(field.GetLength(1) * blockHeight)/2;

        // Instantiate all
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                Color32 ColorToDraw = field[i, j];      // the color of the sprite we want to draw.
                var spriteId = Instantiate(BlockSprite);
                spriteId.transform.position = new Vector3((float)(xleftTop + (i+0.5) * blockWidth), (float)(yleftTop + (j+0.5) * blockHeight), 0);
                spriteId.GetComponent<SpriteRenderer>().color = ColorToDraw;            // change the color of the sprite

                // Save the instantiated versions of the blocks
                FieldSprites[i,j] = spriteId;
            }
        }

        // change the size of our camera so we can see the entire field
        float zwidth = (float)(fieldWidth / widthPerZ);
        float zheight = (float)(fieldHeight / heightPerZ);
        Debug.Log(zwidth.ToString() + " / " + zheight.ToString());
        Debug.Log(cam.transform.position);

        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -Mathf.Max(zwidth, zheight));

    }

    void placeTetromino()
    {
        // This function is called if the tetromino can no longer move down (due to blocked or out of bounds)
        // so we place the tetromino into the field itself

        Vector2[] globalCoor = CurrentTetromino.globalCoordinates;

        for(int i=0;i<globalCoor.Length;i++)
        {
            Vector2 BlockCoor = globalCoor[i];
            if ((int)BlockCoor.y < heightField)
            {
                field[(int)BlockCoor.x, (int)BlockCoor.y] = InertColor;
            }
        }
    }


    void clearField()
    {
        // Set all cells to the background color
        for(int i = 0; i < field.GetLength(0); i++)
        {
            for(int j=0; j < field.GetLength(1); j++)
            {
                field[i,j] = BgColor;
            }
        }

    }

    
    void SetFieldColors()
    {
        // set sprites and colors
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                // Set color of sprite
                Color32 ColorCur = field[i, j];         // the color to draw
                GameObject SpriteCur = FieldSprites[i, j];  // the sprites
                SpriteCur.GetComponent<SpriteRenderer>().color = ColorCur;
            }
        }

        Vector2[] TetrominoCur = CurrentTetromino.globalCoordinates;
        for (int i = 0; i < TetrominoCur.Length; i++)
        {
            Vector2 BlockCoor = TetrominoCur[i];
            if (BlockCoor.x < widthField && BlockCoor.y < heightField)
            {
                GameObject SpriteCur = FieldSprites[Mathf.RoundToInt(BlockCoor.x), Mathf.RoundToInt(BlockCoor.y)];  // the sprites
                SpriteCur.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
            }
        }
    }


    void GetAllTetrominoShapes()
    {
        // Get all of the shapes of the tetrominos we work with
        // A single shape consists of 3 cells, which rotate around and are positionally relative to the first cell (which is always at location (0,0)). A shape will thus have 4 cells in total.

        // Square
        allTetrominoShapes[0, 0] = new Vector2(0, 0);
        allTetrominoShapes[0, 1] = new Vector2(1, 0);
        allTetrominoShapes[0, 2] = new Vector2(0, 1);
        allTetrominoShapes[0, 3] = new Vector2(1, 1);

        // K
        allTetrominoShapes[1, 0] = new Vector2(0, 0);
        allTetrominoShapes[1, 1] = new Vector2(-1, 0);
        allTetrominoShapes[1, 2] = new Vector2(0, -1);
        allTetrominoShapes[1, 3] = new Vector2(1, 0);

        // line
        allTetrominoShapes[2, 0] = new Vector2(0, 0);
        allTetrominoShapes[2, 1] = new Vector2(-1, 0);
        allTetrominoShapes[2, 2] = new Vector2(1, 0);
        allTetrominoShapes[2, 3] = new Vector2(2, 0);

        //z
        allTetrominoShapes[3, 0] = new Vector2(0, 0);
        allTetrominoShapes[3, 1] = new Vector2(1, 0);
        allTetrominoShapes[3, 2] = new Vector2(0, 1);
        allTetrominoShapes[3, 3] = new Vector2(-1, 1);

        //reverse z
        allTetrominoShapes[4, 0] = new Vector2(0, 0);
        allTetrominoShapes[4, 1] = new Vector2(-1, 0);
        allTetrominoShapes[4, 2] = new Vector2(0, 1);
        allTetrominoShapes[4, 3] = new Vector2(1, 1);

        // l
        allTetrominoShapes[5, 0] = new Vector2(0, 0);
        allTetrominoShapes[5, 1] = new Vector2(0, -1);
        allTetrominoShapes[5, 2] = new Vector2(0, -2);
        allTetrominoShapes[5, 3] = new Vector2(1, 0);

        // reverse l
        allTetrominoShapes[6, 0] = new Vector2(0, 0);
        allTetrominoShapes[6, 1] = new Vector2(-1, 0);
        allTetrominoShapes[6, 2] = new Vector2(0, -1);
        allTetrominoShapes[6, 3] = new Vector2(0, -2);
    }

    void GetRandomTetromino()
    {
        // Get a random tetromino from our established list
        int idx = Random.Range(0, allTetrominoShapes.GetLength(0));
        Vector2[] RandomShape = new Vector2[4];

        // Set the Shape
        for (int i = 0; i < 4; i++) { RandomShape[i] = allTetrominoShapes[idx, i]; }
        
        // Set our tetromino object with this new random shape
        CurrentTetromino.setLocalCoordinates(RandomShape);

    }


    void increaseScore(int numClearLines)
    {
        //Increase the scroe by 100 for each line cleared, an extra 100 if you clear 4 lines at once
        Score += 100 * numClearLines;
        if (numClearLines == 4)
        {
            Score += 100;       
        }
        scoreTxt.text = "Score : \n" + Score.ToString();
    }


    void FindFullLines()
    {
        // Find each line in the field which is completely filled with inert blocks, which we will then clear

        // clear our stack
        ClearLines.Clear();


        // check the field to see if any horizontal lines are full, meaning we should remove them
        for(int i = 0; i < heightField; i++)
        {

            // for each row, check all the columns for 
            bool filled = true;
            for(int j=0;j < widthField; j++)
            {
                if (!field[j,i].Equals(InertColor))
                {
                    filled = false;
                    break;
                }
            }
            if (filled)
            {
                ClearLines.Push(i);
            }
        }

    }


    void ClearAwayLines()
    {
        // find fill lines again
        FindFullLines();
        while (ClearLines.Count > 0)
        {
            // clear the line we are currently on
            int LineCur = ClearLines.Pop();
            for (int j = 0; j < widthField; j++)
            {
                field[j, LineCur] = BgColor;
            }
            // lower all of the lines by 1
            for (int k = LineCur; k < heightField - 1; k++)
            {
                for (int j = 0; j < widthField; j++)
                {
                    field[j, k] = field[j, k + 1];
                }
            }
            for (int j = 0; j < widthField; j++)
            {
                field[j, heightField - 1] = BgColor;
            }
            FindFullLines();
        }

    }



    // Update is called once per frame
    void Update()
    {
        // Pre state
        if (State == 0)
        {
            scoreTxt.text = "Score : \n" + Score.ToString() + "\nPress R to \nstart";

            // Start the game
            if (Input.GetKeyDown(KeyCode.R))
            {
                nextActionTime = Time.time;      // reset next action timer
                increaseScore(0);               // reset score
                State = 1;                      // move onto the next state
            }
        }

        // Play state
        else if (State == 1)
        {

            // User input
            if (Input.GetKeyDown(KeyCode.A))
            {
                //left
                CurrentTetromino.MoveTetromino(-1, 0);  // Attempt to move the block
                SetFieldColors();                       // update the render
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                //right
                CurrentTetromino.MoveTetromino(1, 0);  // Attempt to move the block
                SetFieldColors();                       // update the render
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                // up (don't need this in the game)
                CurrentTetromino.MoveTetromino(0, 1);  // Attempt to move the block
                SetFieldColors();                       // update the render
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                // down
                CurrentTetromino.MoveTetromino(0, -1);  // Attempt to move the block
                SetFieldColors();                       // update the render
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                //Rotate left
                CurrentTetromino.RotateTetromino(90);
                SetFieldColors();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Rotate left
                CurrentTetromino.RotateTetromino(-90);
                SetFieldColors();
            }


            // Perform the block dropping down
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;

                // move the tetromino down
                bool WasMoved = CurrentTetromino.MoveTetromino(0, -1);  // Attempt to move the block
                SetFieldColors();                                       // update the render

                if (WasMoved) { /* do nothing, could later put some visual effects here*/}
                else
                {
                    // We didn't move the tetromino, so we place it in the field
                    placeTetromino();       // place the current tetromino in the field

                    
                    // find out if we have any full lines
                    FindFullLines();

                    // increase the score based on lines
                    increaseScore(ClearLines.Count);

                    // for each of the filled lines, we remove the line and lower all tetris blocks by one
                    ClearAwayLines();


                    // find a new tetromino
                    GetRandomTetromino();                                           // get random tetromino and set it to a location on our map
                    CurrentTetromino.setGlobalCoordinates(widthField, heightField); // Set globalcoordinates of tetromino

                    // check if blocked
                    if (CurrentTetromino.IsBlocked(CurrentTetromino.globalCoordinates))
                    {
                        CurrentTetromino.globalCoordinates = new Vector2[4];
                        State = 2;      // move onto the fail state
                    }
                  
                }
            }
        }
        else if (State == 2)
        {
            scoreTxt.text = "Score : \n" + Score.ToString() + "\nPress R to \nReset";
            if (Input.GetKeyDown(KeyCode.R))
            {
                Score = 0;      // reset the score
                clearField();   // clear the fields
                State = 0;      // move to start State

                // find a new tetromino
                GetRandomTetromino();                                           // get random tetromino and set it to a location on our map
                CurrentTetromino.setGlobalCoordinates(widthField, heightField); // Set globalcoordinates of tetromino

            }
        }
    }
}
