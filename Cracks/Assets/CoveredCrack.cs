﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredCrack : MonoBehaviour {

    public struct BoundedCrackSegment
    {
        public bool m_covered;
        public Collider2D m_segmentCollider;
    }

    public List<BoundedCrackSegment> m_boundedCracks = new List<BoundedCrackSegment>();
    public List<Collider2D>     m_coverColliders = new List<Collider2D>();
    public float                m_coverPercentage;

    private void Start()
    {
        foreach (Collider2D crackCollider in GetComponents<Collider2D>())
        {
            var newSegment = new BoundedCrackSegment();
            newSegment.m_covered = false;
            newSegment.m_segmentCollider = crackCollider;
            m_boundedCracks.Add(newSegment);
        }
    }

    void Update()
    {
        bool newSegmentCovered = false;
        foreach (BoundedCrackSegment segment in m_boundedCracks)
        {
            if (!segment.m_covered)
            {
                foreach (Collider2D coverCollider in m_coverColliders)
                {
                    if (BoundsIsEncapsulated(coverCollider.bounds, segment.m_segmentCollider.bounds))
                    {
                        var coveredSegment = new BoundedCrackSegment();
                        coveredSegment.m_covered = true;
                        coveredSegment.m_segmentCollider = segment.m_segmentCollider;
                        m_boundedCracks[m_boundedCracks.IndexOf(segment)] = coveredSegment;
                        newSegmentCovered = true;
                    }
                }
            }
        }

        // Update the coverage percentage according to the segments
        if (newSegmentCovered)
        {
            int coveredCrackCount = 0;
            foreach (BoundedCrackSegment segment in m_boundedCracks)
            {
                if (segment.m_covered)
                    coveredCrackCount++;
            }
            m_coverPercentage = (float)m_boundedCracks.Count / (float)coveredCrackCount;
            Debug.Log(m_coverPercentage);
        }
    }

    static bool BoundsIsEncapsulated(Bounds Encapsulator, Bounds Encapsulating)
    {
        return Encapsulator.Contains(Encapsulating.min) && Encapsulator.Contains(Encapsulating.max);
    }
}
