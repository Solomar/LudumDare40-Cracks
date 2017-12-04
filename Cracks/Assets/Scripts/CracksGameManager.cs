using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum ToolType { HAMMER, HAND, HOTGLUE, PLASTER, DUCKTAPE, MAGICWAND }
public enum GameState { START, MIDDLE, END }

public class CracksGameManager : MonoBehaviour
{
    public static bool CrackCompleted { get; set; }

    private GameState       m_currentState;
    private FixingTool      m_fixingTool;
    private Transform       m_mainCameraTransform;
    private int             m_currentCrackAreaIndex;
    [SerializeField]
    private List<CrackArea> m_allCrackArea = new List<CrackArea>();

    private UnityEngine.PostProcessing.PostProcessingProfile m_postProfile;
    private DialogueWriter m_dialogueWriter;
    private bool m_continue;

    private void Start()
    {
        // Starting the game from the title/start menu
        m_currentState = GameState.START;
        m_dialogueWriter = GetComponent<DialogueWriter>();
        m_mainCameraTransform = Camera.main.transform;

        // Initializing everything needed
        // GetComponentsInChildren<CrackArea>(m_allCrackArea);
        m_fixingTool = FindObjectOfType<FixingTool>();

        // Instantiating post process profile to modify it at runtime
        // in case it's not yet fully optimal like described here
        // https://github.com/Unity-Technologies/PostProcessing/wiki/(v1)-Runtime-post-processing-modification
        var postBehavior = FindObjectOfType<UnityEngine.PostProcessing.PostProcessingBehaviour>();
        m_postProfile = Instantiate(postBehavior.profile);
        postBehavior.profile = m_postProfile;
        //m_postProfile.motionBlur.enabled = false;

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            switch (m_currentState)
            {
                case GameState.START:
                    StartCoroutine(Beginning());
                    break;
                case GameState.MIDDLE:
                    if(!m_continue)
                    {
                        if (m_dialogueWriter.DoneWriting)
                            m_continue = true;
                        else
                            m_dialogueWriter.FinishWritingEarly();
                    }
                    break;
                case GameState.END:
                    break;
            }
        }
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

    private IEnumerator Beginning()
    {

        m_currentState = GameState.MIDDLE;
        m_dialogueWriter.SetNewText("Oh good! I needed a helping hand.", 0);

        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        m_dialogueWriter.SetNewText("This complex infrastructure needs some maintenance", 0);

        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        m_dialogueWriter.SetNewText("", 0);

        StartCoroutine(GoToNextArea());
    }

    private IEnumerator End()
    {
        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;
    }

    private IEnumerator GoToNextArea()
    {
        m_fixingTool.Usable = false;
        m_fixingTool.transform.DOMove(m_allCrackArea[m_currentCrackAreaIndex].transform.position, 0.3f).SetEase(Ease.InExpo);

        // Next Message For Completion Here
        yield return new WaitForSeconds(0.25f);

        // Change tool here
        m_fixingTool.SwitchTool(m_allCrackArea[m_currentCrackAreaIndex].m_areaTool);
        // Reactivate blur for camera movement
        m_postProfile.motionBlur.enabled = true;
        m_mainCameraTransform.transform.DOMove(m_allCrackArea[m_currentCrackAreaIndex].transform.position + new Vector3(0, 0, -10), 0.4f).SetEase(Ease.OutSine).OnComplete(MovementToNextAreaComplete);
    }

    private void MovementToNextAreaComplete()
    {
        StartCoroutine(StartArea());
        m_fixingTool.Usable = true;
        m_fixingTool.SwitchTool(m_allCrackArea[m_currentCrackAreaIndex].m_areaTool);
        m_allCrackArea[m_currentCrackAreaIndex].ActivateCrackInArea();

        // Next Message For Start Here
    }

    private IEnumerator StartArea()
    {

        foreach (string line in m_allCrackArea[m_currentCrackAreaIndex].m_areaStartTexts)
        {
            do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;
        }
    }

    private IEnumerator DialogueWait()
    {
        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;
    }
}
