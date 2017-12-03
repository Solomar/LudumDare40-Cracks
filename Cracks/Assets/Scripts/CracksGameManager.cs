using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType { HAMMER, HAND, HOTGLUE }

public class CracksGameManager : MonoBehaviour
{
    public static bool CrackCompleted { get; set; }

    private int m_currentCrackAreaIndex;
    private List<CrackArea> m_allCrackArea = new List<CrackArea>();

    private void Start()
    {
        GetComponentsInChildren<CrackArea>(m_allCrackArea);
        Camera.main.transform.position = m_allCrackArea[0].transform.position + new Vector3(0,0, -10);

        // Temporary activation for dbug purposes
        m_allCrackArea[0].ActivateCrackInArea();
    }

    private void FixedUpdate()
    {
        if (CrackCompleted)
        {
            m_currentCrackAreaIndex++;
            if (m_currentCrackAreaIndex < m_allCrackArea.Count)
            {
                Camera.main.transform.position = m_allCrackArea[m_currentCrackAreaIndex].transform.position + new Vector3(0, 0, -10);
                m_allCrackArea[m_currentCrackAreaIndex].ActivateCrackInArea();
            }
            else
                Debug.Log("Completed");
            CrackCompleted = false;
        }
    }
}
