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
            material.S‎etColor("Base Color", color);
        }
    }

    

}
