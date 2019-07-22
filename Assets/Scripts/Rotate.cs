using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    Transform _cachedTransform;
    public Vector3 rotationSpeed = new Vector3(0, 0, 10f);

    void Awake()
    {
        _cachedTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _cachedTransform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
