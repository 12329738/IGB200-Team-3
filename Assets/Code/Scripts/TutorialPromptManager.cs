using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TutorialPromptManager : MonoBehaviour
{
    public static TutorialPromptManager Instance
    {
        get;
        private set;
    }


    [System.Serializable]
    public class TutorialStep
    {
        public string stepName;

        [TextArea(3, 7)]
        public string message;

        public TutorialEvent completionEvent;

        public TutorialCondition condition =
            TutorialCondition.Always;

        public TutorialPromptLocation location;

        [Tooltip(
            "Optional. Flips only the speech bubble graphic, " +
            "not the text."
        )]
        public bool flipBubble;
    }


    // =========================================================
    // UI
    // =========================================================

    [Header("Prompt UI")]

    [SerializeField]
    private CanvasGroup promptCanvasGroup;

    [SerializeField]
    private TMP_Text promptText;

    [SerializeField]
    private RectTransform promptRect;

    [Tooltip(
        "Optional. Assign only the speech-bubble graphic. " +
        "Leave empty if flipping is not needed."
    )]
    [SerializeField]
    private RectTransform bubbleVisual;


    // =========================================================
    // PROMPT LOCATIONS
    // =========================================================

    [Header("Prompt Locations")]

    [SerializeField]
    private RectTransform centerAnchor;

    [SerializeField]
    private RectTransform materialsAnchor;

    [SerializeField]
    private RectTransform finalFormsAnchor;

    [SerializeField]
    private RectTransform historyAnchor;

    [SerializeField]
    private RectTransform objectAnchor;

    [SerializeField]
    private RectTransform choicesAnchor;

    [SerializeField]
    private RectTransform galleryAnchor;

    [SerializeField]
    private RectTransform wasteAnchor;


    // =========================================================
    // PROMPTS
    // =========================================================

    [Header("Main Tutorial")]

    [SerializeField]
    private List<TutorialStep> mainSteps =
        new();


    [Header("Waste Side Tutorial")]

    [SerializeField]
    private TutorialStep wasteCreatedPrompt;

    [SerializeField]
    private TutorialStep wasteRecycledPrompt;


    // =========================================================
    // ANIMATION
    // =========================================================

    [Header("Animation")]

    [SerializeField]
    [Min(0f)]
    private float fadeDuration = 0.25f;


    // =========================================================
    // STATE
    // =========================================================

    private int currentMainStep = -1;

    private bool promptsAllowed;

    private bool tutorialStartedThisSession = false;

    private bool wasteTutorialActive;

    private bool wasteTutorialCompleted;

    private bool wasteWaitingForClick;

    private bool? choiceHadRecycling = null;

    private bool hiddenForMenu = false;

    private Zone firstMergeZone;

    private Coroutine transitionRoutine;

    private Vector3 bubbleOriginalScale;

    private Dictionary<Zone, int> mergeCounts =
        new Dictionary<Zone, int>();


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        DontDestroyOnLoad(gameObject);


        if (bubbleVisual != null)
        {
            bubbleOriginalScale =
                bubbleVisual.localScale;
        }


        SceneManager.sceneLoaded +=
            OnSceneLoaded;


        promptsAllowed =
            SceneManager.GetActiveScene().name ==
            "Game";


        HidePromptImmediate();
    }


    private void Start()
    {
        // Fallback in case the object begins life directly inside the Game scene.
        if (promptsAllowed &&
            currentMainStep < 0)
        {
            BeginTutorial();
        }
    }


    private void OnDestroy()
    {
        if (Instance != this)
            return;


        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }


    // =========================================================
    // PUBLIC EVENT API
    // =========================================================

    public static void ReportEvent(
        TutorialEvent tutorialEvent,
        Zone sourceZone = null)
    {
        if (Instance == null)
            return;

        Instance.HandleEvent(
            tutorialEvent,
            sourceZone
        );
    }


    public void OnPromptClicked()
    {
        ReportEvent(
            TutorialEvent.PromptClicked
        );
    }


    // =========================================================
    // EVENT HANDLING
    // =========================================================

    private void HandleEvent(
        TutorialEvent tutorialEvent,
        Zone sourceZone = null)
    {
        if (!promptsAllowed)
            return;


        // ---------------------------------------------
        // WASTE SIDE TUTORIAL
        // ---------------------------------------------

        if (tutorialEvent ==
                TutorialEvent.WasteCreated &&
            !wasteTutorialCompleted &&
            !wasteTutorialActive)
        {
            StartWasteTutorial();
            return;
        }


        if (wasteTutorialActive)
        {
            if (!wasteWaitingForClick &&
                tutorialEvent ==
                    TutorialEvent.WasteRecycled)
            {
                wasteWaitingForClick = true;

                ShowStep(
                    wasteRecycledPrompt
                );

                return;
            }


            if (wasteWaitingForClick &&
                tutorialEvent ==
                    TutorialEvent.PromptClicked)
            {
                wasteTutorialCompleted = true;
                wasteTutorialActive = false;
                wasteWaitingForClick = false;

                ShowCurrentMainStep();

                return;
            }


            return;
        }


        // ---------------------------------------------
        // SUCCESSFUL MERGES
        // ---------------------------------------------

        if (tutorialEvent ==
            TutorialEvent.SuccessfulMerge)
        {
            if (sourceZone == null)
                return;

            if (!mergeCounts.ContainsKey(sourceZone))
            {
                mergeCounts[sourceZone] = 0;
            }

            mergeCounts[sourceZone]++;

            int mergeCount =
                mergeCounts[sourceZone];

            TutorialStep currentStep =
                GetCurrentMainStep();

            if (currentStep == null ||
                currentStep.completionEvent !=
                    TutorialEvent.SuccessfulMerge)
            {
                return;
            }

            if (currentMainStep == 4)
            {
                if (mergeCount >= 1)
                {
                    AdvanceMainStep();
                }

                return;
            }

            if (currentMainStep == 5)
            {
                if (mergeCount >= 2)
                {
                    AdvanceMainStep();
                }

                return;
            }

            return;
        }


        // ---------------------------------------------
        // FINAL CHOICE RECYCLING SNAPSHOT
        // ---------------------------------------------

        if (tutorialEvent ==
            TutorialEvent.ActionClicked)
        {
            choiceHadRecycling =
                GameManager.instance != null &&
                GameManager.instance.currentRecycledWaste > 0;
        }


        // ---------------------------------------------
        // MAIN TUTORIAL
        // ---------------------------------------------

        TutorialStep step =
            GetCurrentMainStep();


        if (step == null)
            return;


        if (step.completionEvent !=
            tutorialEvent)
        {
            return;
        }


        bool finishedChoiceBranch =
            tutorialEvent ==
                TutorialEvent.OptionChosen &&
            (step.condition ==
                TutorialCondition.HasRecycling ||
            step.condition ==
                TutorialCondition.NoRecycling);


        AdvanceMainStep();


        if (finishedChoiceBranch)
        {
            choiceHadRecycling = null;
        }
    }


    // =========================================================
    // MAIN TUTORIAL
    // =========================================================

    private void BeginTutorial()
    {
        if (tutorialStartedThisSession)
        {
            HidePromptImmediate();
            return;
        }

        tutorialStartedThisSession = true;

        currentMainStep = -1;

        AdvanceMainStep();
    }


    private void AdvanceMainStep()
    {
        currentMainStep++;


        while (currentMainStep <
               mainSteps.Count)
        {
            TutorialStep step =
                mainSteps[
                    currentMainStep
                ];


            if (ConditionPasses(
                step.condition))
            {
                ShowStep(step);
                return;
            }


            currentMainStep++;
        }


        HidePrompt();
    }


    private TutorialStep
        GetCurrentMainStep()
    {
        if (currentMainStep < 0 ||
            currentMainStep >=
                mainSteps.Count)
        {
            return null;
        }


        return mainSteps[
            currentMainStep
        ];
    }


    private void ShowCurrentMainStep()
    {
        if (hiddenForMenu)
        {
            HidePromptImmediate();
            return;
        }
        
        TutorialStep step =
            GetCurrentMainStep();


        if (step != null)
        {
            ShowStep(step);
        }
        else
        {
            HidePrompt();
        }
    }


    // =========================================================
    // CONDITIONS
    // =========================================================

    private bool ConditionPasses(
        TutorialCondition condition)
    {
        switch (condition)
        {
            case TutorialCondition.Always:
                return true;

            case TutorialCondition.HasRecycling:
                if (choiceHadRecycling.HasValue)
                    return choiceHadRecycling.Value;

                return GameManager.instance != null &&
                    GameManager.instance.currentRecycledWaste > 0;

            case TutorialCondition.NoRecycling:
                if (choiceHadRecycling.HasValue)
                    return !choiceHadRecycling.Value;

                return GameManager.instance == null ||
                    GameManager.instance.currentRecycledWaste <= 0;
        }

        return true;
    }


    // =========================================================
    // WASTE SIDE TUTORIAL
    // =========================================================

    private void StartWasteTutorial()
    {
        if (wasteCreatedPrompt == null ||
            string.IsNullOrWhiteSpace(
                wasteCreatedPrompt.message))
        {
            return;
        }


        wasteTutorialActive = true;

        wasteWaitingForClick = false;


        ShowStep(
            wasteCreatedPrompt
        );
    }


    // =========================================================
    // DISPLAY
    // =========================================================

    private void ShowStep(
        TutorialStep step)
    {
        if (step == null)
            return;


        if (transitionRoutine != null)
        {
            StopCoroutine(
                transitionRoutine
            );
        }


        transitionRoutine =
            StartCoroutine(
                TransitionToStep(step)
            );
    }


    private IEnumerator TransitionToStep(
        TutorialStep step)
    {
        if (promptCanvasGroup != null &&
            promptCanvasGroup.alpha > 0f)
        {
            yield return FadeCanvasGroup(
                promptCanvasGroup.alpha,
                0f
            );
        }


        ApplyStep(step);


        yield return FadeCanvasGroup(
            0f,
            1f
        );


        transitionRoutine = null;
    }


    private void ApplyStep(
        TutorialStep step)
    {
        if (promptText != null)
        {
            promptText.text =
                step.message;
        }


        RectTransform anchor =
            GetAnchor(
                step.location
            );


        if (anchor != null &&
            promptRect != null)
        {
            promptRect.position =
                anchor.position;
        }


        ApplyBubbleFlip(
            step.flipBubble
        );


        bool clickable =
            step.completionEvent ==
            TutorialEvent.PromptClicked;


        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.interactable =
                clickable;

            promptCanvasGroup.blocksRaycasts =
                clickable;
        }
    }


    private RectTransform GetAnchor(
        TutorialPromptLocation location)
    {
        switch (location)
        {
            case TutorialPromptLocation.Materials:
                return materialsAnchor;

            case TutorialPromptLocation.FinalForms:
                return finalFormsAnchor;

            case TutorialPromptLocation.History:
                return historyAnchor;

            case TutorialPromptLocation.Object:
                return objectAnchor;

            case TutorialPromptLocation.Choices:
                return choicesAnchor;

            case TutorialPromptLocation.Gallery:
                return galleryAnchor;

            case TutorialPromptLocation.Waste:
                return wasteAnchor;

            default:
                return centerAnchor;
        }
    }


    private void ApplyBubbleFlip(
        bool flipped)
    {
        if (bubbleVisual == null)
            return;


        Vector3 scale =
            bubbleOriginalScale;


        scale.x =
            Mathf.Abs(scale.x) *
            (flipped ? -1f : 1f);


        bubbleVisual.localScale =
            scale;
    }


    // =========================================================
    // HIDING / FADING
    // =========================================================

    private void HidePrompt()
    {
        if (transitionRoutine != null)
        {
            StopCoroutine(
                transitionRoutine
            );
        }


        transitionRoutine =
            StartCoroutine(
                HidePromptRoutine()
            );
    }


    private IEnumerator
        HidePromptRoutine()
    {
        if (promptCanvasGroup != null)
        {
            yield return FadeCanvasGroup(
                promptCanvasGroup.alpha,
                0f
            );
        }


        transitionRoutine = null;
    }


    private void HidePromptImmediate()
    {
        if (promptCanvasGroup == null)
            return;


        promptCanvasGroup.alpha = 0f;

        promptCanvasGroup.interactable =
            false;

        promptCanvasGroup.blocksRaycasts =
            false;
    }


    private IEnumerator FadeCanvasGroup(
        float startAlpha,
        float endAlpha)
    {
        if (promptCanvasGroup == null)
        {
            yield break;
        }


        if (fadeDuration <= 0f)
        {
            promptCanvasGroup.alpha =
                endAlpha;

            yield break;
        }


        float elapsed = 0f;

        promptCanvasGroup.alpha =
            startAlpha;


        while (elapsed <
               fadeDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    fadeDuration
                );


            promptCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    t
                );


            yield return null;
        }


        promptCanvasGroup.alpha =
            endAlpha;
    }


    // =========================================================
    // SCENES / RESET
    // =========================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        promptsAllowed =
            scene.name == "Game";

        if (!promptsAllowed)
        {
            HidePromptImmediate();
            return;
        }

        if (!tutorialStartedThisSession)
        {
            ResetTutorialSession();
            BeginTutorial();
        }
        else
        {
            HidePromptImmediate();
        }
    }


    [ContextMenu(
        "Reset Tutorial Session"
    )]
    public void ResetTutorialSession()
    {
        currentMainStep = -1;

        mergeCounts.Clear();

        choiceHadRecycling = null;

        wasteTutorialActive = false;
        wasteTutorialCompleted = false;
        wasteWaitingForClick = false;


        if (transitionRoutine != null)
        {
            StopCoroutine(
                transitionRoutine
            );

            transitionRoutine = null;
        }


        HidePromptImmediate();


        Debug.Log(
            "[TUTORIAL] Tutorial session reset."
        );
    }

    public void HideForMenu()
    {
        hiddenForMenu = true;
        HidePromptImmediate();
    }

    public void RestoreAfterMenu()
    {
        hiddenForMenu = false;

        if (!promptsAllowed)
            return;

        ShowCurrentMainStep();
    }
}