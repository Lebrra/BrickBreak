using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsLoader : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI bouncesText;
    [SerializeField]
    TextMeshProUGUI bricksText;
    [SerializeField]
    TextMeshProUGUI timeText;

    public void LoadText(Stats stats)
    {
        bouncesText.text = stats.bounces.ToString();
        bricksText.text = stats.bricksBroken.ToString();
        timeText.text = Utility.SecondsToTime(stats.timePlayed);
    }
}
