using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    public float mass = 1f;
    public Vector3 velocity = Vector3.zero;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public float floorY = 0f;
    public float radius = 0.5f;
    public float restitution = 0.6f;
    public float drag = 0.05f;

    private Vector3 accumulatedForce = Vector3.zero;


    public Vector3 angularVelocity = Vector3.zero;

    public Vector3 accumulatedTorque = Vector3.zero;

    private Matrix4x4 inertiaTensor;

    public float angularDrag = 0.05f;

    void Start()
    {
        float I = (2f / 5f) * mass * radius * radius;
        inertiaTensor = Matrix4x4.zero;

        inertiaTensor[0,0] = I;
        inertiaTensor[1, 1] = I;
        inertiaTensor[2, 2] = I;
        inertiaTensor[3, 3] = 1f;

        inertiaTensor = inertiaTensor.inverse;
    }

    void FixedUpdate()
    {

        AddForce(mass * gravity);
        AddForce(-drag * velocity);

       
        Vector3 force = accumulatedForce;
        float dt = Time.fixedDeltaTime;

        Vector3 pos = transform.position;

        Vector3 k1_vel = velocity;
        Vector3 k1_acc = force / mass;

      
        Vector3 k2_vel = velocity + k1_acc * (dt / 2f);
        Vector3 k2_acc = force / mass; 


        Vector3 k3_vel = velocity + k2_acc * (dt / 2f);
        Vector3 k3_acc = force / mass;

        Vector3 k4_vel = velocity + k3_acc * dt;
        Vector3 k4_acc = force / mass;

        velocity += (dt / 6f) * (k1_acc + 2f * k2_acc + 2f * k3_acc + k4_acc);
        transform.position += (dt / 6f) * (k1_vel + 2f * k2_vel + 2f * k3_vel + k4_vel);

        Vector3 torque = accumulatedTorque;
        Vector3 angularAcceleration = MultiplyMatrix3x3(inertiaTensor,torque);

        Vector3 k1_angVel = angularVelocity;
        Vector3 k1_angAcc = angularAcceleration;

        Vector3 k2_angVel = angularVelocity + k1_angAcc * (dt / 2f);
        Vector3 k2_angAcc = angularAcceleration;

        Vector3 k3_angVel = angularVelocity + k2_angAcc * (dt / 2f);
        Vector3 k3_angAcc = angularAcceleration;

        Vector3 k4_angVel = angularVelocity + k3_angAcc * dt;
        Vector3 k4_angAcc = angularAcceleration;

        angularVelocity += (dt / 6f) * (k1_angVel + 2f * k2_angVel + 2f * k3_angVel + k4_angVel);

        Quaternion currentRot = transform.rotation;

        Quaternion angVelQuat = new Quaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0f);

        Quaternion deltaQ = MultiplyQuaternions(angVelQuat, currentRot);

        Quaternion newRot = new Quaternion(
            currentRot.x + deltaQ.x * (dt / 2f),
            currentRot.y + deltaQ.y * (dt / 2f),
            currentRot.z + deltaQ.z * (dt / 2f),
            currentRot.w + deltaQ.w * (dt / 2f)
        );

        transform.rotation = NormalizeQuartenion(newRot);

        if (transform.position.y - radius < floorY)
        {
            Vector3 p = transform.position;
            p.y = floorY + radius;
            transform.position = p;
            velocity.y = -velocity.y * restitution;
            velocity.x *= 0.98f;
            velocity.z *= 0.98f;

            angularVelocity *= 0.85f;
        }
        accumulatedTorque = Vector3.zero;
        accumulatedForce = Vector3.zero;
    }

    public void AddForce(Vector3 force)
    {
        accumulatedForce += force;
    }

    public void AddForceAtPoint(Vector3 force, Vector3 point)
    {
        accumulatedForce += force;
        Vector3 r = point - transform.position;
        accumulatedTorque += Vector3.Cross(r, force);
        
    }

    public void AddTorque(Vector3 torque)
    {
        accumulatedTorque += torque;
    }
    private Vector3 MultiplyMatrix3x3(Matrix4x4 m, Vector3 v)
    {
        Vector3 result = new Vector3(
            m[0, 0] * v.x + m[0, 1] * v.y + m[0, 2] * v.z,
            m[1, 0] * v.x + m[1, 1] * v.y + m[1, 2] * v.z,
            m[2, 0] * v.x + m[2, 1] * v.y + m[2, 2] * v.z
        );
        return result;
    }
    private Quaternion MultiplyQuaternions(Quaternion a, Quaternion b)
    {
        return new Quaternion(
            a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
            a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
            a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
        );
    }
    private Quaternion NormalizeQuartenion(Quaternion q)
    {
        float magnitude = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        if(magnitude < 0.0001f)
        {
            return Quaternion.identity;
        }
        return new Quaternion(q.x / magnitude, q.y / magnitude, q.z / magnitude, q.w / magnitude);
    }


}