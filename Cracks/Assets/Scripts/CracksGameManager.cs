using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType { HAMMER, HAND, HOTGLUE }

public class CracksGameManager : MonoBehaviour
{
    private List<CrackArea> m_allCrackArea = new List<CrackArea>();

    private void Start()
    {
        GetComponentsInChildren<CrackArea>(m_allCrackArea);
        Camera.main.transform.position = m_allCrackArea[0].transform.position + new Vector3(0,0, -10);
    }

}
