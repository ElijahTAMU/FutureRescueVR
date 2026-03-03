using System.Collections.Generic;
using UnityEngine;

public class CheckpointNavigationManager : MonoBehaviour
{
    public static CheckpointNavigationManager Instance { get; private set; }

    [Tooltip("Usually your Player root (or XR Origin root) for distance checks.")]
    public Transform PlayerRoot;

    private readonly List<CheckpointPickup> _activeCheckpoints = new();

    public Transform CurrentTarget { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(CheckpointPickup checkpoint)
    {
        if (!_activeCheckpoints.Contains(checkpoint))
            _activeCheckpoints.Add(checkpoint);

        RefreshTarget();
    }

    public void Unregister(CheckpointPickup checkpoint)
    {
        _activeCheckpoints.Remove(checkpoint);

        if (CurrentTarget == checkpoint.transform)
            CurrentTarget = null;

        RefreshTarget();
    }

    public void RefreshTarget()
    {
        if (PlayerRoot == null)
        {
            CurrentTarget = null;
            return;
        }

        float bestDistSq = float.PositiveInfinity;
        Transform bestTarget = null;

        for (int i = 0; i < _activeCheckpoints.Count; i++)
        {
            var cp = _activeCheckpoints[i];
            if (cp == null || !cp.gameObject.activeInHierarchy) continue;

            float dSq = (cp.transform.position - PlayerRoot.position).sqrMagnitude;
            if (dSq < bestDistSq)
            {
                bestDistSq = dSq;
                bestTarget = cp.transform;
            }
        }

        CurrentTarget = bestTarget;
    }
}