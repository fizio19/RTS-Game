using UnityEngine;
using System.Collections.Generic;

public class UnitMovement : MonoBehaviour
{
    public float speed = 5f;

    private Vector3 target;
    private bool moving = false;

    private List<Vector3> path;
    private int pathIndex = 0;

    private bool usePathfinding = true;

    private Animator anim;

    [Header("Selection")]
    public GameObject selectionIndicator;

    void Start()
    {
        anim = GetComponent<Animator>();
        target = transform.position;

        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    void Update()
    {
        if (moving)
        {
            float step = speed * Time.deltaTime;

            Vector3 direction = (target - transform.position).normalized;

            // próba pełnego ruchu
            Vector3 newPos = transform.position + direction * step;

            Collider2D hit = Physics2D.OverlapCircle(newPos, 0.15f, LayerMask.GetMask("Building"));

            if (hit == null)
            {
                transform.position = newPos;
            }
            else
            {
                // SLIDING

                // próbuj ruch tylko w X
                Vector3 moveX = new Vector3(direction.x, 0, 0) * step;
                Vector3 posX = transform.position + moveX;

                if (Physics2D.OverlapCircle(posX, 0.15f, LayerMask.GetMask("Building")) == null)
                {
                    transform.position = posX;
                }
                else
                {
                    // próbuj ruch tylko w Y
                    Vector3 moveY = new Vector3(0, direction.y, 0) * step;
                    Vector3 posY = transform.position + moveY;

                    if (Physics2D.OverlapCircle(posY, 0.15f, LayerMask.GetMask("Building")) == null)
                    {
                        transform.position = posY;
                    }
                    // jeśli nic nie działa → stoi (to OK)
                }
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

            // zmiana punktu dopiero po dojściu
            if (Vector3.Distance(transform.position, target) < 0.2f)
            {
                if (usePathfinding && path != null)
                {
                    pathIndex++;

                    if (pathIndex < path.Count)
                    {
                        target = path[pathIndex];
                    }
                    else
                    {
                        moving = false;
                    }
                }
                else
                {
                    moving = false;
                }
            }
        }

        // animacja
        if (anim != null)
        {
            Vector3 moveDir = moving ? (target - transform.position) : Vector3.zero;

            if (moveDir.magnitude > 0.01f)
            {
                moveDir.Normalize();
                anim.SetFloat("moveX", moveDir.x);
                anim.SetFloat("moveY", moveDir.y);
            }

            anim.SetBool("isMoving", moving);
        }
    }

    public void MoveDirect(Vector3 pos)
    {
        usePathfinding = false;

        target = new Vector3(pos.x, pos.y, 0f);
        moving = true;

        if (anim != null)
            anim.SetBool("isMoving", true);
    }

    public void Select()
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(true);
    }

    public void Deselect()
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    public void MoveTo(Vector3 pos)
    {
        usePathfinding = true;

        path = Pathfinder.Instance.FindPath(transform.position, pos);
        pathIndex = 0;

        if (path != null && path.Count > 0)
        {
            target = path[0];
            moving = true;

            if (anim != null)
                anim.SetBool("isMoving", true);
        }
    }
}