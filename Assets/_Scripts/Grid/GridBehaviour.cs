using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour {

	protected GridManager gridManager;

	protected virtual void Awake () 
	{
		if(gridManager == null) gridManager = FindObjectOfType<GridManager>();
	}
	

}
