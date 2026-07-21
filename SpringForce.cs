using UnityEngine;

public class SpringForce : MonoBehaviour
{
    public Transform anchor;

    public float stiffness = 10f;

    public float damping = 1f;

  
    public float restLength = 2f;

    private CustomPhysics physics;

    void Start()
    {
        physics = GetComponent<CustomPhysics>();
    }

    void FixedUpdate()
    {
        if (anchor == null) return;

        Vector3 delta = anchor.position - transform.position;

        float currentLength = delta.magnitude;
        float stretch = currentLength - restLength;

        Vector3 direction = delta.normalized;

        Vector3 springForce = stiffness * stretch * direction;

        float velocityAlongSpring = Vector3.Dot(
            physics.velocity, direction
        );
        Vector3 dampingForce = -damping * velocityAlongSpring * direction;

        physics.AddForce(springForce + dampingForce);
    }

    void OnDrawGizmos()
    {
        if (anchor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, anchor.position);
        }
    }
}
