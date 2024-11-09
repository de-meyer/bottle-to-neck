using System.Threading;
using TMPro;
using UnityEngine;

public class CalculateRoundPoints : MonoBehaviour
{
    [SerializeField] int points;
    [SerializeField] TextMeshProUGUI gameObjectPoints;

    // Update is called once per frame
    void Update()
    {
        if (gameObjectPoints != null)
        {
            gameObjectPoints.text = points.ToString();
        }
        else
        {
            Debug.LogWarning("Text field reference is not set.");
        }
    }
}
