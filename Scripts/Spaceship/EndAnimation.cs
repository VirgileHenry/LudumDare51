using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimation : MonoBehaviour
{
    public Transform shipTransform;
    public Transform cameraPos;
    public ParticleSystem engine1;
    public ParticleSystem engine2;

    public static EndAnimation instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        engine1.Stop();
        engine2.Stop();
    }

    public void Launch() {
        engine1.Play();
        engine2.Play();
        shipTransform = transform;
        while(shipTransform.parent.gameObject.TryGetComponent<SpaceshipPart>(out SpaceshipPart some)) {
            shipTransform = shipTransform.parent;
        }
        StartCoroutine(TakeOff());
    }

    private IEnumerator TakeOff() {
        Vector3 startPos = shipTransform.position;
        Vector3 targetPos = startPos + shipTransform.forward * 5;
        float timer = 0;
        float duration = 5;
        while(timer < duration) {
            shipTransform.position = startPos + (timer / duration ) * (targetPos - startPos);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }   
    }
}
