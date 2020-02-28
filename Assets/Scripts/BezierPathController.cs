using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPathController : MonoBehaviour
{
    public BezierSpline berzierCurve;
    public Transform target;

    private float currentPercentage = 0f;
    private float percentage = 0f;
    private float currentVelocity = 0f;

    private void Update()
    {
        currentPercentage = Mathf.SmoothDamp(currentPercentage, percentage, ref currentVelocity, Time.deltaTime);
        target.position = berzierCurve.GetPoint(currentPercentage);
    }

    public void UpdatePercentage(float percentage)
    {
        this.percentage = percentage;
    }
}
