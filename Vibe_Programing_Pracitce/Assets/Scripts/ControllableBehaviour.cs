using UnityEngine;

public abstract class ControllableBehaviour : MonoBehaviour, IControlChangeable
{
    [SerializeField] private Transform _cameraAnchor;
    
    protected bool hasControl { get; private set; }

    public Transform ControlOrigin => transform;
    public Transform CameraAnchor => _cameraAnchor != null ? _cameraAnchor : transform;
    public virtual bool CanTakeControl => isActiveAndEnabled;
    public virtual void GetControl()
    {
        hasControl = true;
    }

    public void LostControl()
    {
        hasControl = false;
    }

    public abstract void ApplyInput(ControlInput input);
}
