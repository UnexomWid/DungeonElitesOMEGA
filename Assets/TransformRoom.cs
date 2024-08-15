using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransformRoom : MonoBehaviour
{
    [SerializeField] GameObject wallsObj;
    void Start()
    {
        PolygonCollider2D coll = GetComponent<PolygonCollider2D>();
        coll.points = new Vector2[0];
        foreach(Transform child in wallsObj.transform)
        {
            EdgeCollider2D edge = child.GetComponent<EdgeCollider2D>();
            List<Vector2> points = coll.points.ToList();
            points.Add(edge.points[0]);
            coll.points = points.ToArray();
        }
    }
}
