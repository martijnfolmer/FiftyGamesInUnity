using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino
{

    Vector2[] localCoordinates;                             // the current local coordiantes
    public Vector2[] globalCoordinates;                     // the x,y location of our first piece on the board
    Vector2 WidthAndHeight = new Vector2(1, 1);             // the width and height of the tetromino piece
    Vector2 FieldWidthAndHeight = new Vector2(1, 1);        // the widht and height of the play field
    Color32[,] field;                                       // the field we use to check for collisions
    Color32 InertColor = new Color32(100, 100, 100, 255);   // the color we set all the backgrounds
    

    public void setLocalCoordinates(Vector2[] newCoordinates)
    {
        // Set a local coordinates for the tetromino
        localCoordinates = new Vector2[newCoordinates.Length];
        Array.Copy(newCoordinates, localCoordinates, localCoordinates.Length);

        SetWidthAndHeight();    // the width and height of a tetromino
    }


    public void setGlobalCoordinates(int fieldWidth, int fieldHeight)
    {
        // Set the globalcoordiantes and find out where to spawn
        globalCoordinates = new Vector2[localCoordinates.Length];

        int coorx = (int)(fieldWidth / 2 - 1);  // The x coordinate of the central block
        int coory = (int)(fieldHeight - WidthAndHeight.y);                       // The y coordinate of the central block
        for (int i = 0; i < localCoordinates.Length; i++)
        {
            Vector2 localV = localCoordinates[i];
            globalCoordinates[i] = new Vector2(localV.x + coorx, localV.y + coory);  // get the central coordinates
        }

        // Move the thing up, until we have reached the ceiling, so we start out all the way at the top
        while(true)
        {
            bool moved = MoveTetromino(0, 1);
            if (moved == false) { MoveTetromino(0, -1); break; }
        }
    }


    void SetWidthAndHeight()
    {
        // find the width and height of the localCoordinates
        Vector2 minCoor = new Vector2(999, 999);
        Vector2 maxCoor = new Vector2(-999, -999);
        foreach (Vector2 v in localCoordinates)
        {
            if (v.x < minCoor.x) minCoor.x = v.x;
            if (v.y < minCoor.y) minCoor.y = v.y;
            if (v.x > maxCoor.x) maxCoor.x = v.x;
            if (v.y > maxCoor.y) maxCoor.y = v.y;
        }
        WidthAndHeight.x = maxCoor.x - minCoor.x + 1;
        WidthAndHeight.y = maxCoor.y - minCoor.y + 1;
    }

    public void SetWidthAndHeightField(int _widthField, int _heightField)
    {
        // Set the width and height of the field, must be done at the beginning of the game
        FieldWidthAndHeight.x = _widthField;
        FieldWidthAndHeight.y = _heightField;
    }

    public void SetField(Color32[,] _field)
    {
        // update the field, so we can determine whether we are colliding with anything
        field = _field;
    }

    Vector2 RotateAroundPivot(Vector2 pointToRotate, Vector2 pivot, float angleDegrees)
    {
       
        // Convert the angle to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Subtract the center point's position from the point to rotate
        Vector2 relativePos = pointToRotate - pivot;

        // Apply the rotation using a Quaternion
        Quaternion rotation = Quaternion.Euler(0, 0, angleDegrees);
        Vector2 rotatedPos = rotation * relativePos;

        // Add the center point's position back to the rotated point
        Vector2 finalPos = rotatedPos + pivot;

        return finalPos;
    }


    public bool RotateTetromino(float angle)
    {
        // attempt to rotate the tetromino

        Vector2[] RotatedGlobalCoordinates = new Vector2[globalCoordinates.Length];
        for (int i =0;i< globalCoordinates.Length; i++)
        {
            RotatedGlobalCoordinates[i] = RotateAroundPivot(globalCoordinates[i], globalCoordinates[0], angle);
        }

        bool Blocked = IsBlocked(RotatedGlobalCoordinates);
        if (Blocked)
        {
            return false;
        }
        else
        {
            globalCoordinates = RotatedGlobalCoordinates;
            return true;        // we we able to move
        }

    }



    public bool MoveTetromino(int _xdiff, int _ydiff)
    {

        // create a new shape
        Vector2[] MovedGlobalCoordinates = new Vector2[globalCoordinates.Length];
        for(int i=0;i< globalCoordinates.Length;i++)
        {
            MovedGlobalCoordinates[i].x = globalCoordinates[i].x + _xdiff;
            MovedGlobalCoordinates[i].y = globalCoordinates[i].y + _ydiff;
        }

        // Check if blocked. If not, update our globalCoordinates
        bool Blocked = IsBlocked(MovedGlobalCoordinates);
        if (Blocked){
            return false;
        }
        else
        {
            globalCoordinates = MovedGlobalCoordinates;
            return true;        // we we able to move
        }

    }

    public bool IsBlocked(Vector2[] CoordinatesToCheck)
    {
        // check if the tetromino is blocked by other tetrominos or out of bounds
        foreach(Vector2 point in CoordinatesToCheck)
        {
            
            if(point.x<0  || point.y<0 || point.x>=FieldWidthAndHeight.x || point.y > FieldWidthAndHeight.y)
            {
                return true;
            }
            else
            {
                // check if we collide with inert blocks
                if (point.x < FieldWidthAndHeight.x && point.y < FieldWidthAndHeight.y)
                {
                    Color32 CurColor = field[Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y)];
                    if (CurColor.Equals(InertColor))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    
}
