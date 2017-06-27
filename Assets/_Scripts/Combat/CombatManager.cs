using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public enum BombType
{
    Normal
}

[System.Serializable]
public class BombData
{
    public BombType bombType = BombType.Normal;
    public GameObject model;
    public GameObject bombExplosionParticle;
    public GameObject bombDetonateParticle;

    [HideInInspector]
    public GameObject[] bombDetonateParticles = new GameObject[4];

    public BombData()
    {
        bombDetonateParticles = new GameObject[4];
    }
}


[System.Serializable]
public class DetonateSequence
{
    public List<Vector2Int> xPos = new List<Vector2Int>();
    public List<Vector2Int> xNeg = new List<Vector2Int>();
    public List<Vector2Int> zPos = new List<Vector2Int>();
    public List<Vector2Int> zNeg = new List<Vector2Int>();

    public int Length() //eww.. maybe should've gone with a 2 dimensial array for this.
    {
        int length = 0;

        if (xPos.Count > length)
        {
            length = xPos.Count;
        }

        if (xNeg.Count > length)
        {
            length = xNeg.Count;
        }

        if (zPos.Count > length)
        {
            length = zPos.Count;
        }

        if (zNeg.Count > length)
        {
            length = zNeg.Count;
        }

        return length;
    }
}

public class CombatManager : MonoBehaviour {

    private GridManager gridManager;

    public BombData[] bombData;

    private Bomb[] bombPool = new Bomb[40]; //randomly picked 40 -- rough design doesn't call for more than 6 bombs active per player at one time. 
    private BombData[] bombModelPool;

    private float detonateTravelTime = 0.1f;
  
    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(GeneratePool());
	}

    #region Bomb Dropping and Detonation

    public void DropBomb(Vector2Int gridPosition, BombType bombType)
    {
        Bomb newBomb = GetBomb();

        if (newBomb == null)
        {
            Debug.LogError("Bomb pool returned null, there are not enough bombs in the pool");
            return;
        }

        newBomb.SetGridPosition(gridPosition);
        newBomb.SetType(bombType);
        newBomb.gameObject.SetActive(true);
    }

    public void Detonate(Vector2Int origin, int range)
    {
        DetonateSequence detonateSequence = gridManager.GetDetonateSequence(origin, range);

        StartCoroutine(RunDetonateSequence(detonateSequence));
    }


    private IEnumerator RunDetonateSequence(DetonateSequence detonationSeqeuence)
    {
            for (int i = 0; i < detonationSeqeuence.Length(); i++)
            {
                if(i < detonationSeqeuence.xPos.Count)
                {
                    gridManager.GetGridBlock(detonationSeqeuence.xPos[i]).DetonateDamage();
                }

                if (i < detonationSeqeuence.xNeg.Count)
                {
                    gridManager.GetGridBlock(detonationSeqeuence.xNeg[i]).DetonateDamage();
                }

                if (i < detonationSeqeuence.zPos.Count)
                {
                    gridManager.GetGridBlock(detonationSeqeuence.zPos[i]).DetonateDamage();
                }

                if (i < detonationSeqeuence.zNeg.Count)
                {
                    gridManager.GetGridBlock(detonationSeqeuence.zNeg[i]).DetonateDamage();
                }

                yield return new WaitForSeconds(detonateTravelTime);
            }
    }

    #endregion


    public Bomb GetBomb()
    {
        for (int i = 0; i < bombPool.Length; i++)
        {
            if(!bombPool[i].gameObject.activeSelf) //just realising this could result in pooling issues for the bomb. 
            {
                return bombPool[i];
            }
        }

        return null;
    }

    public GameObject GetModel(BombType bombType)
    {
        for(int i = 0; i < bombModelPool.Length; i++)
        {
            if (bombModelPool[i].bombType != bombType) continue;

            if (bombModelPool[i].model.activeSelf) continue;

            bombModelPool[i].model.SetActive(true); //we activate the model, where we do not activate the bomb itself. The bomb itself will begin its detonation sequence on activation.

            return bombModelPool[i].model;
        }

        return null;
    }

    public GameObject GetDetonationParticle(BombType bombType)
    {
        for (int i = 0; i < bombModelPool.Length; i++)
        {
            if (bombModelPool[i].bombType != bombType) continue;

           for(int j = 0; j < bombModelPool.Length; j++)
            {
                if (!bombModelPool[i].bombDetonateParticles[j].activeSelf) continue;

                bombModelPool[i].bombDetonateParticles[j].SetActive(true);

                return bombModelPool[i].bombDetonateParticles[j];
            }

            //bombModelPool[i].model.SetActive(true); //we activate the model, where we do not activate the bomb itself. The bomb itself will begin its detonation sequence on activation.

            //return bombModelPool[i].model;
        }

        return null;
    }

    #region Pooling
    private IEnumerator GeneratePool()
    {
        Transform _transform = transform;//caching this cause old habbits die hard. 

        bombModelPool = new BombData[bombPool.Length * bombData.Length];

        for (int i = 0; i < bombPool.Length; i++)
        {
            bombPool[i] = CreateBomb();
            bombPool[i].transform.SetParent(_transform);

            for(int j = 0; j < bombData.Length; j++) 
            {
                //we create a model for each type of bomb this bomb could possibly be, and associated particle.
                bombModelPool[i + j] = new BombData();

                bombModelPool[i + j].bombType = bombData[j].bombType;
                bombModelPool[i + j].model = Instantiate (bombData[j].model);
                bombModelPool[i + j].model.transform.SetParent(_transform);
                bombModelPool[i + j].model.SetActive(false);

                //particles - Detonate is for the moving detonation effect we could need up to 4 of these
                for(int k = 0; k < 4; k++)
                { 
                    bombModelPool[i + j].bombDetonateParticles[k] = Instantiate(bombData[j].bombDetonateParticle);
                    bombModelPool[i + j].bombDetonateParticles[k].transform.SetParent(_transform);
                    bombModelPool[i + j].bombDetonateParticles[k].SetActive(false);
                }

                //actual bomb explosion effect, only need one
                bombModelPool[i + j].bombExplosionParticle = Instantiate(bombData[j].bombExplosionParticle);
                bombModelPool[i + j].bombExplosionParticle.transform.SetParent(_transform);
                bombModelPool[i + j].bombExplosionParticle.SetActive(false);
            }

            yield return null;
        }
    }

    private Bomb CreateBomb()
    {
        GameObject newBombObject = new GameObject();
        newBombObject.name = "Bomb";
        newBombObject.AddComponent<BoxCollider>();
        newBombObject.SetActive(false);
        Bomb newBomb = newBombObject.AddComponent<Bomb>();
        newBomb.SetCombatManager(this);
        newBomb.SetGridManager(gridManager);
        return newBomb;
    }
    #endregion
}
