using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineRenderer : MonoBehaviour
{
    public Transform startPoint;
    public Transform Point1;
    public Transform Point2;
    public Transform Point3;
    public Transform Point4;
    public Transform Point5;
    public Transform Point6;
    public Transform Point7;
    
    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        {
            lineRenderer.SetPosition(0,startPoint.position);
            lineRenderer.SetPosition(1, Point1.position);
            lineRenderer.SetPosition(2, Point2.position);
            lineRenderer.SetPosition(3, Point3.position);
            lineRenderer.SetPosition(4, Point4.position);
            lineRenderer.SetPosition(5, Point5.position);
            lineRenderer.SetPosition(6, Point6.position);
            lineRenderer.SetPosition(7, Point7.position);
        }
    }
}
