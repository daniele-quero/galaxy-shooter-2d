using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnLimit
{
    private float _yMin;
    private float _yMax;
    private float _xMin;
    private float _xMax;

    public float YMin { get => _yMin; set => _yMin = value; }
    public float XMin { get => _xMin; set => _xMin = value; }
    public float XMax { get => _xMax; set => _xMax = value; }
    public float YMax { get => _yMax; set => _yMax = value; }

    public SpawnLimit Calculate(GameObject gameObject, SpriteRenderer spriteRenderer, CameraBounds cameraBounds)
    {
        spriteRenderer = spriteRenderer == null ? gameObject.GetComponent<SpriteRenderer>() : spriteRenderer;
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            cameraBounds = cameraBounds == null ? camObj.GetComponent<CameraBounds>() : cameraBounds;
        else
            Utilities.LogNullGrabbed("Camera");

        float yOff = spriteRenderer.sprite.rect.size.y / 2 / spriteRenderer.sprite.pixelsPerUnit * gameObject.transform.lossyScale.y;
        float xOff = spriteRenderer.sprite.rect.size.x / 2 / spriteRenderer.sprite.pixelsPerUnit * gameObject.transform.lossyScale.x;
        this.YMax = cameraBounds.CameraVisual.y + yOff;
        this.YMin = -cameraBounds.CameraVisual.y - yOff;
        this.XMin = -cameraBounds.CameraVisual.x + xOff;
        this.XMax = cameraBounds.CameraVisual.x - xOff;
        return this;
    }
}
