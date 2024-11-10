using System;
using UnityEngine;
using UnityEngine.UI;
public class CameraRotator : MonoBehaviour
{
    float speed = -10f;
    bool isRotating = true;
    // Define the fixed position and rotation for the camera
    Vector3 fixedPosition = new Vector3(0, 0, 0);
    Vector3 fixedRotation = new Vector3(0, 0, 0);

    [SerializeField] GameObject panel;
    [SerializeField] GameObject gamePanel;

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }
    }

    void Start()
    {
        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }
    }

    public void StopRotation()
    {
        isRotating = false;
        // Set the camera's position and rotation when the game starts
        transform.SetPositionAndRotation(fixedPosition, Quaternion.Euler(fixedRotation));

        if (panel != null)
        {
            panel.SetActive(false); // deactivate panel
            gamePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Panel referece not set");
        }
    }
}
