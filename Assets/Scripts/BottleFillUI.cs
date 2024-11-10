using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BottleFillUI : MonoBehaviour
{
    public Image fillImage;  // Reference to the child image used for filling

    int counter = 1;

    void Start()
    {
        UpdateFillLevel(0); // Start with empty bottle
        InvokeRepeating("incrementFill", 1, 1);
    }

    void incrementFill()
    {
        UpdateFillLevel(counter++);
        Debug.Log("Counter" + counter);
    }

    public void UpdateFillLevel(float fillPercentage)
    {
        fillImage.rectTransform.anchoredPosition = new Vector2(fillImage.rectTransform.anchoredPosition.x,
        550 * (fillPercentage / 100f));
    }
}
