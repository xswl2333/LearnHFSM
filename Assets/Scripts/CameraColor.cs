using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColor : MonoBehaviour
{
    [SerializeField] private Color dayColor;
    [SerializeField] private Color nightColor;
    [SerializeField] private Color duskColor;

    //选无参bool才能传入
    public void ChangeColor(SuperState type)
    {
        switch (type)
        {
            case SuperState.NULL:
                break;
            case SuperState.DAY:
                GetComponent<Camera>().backgroundColor = dayColor;
                break;
            case SuperState.NIGHT:
                GetComponent<Camera>().backgroundColor = nightColor;
                break;
            case SuperState.DUSK:
                GetComponent<Camera>().backgroundColor = duskColor;
                break;
            default:
                break;
        }

    }
}
