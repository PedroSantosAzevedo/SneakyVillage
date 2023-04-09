using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrasnparencyController : MonoBehaviour
{

    public TransparencyData transparencyData;
    public Material material;
    public bool isBetween = false;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBetween)
        {
            Color color = material.color;

            // Set the alpha value of the color to the transparency value
            color.a = 0.5f;
            material.color = color;

        }
        else {
            Color color = material.color;

            // Set the alpha value of the color to the transparency value
            color.a = transparencyData.normalTransparency;
            material.color = color;

        }

        isBetween = false;

    }
}
