using UnityEngine;

public class DistanceConstraint : MonoBehaviour
{


    public CustomPhysics ballA;
    public CustomPhysics ballB;

    public float restLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector3.Distance(ballA.transform.position, ballB.transform.position);
        Vector3 normal = (ballA.transform.position - ballB.transform.position).normalized;
        float C = distance - restLength;

        float invMassA = 1f / ballA.mass;
        float invMassB = 1f / ballB.mass;
        float totalInvMass = invMassA + invMassB;
        Vector3 correction = normal * (C / totalInvMass);
        ballA.transform.position -= correction * invMassA;
        ballB.transform.position += correction * invMassB;


        float normalSpeedA = Vector3.Dot(ballA.velocity, normal);
        Vector3 normalVelocity = normal * normalSpeedA;
        ballA.velocity -= normalVelocity;
  
        ballB.velocity += normalVelocity;

    }
}
