using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsController : MonoBehaviour
{
	#region Singleton Implementation

	private static MaterialsController _instance;
	public static MaterialsController Instance { get { return _instance; } }
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

	[SerializeField] private List<Material> _controlledMaterials;

    public void SetMaterialsBaseMapColor(Color color)
    {
        foreach (var material in _controlledMaterials){
            material.S‎etColor("_BaseColor", color);
            material.S‎etColor("_Color", color);
        }
    }

    internal void TweenMaterialsBaseMapColor(Color color, float duration)
    {
        foreach (var material in _controlledMaterials)
        {
            LeanTween.value(gameObject, material.color, color, duration).setOnUpdate((Color val) =>
            {
                material.S‎etColor("_BaseColor", val);
                material.S‎etColor("_Color", val);
            });
        }
    }
}
