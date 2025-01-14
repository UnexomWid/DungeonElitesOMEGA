using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourRoomBlocker : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        GetComponent<Animator>().enabled = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color32((byte)(spriteRenderer.color.r * 255), (byte)(spriteRenderer.color.g * 255), (byte)(spriteRenderer.color.b * 255), 0);
    }
    private void OnWillRenderObject()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Bounds bounds = spriteRenderer.bounds;
        if (GeometryUtility.TestPlanesAABB(planes, bounds))
        {
            spriteRenderer.color = new Color32((byte)(spriteRenderer.color.r * 255), (byte)(spriteRenderer.color.g * 255), (byte)(spriteRenderer.color.b * 255), 255);
        }
    }
}
