using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    public float rotationSpeed = 150f;
    public float movementSpeed = 1.0f;

    public GameObject Asteroid_2;
    public GameObject Asteroid_3;
    


    private Camera mainCamera;
    private Vector3 MovementSpeedCur;
    private Managers ManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get the camera
        mainCamera = Camera.main;       // find the main camera currently

        // get a random movement
        CalculateHorizontalVerticalSpeed(movementSpeed);

        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        ManagerScript = GameManager.GetComponent<Managers>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateObject(rotationSpeed);
        MoveObject();

        // Check if we are too far outside of the room, if so, we destroy
        if (IsOutsideCameraRange(0.2f))
        {
            Destroy(this.gameObject);
        }

    }


    void CalculateHorizontalVerticalSpeed(float speed)
    {
        // find a random position to move to
        Vector3 worldPoint = mainCamera.ViewportToWorldPoint(new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.3f, 0.7f), 0f));
        float angle = Vector2.Angle(this.transform.position, worldPoint);

        // Calculate horizontal and vertical components
        float horizontalSpeed = speed * Mathf.Cos(angle * Mathf.Deg2Rad);
        float verticalSpeed = speed * Mathf.Sin(angle * Mathf.Deg2Rad);

        // update the speed
        MovementSpeedCur = new Vector3(horizontalSpeed, verticalSpeed, 0.0f);

    }


    void RotateObject(float rotationAmount)
    {
        transform.Rotate(new Vector3(0, 0, 1), rotationAmount * Time.deltaTime);
    }

    void MoveObject()
    {
        transform.Translate(MovementSpeedCur * Time.deltaTime, Space.World);       // move relative to the world space, not its rotation
    }

    // check if we are outside of camera range
    bool IsOutsideCameraRange(float r)
    {
        // Get the viewport position of the object
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);


        // Check if the object is outside the camera's viewport (not visible)
        return (viewportPosition.x < -r || viewportPosition.x > 1 + r || viewportPosition.y < -r || viewportPosition.y > 1 + r);
    }


    public void Destruction()
    {
        if (this.gameObject.name.Contains("Asteroid_1"))
        {

            ManagerScript.IncreaseScore(300);       // add the score

            // spawn 3 asteroid 2s
            for (int i = 0; i < 3; i++)
            {
                var AsteroidId = Instantiate(Asteroid_2, this.transform.position, Quaternion.identity);
                AsteroidId.transform.rotation = this.transform.rotation;
                AsteroidId.transform.Rotate(new Vector3(0, 0, 1), ((float)i + 0.5f)*90f);
            }

        }
        else if (this.gameObject.name.Contains("Asteroid_2"))
        {
            ManagerScript.IncreaseScore(200);       // add the score

            // spawn 3 asteroid 1s
            for (int i = 0; i < 3; i++)
            {
                var AsteroidId = Instantiate(Asteroid_3, this.transform.position, Quaternion.identity);
                AsteroidId.transform.rotation = this.transform.rotation;
                AsteroidId.transform.Rotate(new Vector3(0, 0, 1), ((float)i + 0.5f) * 90f);
            }
        }
        else
        {
            ManagerScript.IncreaseScore(100);       // add the score
        }

        // destroy the object as well
        Destroy(this.gameObject);
    }

}
