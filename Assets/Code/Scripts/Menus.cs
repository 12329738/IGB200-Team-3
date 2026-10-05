using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using LitMotion.Animation;

public class Menus : MonoBehaviour
{
    public static Menus instance;

    [Header("MAIN MENUS")]
    public GameObject mainMenu;
    public GameObject helpMenu;
    public GameObject optionsMenu;
    public GameObject gameMenu;
    public GameObject restartMenu;
    public GameObject creditsMenu;
    public LitMotionAnimation creditsAnimation;

    [Header("SCENES")]
    public string gameScene = "Game";
    public string mainMenuScene = "MainMenu";
    public string galleryScene = "Gallery";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
    
        HideAllMenus();

        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    void Update()
    {
        HandleEscapeKey();
    }

    // =========================================================
    // ESCAPE KEY
    // =========================================================

    void HandleEscapeKey()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // Help closes first
        if (helpMenu.activeSelf)
        {
            CloseHelpMenu();
            return;
        }

        // Options closes second
        if (optionsMenu.activeSelf)
        {
            CloseOptionsMenu();
            return;
        }
    }

    // =========================================================
    // MAIN MENU
    // =========================================================

    public void StartGame()
    {
        Time.timeScale = 1f;

        StopLitMotionAnimations();

        SceneManager.LoadScene(gameScene);
    }

    public void StartGallery()
    {
        Time.timeScale = 1f;

        StopLitMotionAnimations();

        SceneManager.LoadScene(galleryScene);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    // =========================================================
    // Game MENU
    // =========================================================

    public void ToggleGameMenu()
    {
        if (gameMenu.activeSelf)
            gameMenu.SetActive(false);
        else
            gameMenu.SetActive(true);
    }

    // =========================================================
    // HELP MENU
    // =========================================================

    public void OpenHelpMenu()
    {
        helpMenu.SetActive(true);
        GameManager.instance.menuOpen = true;
    }

    public void CloseHelpMenu()
    {
        helpMenu.SetActive(false);
        GameManager.instance.menuOpen = false;
    }

    // =========================================================
    // RESTART MENU
    // =========================================================

    public void OpenRestartMenu()
    {
        restartMenu.SetActive(true);
        GameManager.instance.menuOpen = true;
    }

    public void CloseRestartMenu()
    {
        restartMenu.SetActive(false);
        GameManager.instance.menuOpen = false;
    }

    // =========================================================
    // OPTIONS
    // =========================================================

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }

    // =========================================================
    // CREDITS MENU
    // =========================================================

    public void OpenCreditsMenu()
    {
        creditsMenu.SetActive(true);
        creditsAnimation.Restart();
    }

    public void CloseCreditsMenu()
    {
        creditsMenu.SetActive(false);
    }

    // =========================================================
    // UTIL
    // =========================================================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        StopLitMotionAnimations();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        StopLitMotionAnimations();

        SceneManager.LoadScene(mainMenuScene);
    }

    void HideAllMenus()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (helpMenu != null) helpMenu.SetActive(false);
        if (optionsMenu != null) optionsMenu.SetActive(false);
        if (creditsMenu != null) creditsMenu.SetActive(false);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void StopLitMotionAnimations()
    {
        LitMotionAnimation[] animations =
            FindObjectsByType<LitMotionAnimation>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (LitMotionAnimation animation in animations)
        {
            if (animation != null && animation.IsPlaying)
            {
                animation.Stop();
            }
        }
    }
}