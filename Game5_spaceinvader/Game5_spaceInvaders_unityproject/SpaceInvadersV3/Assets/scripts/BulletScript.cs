using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    float BulletSpeed = 1.0f;
    bool collided = false;
    string collisionIgnore = "Player_tank(Clone)";          // which object we can not get

    // Set all of the variables for the bullet from another game object
    public void SetBulletVariables(float _bulletSpeed, Color32 bulletColor, string _nameOfObjectIgnore)
    {
        BulletSpeed = _bulletSpeed;                             // set the speed of the bullet
        GetComponent<SpriteRenderer>().color = bulletColor;     // change the color of the bullet
        collisionIgnore = _nameOfObjectIgnore;        // set which object we don't collide with
    }

    void Update()
    {
        // check for outside of the frame
        Vector3 CurrentPosition = transform.position;
        CurrentPosition.y += BulletSpeed * Time.deltaTime;

        Debug.Log(CurrentPosition.y);
        // udpate the position in case we did no collisions and are not outside of frame
        transform.position = CurrentPosition;

        if (CurrentPosition.y > 10 || CurrentPosition.y < -10)
        {
            // Destroy the gameobject itself that the script is attached to.
            Destroy(gameObject);
        }



    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collided == false)
        {
            if (collision.gameObject.name != collisionIgnore && collision.gameObject.name!="FinalLine")       // make sure we are not colliding with the object which fires the bullet and the final line
            {
                if (collision.gameObject.name == "PlayerTankLeft" || collision.gameObject.name == "PlayerTankRight")
                {
                    // get the health of the player
                    collision.gameObject.GetComponent<TankScript>().setHealth(1);       // reduce health of the tank
                    Destroy(gameObject);        // destroy the laser
                    collided = true;            // make sure we only collide
                }
                else
                {
                    Destroy(collision.gameObject); // destroy the alien/cover
                    Destroy(gameObject);        // destroy the laser
                    collided = true;            // make sure we only collide
                }
            }
        }
    }
}
