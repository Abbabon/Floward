using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
	#region Singleton Implementation

	private static PostProcessingController _instance;
	public static PostProcessingController Instance { get { return _instance; } }
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
			}
		}
	}

	#endregion

	[SerializeField] private PostProcessVolume _openingPostProcessingVolume;
    [SerializeField] private PostProcessVolume _regularPostProcessingVolume;
}
