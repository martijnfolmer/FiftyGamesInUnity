using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Managers : MonoBehaviour
{

    // the asteroid objects
    public GameObject[] Asteroids = new GameObject[3];
    public GameObject Ship;
    public int score = 0;          // the score
    public Text scoreText;
    public Text resetText;
    
    private string AsteroidTag = "enemy";
    private Camera mainCamera;
    private int state = 0;          // state machine AI (only 2 states, Play and Game Over)
    



    // Start is called before the first frame update
    void Start()
    {

        //States:
            // 0 = play
            // 1 = game over and ability to reset

        // Get the camera's viewport bounds in world space
        mainCamera = Camera.main;       // find the main camera currently

        // update text
        scoreText.text = "Score : " + score.ToString();
        resetText.text = "";
    }

    // Update is called once per frame
    void Update()
    {

        if (state == 0)
        {
            // spawn asteroids
            // Output the number of objects found
            Vector2 inRange = CountNumberOfEnemiesInView(AsteroidTag);     // (number in view, total number)
            
            if (inRange.y < 20)
            {
                //Choose a random asteroid
                int randomIndex = Random.Range(0, Asteroids.Length);
                GameObject randAsteroid = Asteroids[randomIndex];

                // instantiate an asteroid somewhere
                Vector3 RandPosition = GetRandomPositionOutsideCamera(mainCamera);

                var AsteroidId = Instantiate(randAsteroid, RandPosition, Quaternion.identity);
                AsteroidId.transform.rotation = this.transform.rotation;
                
            }
        }
        else
        {
            // reset everything
            if (Input.GetKey(KeyCode.R))
            {
                // reset things
                DeleteAllAsteroids(AsteroidTag);
                score = 0;
                state = 0;
                scoreText.text = "Score : " + score.ToString();
                resetText.text = "";

                // instantiate a ship
                Vector3 Middle = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                var shipId = Instantiate(Ship, new Vector3(Middle.x, Middle.y, 0.0f), Quaternion.identity);
            }
        }

    }

    public void IncreaseScore(int score_to_add)
    {
        score += score_to_add;
        scoreText.text = "Score : " + score.ToString();
    }

    void DeleteAllAsteroids(string Tag)
    {
        // Find all GameObjects with the tag 'enemy', and destroy them
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tag);
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void ChangeStateToGameOver()
    {
        state = 1;
        resetText.text = "Game over \n Press R to reset";
    }



    Vector2 CountNumberOfEnemiesInView(string Tag)
    {
        // Find all GameObjects with the tag 'enemy'
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tag);

        // Loop through each enemy GameObject
        int count = 0;
        foreach (GameObject enemy in enemies)
        {
            if (IsInsideCameraRange(enemy.transform.position))
            {
                count++;
            }
        }

        return new Vector2Int(count, enemies.Length);
    }



    bool IsInsideCameraRange(Vector3 _position)
    {
        // Get the viewport position of the object
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(_position);


        // Check if the object is outside the camera's viewport (not visible)
        return (viewportPosition.x >= 0 || viewportPosition.x <= 1 || viewportPosition.y >= 0 || viewportPosition.y <= 1);
    }


    Vector3 GetRandomPositionOutsideCamera(Camera camera)
    {
        // Get camera's viewport bounds in world space
        //Bounds viewportBounds = GetViewportBounds(camera);
        //Debug.Log(viewportBounds);

        var lowerBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)); 
        var upperBound = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));


        // Generate random position just outside the camera's viewport
        float xloc = 0f;
        float yloc = 0f;
        if (Random.Range(0, 100) < 50)
        {
            xloc = Random.Range(lowerBound.x - 1f, lowerBound.x);
        }
        else
        {
            xloc = Random.Range(upperBound.x, upperBound.x + 1f);
        }
        if (Random.Range(0, 100) < 50)
        {
            yloc = Random.Range(lowerBound.y - 1f, lowerBound.y);
        }
        else
        {
            yloc = Random.Range(upperBound.y, upperBound.y + 1f);
        }

        Vector3 randomPosition = new Vector3(
            xloc,
            yloc,
            0f  // Assuming the camera is in 2D, adjust for 3D if needed
        );

        return randomPosition;
    }


}
