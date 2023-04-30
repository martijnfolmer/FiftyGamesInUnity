using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    public int boardWidth = 32;         // how many cells are on the board in width
    public int boardHeight = 20;        // how many cells are on the board in height
    public int numBombs = 10;           // number of boms to set in the grid
    public GameObject cellObject;       // the gameobject representing a single cell
    public Text MinesTxt;               // The text which shows how many mines are in the game
    public Text FlagsTxt;               // The text which shows how many flags we have placed down
    public Text VictoryTxt;             // The text which informs us if we have a victory or fail condition
    public Sprite emptySprite;          // when it is an empty field
    public Sprite coveredSprite;        // when the mine is covered
    public Sprite coveredHoverSprite;   // when we are hovering over it with our stuff
    public Sprite oneSprite;            // The sprite we show if the number is 1
    public Sprite twoSprite;            // The sprite we show if the number is 2
    public Sprite threeSprite;          // The sprite we show if the number is 3
    public Sprite fourSprite;           // The sprite we show if the number is 4
    public Sprite fiveSprite;           // The sprite we show if the number is 5
    public Sprite sixSprite;            // The sprite we show if the number is 6
    public Sprite sevenSprite;          // The sprite we show if the number is 7
    public Sprite eightSprite;          // The sprite we show if the number is 8
    public Sprite mineSprite;           // The sprite we show if it is a mine
    public Sprite flagSprite;           // The sprite we show if a flag is set



    Vector2 TopLeft = new Vector2 (-7.5f, 4.0f);            // The pixel coordinates of the top left of the field
    Vector2 BottomRight = new Vector2 (7.5f, -4.0f);        // the pixel coordinates of the bottom right of the field
    float boardWidthPixel = 0.0f;                           // how wide the board is in pixels
    float boardHeightPixel = 0.0f;                          // how high the board is in pixels
    float cellWidthPixel = 0.0f;                            // how big each cell is
    float cellHeightPixel = 0.0f;
    int numFlags = 0;                                       // How many flags we have created
    Vector2 HoverOver = new Vector2(-1.0f, -1.0f);          // The grid coordinates of the cell we are currently hovering over, gets updated every step
    cell[,] board;                                          // The board which contains the cells which constitute the level
    int[] borderCoorFull = new int[16] { 1, 0, 1, -1, 0, -1, -1, -1, -1, 0, -1, 1, 0, 1, 1, 1 };        // relative border coordinates (E, NE, N, NW, W, SW, S, SE)
    Sprite[] numberSprites;                                 // The array which contains all the numbered sprites
    bool haveStarted= false;                                // if set to true, we haven't pressed anything yet, so bombs and numbers are not yet distributed
    int state = 0;                                          // Current state of play, 0= playing, 1=fail, 2=victory

    // Start is called before the first frame update
    void Start()
    {
        
        // Figure out the width and height of each cell
        boardWidthPixel = Mathf.Abs(TopLeft.x - BottomRight.x);
        boardHeightPixel = Mathf.Abs(TopLeft.y - BottomRight.y);
        cellWidthPixel = boardWidthPixel / boardWidth;
        cellHeightPixel = boardHeightPixel / boardHeight;

        // Which sprites to set based on how many mines are bordering
        numberSprites = new Sprite[8] {oneSprite, twoSprite, threeSprite, fourSprite, fiveSprite, sixSprite, sevenSprite, eightSprite};

        // Create the board
        CreateBoard(boardWidth, boardHeight);

        MinesTxt.text = "Mines : " + numBombs.ToString();
        FlagsTxt.text = "Flags : " + numFlags.ToString(); 
    }


    Vector2 cellCoordinatesToPixelCoordinates(int _xco, int _yco)
    {
        // Turn the cell coordinates to pixel coordinates on the screen
        return new Vector2 ((float)(TopLeft.x + (_xco + 0.5) * cellWidthPixel), (float)(TopLeft.y - (_yco + 0.5) * cellHeightPixel));
    }

    Vector2 pixelCoordinatesToCellCoordinates(float _xco, float _yco)
    {
        // Turn the pixel coordintes to cell coordinates
        int column = (int)(Mathf.Floor((_xco - TopLeft.x) / cellWidthPixel));
        int row = (int)(Mathf.Floor((_yco+TopLeft.y) / cellHeightPixel));
        row = boardHeight - row - 1;

        return new Vector2((float)(column), (float)(row));
    }

    void CreateBoard(int _width, int _height)
    {
        // Initialize the board and all of the cells.
        board = new cell[boardWidth, boardHeight];
        for (int i=0;i<board.GetLength(0);i++)
        {
            for(int j = 0; j < board.GetLength(1); j++)
            {
                cell cell_cur = new cell();
                cell_cur.Position_grid = new Vector2Int { x = i, y = j };
                cell_cur.type = 0;      // empty for now
                cell_cur.Number = 0;    // amount of bombs next to it

                var objID = Instantiate(cellObject);

                // create positions
                Vector2 pixLoc = cellCoordinatesToPixelCoordinates(i, j);
                objID.transform.position = new Vector2{ x = pixLoc.x, y=pixLoc.y };

                // Set the scale so the cells lock together
                var sprRenderer = objID.GetComponent<SpriteRenderer>();
                float spriteWidth = sprRenderer.bounds.size.x;
                float spriteHeight = sprRenderer.bounds.size.y;
                float scaleX = cellWidthPixel / spriteWidth;
                float scaleY = cellHeightPixel / spriteHeight;
                objID.transform.localScale = new Vector2(scaleX, scaleY);

                // Set each sprite to covered
                sprRenderer.sprite = coveredSprite;

                // Set object id
                cell_cur.ObjectId = objID;
                cell_cur.revealed = false;
                cell_cur.Marked = false;

                // hide everything
                updateCellSprite(cell_cur, coveredSprite);
                
                // set board
                board[i, j] = cell_cur;       
            }
        }
    }


    void populateBoard(int xstart, int ystart)
    {
        // Put down bombs on the board (except on location xstart, ystart) and set Numbers for each cell, which denotes how many bordering cells have mines in them

        // set the bombs
        setBombs(xstart, ystart);

        // set how many numbers are around this grid
        setNumbers();
    }



    void setBombs(int nothere_x, int nothere_y)
    {
        
        /* Set a number of bombs inside of the board
         * Do not set a bomb on the coordinates nothere_x and nothere_y, because that is where we pressed our button first
         */


        for (int i = 0; i < numBombs; i++)
        {
            while (true)
            {
                int x = Random.Range(0, boardWidth);
                int y = Random.Range(0, boardHeight);

                int manhattan_dist = Mathf.Max(Mathf.Abs(x - nothere_x), Mathf.Abs(y - nothere_y));

                var cellCheck = board[x, y];
                if (cellCheck.type == cell.Type.Empty && manhattan_dist>1)
                {
                    cellCheck.type = cell.Type.Mine;        // set to mine
                    board[x, y] = cellCheck;                // set back to our board
                    
                    break;
                }
            }
        }
    }


    void setNumbers()
    {
        // find out how many bordering mines are there for each empty field, which will show their number


        // Loop through all of the cells, check if there are bombs to the E, EN, N, NW, W, SW, S, SE
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            { 
                var cellCenter = board[i, j];                 // get the cell at the location
                int numBombsCur = 0;                    // number of bordering bombs
                if (cellCenter.type == cell.Type.Empty) {     // only check empty cells
                    for (int k = 0; k < borderCoorFull.Length; k += 2)
                    {
                        var xcheck = i + borderCoorFull[k];
                        var ycheck = j + borderCoorFull[k + 1];

                        //check if we reference a valid position on the board
                        if (xcheck >= 0 && ycheck >= 0 && xcheck < boardWidth && ycheck < boardHeight)
                        {
                            //check number of mines
                            var cellcheck = board[xcheck, ycheck];
                            if (cellcheck.type == cell.Type.Mine)
                            {
                                numBombsCur += 1;
                            }
                        }
                    }
                    // Set how many bombs are next this bomb
                    cellCenter.Number = numBombsCur;

                }
                board[i, j] = cellCenter;
            }
        }
    }

    bool MouseInRange(Vector3 _MousePosWorld)
    {
        // check if the mouse is hovering over our board
        Vector2 TopLeft = new Vector2(-8.0f, 5.0f);
        Vector2 BottomRight = new Vector2(8.0f, -5.0f);

        if (_MousePosWorld.x<TopLeft.x || _MousePosWorld.y>TopLeft.y || _MousePosWorld.x>BottomRight.x || _MousePosWorld.y < BottomRight.y){
            return false;
        }
        else { return true; }
    }


    cell updateCellSprite(cell CellToCheck, Sprite spriteToUpdate)
    {
        // change the sprite of a cell
        var cellObject = CellToCheck.ObjectId;
        var sprRenderer = cellObject.GetComponent<SpriteRenderer>();
        sprRenderer.sprite = spriteToUpdate;

        return CellToCheck;
    }


    void updateHoverOver()
    {
        // check if we are hovering over a cell and updates their sprites as needed
        Vector3 mousePosPix = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosPix);
        Vector2 _gridToCheck = new Vector2(-1f, -1f);
        if (MouseInRange(mousePosWorld))
        {
            _gridToCheck = pixelCoordinatesToCellCoordinates(mousePosWorld.x, mousePosWorld.y);
        }

        if (_gridToCheck.x != -1 && _gridToCheck.y != -1 && (_gridToCheck.x != HoverOver.x || _gridToCheck.y != HoverOver.y))
        {
            // reset old Hoverover
            if (HoverOver.x >= 0 && HoverOver.y >= 0 && HoverOver.x < boardWidth && HoverOver.y < boardHeight) { 
                cell cellHover = board[(int)HoverOver.x, (int)HoverOver.y];
                if (cellHover.revealed == false && cellHover.Marked == false)
                {
                    updateCellSprite(cellHover, coveredSprite);
                    board[(int)HoverOver.x, (int)HoverOver.y] = cellHover;
                }
            }
            
            // set the new hoverover
            HoverOver = _gridToCheck;

            if (HoverOver.x >= 0 && HoverOver.y >= 0 && HoverOver.x < boardWidth && HoverOver.y < boardHeight)
            {
                cell cell_check = board[(int)HoverOver.x, (int)HoverOver.y];
                if (cell_check.revealed == false && cell_check.Marked == false)
                {
                    updateCellSprite(cell_check, coveredHoverSprite);
                    board[(int)HoverOver.x, (int)HoverOver.y] = cell_check;
                }
            }
            
        }
        else if (_gridToCheck.x == -1 && _gridToCheck.y == -1 && (_gridToCheck.x != HoverOver.x || _gridToCheck.y != HoverOver.y))
        {
            if (_gridToCheck.x >= 0 && _gridToCheck.y >= 0 && _gridToCheck.x < boardWidth && _gridToCheck.y < boardHeight)
            {
                cell cellHover = board[(int)HoverOver.x, (int)HoverOver.y];
                if (cellHover.revealed == false && cellHover.Marked == false)
                {
                    updateCellSprite(cellHover, coveredSprite);
                    board[(int)HoverOver.x, (int)HoverOver.y] = cellHover;

                }
                HoverOver = _gridToCheck;
            }
        }
    }


    void floodCell(int x, int y)
    {
        // Reveal a cell and all of its empty neighbours. This is the main function with which the board state changes 
        // because it only happens after you click on a certain cell

        if (x>= 0 && y>=0 && x<boardWidth && y<boardHeight) {
            cell cellCheck = board[x, y];
            if (cellCheck.revealed == false)
            {
                // set the correct sprite
                if (cellCheck.type == cell.Type.Mine)
                {
                    updateCellSprite(cellCheck, mineSprite);
                    cellCheck.revealed = true;
                    board[x, y] = cellCheck;
                    // set our state to loss
                    state = 1;
                    VictoryTxt.text = "Fail!";
                }
                else if (cellCheck.Number > 0)
                {
                    updateCellSprite(cellCheck, numberSprites[cellCheck.Number - 1]);
                    cellCheck.revealed = true;
                    board[x, y] = cellCheck;

                }
                else
                {
                    updateCellSprite(cellCheck, emptySprite);
                    cellCheck.revealed = true;
                    board[x, y] = cellCheck;
                    for (int k = 0; k < borderCoorFull.Length; k += 2)
                    {
                        floodCell(x + borderCoorFull[k], y + borderCoorFull[k + 1]);
                    }
                }

            }
        }
    }


    void resetGame()
    {
        // Reset all cells in the game
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                var cellCheck = board[i, j];                 // get the cell at the location
                cellCheck.Marked = false;
                cellCheck.type = cell.Type.Empty;
                cellCheck.revealed = false;
                updateCellSprite(cellCheck, coveredSprite);
                board[i, j] = cellCheck;        //update the board
            }
        }

        // Reset other values back to orginal
        HoverOver.x = -1;
        HoverOver.y = -1;
        state = 0;
        numFlags = 0;
        VictoryTxt.text = "";
        MinesTxt.text = "Mines : " + numBombs.ToString();
        FlagsTxt.text = "Flags : " + numFlags.ToString();
        haveStarted = false;       
    }



    bool checkIfVictory()
    {
        // Check if number flags is the same as number of bombs
        if (numBombs > numFlags){return false;}

        // check if all conditions for victory on all cells are correct
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                var cellCheck= board[i, j];                 // get the cell at the location
                if (cellCheck.revealed==true && cellCheck.type == cell.Type.Mine)
                {
                    // if any of the mines have been revealed.
                    return false;
                }
                else if (cellCheck.Marked==true && cellCheck.type != cell.Type.Mine)
                {
                    // If the cell is marked with a flag, but there is no mine there, return false, because we did something wrong
                    return false;
                }
                else if (cellCheck.revealed == false && cellCheck.type == cell.Type.Empty)
                {
                    // if any of the cell is not revealed
                    return false;
                }
            
            }
        }


        // return true if all checks are met
        return true;
    }




    // Update is called once per frame
    void Update()
    {
        // update which cell we are hovering over, and updates their sprites if needed.
        if (state == 0) { updateHoverOver(); }
    
        // check if we are pressing
        if (Input.GetMouseButtonDown(0) && state==0) // left mouse button, reveal
        {
            if (haveStarted == false)
            {
                populateBoard((int)HoverOver.x, (int)HoverOver.y); //populate the board
                haveStarted = true;
            }


            // Flood the cells
            floodCell((int)HoverOver.x, (int)HoverOver.y);

            // Check if victory
            bool victory = checkIfVictory();
            if (victory == true)
            {
                state = 2;
                VictoryTxt.text = "Victory!";
            }

        }
        else if (Input.GetMouseButtonDown(1) && state==0) // right mouse button, set a flag
        {

            cell cellCur = board[(int)HoverOver.x, (int)HoverOver.y];
            if (cellCur.revealed == false)
            {
                if (cellCur.Marked == false)
                {
                    cellCur.Marked = true;
                    updateCellSprite(cellCur, flagSprite);
                    board[(int)HoverOver.x, (int)HoverOver.y] = cellCur;
                    numFlags += 1;
                }
                else
                {
                    cellCur.Marked = false;
                    updateCellSprite(cellCur, coveredHoverSprite);
                    board[(int)HoverOver.x, (int)HoverOver.y] = cellCur;
                    numFlags -= 1;
                }
            }

            MinesTxt.text = "Mines : " + numBombs.ToString();
            FlagsTxt.text = "Flags : " + numFlags.ToString();

            // Check if victory
            bool victory = checkIfVictory();
            if (victory == true)
            {
                state = 2;
                VictoryTxt.text = "Victory!";
            }

        }
        // Reset the game by pressing middle mouse button
        else if (Input.GetMouseButtonDown(2))
        {
            resetGame();
        }
    }
}
