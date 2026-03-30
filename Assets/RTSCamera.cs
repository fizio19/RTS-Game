using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 30f;

    public float edgeSize = 10f; // ile px od krawêdzi

    void Update()
    {
        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 pos = transform.position;

        // WSAD
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        pos += new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;

        // ruch myszk¹ przy krawêdzi
        if (Input.mousePosition.x <= edgeSize)
            pos.x -= moveSpeed * Time.deltaTime;

        if (Input.mousePosition.x >= Screen.width - edgeSize)
            pos.x += moveSpeed * Time.deltaTime;

        if (Input.mousePosition.y <= edgeSize)
            pos.y -= moveSpeed * Time.deltaTime;

        if (Input.mousePosition.y >= Screen.height - edgeSize)
            pos.y += moveSpeed * Time.deltaTime;

        transform.position = pos;
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Camera cam = GetComponent<Camera>();

        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}