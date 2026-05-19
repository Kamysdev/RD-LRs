using UnityEngine;
using UnityEngine.UI;

public class ObjectButtonScript : MonoBehaviour
{
    [SerializeField] private Image objectIcon;
    [SerializeField] private Text objectName;

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
