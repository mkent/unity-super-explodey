using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public struct BlockData 
{
	public BlockType blockType;
	public GameObject model;
}

[System.Serializable]
public enum BlockType 
{
	Empty,
    Spawn,
    Solid,
	Destructible
}

[System.Serializable]
public struct Vector2Int
{
    public int x;
    public int z;

    public Vector2Int (int _x,int _z)
    {
        x = _x;
        z = _z;
    }

}

public class GridManager : MonoBehaviour {

	private PlayerManager playerManager;

    public UnityEvent OnGridLoaded;

	public BlockData[] blockData;
    public int loadData = 0;
    private GridBlock[,] grid;

	private LoadCSV csvData;
	private bool dataReady = false;

	private void Awake ()
	{
		playerManager = FindObjectOfType<PlayerManager> ();
		csvData = GetComponent<LoadCSV> ();

		csvData.OnDataLoaded.AddListener (OnDataLoaded);
	}

    public void SpawnPlayers(int quantity = 4)
    {
        Vector3[] newPlayerSpawnPositions = new Vector3[4];


    }

    public GameObject GetModel (BlockType blockType)
	{	
		for(int i = 0; i < blockData.Length; i++)
		{
			if(blockData[i].blockType != blockType)  continue;

			GameObject newModel = Instantiate (blockData [i].model);

			return newModel;
		}

		return null;
	}

    #region Grid Combat hooks

    public void PlayerDamage(Vector2Int gridPosition, uint originNetID)
    {
        for (int i = 0; i < playerManager.players.Count; i++)
        {
            if (GridManager.IsEqual(playerManager.players[i].playerMovement.GridPosition(), gridPosition))
            {

                playerManager.players[i].playerCombat.DetonateDamage(originNetID);
            }
        }
    }

    public DetonateSequence GetDetonateSequence(Vector2Int origin, int range)
    {
        DetonateSequence detonateSequence = new DetonateSequence();

        range++; //we're adding one to the range to keep the number "logical" to design (A range of 3 means 3 blocks from the origin, not inclusive of the origin) 

        int startX = origin.x - range;
        int endX = origin.x + range;

        if (startX < -1)
        {
            startX = -1;
        }

        if (endX >= grid.GetLength(0))
        {
            endX = grid.GetLength(0);
        }

        for (int x = origin.x; x < endX; x++)
        {
            if (!grid[x, origin.z].IsDestructible())
            { //when we hit a block that can't be detonated the explosion shouldn't continue.
                break;
            }
            else
            {
                detonateSequence.xPos.Add(new Vector2Int(x, origin.z));

                if(grid[x, origin.z].StopsDetonation()) 
                {
                    break;
                }

            }
        }

        for (int x = origin.x; x > startX; x--)
        {
            if (x == origin.x) continue; //we already hit the origin. 

            if (!grid[x, origin.z].IsDestructible()) //when we hit a block that can't be detonated the explosion shouldn't continue.
            {
                break;
            }
            else
            {
                detonateSequence.xNeg.Add(new Vector2Int(x, origin.z));

                if(!grid[x, origin.z].StopsDetonation())
                {
                    break;
                }
            }
        }

        int startZ = origin.z - range;
        int endZ = origin.z + range;

        if (startZ < -1)
        {
            startZ = -1;
        }

        if (endZ >= grid.GetLength(1))
        {
            endZ = grid.GetLength(1);
        }

        for (int z = origin.z; z < endZ; z++)
        {
            if (z == origin.z) continue;

            if (!grid[origin.x, z].IsDestructible()) //when we hit a block that can't be detonated the explosion shouldn't continue.
            {
                break;
            }
            else
            {
                detonateSequence.zPos.Add(new Vector2Int(origin.x, z));

                if(grid[origin.x, z].StopsDetonation())
                {
                    break;
                }

            }
        }

        for (int z = origin.z; z > startZ; z--)
        {
            if (z == origin.z) continue; //we already hit the origin. 

            if (!grid[origin.x, z].IsDestructible()) //when we hit a block that can't be detonated the explosion shouldn't continue.
            {
                break;
            }
            else
            {
                detonateSequence.zNeg.Add(new Vector2Int(origin.x, z));

                if(grid[origin.x, z].StopsDetonation())
                {
                    break;
                }
            }
        }

        return detonateSequence;
    }

    public GridBlock GetGridBlock(Vector2Int gridPosition)
    {
        return grid[gridPosition.x, gridPosition.z];
    }

    #endregion

    #region Field Building

    public void OnDataLoaded()
    {
        dataReady = true;

        Generate(loadData);
    }

