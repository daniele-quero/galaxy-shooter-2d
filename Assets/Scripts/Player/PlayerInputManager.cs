using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public float xAxis;
    public float yAxis;
    public bool shootButtonPressed;
    public bool collectButtonHeld;
    public bool collectButtonPressed;

    void Update()
    {
        xAxis = Input.GetAxis("Horizontal"); 
        yAxis = Input.GetAxis("Vertical");
        shootButtonPressed = Input.GetKeyDown(KeyCode.Space);
        collectButtonHeld = Input.GetKey(KeyCode.C);
        collectButtonPressed = Input.GetKeyDown(KeyCode.C);
    }

    
}
