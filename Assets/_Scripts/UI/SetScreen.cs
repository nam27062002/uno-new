using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreen : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(640, 360, FullScreenMode.Windowed);
    }
}
