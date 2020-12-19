using UnityEngine;
using UnityEngine.UI;

public class FadeInOutOnMouseEnterLeave : MonoBehaviour
{
    private RectTransform _parentText;
    
    // Children
    private GameObject _panel;
    private Image _panelImageComponent;
    private float _panelImageStartingAlpha;
    
    private GameObject _arrow;
    private Image _arrowImageComponent;
    private float _arrowImageStartingAlpha;

    private GameObject _description;
    private Text _descriptionTextComponent;
    private float _descriptionStartingAlpha;

    private GameObject _value;
    private Text _valueTextComponent;
    private float _valueStartingAlpha;
    
    private float _time;
    private float _panelChange;
    private float _descriptionChange;
    private float _valueChange;
    private float _arrowChange;

    void Start()
    {
        _parentText = GetComponent<RectTransform>();
        
        // Get children / components.
        _panel = gameObject.transform.Find("Descriptor").gameObject;
        _panelImageComponent = _panel.GetComponent<Image>();
        _panelImageStartingAlpha = _panelImageComponent.color.a;
        
        _description = _panel.gameObject.transform.Find("Description").gameObject;
        _descriptionTextComponent = _description.GetComponent<Text>();
        _descriptionStartingAlpha = _descriptionTextComponent.color.a;
        
        _value = _panel.gameObject.transform.Find("Value").gameObject;
        _valueTextComponent = _value.GetComponent<Text>();
        _valueStartingAlpha = _valueTextComponent.color.a;
        
        _arrow = _panel.gameObject.transform.Find("Arrow").gameObject;
        _arrowImageComponent = _arrow.GetComponent<Image>();
        _arrowImageStartingAlpha = _arrowImageComponent.color.a;
        
        _time = 0.5f;
        _panelChange = _panelImageStartingAlpha / _time;
        _descriptionChange = _descriptionStartingAlpha / _time;
        _valueChange = _valueStartingAlpha / _time;
        _arrowChange = _arrowImageStartingAlpha / _time;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (IsMouseInside(_parentText))
            {
                OnMouseEnter();
            }
            else
            {
                OnMouseExit();
            }
        }
    }

    private bool IsMouseInside(RectTransform rectTransform)
    {
        Vector2 mousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(mousePosition);
    }
    
    public void OnMouseEnter()
    {
        _panel.SetActive(true);
        
        Color panelColor = _panelImageComponent.color;
        Color arrowColor = _arrowImageComponent.color;
        Color descriptionColor = _descriptionTextComponent.color;
        Color valueColor = _valueTextComponent.color;
        
        // Update panel color.
        if (panelColor.a <= _panelImageStartingAlpha)
        {
            panelColor.a += _panelChange * Time.deltaTime;
        }
        // Finished fading in.
        else
        {
            panelColor.a = _panelImageStartingAlpha;
        }
        
        _panelImageComponent.color = panelColor;
      
        
        // Update arrow color.
        if (arrowColor.a <= _arrowImageStartingAlpha)
        {
            arrowColor.a += _arrowChange * Time.deltaTime;
        }
        // Finished fading in.
        else
        {
            arrowColor.a = _arrowImageStartingAlpha;
        }
        
        _arrowImageComponent.color = arrowColor;

        
        // Update description color.
        if (descriptionColor.a <= _descriptionStartingAlpha)
        {
            descriptionColor.a += _descriptionChange * Time.deltaTime;
        }
        // Finished fading in.
        else
        {
            descriptionColor.a = _descriptionStartingAlpha;
        }

        _descriptionTextComponent.color = descriptionColor;
        
        
        // Update value color.
        if (valueColor.a <= _valueStartingAlpha)
        {
            valueColor.a += _valueChange * Time.deltaTime;
        }
        // Finished fading in.
        else
        {
            valueColor.a = _valueStartingAlpha;
        }

        _valueTextComponent.color = valueColor;
    }
    
    public void OnMouseExit()
    {
        Color panelColor = _panelImageComponent.color;
        Color arrowColor = _arrowImageComponent.color;
        Color descriptionColor = _descriptionTextComponent.color;
        Color valueColor = _valueTextComponent.color;
    
        // Update panel color.
        if (panelColor.a >= 0)
        {
            panelColor.a -= _panelChange * Time.deltaTime;
        }
        // Finished fading out.
        else
        {
            panelColor.a = 0;
            _panel.SetActive(false);
        }
    
        _panelImageComponent.color = panelColor;
  
    
        // Update arrow color.
        if (arrowColor.a >= 0)
        {
            arrowColor.a -= _arrowChange * Time.deltaTime;
        }
        // Finished fading out.
        else
        {
            arrowColor.a = 0;
        }
    
        _arrowImageComponent.color = arrowColor;

    
        // Update panel color.
        if (descriptionColor.a >= 0)
        {
            descriptionColor.a -= _descriptionChange * Time.deltaTime;
        }
        // Finished fading out.
        else
        {
            descriptionColor.a = 0;
        }

        _descriptionTextComponent.color = descriptionColor;
        
        
        // Update value color.
        if (valueColor.a >= 0)
        {
            valueColor.a -= _valueChange * Time.deltaTime;
        }
        // Finished fading in.
        else
        {
            valueColor.a = 0;
        }

        _valueTextComponent.color = valueColor;
    }

}
