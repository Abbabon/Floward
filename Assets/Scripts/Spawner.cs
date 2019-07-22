using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //TODO: randomize
    public GameObject[] clouds;
    public Transform CloudsParent;

    public float minSpawnRate = 10.0f;
    public float maxSpawnRate = 15.0f;

    public float timer;
    private float currentSpawnRate;

    // Start is called before the first frame update
    void Start(){
        Spawn();
        RefreshSpawnRate();
    }

    private void RefreshSpawnRate()
    {
        timer = 0;
        currentSpawnRate = Random.Range(minSpawnRate, maxSpawnRate);
    }

    // Update is called once per frame
    void Update(){
        if (GameManager.Instance.IsRunning)
        {
            timer += Time.deltaTime;
            if (timer >= currentSpawnRate){
                Spawn();
                RefreshSpawnRate();
            }
        }
    }

    private void Spawn(){
        GameObject newCloud = Instantiate(clouds[Random.Range(0, clouds.Length)], transform.position, Quaternion.identity);
        newCloud.transform.SetParent(CloudsParent);
    }
}
