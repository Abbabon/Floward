using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] _objects;
    public GameObject _previousPrefab;
    public Transform _parent;

    public float _minSpawnRate = 10.0f;
    public float _maxSpawnRate = 15.0f;

    public float _minScale = 0.8f;
    public float _maxScale = 2.0f;

    public float _minZAddition = -4f;
    public float _maxZAddition = 4f;

    public float _timer;
    private float _currentSpawnRate;

    // Start is called before the first frame update
    void Start(){
        Spawn();
        RefreshSpawnRate();
    }

    private void RefreshSpawnRate()
    {
        _timer = 0;
        _currentSpawnRate = Random.Range(_minSpawnRate, _maxSpawnRate);
    }

    // Update is called once per frame
    void Update(){
        if (GameManager.Instance.IsRunning)
        {
            _timer += Time.deltaTime;
            if (_timer >= _currentSpawnRate){
                Spawn();
                RefreshSpawnRate();
            }
        }
    }

    private void Spawn(){

        GameObject prefab;
        do{
            prefab = _objects[Random.Range(0, _objects.Length)];
        } while (_objects.Length > 1 && prefab == _previousPrefab);

        GameObject newCloud = Instantiate(prefab, transform.position, Quaternion.identity);

        float scale = Random.Range(_minScale, _maxScale);
        newCloud.transform.localScale = new Vector3(scale, scale, newCloud.transform.localScale.z);

        newCloud.transform.localPosition += new Vector3(0, 0, Random.Range(_minZAddition, _maxZAddition));

        newCloud.transform.SetParent(_parent);
    }
}
