using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Vector2 offset;
    [SerializeField] 
    private float speed = 0.15f;
    public float SpeedFactor = 1f;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        offset = meshRenderer.material.GetTextureOffset("_MainTex");
    }

    // Update is called once per frame
    void Update()
    {
        offset.x += speed * SpeedFactor * Time.deltaTime;
        meshRenderer.material.SetTextureOffset("_MainTex", offset);
    }
}
