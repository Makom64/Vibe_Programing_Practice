using UnityEngine;

public class ControlInput
{
    public Vector2 Move { get; }
    public Quaternion ViewYaw { get; }

    public ControlInput(Vector2 move, Quaternion viewYaw)
    {
        Move = Vector2.ClampMagnitude(move, 1f);
        ViewYaw = viewYaw;
    }
}
