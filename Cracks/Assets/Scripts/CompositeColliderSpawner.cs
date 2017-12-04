using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeColliderSpawner : MonoBehaviour
{
    public bool             m_creating;
    public float            m_minimalDistanceToSpawn;
    public float            m_maxSize;
    public GameObject       m_compositeColliderSegmentPrefab;

    private Transform           m_parentTransform;
    private List<GameObject>    m_spawnedSegments = new List<GameObject>();

    void Start()
    {
        m_creating = true;
        m_parentTransform = transform;
        m_spawnedSegments.Add(Instantiate(m_compositeColliderSegmentPrefab, m_parentTransform.position, Quaternion.identity));
    }

    private void Update()
    {
        if (m_creating)
        { 
            if (Vector3.Distance(m_parentTransform.position, m_spawnedSegments[m_spawnedSegments.Count-1].transform.position) > m_minimalDistanceToSpawn)
            {
                m_spawnedSegments.Add(Instantiate(m_compositeColliderSegmentPrefab, transform.position, Quaternion.identity));
            }
        }
    }

    public void StopSpawningAndCompose()
    {
        m_creating = false;
        foreach (GameObject go in m_spawnedSegments)
            go.transform.parent = m_parentTransform;
        m_spawnedSegments.Clear();
    }
}