    private void Generate(int dataID)
    {
        BlockType[,] blockType = GetGridField(dataID);

        GameObject newField = new GameObject();

        grid = new GridBlock[blockType.GetLength(0), blockType.GetLength(1)];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                GridBlock newGridBlock = CreateGridBlock();

                newGridBlock.transform.SetParent(newField.transform);
                newGridBlock.transform.position = new Vector3(x, 0, z);
                newGridBlock.name += "[" + x.ToString() + "," + z.ToString() + "]" + blockType[x, z].ToString();
                newGridBlock.SetType(blockType[x, z]);
				newGridBlock.SetGridPosition(new Vector2Int(x,z));
                grid[x, z] = newGridBlock;
            }
        }

        newField.name = "GridField";

        if (OnGridLoaded != null) OnGridLoaded.Invoke();
    }

    private GridBlock CreateGridBlock()
    {
        GameObject newBlock = new GameObject();
        newBlock.name = "GridBlock";
        newBlock.AddComponent<BoxCollider>();
        GridBlock newGridBlock = newBlock.AddComponent<GridBlock>();
        return newGridBlock;
    }

    //2017-03-01 This may not be a necssary function to have. This is currently unused. 
    public Vector2 GetDimensions(int dataID)
    {
        if (!dataReady)
        {
            Debug.LogWarning("Dimensions requested for dataID " + dataID.ToString() + " before file has completed loading");
            return Vector2.zero;
        }

        return csvData.GetDimensions(dataID);
    }

    public BlockType[,] GetGridField(int dataID)
	{
		LoadCSV.FileData fileData = csvData.GetData (dataID);

		if (csvData == null) 
		{
			Debug.LogError("GridField requested for dataID + " + dataID.ToString() + ". Does not exist");
			return new BlockType[0, 0];
		}

		if (fileData.lineData.Length == 0) 
		{
			Debug.LogWarning ("GridField requested for dataID " + dataID.ToString () + ". DataID exists but has no data");
			return new BlockType[0, 0];
		}

		BlockType[,] newGridField = new BlockType[(int)fileData.lineData.Length, (int)fileData.lineData [0].cellData.Length];

		for (int x = 0; x < fileData.lineData.Length; x++) 
		{
			for (int z = 0; z < fileData.lineData [x].cellData.Length; z++) 
			{
				newGridField [x, z] = (BlockType)fileData.lineData [x].cellData [z].data;
			}
		}

		return newGridField;
	}
    #endregion

    #region Grid Utils

    public bool IsValidMovePosition(Vector2Int gridPosition)
    {
        if (!IsValidGridPosition(gridPosition)) return false;

        return grid[gridPosition.x, gridPosition.z].IsValidMovePosition();
    }


    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < grid.GetLength(0) && gridPosition.z >= 0 && gridPosition.z < grid.GetLength(1)) return true;

        return false;

    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        if (grid == null)
        {
            Debug.LogError("World position requested from grid before grid innitialized.");
            return Vector3.zero;
        }

        return grid[gridPosition.x, gridPosition.z].Position();
    }

    //I don't love doing this, but I'm using this for early testing
    public Vector2Int WorldToGridPosition(Vector3 position)
    {
        float distance = 99999f;

        Vector2Int gridPosition = new Vector2Int();

        if (grid == null)
        {
            Debug.LogError("Grid position requested from grid before grid innitialized.");
            return gridPosition;
        }

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for(int z = 0; z < grid.GetLength(1); z++)
            {
                float distanceToBlock = (grid[x, z].Position() - position).sqrMagnitude;

                if (distanceToBlock < distance)
                {
                    distance = distanceToBlock;
                    gridPosition.x = x;
                    gridPosition.z = z;
                }
            }
        }

        return gridPosition;
    }

    public static bool IsEqual(Vector2Int a, Vector2Int b)
    {
        if (a.x == b.x && a.z == b.z) return true;

        return false;
    }
    
    public Vector3 GetSpawnPosition()
    {
        List<GridBlock> gridBlocks = GetBlocksOfType(BlockType.Spawn);

        for (int i = 0; i < gridBlocks.Count; i++)
        {
            if (gridBlocks[i].IsOccupied()) continue;

            return gridBlocks[i].Position();
        }

        Debug.LogWarning("No open spawn position found, returning origin");

        return Vector3.zero;
    }
    
    public List<Vector3> GetSpawnPositions()
    {
        List<GridBlock> gridBlocks = GetBlocksOfType(BlockType.Spawn);
        List<Vector3> spawnPositions = new List<Vector3>();
        for (int i = 0; i < gridBlocks.Count; i++)
        {
            spawnPositions.Add(gridBlocks[i].Position());
        }

        if(spawnPositions.Count == 0) Debug.LogWarning("No spawn positions found, returning empty list");

        return spawnPositions;
    }

    public List<GridBlock> GetBlocksOfType(BlockType blockType)
    {
        List<GridBlock> gridBlocks = new List<GridBlock>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                if(grid[x,z].IsType(blockType))
                {
                    gridBlocks.Add(grid[x, z]);
                }
            }
        }

        return gridBlocks;
    }

    #endregion
}
