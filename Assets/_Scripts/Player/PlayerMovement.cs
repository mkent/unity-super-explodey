using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour {


    private GridManager gridManager;

    private Vector2Int gridPosition;
    private Transform _transform;

    private float moveDuration = 0.25f;
    private Vector2Int gridDestination;
    private Vector3 destination;
    public AnimationCurve moveCurve;


    private bool isMoving = false;

    private Player player;

    private void OnEnable()
    {
        gridManager = FindObjectOfType<GridManager>();

        _transform = transform;
        //Just making sure we're on the grid.
        gridPosition = gridManager.WorldToGridPosition(_transform.position);
        //annd actually set us to be on the grid.
        _transform.position = gridManager.GridToWorldPosition(gridPosition);
        //make sure our destinations are reset.
        destination = _transform.position;
        gridDestination = gridPosition;

        player = GetComponent<Player>();
    }


    void Update()
    {    
        if (isMoving) return; 

        PlayerInput();  
    }

  //  bool moved = false;
    private void PlayerInput() 
    {
        if (player.inputID < 0) return;

        if (Input.GetButtonDown("Left_Player" + player.inputID)) //left
        {
            Move(new Vector2Int(0, -1));
        }
      
        if (Input.GetButtonDown("Right_Player" + player.inputID)) //right
        {
            Move(new Vector2Int(0, 1));
        }
     
        if (Input.GetButtonDown("Up_Player" + player.inputID)) //up
        {
            Move(new Vector2Int(-1, 0));
        }
      
        if (Input.GetButtonDown("Down_Player" + player.inputID)) //down
        {
            Move(new Vector2Int(1, 0));
        }
    }


    void Move(Vector2Int direction) 
    {
        Vector2Int targetPosition = new Vector2Int(gridPosition.x + direction.x, gridPosition.z + direction.z);

        if (!gridManager.IsValidMovePosition(targetPosition)) return;

        gridDestination = targetPosition;
        destination = gridManager.GridToWorldPosition(targetPosition);
        isMoving = true;

        StartCoroutine(MoveLoop());
    }
    IEnumerator MoveLoop ()
    {
        float startTime = Time.time;
        Vector3 startPosition = _transform.position;

        while(Time.time - startTime < moveDuration)
        {
                    
            _transform.position =  Vector3.Lerp(startPosition, destination, moveCurve.Evaluate((Time.time - startTime) / moveDuration));
            yield return null;
        }

        isMoving = false;
        _transform.position = destination;
        gridPosition = gridDestination;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public Vector2Int GridPosition()
    {
        return gridPosition;
    }

}
