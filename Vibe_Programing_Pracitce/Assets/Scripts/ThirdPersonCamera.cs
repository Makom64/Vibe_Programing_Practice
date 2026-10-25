using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _sensitivity = 0.12f;
    [SerializeField] private float _minPitch = -25f;
    [SerializeField] private float _maxPitch = 70f;

    private Transform _target;
    private float _yaw;
    private float _pitch = 15f;

    public Quaternion ViewYaw => Quaternion.Euler(0f, _yaw, 0f);

    private void Awake()
    {
        _yaw = transform.eulerAngles.y;
    }

    public void SetTarget(Transform anchor)
    {
        _target = anchor;
        RefreshPosition();
    }

    public void ApplyLook(Vector2 mouseInput)
    {
        _yaw += mouseInput.x * _sensitivity;
        _pitch = Mathf.Clamp(
            _pitch - mouseInput.y * _sensitivity,
            _minPitch,
            _maxPitch);
    }

    public void RefreshPosition()
    {
        if (_target == null)
            return;

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        
        transform.SetPositionAndRotation(_target.position - rotation * Vector3.forward * _distance, rotation);
    }

    private void LateUpdate()
    {
        RefreshPosition();
    }
}
