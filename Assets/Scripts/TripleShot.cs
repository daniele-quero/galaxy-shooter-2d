using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : MonoBehaviour
{
    void Update()
    {
        Clean();
    }

    private void Clean()
    {
        if (transform.childCount == 0)
            GameObject.Destroy(this.gameObject);
    }
}
