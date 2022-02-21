using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{       
    //CAR SETUP

        [Space(20)]
        [Header("Car Setup")]
        [Space(10)]
        [Range(20, 250)]
        public int maxSpeed = 90;
        [Range(10, 120)]
        public int maxReverseSpeed = 45;
        [Range(1, 10)]
        public int accelerationMultiplier = 2;
        [Space(10)]
        [Range(10, 45)]
        public int maxSteeringAngle = 27;
        [Range(0.1f, 1f)]
        public float steeringSpeed = 0.5f;
        [Space(10)]
        [Range(100, 600)]
        public int brakeForce = 350;
        [Range(1, 10)]
        public int decelerationMultiplier = 2;
        [Space(10)]
        public Vector3 bodyMassCenter;

    //WHEELS

        [Space(20)]
        [Header("Wheels")]
        [Space(10)]
        public GameObject frontLeftMesh;
        public WheelCollider frontLeftCollider;
        [Space(10)]
        public GameObject frontRightMesh;
        public WheelCollider frontRightCollider;
        [Space(10)]
        public GameObject rearLeftMesh;
        public WheelCollider rearLeftCollider;
        [Space(10)]
        public GameObject rearRightMesh;
        public WheelCollider rearRightCollider;

    //PARTICLE SYSTEMS

        [Space(20)]
        [Header("Effects")]
        [Space(10)]
        public ParticleSystem RLWParticleSystem;
        public ParticleSystem RRWParticleSystem;
        [Space(10)]
        public TrailRenderer RLWTireSkid;
        public TrailRenderer RRWTireSkid;

    //CAR DATA

        [HideInInspector]
        public float carSpeed;
        [HideInInspector]
        public bool isTractionLocked;

    //SPEED TEXT (UI)

        [Space(20)]
        [Header("Speed Text")]
        [Space(10)]
        public Text carSpeedText;

    //PRIVATE VARIABLES

        Rigidbody carRigidbody;
        float steeringAxis;
        float throttleAxis;
        float localVelocityX;
        float localVelocityZ;
        bool deceleratingCar;
        WheelFrictionCurve FLwheelFriction;
        float FLWextremumSlip;
        WheelFrictionCurve FRwheelFriction;
        float FRWextremumSlip;
        WheelFrictionCurve RLwheelFriction;
        float RLWextremumSlip;
        WheelFrictionCurve RRwheelFriction;
        float RRWextremumSlip;  
        private float speedModifier;  
        [SerializeField] private GameObject winPanel;
        
        
        
    //MONEY AND OBSTACLE
        [Space(20)]
        [Header("Trigger and Hit")]
        [Space(10)]
        [SerializeField] private int money;
        [SerializeField] private Text moneyText;

    void Start()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;
        moneyText.text = ": " + money.ToString();

        FLwheelFriction = new WheelFrictionCurve();
            FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
            FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
            FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
            FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
            FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
            FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
        FRwheelFriction = new WheelFrictionCurve ();
            FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
            FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
            FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
            FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
            FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
            FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
        RLwheelFriction = new WheelFrictionCurve ();
            RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
            RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
            RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
            RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
            RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
            RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
        RRwheelFriction = new WheelFrictionCurve ();
            RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
            RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
            RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
            RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
            RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
            RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        speedModifier = 0.01f;
    }

    void LaunchGame()
    {
        
    }

    void Update()
    {

        //CAR DATA

        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;

        localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        //CAR PHYSICS

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Stationary)
            {
                GoForward();
                // print("Stationary");
            }

            if(touch.phase == TouchPhase.Moved)
            {
                transform.position = new Vector3 (
                    transform.position.x + touch.deltaPosition.x * speedModifier,
                    transform.position.y,
                    transform.position.z
                );
            }

            if (touch.phase == TouchPhase.Ended)
            {
                DecelerateCar();
                // print("Ended");
            }
        }
        
        AnimateWheelMeshes();
    }

    public void CarSpeedUI()
    {
        try
        {
            float absoluteCarSpeed = Mathf.Abs(carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Money")
        {
            money++;
            moneyText.text = ": " + money.ToString();
            Destroy(other.gameObject);
        }

        if(other.gameObject.tag == "Finish")
        {
            winPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    //
    //STEERING METHODS
    //

    public void ResetSteeringAngle()
    {
        if(steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if(steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }

        if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }

        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    void AnimateWheelMeshes()
    {
        try
        {
            Quaternion FLWRotation;
            Vector3 FLWPosition;
            frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
            frontLeftMesh.transform.position = FLWPosition;
            frontLeftMesh.transform.rotation = FLWRotation;

            Quaternion FRWRotation;
            Vector3 FRWPosition;
            frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
            frontRightMesh.transform.position = FRWPosition;
            frontRightMesh.transform.rotation = FRWRotation;

            Quaternion RLWRotation;
            Vector3 RLWPosition;
            rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
            rearLeftMesh.transform.position = RLWPosition;
            rearLeftMesh.transform.rotation = RLWRotation;

            Quaternion RRWRotation;
            Vector3 RRWPosition;
            rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
            rearRightMesh.transform.position = RRWPosition;
            rearRightMesh.transform.rotation = RRWRotation;
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    //
    //ENGINE AND BRACKING METHODS
    //

    public void GoForward()
    {
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);

        if(throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }

        if(localVelocityX < -1f)
        {
            Brakes();
        }
        else
        {
            if(Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void GoReverse()
    {
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);

        if(throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }

        if(localVelocityX > 1f)
        {
            Brakes();
        }
        else
        {
            if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void DecelerateCar()
    {
        if(throttleAxis != 0f)
        {
            if(throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if(throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }

            if(Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }

        carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));

        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;

        if(carRigidbody.velocity.magnitude < 0.25f)
        {
            carRigidbody.velocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }
}
