using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRectOutline : MonoBehaviour
{

    public float width = 0.5f;                  // Width of the line we wish to draw
    public Vector2 size = new Vector2(1f, 1f);  // This is the size of the rectangle we wish to draw


    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();    //get our linerenderer
        SetLineRendererStaticValues();      // set the values that we only set once
        SetLineRenderer();                  // set the values that might change later
    }

    void SetLineRendererStaticValues()
    {
        lineRenderer.startWidth = width;    //set the width
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = 5;
        lineRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    }


    void SetLineRenderer()
    {
        lineRenderer.SetPosition(0, new Vector3(-size.x / 2f, -size.y / 2f, 0f));
        lineRenderer.SetPosition(1, new Vector3(size.x / 2f, -size.y / 2f, 0f));
        lineRenderer.SetPosition(2, new Vector3(size.x / 2f, size.y / 2f, 0f));
        lineRenderer.SetPosition(3, new Vector3(-size.x / 2f, size.y / 2f, 0f));
        lineRenderer.SetPosition(4, new Vector3(-size.x / 2f, -size.y / 2f - width/2, 0f));
    }

    // update the size of our outline if we want to
    public void SetSize(float width, float height)
    {
        size = new Vector2(width, height);
        SetLineRenderer();
    }
}
