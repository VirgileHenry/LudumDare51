using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// randomize the behaviour of the geyser
public class Geyser : MonoBehaviour
{
    public ParticleSystem particles;
    public SphereCollider collider;
    public bool active;

    private void Start() {
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop() {
        while(true) {
            yield return new WaitForSeconds(Random.Range(0f, 20f));
            active = !active;
            if(active) {
                particles.Play();
            }
            else {
                particles.Stop();
            }
            collider.enabled = active;
        }
    }
}
