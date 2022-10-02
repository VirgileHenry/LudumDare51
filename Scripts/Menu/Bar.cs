using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public RectTransform bar;

    public void Set(float value) {
        bar.anchorMax = new Vector2(1, value);
        bar.offsetMin = new Vector2(5, 5);
        bar.offsetMax = new Vector2(-5, -5);
    }

}
