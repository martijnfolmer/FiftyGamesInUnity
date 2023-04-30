using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Data structure for a single cell in the board
public struct cell
{
    public enum Type {Empty, Mine, Flag };

    public Type type;                   // The type of the cell (empty, mine or a number)
    public int Number;                  // How many are bordering
    public Vector2Int Position_grid;    // The x and y location on the grid
    public Vector2 Position_pix;        // The x and y location of the grid in pixel coordinates 
    public bool revealed;               // whether the cell has been revealed or not
    public bool Marked;                 // Whether we placed a flag on it or not
    public GameObject ObjectId;           // the gameobject we are calling
}
