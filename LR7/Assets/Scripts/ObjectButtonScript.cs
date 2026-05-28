using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectButtonScript : MonoBehaviour
{
    [SerializeField] private Image objectIcon;
    [SerializeField] private TMP_Text objectName;

    public void SetSprite(Sprite sprite)
    {
        if (objectIcon != null)
        {
            objectIcon.sprite = sprite;
            objectIcon.enabled = sprite != null;
        }
    }

    public void SetText(string text)
    {
        if (objectName != null)
        {
            objectName.text = text;
        }
    }
}
