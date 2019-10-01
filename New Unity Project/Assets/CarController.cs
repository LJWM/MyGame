using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public UIManager uim;

    public float throttle;
    public float steer;
    public bool l;
    public bool brake;

    public List<WheelCollider> throttlewheels;  
    public List<GameObject> steeringwheels;
    public List<GameObject> meshes;
    public float strengthCoefficient = 10000f;
    public float maxTurn = 20f ;
    public Transform CM;
    public Rigidbody rb;
    public float brakeStrength;

    //lights
    public List<Light> lights;
    public List<GameObject> tailLights;

    public virtual void ToggleHeadlights()
    {
        foreach(Light  light in lights)
        {
            light.intensity = light.intensity == 0 ? 2 : 0;
        }
    }

   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
      if (CM)
        {
            rb.centerOfMass = CM.localPosition;
        }
    }

    // Update is called once per frame

    void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        l = Input.GetKeyDown(KeyCode.L);

        //lights
        if (l)
        {
            ToggleHeadlights();
        }

        foreach (GameObject tl in tailLights)
        { 
            tl.GetComponent<Renderer>().material.SetColor("_EmissionColor", brake ? new Color(0.5f, 0.111f, 0.111f) : Color.black);
        }
       

        //brake
        brake = Input.GetKey(KeyCode.Space);

        //UIManager
        uim.changeText(transform.InverseTransformVector(rb.velocity).z);

    }
    void FixedUpdate()
    {
        foreach (WheelCollider wheel in throttlewheels)
        {
            

            if(brake)
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = brakeStrength * Time.deltaTime;
            }
            else
            {
                wheel.motorTorque = strengthCoefficient * Time.deltaTime * throttle;
                wheel.brakeTorque = 0f;
            }
        }

        foreach (GameObject wheel in steeringwheels)
        {
            wheel.GetComponent<WheelCollider>().steerAngle = maxTurn * steer;
            wheel.transform.localEulerAngles = new Vector3(0f, steer * maxTurn, 0f);
        }

        foreach(GameObject mesh in meshes)
        {
            mesh.transform.Rotate(rb.velocity.magnitude * (transform.InverseTransformDirection(rb.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * 0.33f), 0f , 0f);
        }
    }
}
