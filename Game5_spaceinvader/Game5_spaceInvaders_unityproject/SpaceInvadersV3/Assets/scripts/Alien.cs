using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Alien : MonoBehaviour
{

    public Sprite FirstSprite;              // the first sprite
    public Sprite SecondSprite;             // the second sprite

    float x_speed = 0.5f;                       // speed of a single step to the side
    float y_speed = 0.5f;                       // speed of a single step down
    
    Sprite[] allSprites;
    int SpriteIndex = 1;                // which one of the sprites we are currently doing

    SpriteRenderer m_SpriteRenderer;
    Transform m_Transform;

    bool collided = false;              // if this is set to true, it means we have lost

    void Start()
    {
        allSprites = new Sprite[2] {FirstSprite, SecondSprite};     // put the sprites in an array
        m_SpriteRenderer = GetComponent<SpriteRenderer>();          // get the sprite renderer
        m_Transform = GetComponent<Transform>();                    // get the transform

        SetSprite();            // set initial sprite
    }


    public bool getCollided() { return collided; }

    public void SetSprite() { m_SpriteRenderer.sprite = allSprites[SpriteIndex]; }      // set the sprite

    public void MoveAlien(int xdiff, int ydiff)
    {
        if (xdiff != 0 || ydiff != 0)
        {
            // change the alien sprite
            SpriteIndex = Mathf.Abs(SpriteIndex - 1);
            SetSprite();

            // move the alien

            Vector3 AlienPosition = m_Transform.position;
            float NewXPosition = AlienPosition.x + xdiff * x_speed;
            float NewYPosition = AlienPosition.y + ydiff * y_speed;


            m_Transform.position = new Vector3(NewXPosition, NewYPosition, m_Transform.position.z);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "FinalLine")
        {
            collided = true;
        }

    }
}
