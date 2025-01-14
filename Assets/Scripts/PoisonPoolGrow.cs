using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPoolGrow : MonoBehaviour
{
    public bool active = false;
    public float growCoef;
    public float maxScale;

    Transform myTransform;
    private void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(myTransform.localScale.x<maxScale&&active)
        {
            float size = myTransform.localScale.x + PublicVariables.deltaTime * growCoef;

            myTransform.localScale = new Vector3(size, size, 1);
        }
    }
}
