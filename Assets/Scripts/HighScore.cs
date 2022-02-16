using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{
    public TextMeshProUGUI FirstScoreText;
    public TextMeshProUGUI FirstMovesText;
    public TextMeshProUGUI SecondScoreText;
    public TextMeshProUGUI SecondMovesText;
    public TextMeshProUGUI ThirdScoreText;
    public TextMeshProUGUI ThirdMovesText;

    void Start()
    {
        string score = ScoreStringFromFloat(PlayerPrefs.GetFloat("FirstScore", 0f));
        FirstScoreText.text = score == "0:00" ? "..." : score;
        FirstMovesText.text = PlayerPrefs.GetInt("FirstMoves", 0).ToString();
        score = ScoreStringFromFloat(PlayerPrefs.GetFloat("SecondScore", 0f));
        SecondScoreText.text = score == "0:00" ? "..." : score;
        SecondMovesText.text = PlayerPrefs.GetInt("SecondMoves", 0).ToString();
        score = ScoreStringFromFloat(PlayerPrefs.GetFloat("ThirdScore", 0f));
        ThirdScoreText.text = score == "0:00" ? "..." : score;
        ThirdMovesText.text = PlayerPrefs.GetInt("ThirdMoves", 0).ToString();
    }

    string ScoreStringFromFloat(float score)
    {
        int m = (int)(score / 60);
        int s = (int)(score % 60);
        return s < 10 ? m + ":0" + s : m + ":" + s;
    }
}
