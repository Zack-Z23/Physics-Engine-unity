using UnityEngine;

public class TestKick : MonoBehaviour
{
    private CustomPhysics physics;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        physics = GetComponent<CustomPhysics>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           physics.AddTorque(new Vector3(0, 10f, 0));
        }
    }
}
