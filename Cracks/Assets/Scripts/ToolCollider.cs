using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCollider : MonoBehaviour {

    [SerializeField]
    private ToolType m_toolType;
    
    private void Awake()
    {
        FixingTool.AddOnToolChangeFunction(ToggleTool);
    }

    void ToggleTool(ToolType tool)
    {
        if (tool == m_toolType)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
