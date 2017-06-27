using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour {

	protected GridBlock gridBlock;

	protected void Start()
	{
		gridBlock = GetComponent<GridBlock> ();
	}
}
