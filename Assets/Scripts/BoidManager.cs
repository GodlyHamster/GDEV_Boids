using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Boids")]
    [SerializeField]
    private int boidCount = 10;
    private List<Boid> boids = new List<Boid>();

    [Header("Bounds")]
    [SerializeField]
    Bounds bounds;
    [SerializeField]
    private bool debugBounds;

    private void Start()
    {
        for (int i = 0; i < boidCount; i++) {
            Vector2 randomPos = new Vector2(
                bounds.center.x + Random.Range(-bounds.extents.x, bounds.extents.x),
                bounds.center.y +  Random.Range(-bounds.extents.y, bounds.extents.y)
                );
            boids.Add(new Boid(randomPos));
        }
    }

    private void Update()
    {
        foreach (Boid boid in boids)
        {
            //boid centering
            Vector2 v1 = PerceivedCenter(boid);
            //boid avoiding
            Vector2 v2 = Avoiding(boid);
            //boid matching speed
            Vector2 v3 = VelocityMatching(boid);
            //keep within bounds
            Vector2 v4 = KeepWithinBounds(boid);

            boid.velocity += v1 + v2 + v3 + v4;
            boid.velocity = LimitVelocity(boid);
            boid.position += boid.velocity * Time.deltaTime;
        }
    }

    private Vector2 PerceivedCenter(Boid b)
    {
        Vector2 center = Vector2.zero;
        foreach (Boid boid in boids)
        {
            if (b != boid)
            {
                center += boid.position;
            }
        }
        center /= (boids.Count - 1);
        return (center - b.position) / 50;
    }

    private Vector2 Avoiding(Boid b)
    {
        Vector2 displacement = Vector2.zero;
        foreach (Boid boid in boids)
        {
            if (b != boid)
            {
                if ((b.position - boid.position).magnitude < 0.4f)
                {
                    displacement += (b.position - boid.position);
                }
            }
        }
        return displacement;
    }

    private Vector2 VelocityMatching(Boid b)
    {
        Vector2 perceivedVel = Vector2.zero;
        foreach (Boid boid in boids)
        {
            if (b != boid)
            {
                perceivedVel += boid.velocity;
            }
        }
        perceivedVel /= (boids.Count - 1);
        return (perceivedVel - b.velocity) / 16;
    }

    private Vector2 KeepWithinBounds(Boid b)
    {
        Vector2 velocity = Vector2.zero;

        if (b.position.x < bounds.center.x - bounds.extents.x) { velocity.x = 2f; }
        else if (b.position.x > bounds.center.x + bounds.extents.x) { velocity.x = -2f; }

        if (b.position.y < bounds.center.y - bounds.extents.y) { velocity.y = 2f; }
        else if (b.position.y > bounds.center.y + bounds.extents.y) { velocity.y = -2f; }

        return velocity;
    }

    private Vector2 LimitVelocity(Boid b)
    {
        Vector2 velocity = b.velocity;
        float speedLimit = 5f;
        if (b.velocity.magnitude > speedLimit)
        {
            velocity = (b.velocity / b.velocity.magnitude) * speedLimit;
        }
        return velocity;
    }

    private void OnDrawGizmos()
    {
        if (debugBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        foreach (Boid boid in boids)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(boid.position, 0.2f);
        }
    }
}
