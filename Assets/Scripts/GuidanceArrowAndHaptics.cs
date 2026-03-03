using UnityEngine;
using UnityEngine.XR;

public class GuidanceArrowAndHaptics : MonoBehaviour
{
    [Header("Target Source")]
    public CheckpointNavigationManager NavigationManager;

    [Header("Arrow Placement")]
    public Transform CameraTransform;
    public float DistanceFromCamera = 1.2f;
    public Vector3 LocalOffset = new(0f, -0.15f, 0f);
    public float RotationSmoothing = 12f;

    [Header("Aim Transforms (what 'forward' means)")]
    public Transform LeftHandAim;
    public Transform RightHandAim;

    [Header("Haptics Behavior")]
    [Tooltip("No vibration if angle is greater than this (degrees).")]
    public float CutoffAngle = 90f;

    [Range(0f, 1f)] public float MinAmplitude = 0.05f;
    [Range(0f, 1f)] public float MaxAmplitude = 0.35f;

    [Tooltip("Length of each haptic pulse (seconds).")]
    public float PulseDuration = 0.05f;

    [Tooltip("How often pulses are sent.")]
    public float PulseRateHz = 20f;

    [Tooltip("Haptic channel (usually 0).")]
    public uint HapticChannel = 0;

    [Header("Optional gating")]
    public bool OnlyWhenRocketsEquipped = true;
    public Player Player;

    private float _nextPulseTime;
    private InputDevice _leftDevice;
    private InputDevice _rightDevice;

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

        // Keep arrow in front of headset
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

        // Rate-limit pulses
        if (Time.time < _nextPulseTime)
            return;

        _nextPulseTime = Time.time + (1f / Mathf.Max(1f, PulseRateHz));

        if (OnlyWhenRocketsEquipped && Player != null && !Player.rocketsEquipped)
            return;

        // Acquire devices if needed
        if (!_leftDevice.isValid) _leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!_rightDevice.isValid) _rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        SendHandHaptics(_leftDevice, LeftHandAim, target);
        SendHandHaptics(_rightDevice, RightHandAim, target);
    }

    void SendHandHaptics(InputDevice device, Transform aim, Transform target)
    {
        if (!device.isValid || aim == null || target == null)
            return;

        if (!device.TryGetHapticCapabilities(out var caps) || !caps.supportsImpulse)
            return;

        Vector3 toTarget = (target.position - aim.position).normalized;
        float angle = Vector3.Angle(aim.forward, toTarget);

        // > 90° away => nothing
        if (angle > CutoffAngle)
            return;

        // 0..90 => lerp amplitude (closer = stronger)
        float t = Mathf.InverseLerp(CutoffAngle, 0f, angle);
        float amp = Mathf.Lerp(MinAmplitude, MaxAmplitude, t);
        amp = Mathf.Clamp01(amp);

        device.SendHapticImpulse(HapticChannel, amp, PulseDuration);
    }
}