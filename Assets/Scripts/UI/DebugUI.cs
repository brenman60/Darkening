using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    private List<DebugBeastPosition> beastPositions = new List<DebugBeastPosition>();

    private DebugBeastPosition? currentPositionIndicator = null;
    private float positionIndicatorTimer;

    private void Start()
    {
        BeastController.BeastRoomChanged += BeastRoomChanged;
        BeastRoomChanged(string.Empty, gameObject);
    }

    private void BeastRoomChanged(string roomName, GameObject beastPosition)
    {
        foreach (DebugBeastPosition debugBeastPosition in beastPositions)
        {
            if (debugBeastPosition.positionIndicator.name == beastPosition.name)
                currentPositionIndicator = debugBeastPosition;

            debugBeastPosition.positionIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentPositionIndicator != null)
            FlashDebugPosition();
    }

    private void FlashDebugPosition()
    {
        positionIndicatorTimer += Time.deltaTime;
        if (positionIndicatorTimer >= currentPositionIndicator?.flashTime / 3)
        {
            positionIndicatorTimer = 0;
            GameObject positionIndicator = currentPositionIndicator.Value.positionIndicator;
            positionIndicator.SetActive(!positionIndicator.activeSelf);
        }
    }

    private void OnDestroy()
    {
        BeastController.BeastRoomChanged -= BeastRoomChanged;
    }

    [Serializable]
    private struct DebugBeastPosition 
    {
        public GameObject positionIndicator;
        public float flashTime;
    }
}
