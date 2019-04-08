using UnityEngine;

public class CableController : MonoBehaviour
{
    #region MEMBERS

    [SerializeField] private LineRenderer cable;
    [SerializeField] private Transform[] cablePoints;

    #endregion

    #region PROPERTIES

    private LineRenderer Cable { get { return cable; } }
    private Transform[] CablePoints { get { return cablePoints; } }

    #endregion

    #region UNITY_EVENTS

    private void Update()
    {
        UpdateLineRenderer();
    }

    #endregion

    #region FUNCTIONS

    private void UpdateLineRenderer()
    {
        cable.positionCount = cablePoints.Length;

        for (int i = 0; i < cablePoints.Length; i++)
            cable.SetPosition(i, cablePoints[i].position);
    }

    #endregion
}