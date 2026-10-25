using UnityEngine;

public interface IControlChangeable
{
    public Transform ControlOrigin { get; }
    public Transform CameraAnchor { get; }

    public bool CanTakeControl { get; }

    public void GetControl();
    public void LostControl();
    public void ApplyInput(ControlInput input);
}
