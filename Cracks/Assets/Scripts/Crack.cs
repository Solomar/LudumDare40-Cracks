using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crack : MonoBehaviour
{
    public enum CrackType { CLICKED, COVERED, PARTIALLY_COVERED }

    public bool     Active { get; set; } // Is in the active crack area
    public bool     m_spriteChange;
    public float    m_progression;

    [SerializeField]
    private Sprite m_baseSprite;
    [SerializeField]
    private Sprite[] m_progressionSprites;
    [SerializeField]
    private CrackType m_crackType;

    private SpriteRenderer  m_spriteRenderer;
    private BoxCollider2D   m_collider;
    private CrackArea       m_belongingArea;

    private void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        m_belongingArea = GetComponentInParent<CrackArea>();
        m_spriteRenderer.sprite = m_baseSprite;
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

    private void ChangeProgressionSprite()
    {
        m_spriteRenderer.sprite = m_progressionSprites[(int)(m_progression / (1.0f / (float)m_progressionSprites.Length))];
        m_collider.size = m_spriteRenderer.sprite.bounds.size;

        if ((int)(m_progression / (1.0f / (float)m_progressionSprites.Length)) == (m_progressionSprites.Length - 1))
        {
            Active = false;
            Destroy(m_collider);
            m_belongingArea.FixedCrack();
        }
        
    } 
}
