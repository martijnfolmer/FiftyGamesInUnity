using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    private bool ToDestroy = false;

    public void setTodestroy()
    {
        ToDestroy = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // set a random color
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = GetRandomLightColor();

    }

    Color GetRandomLightColor()
    {
        // Generate random values for the color components (higher values for lighter colors)
        float red = Random.Range(0.5f, 0.9f);
        float green = Random.Range(0.5f, 0.9f);
        float blue = Random.Range(0.5f, 0.9f);

        // Create and return the random color
        return new Color(red, green, blue);
    }


    // Update is called once per frame
    void Update()
    {
        // destroy self
        if (ToDestroy)
        {
            Destroy(this.gameObject);
        }

    }
}
