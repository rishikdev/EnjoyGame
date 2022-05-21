using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float MAXIMUM_SPEED;

    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;
    private float currentSteerAngle;
    private float currentbreakForce;
    private float currentVelocity;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;
    public Transform steeringWheel;
    public Rigidbody rigidbodyCar;

    public float maximumSteerAngle = 30f;
    public float motorForce = 50f;
    public float brakeForce = 0f;


    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        UpdateCurrentVelocity();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetButton("Jump");
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = (currentVelocity < MAXIMUM_SPEED) ? verticalInput * motorForce : 0;
        rearRightWheelCollider.motorTorque = (currentVelocity < MAXIMUM_SPEED) ? verticalInput * motorForce : 0;

        if (currentVelocity * verticalInput < 0) // || isBreaking
            currentbreakForce = brakeForce;

        else if (verticalInput == 0)
            currentbreakForce = brakeForce / 7;

        else
            currentbreakForce = 0;

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        rearRightWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maximumSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
        rearLeftWheelCollider.steerAngle = -currentSteerAngle / 4;
        rearRightWheelCollider.steerAngle = -currentSteerAngle / 4;

        RotateSteeringWheel(currentSteerAngle);
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void RotateSteeringWheel(float angle)
    {
        steeringWheel.localEulerAngles = Vector3.up * angle * 2;
    }

    private void UpdateCurrentVelocity()
    {
        currentVelocity = transform.InverseTransformDirection(rigidbodyCar.velocity).z;
        currentVelocity = (currentVelocity > 0.1f || currentVelocity < -0.1f) ? currentVelocity : 0;
    }
}