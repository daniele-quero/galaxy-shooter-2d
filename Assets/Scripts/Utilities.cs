using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
 public static void CheckNullGrabbed(Object obj, string name)
    {
        if (obj == null)
            Debug.LogError("No " + name + " found");
    }

    public static void LogNullGrabbed(string name)
    {
        Debug.LogError("No " + name + " found");
    }
}
