﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackArea : MonoBehaviour
{
    public ToolType m_areaTool;
    public List<Crack> m_areaCracks = new List<Crack>();
    public string[] m_areaStartTexts;
    public string[] m_areaEndTexts;

    private void Awake()
    {
        GetComponentsInChildren<Crack>(m_areaCracks);
    }

    public void ActivateCrackInArea()
    {
        foreach (Crack crack in m_areaCracks)
            crack.m_active = true;
    }

    public void FixedCrack()
    {
        foreach(Crack crack in m_areaCracks)
        {
            if (crack.m_active)
                return;
        }

        CracksGameManager.CrackCompleted = true;
    }
}
