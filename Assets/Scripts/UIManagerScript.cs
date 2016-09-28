using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

    public void StartGame() {
        SceneManager.LoadScene("minigame");
    }

}
