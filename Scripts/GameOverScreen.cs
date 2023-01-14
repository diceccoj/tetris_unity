using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverScreen : MonoBehaviour
{
    // set up gameover screen
    public void SetUp() {
        gameObject.SetActive(true);
    }

    //restart button functionality
    public void RestartButton() {
        SceneManager.LoadScene("Tetris");
    }

    //quit button functionality
    public void QuitButtonGO() {
        Application.Quit();
        Debug.Log("Quit");
    }


}
