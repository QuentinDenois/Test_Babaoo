using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    //Singleton
    public static DragController Instance;

    private bool isDragActive = false;
    public bool isDragAllowed = false;
    private Vector2 screenPosition;
    private Vector3 worldPosition;
    private Tile lastDragged;
    private Vector3 startPos;
    private Vector3 emptyPos;
    private float hitDistance = 0f;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDragAllowed)
        {
            return;
        }
        
        if(isDragActive)
        {
            if(Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                Drop();
                return;
            }
        }
        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            screenPosition = new Vector2(mousePos.x, mousePos.y);
        }
        else if(Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            return;
        }

        worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, hitDistance));

        if(isDragActive)
        {
            Drag();
        }
        else
        {
            LayerMask mask = LayerMask.GetMask("Tile");
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 10, mask);
            if(hit.collider != null)
            {
                Draggable draggable = hit.transform.gameObject.GetComponent<Draggable>();
                if(draggable != null)
                {
                    if(draggable.canBeDragged)
                    {
                        lastDragged = draggable.GetComponent<Tile>();
                        hitDistance = hit.distance;
                        InitDrag();
                    }
                }
            }
        }
    }

    void InitDrag()
    {
        startPos = lastDragged.transform.position;
        emptyPos = GameManager.Instance.emptySlot;
        isDragActive = true;
    }

    void Drag()
    {
        float minPos, maxPos;
        if(startPos.x != emptyPos.x)
        {
            if(startPos.x > emptyPos.x)
            {
                minPos = emptyPos.x;
                maxPos = startPos.x;
            }
            else
            {
                minPos = startPos.x;
                maxPos = emptyPos.x;
            }
            lastDragged.transform.position = new Vector3(Mathf.Clamp(worldPosition.x, minPos, maxPos), Mathf.Clamp(worldPosition.y, 0f, 0f), startPos.z);
        }
        else if(startPos.z != emptyPos.z)
        {
            if(startPos.z > emptyPos.z)
            {
                minPos = emptyPos.z;
                maxPos = startPos.z;
            }
            else
            {
                minPos = startPos.z;
                maxPos = emptyPos.z;
            }
            lastDragged.transform.position = new Vector3(startPos.x, Mathf.Clamp(worldPosition.y, 0f, 0f), Mathf.Clamp(worldPosition.z, minPos, maxPos));
        }
    }

    void Drop()
    {
        float distFromStart = Vector3.Distance(lastDragged.transform.position, startPos);
        float distFromEmpty = Vector3.Distance(lastDragged.transform.position, emptyPos);
        if(distFromEmpty < distFromStart)
        {
            isDragActive = false;
            lastDragged.transform.position = emptyPos;
            GameManager.Instance.UpdateBoard(lastDragged.tileID);
        }
        else
        {
            isDragActive = false;
            lastDragged.transform.position = startPos;
        }
    }
}
