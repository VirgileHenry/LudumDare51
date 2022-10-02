using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    private TileState currentState;
    private float shakingTimer;
    public float shakingStrength = 10;
    public float shakingSpeed = 5;
    public float elevationStrength = 0.06f;
    public float randomOffset;
    private Vector3 normal;

    public void SetNormal(Vector3 normal) {
        this.normal = normal;
    }

    private void Start()
    {
        currentState = TileState.Alive;
        shakingTimer = 0;
        randomOffset = Random.Range(0f, 1000f);
    }

    public void Shake() {
        currentState = TileState.Shaking;
    }

    public void Collapse() {
        currentState = TileState.Collapsed;
        StartCoroutine(CollapseCoroutine(0.5f));
    }

    private IEnumerator CollapseCoroutine(float duration) {
        float timer = 0;
        Vector3 startPos = Vector3.zero;
        Vector3 targetPos = -normal * 0.5f;
        while(timer < duration) {
            transform.position = startPos + (timer / duration) * (targetPos - startPos);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPos;
    }

    private void Update() {
        switch(currentState) {
            case TileState.Shaking:
                shakingTimer += Time.deltaTime * shakingSpeed;
                transform.rotation = Quaternion.AngleAxis((Mathf.PerlinNoise(randomOffset + shakingTimer, 0) - 0.5f) * shakingStrength, normal);
                transform.position = normal * elevationStrength * (Mathf.PerlinNoise(randomOffset + shakingTimer, 0) - 0.5f);
                break;
            default:
                break;
        }
    }
}
