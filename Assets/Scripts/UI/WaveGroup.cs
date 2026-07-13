using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveGroup : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;

    public void DrawText(int wave)
    {
        if (waveText != null)
            waveText.text= "Wave " + wave.ToString(); 
    }
}
