using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public float speed = 5f;

    private Vector3 target;
    private bool moving = false;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.green;
    }

    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                moving = false;
            }
        }
    }

    public void MoveTo(Vector3 pos)
    {
        target = pos;
        moving = true;
    }

    public void Select()
    {
        sr.color = Color.yellow;
    }

    public void Deselect()
    {
        sr.color = Color.green;
    }
}