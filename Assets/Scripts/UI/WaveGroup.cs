using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveGroup : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;

    private void Awake()
    {
        if ( GameManager.Instance != null)
        {
            StageManager sm = GameManager.Instance.StageManager;

            if (sm != null)
            {
                sm.SetWaveGroup(this);
            }
        }
    }

    public void DrawText(int wave)
    {
        if (waveText != null)
            waveText.text= "Wave " + wave.ToString(); 
    }
}
