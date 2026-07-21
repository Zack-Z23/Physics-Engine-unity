using Unity.VisualScripting;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    public BoxCollider box;
    public CustomPhysics physics;

    public float bounce = 0.6f; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 closestPoint = box.ClosestPoint(physics.transform.position);
        float distance = Vector3.Distance(physics.transform.position, closestPoint);


        if (distance < physics.radius)

        {
            Vector3 normal = (physics.transform.position - closestPoint).normalized;

            float normalSpeed = Vector3.Dot(physics.velocity, normal);
            Vector3 normalVelocity = normal * normalSpeed;
            Vector3 tangentVelocity = physics.velocity - normalVelocity;

            Vector3 spinAxis = Vector3.Cross(normal, tangentVelocity);

            physics.velocity = Vector3.Reflect(physics.velocity, normal) * bounce;

            physics.AddTorque(spinAxis * 0.5f);


            physics.transform.position = closestPoint + normal * physics.radius;
        }

    }
}
