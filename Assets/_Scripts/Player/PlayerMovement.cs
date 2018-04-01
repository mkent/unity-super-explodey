using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    private GridManager gridManager;

    private Vector2Int gridPosition;
    private Transform _transform;

    private float arrivalRange = 0.1f; //we've considered ourselves "Arrived"
    private float moveSpeed = 4.0f;

    private Vector2Int gridDestination;
    private Vector3 moveDirection;
    private Vector3 destination;

    private bool isMoving = false;

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
    }


    void Update()
    {
        if (AtDestination())
        {
            gridPosition = gridDestination;
            _transform.position = destination;
            isMoving = false;
        }
        else
        {
            _transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            isMoving = true;
        }

        if (isMoving) return; 

        PlayerInput();  
    }

    private void PlayerInput() //temporary -- get somethign going in input manager.
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        if (Input.GetAxis("Horizontal") < -0.1f) //left
        {
            CmdMove(new Vector2Int(0, -1));

        }
        else if (Input.GetAxis("Horizontal") > 0.1f) //right
        {
            CmdMove(new Vector2Int(0, 1));
        }
        else if (Input.GetAxis("Vertical") > 0.1f)
        {
            CmdMove(new Vector2Int(-1, 0));
        }
        else if (Input.GetAxis("Vertical") < -0.1f)
        {
            CmdMove(new Vector2Int(1, 0));
        }
    }

    [Command]
    private void CmdMove(Vector2Int direction)
    {
        RpcMove(direction);
    }

    [ClientRpc]
    public void RpcMove(Vector2Int direction)
    {
        Vector2Int targetPosition = new Vector2Int(gridPosition.x + direction.x, gridPosition.z + direction.z);

        if (!gridManager.IsValidMovePosition(targetPosition)) return;

        gridDestination = targetPosition;
        destination = gridManager.GridToWorldPosition(targetPosition);
        moveDirection = (destination - _transform.position).normalized;
    }

    private float lastDistance = 999f;

    private bool AtDestination()
    {
        float distanceToDestination = (destination - _transform.position).sqrMagnitude;

        if (distanceToDestination < arrivalRange * arrivalRange)
        {
            lastDistance = 999f;
            return true;
        }

        if(distanceToDestination > lastDistance)
        {
            //we overshot..
            lastDistance = 999f;
            return true;
        }

        lastDistance = distanceToDestination;

        return false;
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
