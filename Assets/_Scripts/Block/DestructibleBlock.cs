using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBlock : BlockBehaviour {


	public void Hit()
	{
		//hit sequence here

		gridBlock.SetType (BlockType.Empty);
	}
}
