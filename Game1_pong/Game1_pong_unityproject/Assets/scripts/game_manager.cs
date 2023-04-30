using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class game_manager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ball;
    public GameObject leftPaddle;
    public GameObject rightPaddle;
    public Text txt;

    int ScoreLeft = 0;      // The score of the left player
    int ScoreRight = 0;     // The score of the right player

    ball_movement bm;
    paddle_movement lp;
    paddle_movement rp;

    void Start()
    {

        bm = ball.GetComponent<ball_movement>();
        lp = leftPaddle.GetComponent<paddle_movement>();
        rp = rightPaddle.GetComponent<paddle_movement>();

        bm.shootBall(); //start the game
    }

    private void Update()
    {
        //check if the reset button is being pressed
        if (Input.GetAxis("Reset")!=0)
        {
            ResetGame();
        }
    }


    void ResetGame()
    {
        //reset score
        IncrementScore(-ScoreLeft, -ScoreRight);
        ScoreLeft = 0;
        ScoreRight = 0;

        // Reset the field as well 
        ResetField();
    }



    public void IncrementScore(int leftIncrease, int rightIncrease)
    {
        // Change our score on the text
        ScoreLeft += leftIncrease;
        ScoreRight += rightIncrease;
        string TxtValue = ScoreLeft.ToString() + " : " + ScoreRight.ToString();
        txt.text = TxtValue;
    }

    public void ResetField()
    {
        //reset ball to center field and reset speed
        ball.transform.position = new Vector3(bm.orx, bm.ory, 0); //reset position
        bm.curVx = 0.0f;    // reset speed of ball (horizontal)
        bm.curVy = 0.0f;    // reset speed of ball (vertical)
        bm.shootBall();     // shoot the ball away


        // reset paddles
        leftPaddle.transform.position = new Vector3(lp.orx, lp.ory, 0);
        rightPaddle.transform.position = new Vector3(rp.orx, rp.ory, 0);
    }


 



}
