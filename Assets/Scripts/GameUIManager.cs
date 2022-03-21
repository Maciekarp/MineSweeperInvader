using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour {
    
    public bool isPaused = false;

    [SerializeField] private GridController gridCont;
    [SerializeField] private TMPro.TextMeshProUGUI endRoundText;

    [SerializeField] private GameObject pauseMenu;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(isPaused) {
                Resume();
            } else {
                Pause();
            }
        } 
    }

    // Resumes the game
    public void Resume() {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Pauses the game
    public void Pause() {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    // Starts coroutine that restarts the game
    public void RestartGame() {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsynchronously(SceneManager.GetActiveScene().buildIndex));
    }

    // Coroutine that manages the loading screen while scene restarts
    IEnumerator LoadSceneAsynchronously(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while(!operation.isDone) {
            Debug.Log(operation.progress);
            yield return null;
        }
        
    }

    // Quits the application
    public void QuitGame() {
        Application.Quit();
    }

    // Starts coroutine that ends the round
    public void EndRound() {
        StartCoroutine(EndingRound());
    }

    private IEnumerator EndingRound() {
        StartCoroutine(WaitingAnimation());
        // Initiates collection of resource extractors
        //gridCont.Extract();

        yield return new WaitForSeconds(5f);
        // Changes to the hub scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    
    }

    // Corutine that animates the exit button text while scene is ending
    private IEnumerator WaitingAnimation() {
        while(true) {
            endRoundText.text = "    .";
            yield return new WaitForSeconds(0.5f);
            endRoundText.text = "  . .";
            yield return new WaitForSeconds(0.5f);
            endRoundText.text = ". . .";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
