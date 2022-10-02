using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipPart : MonoBehaviour
{
    public int part;

    public static float fuel;

    public void Combine(SpaceshipPart other) {
        other.transform.SetParent(transform);
        other.transform.localPosition = Vector3.zero;
        other.transform.localRotation = Quaternion.identity;
        part += other.part;
        other.part = part;
    }

    public void refill(float amount)
    {
        fuel += amount;
        GameManager.instance.spaceshipBar.Set(fuel / 15.0f);
        if(fuel >= 15) {
            GameManager.instance.PlayerVictory();
        }
    }

}
