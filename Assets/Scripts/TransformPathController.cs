using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPathController : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;

    private float percentage = 0f;

    private void Update()
    {
        transform.position = Vector3.Lerp(start, end, percentage);
    }

    public void UpdatePercentage(float percentage)
    {
        this.percentage = percentage;
    }
}
