﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum ToolType { HAMMER, HAND, HOTGLUE, PLASTER, DUCKTAPE, MAGICWAND }

public class CracksGameManager : MonoBehaviour
{
    public static bool CrackCompleted { get; set; }

    private FixingTool      m_fixingTool;
    private Transform       m_mainCameraTransform;
    private int             m_currentCrackAreaIndex;
    [SerializeField]
    private List<CrackArea> m_allCrackArea = new List<CrackArea>();

    private UnityEngine.PostProcessing.PostProcessingProfile m_postProfile;

    private void Start()
    {
        // Initializing everything needed
        // GetComponentsInChildren<CrackArea>(m_allCrackArea);
        m_fixingTool = FindObjectOfType<FixingTool>();
        m_fixingTool.Usable = true;

        // Instantiating post process profile to modify it at runtime
        // in case it's not yet fully optimal like described here
        // https://github.com/Unity-Technologies/PostProcessing/wiki/(v1)-Runtime-post-processing-modification
        var postBehavior = FindObjectOfType<UnityEngine.PostProcessing.PostProcessingBehaviour>();
        m_postProfile = Instantiate(postBehavior.profile);
        postBehavior.profile = m_postProfile;
        m_postProfile.motionBlur.enabled = false;

        m_mainCameraTransform = Camera.main.transform;
        m_mainCameraTransform.position = m_allCrackArea[0].transform.position + new Vector3(0,0, -10);
        m_fixingTool.SwitchTool(m_allCrackArea[m_currentCrackAreaIndex].m_areaTool);

        // Temporary activation for dbug purposes
        m_allCrackArea[0].ActivateCrackInArea();

    }

    private void FixedUpdate()
    {
        if (CrackCompleted)
        {
            m_currentCrackAreaIndex++;
            if (m_currentCrackAreaIndex < m_allCrackArea.Count)
            {
                StartCoroutine(GoToNextArea());
            }
            else
                Debug.Log("Completed");
            CrackCompleted = false;
        }
    }

    private IEnumerator GoToNextArea()
    {
        m_fixingTool.Usable = false;
        m_fixingTool.transform.DOMove(m_allCrackArea[m_currentCrackAreaIndex].transform.position, 0.3f).SetEase(Ease.InExpo);

        // Next Message For Completion Here
        yield return new WaitForSeconds(0.25f);

        // Change tool heere

        // Reactivate blur for camera movement
        m_postProfile.motionBlur.enabled = true;
        m_mainCameraTransform.transform.DOMove(m_allCrackArea[m_currentCrackAreaIndex].transform.position + new Vector3(0, 0, -10), 0.4f).SetEase(Ease.OutSine).OnComplete(MovemenToNextAreaComplete);
    }

    private void MovemenToNextAreaComplete()
    {
        m_postProfile.motionBlur.enabled = false;
        m_fixingTool.Usable = true;
        m_fixingTool.SwitchTool(m_allCrackArea[m_currentCrackAreaIndex].m_areaTool);
        m_allCrackArea[m_currentCrackAreaIndex].ActivateCrackInArea();

        // Next Message For Start Here
    }
}
