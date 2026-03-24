using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class PuzzleTile : MonoBehaviour
{
    public Vector2Int gridPos;
    public SlidePuzzleManager manager;

    private XRSimpleInteractable interactable;
    private Renderer rend;

    private Color originalColor;
    public Color highlightColor = Color.green;
    public Color presentColor = Color.blue;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        rend = GetComponent<Renderer>();

        // Prevent shared material issue
        rend.material = new Material(rend.material);
        originalColor = rend.material.color;
    }

    void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelect);
    }

    void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnSelect);
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (manager.IsValidMove(this))
            rend.material.color = highlightColor;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        rend.material.color = originalColor;
    }

    void OnSelect(SelectEnterEventArgs args)
    {
        if (manager.IsValidMove(this))
            manager.TryMove(this);
    }
}