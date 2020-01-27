using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public Vector3[] points;

    public Transform tBound;
    public Transform bBound;

    public float cameraSpeed;

    private PlayerMovements player;

    //private bool firstMove;
    private bool moving;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<PlayerMovements>();
        
        moving = false;
	}

    private void LateUpdate()
    {
        if (!(player.transform.position.x < bBound.position.x && 
            player.transform.position.x > tBound.position.x &&
            player.transform.position.y < tBound.position.y &&
            player.transform.position.y > bBound.position.y) &&
            !player.dead)
        {
            player.Reset();

            StartCoroutine(player.DeadRoutine());
        }
    }

    // Update is called once per frame
    void Update () {
        Vector3 nearestPoint = GetNearestPoint();

        if (transform.position != nearestPoint)
        {
            if (!moving)
            {
                moving = true;

                player.Reset();

                Time.timeScale = 0.0f;

                StartCoroutine(MoveCameraRoutine(nearestPoint));
            }
        }
	}

    public Vector3 GetNearestPoint()
    {
        Vector3 nearestPoint = points[0];

        foreach (Vector3 point in points)
        {
            float distanceFromPlayer = Vector3.Distance(player.transform.position, point);

            if (distanceFromPlayer < Vector3.Distance(nearestPoint, player.transform.position))
            {
                nearestPoint = point;
            }
        }

        return nearestPoint;
    }

    public IEnumerator MoveCameraRoutine(Vector3 point)
    {
        while (Vector3.Distance(transform.position, point) > 0.5f)
        {
            transform.Translate((point - transform.position).normalized * cameraSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        transform.position = point;

        float timer = 0.0f;
        
        while (timer < 0.2f)
        {
            timer += Time.unscaledDeltaTime;

            yield return null;
        }
        
        Time.timeScale = 1.0f;

        yield return new WaitForSeconds(1.0f);

        moving = false;
    }

    public void Reset(Vector3 point)
    {
        Vector3 nearestPoint = points[0];

        foreach (Vector3 cameraPoint in points)
        {
            float distanceFromPlayer = Vector3.Distance(player.transform.position, cameraPoint);

            if (distanceFromPlayer < Vector3.Distance(nearestPoint, player.transform.position))
            {
                nearestPoint = cameraPoint;
            }
        }

        transform.position = nearestPoint;
    }
}
