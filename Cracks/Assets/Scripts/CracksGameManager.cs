using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ToolType { HAMMER, HAND, HOTGLUE, PLASTER, DUCKTAPE, MAGICWAND }
public enum GameState { START, MIDDLE, END, ENDEND }

public class CracksGameManager : MonoBehaviour
{
    public static bool CrackCompleted { get; set; }

    private GameState       m_currentState;
    private FixingTool      m_fixingTool;
    private Transform       m_mainCameraTransform;
    private int             m_currentCrackAreaIndex;
    private AudioSource     m_audioSource;
    [SerializeField]
    private List<CrackArea> m_allCrackArea = new List<CrackArea>();
    [SerializeField]
    private RectTransform   m_narrationText;
    [SerializeField]
    private GameObject      m_titleText;
    [SerializeField]
    private SpriteRenderer  m_transitionScreen;

    private UnityEngine.PostProcessing.PostProcessingProfile m_postProfile;
    private DialogueWriter m_dialogueWriter;
    private bool m_continue;
    
    private void Start()
    {
        Application.targetFrameRate = 60;

        StartCoroutine(FadeOutTransition());

        // Starting the game from the title/start menu
        m_currentState = GameState.START;
        m_dialogueWriter = GetComponent<DialogueWriter>();
        m_audioSource = GetComponent<AudioSource>();
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
                    StartCoroutine(FadeTitle());
                    StartCoroutine(StartMusic());
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
                case GameState.ENDEND:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
                StartCoroutine(End());
            CrackCompleted = false;
        }
    }

    private IEnumerator StartMusic()
    {
        float volume = 0.0f;
        m_audioSource.Play();
        do
        {
            volume += 0.00025f;
            m_audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }
        while (volume < 0.6f);
    }

    private IEnumerator FadeTitle()
    {
        TMPro.TextMeshPro titleTextMeshPro = m_titleText.GetComponent<TMPro.TextMeshPro>();
        Color color = titleTextMeshPro.faceColor;
        m_audioSource.Play();
        do
        {
            color.a -= 0.005f;
            titleTextMeshPro.faceColor = color;
            yield return new WaitForEndOfFrame();
        }
        while (color.a > 0.0f);
        m_titleText.SetActive(false);
    }

    private IEnumerator Beginning()
    {
        m_currentState = GameState.MIDDLE;
        m_dialogueWriter.SetNewText("Oh good! I needed a helping hand.", 0);

        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        m_dialogueWriter.SetNewText("This complex infrastructure needs some maintenance", 0);

        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        m_dialogueWriter.SetNewText("", 0);

        StartCoroutine(GoToNextArea(true));
    }

    private IEnumerator End()
    {
        foreach (string line in m_allCrackArea[m_currentCrackAreaIndex - 1].m_areaEndTexts)
        {
            m_dialogueWriter.SetNewText(line, 0); m_continue = false;
            do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;
        }
        m_dialogueWriter.SetNewText("", 0);

        // Stop music and zoom out
        float volume = 0.6f;
        float scalingFactor = 1.0f;
        do
        {
            volume -= 0.005f;
            m_audioSource.volume = volume;

            scalingFactor = scalingFactor * 1.01f;
            if (scalingFactor > 16.67f)
                scalingFactor = 16.67f;
            m_narrationText.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
            Camera.main.orthographicSize = 0.6f * scalingFactor;

            yield return new WaitForEndOfFrame();
        }
        while (scalingFactor < 16.67f);
        m_audioSource.Stop();

        // End dialogue
        m_dialogueWriter.SetNewText("I know it isn't that simple. I wish it was.", 0); m_continue = false;
        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        m_dialogueWriter.SetNewText("At least I had your help. Please don't be afraid to ask for help when you need it.", 0); m_continue = false;
        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        m_dialogueWriter.SetNewText("Get better. Be better. You got this far, I'm proud of you.", 0); m_continue = false;
        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        m_dialogueWriter.SetNewText("Best of luck friend. Stay strong and thanks for playing.", 0); m_continue = false;
        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;

        StartCoroutine(FadeInTransition());
    }

    private IEnumerator GoToNextArea(bool beginning = false)
    {
        m_fixingTool.Usable = false;
        m_fixingTool.transform.DOMove(m_allCrackArea[m_currentCrackAreaIndex].transform.position, 0.3f).SetEase(Ease.InExpo);

        if (!beginning)
        {
            foreach (string line in m_allCrackArea[m_currentCrackAreaIndex - 1].m_areaEndTexts)
            {
                m_dialogueWriter.SetNewText(line, 0); m_continue = false;
                do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;
            }
            m_dialogueWriter.SetNewText("", 0);
        }

        // Next Message For Completion Here
        yield return new WaitForSeconds(0.25f);

        // Change tool here
        //m_fixingTool.SwitchTool(m_allCrackArea[m_currentCrackAreaIndex].m_areaTool);
        // Reactivate blur for camera movement
        m_postProfile.motionBlur.enabled = true;
        m_mainCameraTransform.transform.DOMove(m_allCrackArea[m_currentCrackAreaIndex].transform.position + new Vector3(0, 0, -10), 0.4f).SetEase(Ease.OutSine).OnComplete(MovementToNextAreaComplete);
    }

    private void MovementToNextAreaComplete()
    {
        StartCoroutine(StartArea());
        //m_fixingTool.Usable = true;
        //m_fixingTool.SwitchTool(m_allCrackArea[m_currentCrackAreaIndex].m_areaTool);
        //m_allCrackArea[m_currentCrackAreaIndex].ActivateCrackInArea();
    }

    private IEnumerator StartArea()
    {
        foreach (string line in m_allCrackArea[m_currentCrackAreaIndex].m_areaStartTexts)
        {
            m_dialogueWriter.SetNewText(line, 0); m_continue = false;
            do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;
        }
        m_dialogueWriter.SetNewText("", 0);

        // Start area when dialogue is done..
        m_fixingTool.Usable = true;
        m_fixingTool.SwitchTool(m_allCrackArea[m_currentCrackAreaIndex].m_areaTool);
        m_allCrackArea[m_currentCrackAreaIndex].ActivateCrackInArea();
    }

    private IEnumerator DialogueWait()
    {
        do { yield return new WaitForEndOfFrame(); } while (!m_continue); m_continue = false;
    }

    private IEnumerator FadeOutTransition()
    {
        Color color = m_transitionScreen.color;
        do
        {
            color.a -= 0.005f;
            m_transitionScreen.color = color;
            yield return new WaitForEndOfFrame();
        }
        while (color.a > 0.0f);
    }

    private IEnumerator FadeInTransition()
    {
        Color color = m_transitionScreen.color;
        do
        {
            color.a += 0.005f;
            m_transitionScreen.color = color;
            yield return new WaitForEndOfFrame();
        }
        while (color.a < 1.0f);

        m_currentState = GameState.ENDEND;
    }
}
