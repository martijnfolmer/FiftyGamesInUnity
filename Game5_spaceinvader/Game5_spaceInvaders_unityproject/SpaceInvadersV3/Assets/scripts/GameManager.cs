using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject playerTankLeft;               // The left tank
    public GameObject playerTankRight;              // The right tank

    public GameObject PlayerBullet;             // The prefab of the bullet that the tanks shoot
    public float PlayerBulletSpeed= 5.0f;       // set the bullet speed of the player bullets the tanks shoot
    public GameObject AlienObject;              // the prefab of the alien that we spawn
    public float AlienBulletSpeed = -4.0f;      // set the bullet speed of bullets that the alien shoots
    public Text VictoryOrLossTxt;               // what we use to show the state of the game

    public GameObject leftHeart_top;
    public GameObject leftHeart_middle;
    public GameObject leftHeart_bottom;

    public GameObject rightHeart_top;
    public GameObject rightHeart_middle;
    public GameObject rightHeart_bottom;

    int State = 0;                  // 0 = preBuild state, paused, 1=game movement, 2=game over

    GameObject[] allAliens;


    GameObject[] playerTankLeftHearts = new GameObject[3];
    int playerTankLeft_hp = 3;          // the health of the left tank
    GameObject[] playerTankRightHearts = new GameObject[3];
    int playerTankRight_hp = 3;         //the health of the right tank


    float LeftBorder_x = -6.8f;             // the farthest we can move to the left.
    float RightBorder_x = 6.8f;             // the farthest we can move to the right
 
    Vector2 TankSize;

    // tank Variables
    float tankSpeed = 10.0f;
    float leftReload_c = 0f;        // The timer for the left tank reloading
    float rightReload_c = 0f;       // The timer for the right tank reloading
    float reload_t = 0.2f;          // how long between being able to fire

    // Alien auto movement
    bool moveAliens = true;         // if set to true, we are moving the aliens
    float movement_c = 0f;
    float movement_t = 0.15f;        // how long before we move
    int movement_hor_c = 0;         // how many steps we have set
    int movement_hor_t = 5;
    int movement_hor_dir = 1;       // the current direction of the horizontal movement
    int movement_ver_dir = 0;       // the current direction of the vertical movement
    int movement_idx = 0;           // the id of the alien we want to currently move

    // Alien bullet creation
    float Alienattack_c = 0f;
    float Alienattack_t = 1.0f;           // how often we fire a bullet

    GameObject CurrentAlien;

    // Start is called before the first frame update
    void Start()
    {
        // remove the text about whether we won or lost
        VictoryOrLossTxt.GetComponent<Text>().text = "";

        // instantiate the tanks that the player will controll
        CreatePlayerTanks();

        // Instantiate the aliens
        CreateAliens();

        playerTankLeftHearts = new GameObject[3] { leftHeart_top, leftHeart_middle, leftHeart_bottom };
        playerTankRightHearts = new GameObject[3] { rightHeart_top, rightHeart_middle, rightHeart_bottom };

    }


    void CreateAliens()
    {
        // Create all the aliens above
        float xtop = -6.85f;         // The top left of where to spawn aliens
        float ytop = 4.0f;
        int numWidth = 10;
        int numHeight = 4;
        allAliens = new GameObject[numWidth * numHeight];       // initialize the list
        int k = 0;
        for (int j = 0; j < numHeight; j++) 
        { 
            for (int i = 0; i < numWidth; i++)
            {
                GameObject Alienc = Instantiate(AlienObject);
                Alienc.transform.position = new Vector3(xtop + 1.2f * i, ytop - 0.8f * j, 0.0f);
                allAliens[k] = Alienc;
                k++;
            }
        }
    }


    void CreatePlayerTanks()
    {
        // Check the tanksize for collisions between the tanks
        TankSize = playerTankLeft.GetComponent<SpriteRenderer>().size * playerTankLeft.GetComponent<Transform>().localScale.x;

    }


    void TankMovement(GameObject _tank, GameObject _other_tank, float _horizontal_movement)
    {
        /* Use this script to move one of the tanks and check for collisions with the edge and the other tanks
            */
        Vector3 TankPosition = _tank.transform.position;

        float NewXPosition = TankPosition.x + _horizontal_movement * Time.deltaTime;        // Find out the new desired position of the tank
        if (NewXPosition < LeftBorder_x) { NewXPosition = LeftBorder_x; }                      // make sure we don't go over the edge on the left
        else if (NewXPosition > RightBorder_x) { NewXPosition = RightBorder_x; }             // make sure we don't go over the egde on the right
        else if (_other_tank && Mathf.Abs(NewXPosition - _other_tank.transform.position.x) < TankSize.x)   // check collision with other tank
        {
            NewXPosition = TankPosition.x;
        }

        // update the position of the tank
        _tank.transform.position = new Vector3(NewXPosition, TankPosition.y, TankPosition.z);

    }

    void CreateBullet(GameObject _ObjectFrom, float _speed)
    {
        /* Create the bullet that the alien or the tank shoots
         */
        Color bulletColor = _ObjectFrom.GetComponent<SpriteRenderer>().color;
        GameObject Bullet = Instantiate(PlayerBullet);
        Bullet.GetComponent<BulletScript>().SetBulletVariables(_speed, bulletColor, _ObjectFrom.name);
        Bullet.transform.position = new Vector3(_ObjectFrom.transform.position.x, _ObjectFrom.transform.position.y, _ObjectFrom.transform.position.z);

    }


    // Update is called once per frame
    void Update()
    {

        // If state = 0, means we are playing the game
        if (State == 0)
        {
            // player movement : left and right for both tanks
            if (playerTankLeft)
            {
                if (Input.GetKey(KeyCode.A)) { TankMovement(playerTankLeft, playerTankRight, -tankSpeed); }
                if (Input.GetKey(KeyCode.D)) { TankMovement(playerTankLeft, playerTankRight, tankSpeed); }
            }
            if (playerTankRight)
            {
                if (Input.GetKey(KeyCode.LeftArrow)) { TankMovement(playerTankRight, playerTankLeft, -tankSpeed); }
                if (Input.GetKey(KeyCode.RightArrow)) { TankMovement(playerTankRight, playerTankLeft, tankSpeed); }
            }

            // shooting bullets from the tank
            if (Input.GetKeyDown(KeyCode.W) && leftReload_c == 0.0f) { CreateBullet(playerTankLeft, PlayerBulletSpeed); leftReload_c = reload_t; }
            if (Input.GetKeyDown(KeyCode.UpArrow) && rightReload_c == 0.0f) { CreateBullet(playerTankRight, PlayerBulletSpeed); rightReload_c = reload_t; }
        }

        // Decrease the timers for reloading the bullet shooting of the tanks
        leftReload_c = Mathf.Max(leftReload_c - Time.deltaTime, 0.0f);
        rightReload_c = Mathf.Max(rightReload_c - Time.deltaTime, 0.0f);

        // get the health of the tanks
        if (playerTankLeft) { playerTankLeft_hp = playerTankLeft.GetComponent<TankScript>().getHealth(); }
        if (playerTankRight) { playerTankRight_hp = playerTankRight.GetComponent<TankScript>().getHealth(); }

        // Destroy tanks
        if (playerTankLeft && playerTankLeft_hp <= 0) { Destroy(playerTankLeft); }
        if (playerTankRight && playerTankRight_hp <= 0) { Destroy(playerTankRight); }

        // Check if both tanks are destroyed
        if (playerTankLeft_hp <= 0 && playerTankRight_hp <= 0) { State = 2; VictoryOrLossTxt.GetComponent<Text>().text = "The Aliens have won!"; moveAliens = false; }

        // set health sprites of the tanks
        for (int i = 0; i < Mathf.Max(playerTankLeftHearts.Length - playerTankLeft_hp, 0); i++) { playerTankLeftHearts[i].GetComponent<SpriteRenderer>().enabled = false; }
        for (int i = 0; i < Mathf.Max(playerTankRightHearts.Length - playerTankRight_hp, 0); i++) { playerTankRightHearts[i].GetComponent<SpriteRenderer>().enabled = false; }

        // Check if any aliens are still alive and if any of them have collided
        int numAliens = 0;
        for (int i = 0; i < allAliens.Length; i++)
        {
            if (allAliens[i])
            {
                numAliens++;
                if (allAliens[i].GetComponent<Alien>().getCollided())
                {
                    State = 2;  //fail state
                    VictoryOrLossTxt.GetComponent<Text>().text = "The Aliens have won!";
                    moveAliens = false;
                }
            }
        }

        // check if we have won by killing all the aliens.
        if (numAliens == 0)
        {
            State = 1; moveAliens = false;  // State = 1, means we have won
            VictoryOrLossTxt.GetComponent<Text>().text = "Victory over the Aliens!";
        }

        // Alien shooting
        if (moveAliens && numAliens > 0)
        {
            Alienattack_c = Mathf.Max(Alienattack_c - Time.deltaTime, 0.0f); // subtract from the attack timer
            if (Alienattack_c <= 0.0f)
            {
                // find an alien to move
                int kn = 0;
                while (true)
                {
                    CurrentAlien = allAliens[Random.Range(0, allAliens.Length)];
                    if (CurrentAlien != null)
                    {
                        break;
                    }
                    else
                    {
                        kn++;
                        if (kn > allAliens.Length) { break; }
                    }
                }

                if (CurrentAlien != null)
                {
                    CreateBullet(CurrentAlien, AlienBulletSpeed);
                }


                Alienattack_c = Alienattack_t;      // reset the timer
            }
        }


        // Alien movement
        if (moveAliens && numAliens > 0)
        {
            movement_c = Mathf.Max(movement_c - Time.deltaTime, 0.0f);
            if (movement_c <= 0)
            {

                // Find the next alien (so it finds the next alien)
                int kn = 0;
                while (true)
                {
                    CurrentAlien = allAliens[movement_idx];
                    if (CurrentAlien != null)
                    {
                        break;
                    }
                    else
                    {
                        movement_idx++;
                        if (movement_idx >= allAliens.Length)
                        {
                            movement_idx = 0;
                            if (movement_ver_dir != 0)
                            {
                                movement_ver_dir = 0;
                                movement_hor_dir = movement_hor_dir * -1;
                                movement_hor_c = 0;
                            }
                            else if (movement_hor_c >= movement_hor_t)
                            {
                                movement_ver_dir = -1;
                            }
                            else
                            {
                                movement_hor_c += 1;
                            }
                        }

                        kn++;
                        if (kn > allAliens.Length) { break; }
                    }
                }


                // Find the alien we want to move
                if (CurrentAlien != null)
                {
                    // Move the alien
                    if (movement_ver_dir != 0)
                    {
                        CurrentAlien.GetComponent<Alien>().MoveAlien(0, movement_ver_dir);
                    }
                    else
                    {
                        CurrentAlien.GetComponent<Alien>().MoveAlien(movement_hor_dir, 0);
                    }
                }



                //increse movement idx
                movement_idx++;
                if (movement_idx >= allAliens.Length)
                {
                    movement_idx = 0;
                    if (movement_ver_dir != 0)
                    {
                        movement_ver_dir = 0;
                        movement_hor_dir = movement_hor_dir * -1;
                        movement_hor_c = 0;
                    }
                    else if (movement_hor_c >= movement_hor_t)
                    {
                        movement_ver_dir = -1;
                    }
                    else
                    {
                        movement_hor_c += 1;
                    }
                }
                //reset timer
                movement_c = movement_t;
            }
        }
    }
    
}
