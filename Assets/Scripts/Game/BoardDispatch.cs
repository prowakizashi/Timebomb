using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDispatch
{
    private Vector3[] positions = new Vector3[8];
    float BoardHalfHeight = 2.25f;
    float BoardHalfWidth = 5f;
    float VerticalMargin = 1.1f;
    float HoritontalMargin = 1.8f;

    public BoardDispatch()
    {
        int zoomValue = GameManager.Instance.DeZoom;
        int unitLength = Screen.currentResolution.height / (zoomValue * 2);
        float widthUnits = Screen.currentResolution.width / unitLength;

        positions[0] = new Vector3(0, (-zoomValue) + BoardHalfHeight + VerticalMargin, 0);
        positions[1] = new Vector3((-widthUnits / 2) + BoardHalfWidth + HoritontalMargin, (-zoomValue) + BoardHalfHeight + VerticalMargin, 0);
        positions[2] = new Vector3((-widthUnits / 2) + BoardHalfWidth + HoritontalMargin, 0, 0);
        positions[3] = new Vector3((-widthUnits / 2) + BoardHalfWidth + HoritontalMargin, (zoomValue) - BoardHalfHeight - VerticalMargin, 0);
        positions[4] = new Vector3(0, (zoomValue) - BoardHalfHeight - VerticalMargin, 0);
        positions[5] = new Vector3((widthUnits / 2) - BoardHalfWidth - HoritontalMargin, (zoomValue) - BoardHalfHeight - VerticalMargin, 0);
        positions[6] = new Vector3((widthUnits / 2) - BoardHalfWidth - HoritontalMargin, 0, 0);
        positions[7] = new Vector3((widthUnits / 2) - BoardHalfWidth - HoritontalMargin, (-zoomValue) + BoardHalfHeight + VerticalMargin, 0);
    }

    public Vector3[] GetPositions(int boardCount)
    {
        if (boardCount == 1)
            return new Vector3[] { positions[0] };
        if (boardCount == 2)
            return new Vector3[] { positions[0], positions[4] };
        if (boardCount == 3)
            return new Vector3[] { positions[0], positions[3], positions[5] };
        if (boardCount == 4)
            return new Vector3[] { positions[0], positions[2], positions[4], positions[6] };
        if (boardCount == 5)
            return new Vector3[] { positions[0], positions[2], positions[3], positions[5], positions[6] };
        if (boardCount == 6)
            return new Vector3[] { positions[0], positions[2], positions[3], positions[4], positions[5], positions[6] };
        if (boardCount == 7)
            return new Vector3[] { positions[0], positions[1], positions[2], positions[3], positions[5], positions[6], positions[7] };
        if (boardCount == 8)
            return new Vector3[] { positions[0], positions[1], positions[2], positions[3], positions[4], positions[5], positions[6], positions[7] };
        return new Vector3[] { };
    }
}
