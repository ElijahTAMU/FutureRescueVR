using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRKey : MonoBehaviour
{
    [Header("Key Settings")]
    public int KeyIndex = 0;

    [Header("Core References")]
    public XRGrabInteractable GrabInteractable;
    public Collider KeyCollider;
    public Rigidbody KeyRigidbody;
    public CharacterController PlayerBodyController;

    [Header("Interactor References")]
    public XRBaseInteractor LeftInteractor;
    public XRBaseInteractor RightInteractor;

    [Header("Tracked Controller Transforms")]
    public Transform LeftControllerTransform;
    public Transform RightControllerTransform;

    [Header("Debug")]
    public bool EnableLogs = true;

    [HideInInspector] public bool IsInserted = false;
    [HideInInspector] public DoorLock CurrentLock = null;
    [HideInInspector] public Transform CurrentControllerTransform = null;

    private bool isHeld = false;

    private void Reset()
    {
        GrabInteractable = GetComponent<XRGrabInteractable>();
        KeyCollider = GetComponent<Collider>();
        KeyRigidbody = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        if (GrabInteractable == null)
            GrabInteractable = GetComponent<XRGrabInteractable>();

        if (KeyCollider == null)
            KeyCollider = GetComponent<Collider>();

        if (KeyRigidbody == null)
            KeyRigidbody = GetComponent<Rigidbody>();

        Log("[VRKey Awake] Key=" + name + " | KeyIndex=" + KeyIndex);
    }

    private void OnEnable()
    {
        if (GrabInteractable != null)
        {
            GrabInteractable.selectEntered.AddListener(OnGrab);
            GrabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (GrabInteractable != null)
        {
            GrabInteractable.selectEntered.RemoveListener(OnGrab);
            GrabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;

        IXRSelectInteractor interactorObject = args.interactorObject;
        Transform interactorTransform = interactorObject != null ? interactorObject.transform : null;

        CurrentControllerTransform = null;

        if (LeftInteractor != null && interactorTransform == LeftInteractor.transform)
        {
            CurrentControllerTransform = LeftControllerTransform;
            Log("[VRKey OnGrab] LEFT interactor matched");
        }
        else if (RightInteractor != null && interactorTransform == RightInteractor.transform)
        {
            CurrentControllerTransform = RightControllerTransform;
            Log("[VRKey OnGrab] RIGHT interactor matched");
        }
        else
        {
            LogWarning("[VRKey OnGrab] Interactor did not match LeftInteractor or RightInteractor. Falling back to interactor transform.");
            CurrentControllerTransform = interactorTransform;
        }

        if (KeyCollider != null && PlayerBodyController != null)
        {
            Physics.IgnoreCollision(KeyCollider, PlayerBodyController, true);
        }

        Log(
            "[VRKey OnGrab] " +
            "Interactor=" + (interactorTransform != null ? interactorTransform.name : "NULL") +
            " | ControllerRef=" + (CurrentControllerTransform != null ? CurrentControllerTransform.name : "NULL") +
            " | KeyIndex=" + KeyIndex +
            " | KeyPos=" + transform.position +
            " | KeyEuler=" + transform.eulerAngles
        );
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // If the key is already inserted, this release is expected because DoorLock disables the grab interactable.
        // In that case, preserve the controller ref.
        if (IsInserted)
        {
            isHeld = false;

            Log(
                "[VRKey OnRelease] Release happened during insertion. Preserving controller ref. " +
                "ControllerRef=" + (CurrentControllerTransform != null ? CurrentControllerTransform.name : "NULL")
            );

            return;
        }

        isHeld = false;
        CurrentControllerTransform = null;

        if (KeyCollider != null && PlayerBodyController != null)
        {
            Physics.IgnoreCollision(KeyCollider, PlayerBodyController, false);
        }

        Log("[VRKey OnRelease] Normal release. Controller ref cleared.");
    }

    public bool IsHeld()
    {
        return isHeld;
    }

    public void InsertIntoLock(DoorLock doorLock)
    {
        IsInserted = true;
        CurrentLock = doorLock;

        Log(
            "[VRKey InsertIntoLock] " +
            "Lock=" + (doorLock != null ? doorLock.name : "NULL") +
            " | KeyIndex=" + KeyIndex +
            " | ControllerRef=" + (CurrentControllerTransform != null ? CurrentControllerTransform.name : "NULL")
        );
    }

    public void RemoveFromLock()
    {
        Log(
            "[VRKey RemoveFromLock] " +
            "Lock=" + (CurrentLock != null ? CurrentLock.name : "NULL")
        );

        IsInserted = false;
        CurrentLock = null;
        CurrentControllerTransform = null;

        if (KeyCollider != null && PlayerBodyController != null)
        {
            Physics.IgnoreCollision(KeyCollider, PlayerBodyController, false);
        }
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