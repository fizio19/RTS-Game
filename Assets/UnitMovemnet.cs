using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public float speed = 5f;

    private Vector3 target;
    private bool moving = false;

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

            transform.position = Vector3.MoveTowards(transform.position, target, step);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

            if (Vector3.Distance(transform.position, target) <= step)
            {
                transform.position = target;
                moving = false;

                if (anim != null)
                    anim.SetBool("isMoving", false);
            }
        }

        if (anim != null)
        {
            Vector3 dir = target - transform.position;

            if (dir.magnitude > 0.01f)
            {
                dir.Normalize();
                anim.SetFloat("moveX", dir.x);
                anim.SetFloat("moveY", dir.y);
            }

            anim.SetBool("isMoving", moving);
        }
    }

    public void MoveTo(Vector3 pos)
    {
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
}