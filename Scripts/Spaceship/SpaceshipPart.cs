using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipPart : MonoBehaviour
{
    public int part;

    public float fuel;

    public void Combine(SpaceshipPart other) {
        other.transform.SetParent(transform);
        other.transform.localPosition = Vector3.zero;
        other.transform.localRotation = Quaternion.identity;
        Destroy(other.GetComponent<SphereCollider>());
        part += other.part;
        if(part >= 15) {
            SpaceshipComplete();
        }
    }

    public void refill(float amount)
    {
        fuel += amount;
    }

    private void SpaceshipComplete() {
        // todo: victory
        GameManager.instance.PlayerVictory();
    }
}
