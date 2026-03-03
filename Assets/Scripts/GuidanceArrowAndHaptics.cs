using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GuidanceArrowAndHaptics : MonoBehaviour
{
    [Header("Target Source")]
    public CheckpointNavigationManager NavigationManager;

    [Header("Arrow Placement")]
    public Transform CameraTransform;
    public float DistanceFromCamera = 1.2f;
    public Vector3 LocalOffset = new(0f, -0.15f, 0f);
    public float RotationSmoothing = 12f;

    [Header("Haptics References")]
    public XRBaseController LeftController;
    public XRBaseController RightController;
    public Transform LeftHand;
    public Transform RightHand;

    [Header("Haptics Behavior")]
    [Tooltip("No vibration if angle is greater than this (degrees).")]
    public float CutoffAngle = 90f;

    public float MinAmplitude = 0.05f;
    public float MaxAmplitude = 0.35f;

    [Tooltip("Length of each haptic pulse (seconds).")]
    public float PulseDuration = 0.05f;

    [Tooltip("How often pulses are sent. Higher = smoother but more spam.")]
    public float PulseRateHz = 20f;

    [Header("Optional gating")]
    public bool OnlyWhenRocketsEquipped = true;
    public Player Player;

    float _nextPulseTime;

    void Reset()
    {
        if (Camera.main != null)
            CameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (NavigationManager == null)
            NavigationManager = CheckpointNavigationManager.Instance;

        if (CameraTransform == null)
            return;

        Transform target = NavigationManager != null ? NavigationManager.CurrentTarget : null;

        // Keep arrow in front of headset (3D "HUD")
        transform.position = CameraTransform.position
                           + CameraTransform.forward * DistanceFromCamera
                           + CameraTransform.TransformVector(LocalOffset);

        if (target == null)
            return;

        // Rotate arrow toward target
        Vector3 toTarget = target.position - transform.position;
        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion desired = Quaternion.LookRotation(toTarget.normalized, CameraTransform.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desired,
                1f - Mathf.Exp(-RotationSmoothing * Time.deltaTime)
            );
        }

        // Rate-limit haptic pulses
        if (Time.time < _nextPulseTime)
            return;

        _nextPulseTime = Time.time + (1f / Mathf.Max(1f, PulseRateHz));

        if (OnlyWhenRocketsEquipped && Player != null && !Player.rocketsEquipped)
            return;

        SendHandHaptics(LeftController, LeftHand, target);
        SendHandHaptics(RightController, RightHand, target);
    }

    void SendHandHaptics(XRBaseController controller, Transform hand, Transform target)
    {
        if (controller == null || hand == null || target == null)
            return;

        Vector3 toTarget = (target.position - hand.position).normalized;
        float angle = Vector3.Angle(hand.forward, toTarget);

        // > 90° away: no vibration
        if (angle > CutoffAngle)
            return;

        // 0..90 lerp: closer to 0° => stronger
        float t = Mathf.InverseLerp(CutoffAngle, 0f, angle); // 0 at cutoff, 1 at perfect
        float amp = Mathf.Lerp(MinAmplitude, MaxAmplitude, t);

        controller.SendHapticImpulse(amp, PulseDuration);
    }
}