using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GridBlock : GridBehaviour {

	private BlockType blockType;
	private GameObject model;
    private Transform _transform;
	private Vector2Int gridPosition;

    private void Start()
    {
        _transform = transform; //old habbits die hard.
    }

    public bool IsType(BlockType _blockType)
    {
        return (blockType == _blockType);
    }

    public bool IsValidMovePosition()
    {
        if (blockType == BlockType.Empty || blockType == BlockType.Spawn) return true;
        return false;
    }

	public void SetType (BlockType _blockType)
	{
		blockType = _blockType;

		if (model) 
		{
			Destroy (model);
		}

		model = gridManager.GetModel (blockType);

		model.transform.SetParent (transform);
		model.transform.localPosition = Vector3.zero;
		model.transform.localScale = Vector3.one;

        if (blockType == BlockType.Spawn)
        {
            gameObject.AddComponent<UnityEngine.Networking.NetworkStartPosition>();
        }

    }

	public void SetGridPosition(Vector2Int _gridPosition)
	{
		gridPosition = _gridPosition;
	}

    public bool IsDestructible()
    {
       //Solid blocks can't be damaged and also stop the explosion 
        if (blockType == BlockType.Solid) return false;

        return true;
    }


    public void DetonateDamage(NetworkInstanceId netID)
    {
        //Solid blocks can't be damaged and also stop the explosion 
        if (!IsDestructible()) return;

        //so this block is already empty, there could be a player here. Get gridmanager to check. Spawns are also empty.
        if (blockType == BlockType.Empty || blockType == BlockType.Spawn)
        {
            gridManager.PlayerDamage(gridPosition, netID);
        }

        if (blockType == BlockType.Spawn) return; //we don't change spawn blocks.

        SetType(BlockType.Empty);
    }

    public Vector3 Position()
    {
        if (_transform == null)
        {
            _transform = transform;
        }

        return _transform.position;
    }

    public bool IsOccupied()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.45f);

        if (hitColliders.Length > 0)
        {
            return true;
        }

        return false;
    }

   
}
