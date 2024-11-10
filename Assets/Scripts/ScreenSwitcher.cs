using UnityEngine;

public class ScreenSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] toHide;
    [SerializeField] private GameObject[] toShow;
    public void SwitchScreen()
    {
        foreach (GameObject hidable in toHide)
        {
            hidable.SetActive(false);
        }
        foreach (GameObject showable in toShow)
        {
            showable.SetActive(true);
        }
    }
}
