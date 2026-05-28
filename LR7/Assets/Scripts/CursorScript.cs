using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CursorScript : MonoBehaviour
{
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask objects;
    [SerializeField] private ObjectPanelScript objectPanel;

    private GameObject selectedObject;
    private bool drag;
    private Vector2 cursorPosition;
    private bool cursorOnUi;
    private Vector3 startingPosition;

    private void UpdatePanel()
    {
        if (selectedObject != null && objectPanel != null)
        {
            objectPanel.SetObject(selectedObject.GetComponent<ObjectDescription>());
        }
    }

    private void ClearPanel()
    {
        if (objectPanel != null)
        {
            objectPanel.ClearPanel();
        }
    }

    private GameObject CastRay()
    {
        if (Camera.main == null)
        {
            return null;
        }

        Ray ray = Camera.main.ScreenPointToRay(cursorPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, objects))
        {
            return hit.transform.gameObject;
        }

        return null;
    }

    public void Select(GameObject targetObject)
    {
        if (selectedObject == targetObject)
        {
            return;
        }

        if (selectedObject != null)
        {
            Deselect();
        }

        selectedObject = targetObject;

        HighlightScript highlight = selectedObject != null ? selectedObject.GetComponent<HighlightScript>() : null;
        if (highlight != null)
        {
            highlight.ToggleHighlight(true);
        }

        UpdatePanel();

        if (selectedObject != null)
        {
            startingPosition = selectedObject.transform.position;
        }
    }

    public void Deselect()
    {
        if (selectedObject != null)
        {
            HighlightScript highlight = selectedObject.GetComponent<HighlightScript>();
            if (highlight != null)
            {
                highlight.ToggleHighlight(false);
            }

            selectedObject = null;
        }
    }

    private void MouseDown()
    {
        GameObject targetObject = CastRay();

        if (targetObject != null)
        {
            Select(targetObject);
            drag = true;
        }
        else
        {
            ClearPanel();
            Deselect();
        }
    }

    private void MouseUp()
    {
        drag = false;

        if (selectedObject != null && selectedObject.transform.position != startingPosition)
        {
            ICommand command = new MoveCommand(selectedObject, startingPosition, selectedObject.transform.position);
            CommandInvoker.ExecuteCommand(command);
            startingPosition = selectedObject.transform.position;
        }
    }

    public void OnClick(InputValue button)
    {
        if (cursorOnUi)
        {
            return;
        }

        if (button.isPressed)
        {
            MouseDown();
        }
        else
        {
            MouseUp();
        }
    }

    public void OnPoint(InputValue mousePosition)
    {
        cursorPosition = mousePosition.Get<Vector2>();

        if (drag && selectedObject != null && Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(cursorPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ground))
            {
                selectedObject.transform.position = hit.point;
            }
        }
    }

    private void Update()
    {
        cursorOnUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
