using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ship_movement : MonoBehaviour
{

    public float rotationSpeed = 50.0f;
    public float movementSpeed = 10.0f;
    public GameObject BulletObject;
    public float TimeBetweenShots = 1.0f;

    Managers ManagerScript;
    float TimerBullets = 0.0f;

    private void Start()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        ManagerScript = GameManager.GetComponent<Managers>();
    }


    // Update is called once per frame
    void Update()
    {
        // USER INPUT

        // Rotate the object when the A key is pressed
        if (Input.GetKey(KeyCode.D))
        {
            RotateObject(-rotationSpeed);
        }

        // Rotate the object when the A key is pressed
        if (Input.GetKey(KeyCode.A))
        {
            RotateObject(rotationSpeed);
        }

        // movement of the ship in a forward direction
        if (Input.GetKey(KeyCode.W))
        {
            MoveObject(movementSpeed);  
        }

        // shooting a bullet if the timer is 0
        if (Input.GetKey(KeyCode.Space) && TimerBullets <= 0.1f)
        {
            // create a bullet
            var bulletId = Instantiate(BulletObject, this.transform.position, Quaternion.identity);
            bulletId.transform.rotation = this.transform.rotation;
            TimerBullets = TimeBetweenShots;
        }

        // Timer between shots
        TimerBullets = Mathf.Max(TimerBullets-Time.deltaTime, 0.0f);

    }


    void RotateObject(float rotationAmount)
    {
        transform.Rotate(new Vector3 (0, 0, 1), rotationAmount * Time.deltaTime);
    }

    void MoveObject(float movementAmount)
    {
        transform.Translate(new Vector3(movementAmount, 0, 0) * Time.deltaTime);
    }


    // collision with an asteroid
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this happens every time we collide with another body
        if (collision.gameObject.tag == "enemy")
        {

            // get the scrip of the other object and activate the destruction button.
            Asteroid asteroidScript = collision.gameObject.GetComponent<Asteroid>();
            asteroidScript.Destruction();

            ManagerScript.ChangeStateToGameOver();      // this means the game is over

            // Destroy ourselves
            Destroy(this.gameObject);
        }
    }


}
