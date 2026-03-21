using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorLock : MonoBehaviour
{
    [Header("Lock Settings")]
    public int RequiredKeyIndex = 0;

    [Header("References")]
    public Transform LockPoint;
    public GameObject DoorObject;

    [Header("Rotation")]
    public float CurrentRotation = 0f;
    public float UnlockRotation = 90f;
    public float RotationTolerance = 5f;
    public float RotationMultiplier = 1f;

    [Header("Axis Settings")]
    public Vector3 LocalTurnAxis = Vector3.forward;
    public Vector3 LocalReferenceVector = Vector3.right;

    [Header("Debug")]
    public bool EnableLogs = true;
    public bool UseAbsoluteAngleForTesting = true;

    private VRKey currentKey;
    private bool unlocked = false;

    private XRGrabInteractable currentGrabInteractable;
    private Rigidbody currentKeyRigidbody;

    private Vector3 worldTurnAxis;
    private Vector3 startingControllerReferenceProjected;

    private void Awake()
    {
        Log(
            "[DoorLock Awake] " +
            "Lock=" + name +
            " | RequiredKeyIndex=" + RequiredKeyIndex +
            " | LockPoint=" + (LockPoint != null ? LockPoint.name : "NULL") +
            " | DoorObject=" + (DoorObject != null ? DoorObject.name : "NULL")
        );
    }

    private void Update()
    {
        if (unlocked) return;
        if (currentKey == null) return;
        if (!currentKey.IsInserted) return;
        if (currentKey.CurrentControllerTransform == null) return;

        Transform controllerTransform = currentKey.CurrentControllerTransform;

        Vector3 currentControllerReference =
            Vector3.ProjectOnPlane(
                controllerTransform.rotation * LocalReferenceVector,
                worldTurnAxis
            ).normalized;

        if (currentControllerReference.sqrMagnitude < 0.0001f) return;
        if (startingControllerReferenceProjected.sqrMagnitude < 0.0001f) return;

        float rawSignedAngle = Vector3.SignedAngle(
            startingControllerReferenceProjected,
            currentControllerReference,
            worldTurnAxis
        );

        float adjustedAngle = rawSignedAngle * RotationMultiplier;

        if (UseAbsoluteAngleForTesting)
        {
            CurrentRotation = Mathf.Clamp(Mathf.Abs(adjustedAngle), 0f, UnlockRotation);
        }
        else
        {
            CurrentRotation = Mathf.Clamp(adjustedAngle, 0f, UnlockRotation);
        }

        currentKey.transform.position = LockPoint.position;
        currentKey.transform.rotation =
            LockPoint.rotation * Quaternion.AngleAxis(CurrentRotation, LocalTurnAxis);

        Log(
            "[DoorLock Update] " +
            "Controller=" + controllerTransform.name +
            " | RequiredKeyIndex=" + RequiredKeyIndex +
            " | CurrentKeyIndex=" + currentKey.KeyIndex +
            " | ControllerEuler=" + controllerTransform.eulerAngles +
            " | RawSignedAngle=" + rawSignedAngle +
            " | AdjustedAngle=" + adjustedAngle +
            " | CurrentRotation=" + CurrentRotation
        );

        if (Mathf.Abs(CurrentRotation - UnlockRotation) <= RotationTolerance)
        {
            UnlockDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (unlocked) return;

        VRKey key = other.GetComponent<VRKey>();
        if (key == null) return;
        if (!key.IsHeld()) return;

        if (key.KeyIndex != RequiredKeyIndex)
        {
            LogWarning(
                "[DoorLock OnTriggerEnter] Wrong key. " +
                "RequiredKeyIndex=" + RequiredKeyIndex +
                " | KeyIndex=" + key.KeyIndex +
                " | KeyName=" + key.name
            );
            return;
        }

        currentKey = key;
        currentGrabInteractable = currentKey.GetComponent<XRGrabInteractable>();
        currentKeyRigidbody = currentKey.GetComponent<Rigidbody>();

        Log(
            "[DoorLock OnTriggerEnter BEFORE INSERT] " +
            "Key=" + currentKey.name +
            " | RequiredKeyIndex=" + RequiredKeyIndex +
            " | CurrentKeyIndex=" + currentKey.KeyIndex +
            " | ControllerRef=" + (currentKey.CurrentControllerTransform != null ? currentKey.CurrentControllerTransform.name : "NULL") +
            " | KeyPos=" + currentKey.transform.position +
            " | KeyEuler=" + currentKey.transform.eulerAngles
        );

        // Mark inserted BEFORE disabling grab interactable, so the forced release preserves controller ref
        currentKey.InsertIntoLock(this);

        if (currentGrabInteractable != null)
        {
            currentGrabInteractable.enabled = false;
        }

        if (currentKeyRigidbody != null)
        {
            currentKeyRigidbody.linearVelocity = Vector3.zero;
            currentKeyRigidbody.angularVelocity = Vector3.zero;
            currentKeyRigidbody.isKinematic = true;
        }

        currentKey.transform.position = LockPoint.position;
        currentKey.transform.rotation = LockPoint.rotation;

        CurrentRotation = 0f;
        worldTurnAxis = LockPoint.TransformDirection(LocalTurnAxis).normalized;

        if (currentKey.CurrentControllerTransform != null)
        {
            startingControllerReferenceProjected =
                Vector3.ProjectOnPlane(
                    currentKey.CurrentControllerTransform.rotation * LocalReferenceVector,
                    worldTurnAxis
                ).normalized;

            Log(
                "[DoorLock OnTriggerEnter AFTER INSERT] " +
                "ControllerRef=" + currentKey.CurrentControllerTransform.name +
                " | RequiredKeyIndex=" + RequiredKeyIndex +
                " | CurrentKeyIndex=" + currentKey.KeyIndex +
                " | ControllerEuler=" + currentKey.CurrentControllerTransform.eulerAngles +
                " | WorldTurnAxis=" + worldTurnAxis +
                " | StartReference=" + startingControllerReferenceProjected
            );
        }
        else
        {
            LogWarning("[DoorLock OnTriggerEnter] CurrentControllerTransform is NULL after insertion");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        VRKey key = other.GetComponent<VRKey>();
        if (key == null) return;
        if (key != currentKey) return;

        ReleaseKeyFromLock();
    }

    private void ReleaseKeyFromLock()
    {
        Log(
            "[DoorLock ReleaseKeyFromLock] " +
            "CurrentKey=" + (currentKey != null ? currentKey.name : "NULL")
        );

        if (currentKey != null)
        {
            currentKey.RemoveFromLock();
        }

        if (currentKeyRigidbody != null)
        {
            currentKeyRigidbody.isKinematic = false;
        }

        if (currentGrabInteractable != null)
        {
            currentGrabInteractable.enabled = true;
        }

        currentKey = null;
        currentGrabInteractable = null;
        currentKeyRigidbody = null;
        CurrentRotation = 0f;
    }

    private void UnlockDoor()
    {
        unlocked = true;

        Log("[DoorLock UnlockDoor] Door unlocked");

        if (DoorObject != null)
        {
            DoorObject.SetActive(false);
        }

        ReleaseKeyFromLock();
    }

    private void Log(string message)
    {
        if (EnableLogs)
            Debug.Log(message);
    }

    private void LogWarning(string message)
    {
        if (EnableLogs)
            Debug.LogWarning(message);
    }
}