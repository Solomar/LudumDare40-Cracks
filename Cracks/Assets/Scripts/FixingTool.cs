using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixingTool : MonoBehaviour {

    private Transform       m_transform;
    private BoxCollider2D   m_currentCollider;
    private ToolType        m_currentTool;
    private Animator        m_toolAnimator;
    private List<GameObject> m_collidingGameObjects = new List<GameObject>();

    public delegate void OnToolChange(ToolType type);
    public static OnToolChange toolChanged;
    public static void AddOnToolChangeFunction(OnToolChange function)       { toolChanged += function; }
    public static void RemoveOnToolChangeFunction(OnToolChange function)    { toolChanged -= function; }

    private void Start()
    {
        m_transform = transform;
        m_toolAnimator = GetComponentInChildren<Animator>();
        GetComponentInChildren<AnimationEventHelper>().AddOnAnimationEvent(UseTool);
        toolChanged(ToolType.HAMMER);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crack"))
            m_collidingGameObjects.Add(collision.gameObject);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Crack"))
            m_collidingGameObjects.Remove(collision.gameObject);
    }

    private void Update()
    {
        m_transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10);

        if(Input.GetMouseButtonDown(0))
        {
            m_toolAnimator.SetBool("InUse", true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_toolAnimator.SetBool("InUse", false);
        }
    }

    public void SwitchTool(ToolType tool)
    {
        m_currentTool = tool;
        toolChanged(tool);
    }

    public void SetCollider(BoxCollider2D collider)
    {
        m_currentCollider = collider;
    }

    public void UseTool()
    {
        switch (m_currentTool)
        {
            case ToolType.HAMMER:
                if(m_collidingGameObjects.Count > 0)
                {
                    bool objectFound = false;
                    float closestDistance = float.MaxValue;
                    GameObject closestObject = m_collidingGameObjects[0];

                    foreach(GameObject go in m_collidingGameObjects)
                    {
                        if(go.GetComponent<Crack>().Active)
                        { 
                            float distance = Vector2.Distance(go.transform.position, m_currentCollider.bounds.center);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestObject = go;
                                objectFound = true;
                            }
                        }
                    }

                    if (objectFound)
                        closestObject.GetComponent<Crack>().IncrementProgression();
                }
                break;
        }
    }
}
