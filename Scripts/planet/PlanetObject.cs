using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObject : MonoBehaviour
{
    public Material material;
    Planet planet;
    // Start is called before the first frame update
    void Start()
    {
        planet = new Planet(1, material);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
