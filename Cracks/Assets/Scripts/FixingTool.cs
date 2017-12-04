using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixingTool : MonoBehaviour {

    public bool Usable { get; set; }

    private Transform       m_transform;
    private BoxCollider2D   m_currentCollider;
    private ToolType        m_currentTool;
    private Animator        m_toolAnimator;
    private List<GameObject> m_collidingGameObjects = new List<GameObject>();
    private GameObject      m_currentAddedItem; // This is to keep reference of composite colliders and line renderers
    private bool            m_addingItem;       // of some of the items the tools could add

    // Created prefabs from tools
    [SerializeField]
    private GameObject m_caulkingObject;
    [SerializeField]
    private GameObject m_plasterObject;
    [SerializeField]
    private GameObject m_duckTape;

    public delegate void OnToolChange(ToolType type);
    public static OnToolChange toolChanged;
    public static void AddOnToolChangeFunction(OnToolChange function)       { toolChanged += function; }
    public static void RemoveOnToolChangeFunction(OnToolChange function)    { toolChanged -= function; }

    private void Start()
    {
        m_transform = transform;
        m_toolAnimator = GetComponentInChildren<Animator>();
        GetComponentInChildren<AnimationEventHelper>().AddOnAnimationEvent(UseTool);
        toolChanged(ToolType.HOTGLUE);
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
        if (Usable)
        {
            m_transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10);

            switch (m_currentTool)
            {
                case ToolType.HAMMER:
                    if (Input.GetMouseButtonDown(0))
                    {
                        m_toolAnimator.SetBool("InUse", true);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        m_toolAnimator.SetBool("InUse", false);
                    }
                    break;
                case ToolType.HOTGLUE:
                case ToolType.PLASTER:
                case ToolType.DUCKTAPE:
                    if (Input.GetMouseButton(0))
                    {
                        UseTool();
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        m_currentAddedItem.GetComponent<CompositeColliderSpawner>().StopSpawningAndCompose();
                        m_addingItem = false;
                        m_currentAddedItem = null;
                    }

                    break;
            }
        }
        else
        {
            // This is just to be sure we set the animation off on the last
            // clicked type cracks before desactivating the tool
            m_toolAnimator.SetBool("InUse", false);
            m_addingItem = false;
            m_currentAddedItem = null;
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
                        if(go.GetComponent<Crack>().m_active)
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
            case ToolType.HOTGLUE:
            case ToolType.PLASTER:
            case ToolType.DUCKTAPE:
                // This is the only difference between the tools are the instantiated objects
                // Gameplay wise they do the same thing
                if (m_addingItem)
                {
                    m_currentAddedItem.transform.position = m_transform.position;
                }
                else
                {
                    if(m_currentTool == ToolType.HOTGLUE)
                        m_currentAddedItem = Instantiate(m_caulkingObject, m_transform.position, Quaternion.identity);
                    else if (m_currentTool == ToolType.PLASTER)
                        m_currentAddedItem = Instantiate(m_plasterObject, m_transform.position, Quaternion.identity);
                    else if (m_currentTool == ToolType.DUCKTAPE)
                        m_currentAddedItem = Instantiate(m_duckTape, m_transform.position, Quaternion.identity);

                    m_addingItem = true;
                }

                // Just check if I need to add it every frame
                // Not great but faster for me
                if (m_collidingGameObjects.Count > 0)
                {
                    foreach (GameObject go in m_collidingGameObjects)
                    {
                        if (go.GetComponent<Crack>().m_active)
                        {
                            go.GetComponent<CoveredCrack>().AddCoverCollider(m_currentAddedItem.GetComponent<Collider2D>());
                        }
                    }
                }
                break;
        }
    }
}
