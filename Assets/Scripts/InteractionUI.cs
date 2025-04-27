using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject _panelUI; // El panel que contiene el texto
    [SerializeField] private Text _interactionText; // El texto que mostrará la tecla

    private void Start()
    {
        _panelUI.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        _interactionText.text = message;
        _panelUI.SetActive(true);
    }

    public void HideMessage()
    {
        _panelUI.SetActive(false);
    }
}
