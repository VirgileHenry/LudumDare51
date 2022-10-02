using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    private TileState currentState;
    private Quaternion defaultRotation;
    private float shakingTimer;
    public float shakingStrength = 5;
    public float shakingSpeed = 5;

    private void Start()
    {
        currentState = TileState.Alive;
        defaultRotation = transform.rotation;
        shakingTimer = 0;   
    }

    public void Shake() {
        currentState = TileState.Shaking;
    }

    public void Collapse() {
        currentState = TileState.Collapsed;
        StartCoroutine(Collapse(0.5f));
    }

    private IEnumerator Collapse(float duration) {
        float timer = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos * 0.5f;
        while(timer < duration) {
            transform.position = startPos + (timer / duration) * (targetPos - startPos);

            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPos;
    }

    private void Update() {
        switch(currentState) {
            case TileState.Shaking:
                shakingTimer += Time.deltaTime * shakingSpeed;
                transform.rotation = defaultRotation * Quaternion.AngleAxis((Mathf.PerlinNoise(shakingTimer, 0) - 0.5f) * shakingStrength, transform.up);
                break;
            default:
                break;
        }
    }
}
