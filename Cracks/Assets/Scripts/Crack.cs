using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crack : MonoBehaviour
{
    public enum CrackType { CLICKED, COVERED, PARTIALLY_COVERED }

    public bool     m_active; // Is in the active crack area
    public bool     m_spriteChange;
    public float    m_progression;

    [SerializeField]
    private Sprite m_baseSprite;
    [SerializeField]
    private Sprite[] m_progressionSprites;
    [SerializeField]
    private CrackType m_crackType;
    [SerializeField]
    private GameObject m_particleSystemOnProgression;

    // m_collider is used for the simple clicks 
    // and m_crackColliders for the covered cracks
    private SpriteRenderer      m_spriteRenderer;
    private BoxCollider2D       m_collider;
    private CoveredCrack        m_coveredCrack;
    private List<Collider2D>    m_crackColliders;
    private CrackArea           m_belongingArea;

    private void Start()
    {
        m_spriteRenderer    = GetComponent<SpriteRenderer>();
        m_belongingArea     = GetComponentInParent<CrackArea>();
        m_spriteRenderer.sprite = m_baseSprite;

        if (m_crackType == CrackType.CLICKED)
            m_collider = GetComponent<BoxCollider2D>();
        else
            m_coveredCrack = gameObject.AddComponent<CoveredCrack>();
    }

    public void IncrementProgression()
    {
        switch(m_crackType)
        {
            case CrackType.CLICKED:
                m_progression += 1.0f / (float)m_progressionSprites.Length;
                ChangeProgressionSprite();
                break;
        }
    }

    private void Update()
    {
        if(m_active && m_crackType == CrackType.COVERED)
        {
            if (m_coveredCrack.m_coverPercentage >= 1.0f)
            {
                m_active = false;
                m_belongingArea.FixedCrack();
            }
        }
    }

    private void ChangeProgressionSprite()
    {
        m_spriteRenderer.sprite = m_progressionSprites[(int)(m_progression / (1.0f / (float)m_progressionSprites.Length))];
        m_collider.size = m_spriteRenderer.sprite.bounds.size;

        if (m_particleSystemOnProgression != null)
            Instantiate(m_particleSystemOnProgression, transform.position, Quaternion.identity);

        if ((int)(m_progression / (1.0f / (float)m_progressionSprites.Length)) == (m_progressionSprites.Length - 1))
        {
            m_active = false;
            Destroy(m_collider);
            m_belongingArea.FixedCrack();
        }
    } 
}
