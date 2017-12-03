using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Delegates the activation of the proper collider
// when changing tools
public class ToolCollider : MonoBehaviour {

    [SerializeField]
    private ToolType m_toolType;
    private FixingTool m_fixingToolRef;

    private void Awake()
    {
        m_fixingToolRef = GetComponentInParent<FixingTool>();
        FixingTool.AddOnToolChangeFunction(ToggleTool);
    }

    private void ToggleTool(ToolType tool)
    {
        if (tool == m_toolType)
        {
            gameObject.SetActive(true);
            m_fixingToolRef.SetCollider(GetComponent<BoxCollider2D>());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
