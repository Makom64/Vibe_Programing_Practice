using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ControlManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ControllableBehaviour initialTarget;
    [SerializeField] private ThirdPersonCamera cameraRig;

    [Header("Input")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference look;
    [SerializeField] private InputActionReference switchControl;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 4f;
    [SerializeField] private float aimDistance = 100f;

    // 조종 대상과 시선을 막는 벽을 모두 포함해야 합니다.
    [SerializeField] private LayerMask aimMask = ~0;

    private IControlChangeable current;

    private void OnEnable()
    {
        move.action.Enable();
        look.action.Enable();
        switchControl.action.Enable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ChangeControl(initialTarget);
    }

    private void OnDisable()
    {
        move.action.Disable();
        look.action.Disable();
        switchControl.action.Disable();

        current?.LostControl();
        current = null;
    }

    private void Update()
    {
        // 비활성화 후 재활성화된 경우에도 초기 대상을 복구합니다.
        if (current == null && !ChangeControl(initialTarget))
            return;

        cameraRig.ApplyLook(look.action.ReadValue<Vector2>());

        // 이번 프레임의 시점으로 F 판정을 수행합니다.
        cameraRig.RefreshPosition();

        if (switchControl.action.WasPressedThisFrame())
            TrySwitchControl();

        current.ApplyInput(new ControlInput(
            move.action.ReadValue<Vector2>(),
            cameraRig.ViewYaw));
    }

    private bool ChangeControl(IControlChangeable next)
    {
        if (next == null || !next.CanTakeControl)
            return false;

        if (ReferenceEquals(current, next))
            return true;

        current?.LostControl();

        current = next;
        current.GetControl();

        cameraRig.SetTarget(current.CameraAnchor);
        return true;
    }

    private void TrySwitchControl()
    {
        Transform view = cameraRig.transform;

        RaycastHit[] hits = Physics.RaycastAll(
            view.position,
            view.forward,
            aimDistance,
            aimMask,
            QueryTriggerInteraction.Ignore);

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            var candidate = hit.collider
                .GetComponentInParent<ControllableBehaviour>();

            // 현재 조종 대상의 Collider는 시선 판정에서 제외합니다.
            if (ReferenceEquals(candidate, current))
                continue;

            // 가장 먼저 보이는 외부 물체가 벽이거나
            // 조종 불가능한 대상이면 뒤쪽 대상에는 접근하지 않습니다.
            if (candidate == null || !candidate.CanTakeControl)
                return;

            float distance = Vector3.Distance(
                current.ControlOrigin.position,
                candidate.ControlOrigin.position);

            if (distance <= interactionRange)
                ChangeControl(candidate);

            return;
        }
    }
}
