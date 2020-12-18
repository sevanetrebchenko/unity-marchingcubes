using UnityEngine;
using UnityEngine.UI;

public class ToggleMenuVisible : MonoBehaviour
{
    public Button button;
    public GameObject panel;

    public string enabledText;
    public string disabledText;
    public bool active;

    private Text _buttonText;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        _buttonText = button.gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
        if (active)
        {
            panel.SetActive(true);
            _buttonText.text = enabledText;
        }
        else
        {
            panel.SetActive(false);
            _buttonText.text = disabledText;
        }
    }

    private void OnClick()
    {
        active = !active;
        
        if (active)
        {
            panel.SetActive(true);
            _buttonText.text = enabledText;
        }
        else
        {
            panel.SetActive(false);
            _buttonText.text = disabledText;
        }
    }
    
}
