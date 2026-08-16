using UnityEngine;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour
{
    public List<Vector2> path;
    public float speed = 5f;
    public float rotationSpeed = 360f;


    private int currentPoint = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null || path.Count == 0 || currentPoint >= path.Count)
            return;

        Vector2 target = path[currentPoint];
        Vector2 direction = target - (Vector2)transform.position;

        // Face the next waypoint
        if (direction.sqrMagnitude > 0.001f)
        {
            float targetAngle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            float angle = Mathf.MoveTowardsAngle(
                transform.eulerAngles.z,
                targetAngle,
                rotationSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Move toward it
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        // Go to next waypoint
        if (Vector2.Distance(transform.position, target) < 0.001f)
        {
            currentPoint++;
            if(currentPoint >= path.Count)
            {
                currentPoint = 0;
                transform.position = new Vector3(path[0].x, path[0].y, 0);
            }
        }
    }
    public void NewPath(List<Vector2> path)
    {
        transform.position = new Vector3(path[0].x, path[0].y, 0);
        this.path = path;
        currentPoint = 0;
    }
}
