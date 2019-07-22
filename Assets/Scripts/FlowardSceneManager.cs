using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//the enum nubers here correspond the scenes build index
public enum FlowardScene
{
    Gameplay=0,
    Score=1,
}

public class FlowardSceneManager : MonoBehaviour
{
	private static FlowardSceneManager _instance;
	public static FlowardSceneManager Instance { get { return _instance; } }
    private static readonly object padlock = new object();

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
			}
		}
		DontDestroyOnLoad(this.gameObject);
	}

    public void LoadFloawardScene(FlowardScene scene)
	{
        SceneManager.LoadScene((int)scene);
	}
}
