using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class ControllerStickHandler : MonoBehaviour
{
    [Header("Input Actions (assign in editor)")]
    public InputActionReference leftMoveAction;   // Vector2
    public InputActionReference rightTurnAction;  // Vector2
    public InputActionReference rightTeleportAction; // Button

    [Header("Providers")]
    public DynamicMoveProvider moveProvider;
    public SnapTurnProvider snapTurnProvider;
    public TeleportationProvider teleportProvider;

    void OnEnable()
    {
        leftMoveAction.action.Enable();
        rightTurnAction.action.Enable();
        rightTeleportAction.action.Enable();
    }

    void OnDisable()
    {
        leftMoveAction.action.Disable();
        rightTurnAction.action.Disable();
        rightTeleportAction.action.Disable();
    }

    void Update()
    {
        // Nothing needed here for movement or turn.
        // ContinuousMoveProvider and SnapTurnProvider will read the InputActions automatically.

        // Optional: teleport trigger
        if (rightTeleportAction.action.triggered)
        {
            teleportProvider.QueueTeleportRequest(new TeleportRequest
            {
                destinationPosition = transform.position + transform.forward * 2f,
                matchOrientation = MatchOrientation.WorldSpaceUp
            });
        }
    }
}