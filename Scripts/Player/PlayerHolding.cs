
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolding : MonoBehaviour
{
    public enum HoldingState {
        Empty,
        Spaceship,
    }

    public HoldingState currentState;
    public Transform holdingSpot;
    public float grabRadius = 0.05f;
    GameObject holdedObject;

    private void Start() {
        currentState = HoldingState.Empty;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            TriggerPushed();
        }
        if(Input.GetKeyUp(KeyCode.Space)) {
            TriggerReleased();
        }
    }


    private void TriggerPushed() {
        switch(currentState) {
            case HoldingState.Empty:
                // try to grab something !
                foreach(Collider collider in Physics.OverlapSphere(holdingSpot.position, grabRadius, 1 << 8)) {
                    collider.gameObject.transform.SetParent(holdingSpot);
                    collider.gameObject.transform.localPosition = Vector3.zero;
                    collider.gameObject.GetComponent<SphereCollider>().enabled = false;
                    holdedObject = collider.gameObject;
                    currentState = HoldingState.Spaceship;
                    break;
                }
                break;
        }
    }

    private void TriggerReleased() {
        switch(currentState) {
            case HoldingState.Spaceship:
                Collider[] colliders = Physics.OverlapSphere(holdingSpot.position, grabRadius, 1 << 8);
                if(colliders.Length > 0) {
                    // combine spaceships
                    GameObject spaceship = colliders[0].gameObject;
                    spaceship.GetComponent<SpaceshipPart>().Combine(holdedObject.GetComponent<SpaceshipPart>());
                    currentState = HoldingState.Empty;
                    holdedObject = null;
                }
                else {
                    // put the spaceship down
                    holdedObject.transform.SetParent(null);
                    holdedObject.GetComponent<SphereCollider>().enabled = true;
                    currentState = HoldingState.Empty;
                    holdedObject = null;
                }
                break;
        }
    }
}
