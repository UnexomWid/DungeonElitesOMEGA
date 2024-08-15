using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTarget : MonoBehaviour
{
    public Transform m_target;
    Vector3 offset;
    public Transform target
    {
        get
        {
            return m_target;
        }
        set
        {

            m_target = value;

            if(value != null)
            offset = value.transform.position - transform.position;
        }
    }

    private void Update()
    {
        if(m_target != null)
        {
            transform.position = m_target.transform.position + offset;
        }
    }
}
