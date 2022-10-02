using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLights : MonoBehaviour
{
    public Light light;
    public float toggleSpeed;
    private float timer;

    private void Update() {
        timer += Time.deltaTime;
        if(timer > toggleSpeed) {
            light.enabled = !light.enabled;
            timer = 0;
        }
    }
}
