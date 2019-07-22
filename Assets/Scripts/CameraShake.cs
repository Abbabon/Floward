using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake _instance;
    public static CameraShake Instance { get { return _instance; } }
    private static readonly object padlock = new object();

    [SerializeField] private GameObject shakedCamera;

    private Vector3 origianlPosition;

    private void Awake()
    {
        lock (padlock)
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                //Here any additional initialization should occur:
                origianlPosition = shakedCamera.transform.localPosition;
            }
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1f) * magnitude;
            float y = Random.Range(-1, 1f) * magnitude;

            shakedCamera.transform.localPosition = new Vector3(x, y, origianlPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        shakedCamera.transform.localPosition = origianlPosition;
    }
}
