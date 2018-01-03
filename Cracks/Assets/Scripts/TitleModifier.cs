using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class TitleModifier : MonoBehaviour
{
    private TextMeshPro m_textmeshPro;
    public float m_min;
    public float m_max;
    public bool  m_increment;

    private float m_current;

    void Start ()
    {
        m_textmeshPro = GetComponent<TextMeshPro>();
        m_current = m_textmeshPro.fontSharedMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
        m_increment = true;
    }
	

	void Update ()
    {
		if(m_increment)
        {
            m_current += 0.00075f;
            if (m_current > m_max)
            {
                m_current = m_max;
                m_increment = false;
            }
        }
        else
        {
            m_current -= 0.00075f;
            if (m_current < m_min)
            {
                m_current = m_min;
                m_increment = true;
            }
        }
        m_textmeshPro.fontSharedMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, m_current);
    }
}
