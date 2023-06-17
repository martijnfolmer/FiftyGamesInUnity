using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    // This entire script is just to handle the healthpools of the tanks
    int health_c = 3;

    public void setHealth(int damage)
    {
        health_c -= damage;
    }

    public int getHealth()
    {
        return health_c;
    }
}
