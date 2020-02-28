using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothController : MonoBehaviour
{
    public Cloth cloth;
    public float forceMultiplyer = 100f;

    private void Reset()
    {
        cloth = GetComponent<Cloth>();
    }

    public void AddForce(float percentage)
    {
        cloth.externalAcceleration = Vector3.one * (percentage * forceMultiplyer);
    }
}
