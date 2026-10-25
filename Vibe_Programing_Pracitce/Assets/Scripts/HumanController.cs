using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class HumanController : ControllableBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float gravity = -20f;

    private CharacterController motor;
    private float verticalSpeed;

    private void Awake()
    {
        motor = GetComponent<CharacterController>();
    }

    public override void ApplyInput(ControlInput input)
    {
        if (!hasControl)
            return;

        Vector3 localMove = new Vector3(
            input.Move.x, 0f, input.Move.y);

        Vector3 worldMove = input.ViewYaw * localMove;

        if (worldMove.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(worldMove, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime);
        }

        motor.Move(worldMove * moveSpeed * Time.deltaTime);
    }

    private void Update()
    {
        // 조종을 잃어도 중력과 접지는 유지합니다.
        if (motor.isGrounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        verticalSpeed += gravity * Time.deltaTime;

        motor.Move(Vector3.up * verticalSpeed * Time.deltaTime);
    }
}
