using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseController : MonoBehaviour {

    private Image winLosePanel;
    private Text winLoseText;
    private Text pressAnyKeyText;

    // Use this for initialization
	void Awake() {
        this.winLosePanel = GetComponent<Image>();
        Text[] comps = GetComponentsInChildren<Text>();

        for (int i = 0 ; i < comps.Length ; i++) {
            if (comps[i].name == "WinLoseText") {
                this.winLoseText = comps[i];
            }
            else if (comps[i].name == "PressAnyKeyText") {
                this.pressAnyKeyText = comps[i];
            }
        }

        if (this.winLosePanel == null) {
            throw new Exception("Cannot locate winLosePanel component");
        }

        if (this.winLoseText == null) {
            throw new Exception("Cannot locate winLoseText component");
        }

        if (this.pressAnyKeyText == null) {
            throw new Exception("Cannot locate pressAnyKeyText component");
        }
	}
	
	// Update is called once per frame
	void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("TitleScreen");
        }
	}

    public void activate(PlayerNumber winner)
    {
        gameObject.SetActive(true);

        if (PlayerNumber.PLAYER1 == winner) {
            this.winLosePanel.color = new Color(1F, 1F, 0F);
            this.winLoseText.text = "You Win!";
            this.winLoseText.color = new Color(0F, 0F, 0F);
            this.pressAnyKeyText.color = this.winLoseText.color;
        }
        else if (PlayerNumber.PLAYER2 == winner) {
            this.winLosePanel.color = new Color(0F, 0F, 1F);
            this.winLoseText.text = "You Lose!";
            this.winLoseText.color = new Color(1F, 1F, 1F);
            this.pressAnyKeyText.color = this.winLoseText.color;
        }
        else {
            this.winLosePanel.color = new Color(0F, 1F, 1F);
            this.winLoseText.text = "Draw!";
            this.winLoseText.color = new Color(0F, 0F, 0F);
            this.pressAnyKeyText.color = this.winLoseText.color;
        }

        Time.timeScale = 0;
    }
}
