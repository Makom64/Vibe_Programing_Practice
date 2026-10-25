using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class CarController : ControllableBehaviour
{
    [Header("Wheels")]
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider rearLeft;
    [SerializeField] private WheelCollider rearRight;

    [Header("Driving")]
    [SerializeField] private float motorTorque = 1200f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float parkingBrakeTorque = 5000f;

    private Vector2 moveInput;

    public override void ApplyInput(ControlInput input)
    {
        if (!hasControl)
            return;

        moveInput = input.Move;

        // input.ViewYaw는 사용하지 않습니다.
    }

    public void OnControlLost()
    {
        base.LostControl();
        moveInput = Vector2.zero;

        // 다음 물리 프레임 전에도 구동력을 바로 제거합니다.
        SetWheelState(0f, 0f, parkingBrakeTorque);
    }

    private void FixedUpdate()
    {
        float steer = hasControl
            ? moveInput.x * maxSteerAngle
            : 0f;

        float torque = hasControl
            ? moveInput.y * motorTorque
            : 0f;

        float brake = hasControl
            ? 0f
            : parkingBrakeTorque;

        SetWheelState(steer, torque, brake);
    }

    private void SetWheelState(
        float steer, float torque, float brake)
    {
        frontLeft.steerAngle = steer;
        frontRight.steerAngle = steer;

        // 예제는 후륜 구동입니다.
        frontLeft.motorTorque = 0f;
        frontRight.motorTorque = 0f;
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;

        frontLeft.brakeTorque = brake;
        frontRight.brakeTorque = brake;
        rearLeft.brakeTorque = brake;
        rearRight.brakeTorque = brake;
    }
}
