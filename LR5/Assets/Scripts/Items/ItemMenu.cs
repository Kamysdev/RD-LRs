using UnityEngine;

public class ItemMenu : MonoBehaviour
{
    public PlayerScript player;
    public int i;
    private RectTransform rectTransform;
    private RectTransform rootParent;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        rootParent = transform.parent as RectTransform;
    }

    public void use()
    {
        if (player == null)
            return;

        player.use(i);
        hide();
    }

    public void drop()
    {
        if (player == null)
            return;

        player.drop(i);
        hide();
    }

    public void destroy()
    {
        if (player == null)
            return;

        player.destroy(i);
        hide();
    }

    public void show(RectTransform target, int ind)
    {
        i = ind;

        // if (rootParent != null && transform.parent != rootParent)
        //     transform.SetParent(rootParent, false);

        // if (target != null && rectTransform != null && rootParent != null)
        // {
        //     Vector3[] corners = new Vector3[4];
        //     target.GetWorldCorners(corners);

        //     rectTransform.position = corners[2] + new Vector3(16f, -16f, 0f);
        // }

        gameObject.SetActive(true);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1))
            hide();
    }
}
