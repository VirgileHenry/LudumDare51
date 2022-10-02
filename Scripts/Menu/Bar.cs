using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public RectTransform bar;
    public bool vertical;

    public void Set(float value) {
        if(vertical) {
            bar.anchorMax = new Vector2(1, value);
            bar.offsetMin = new Vector2(5, 5);
            bar.offsetMax = new Vector2(-5, -5);
        }
        else {
            bar.anchorMax = new Vector2(value, 1);
            bar.offsetMin = new Vector2(5, 5);
            bar.offsetMax = new Vector2(-5, -5);
        }
        
    }

}
