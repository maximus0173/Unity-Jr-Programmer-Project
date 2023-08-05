using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handle all the control code, so detecting when the users click on a unit or building and selecting those
/// </summary>
public class UserControl : MonoBehaviour
{

    public static UserControl Instance { get; private set; }

    [SerializeField] private Camera gameCamera;
    [SerializeField] private float PanSpeed = 10.0f;

    private RaycastHit? mouseRaycastHit = null;

    public event System.EventHandler<OnMouseHoverEventArgs> OnMouseHover;
    public event System.EventHandler<OnMouseClickEventArgs> OnMouseClick;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        HandleCameraMove();
        HandleMouseRaycastHit();
        HandleMouseHover();
        HandleMouseButtonClick();
        return;
    }

    void HandleCameraMove()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        gameCamera.transform.position = gameCamera.transform.position + new Vector3(move.y, 0, -move.x) * PanSpeed * Time.deltaTime;
    }

    void HandleMouseRaycastHit()
    {
        Vector3 mousePosition = Input.mousePosition;
        var ray = gameCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            this.mouseRaycastHit = hit;
        }
        else
        {
            this.mouseRaycastHit = null;
        }
    }

    void HandleMouseHover()
    {
        if (this.mouseRaycastHit == null)
        {
            return;
        }
        Vector3 mousePosition = Input.mousePosition;
        RaycastHit hit = (RaycastHit)this.mouseRaycastHit;
        GameObject mouseHoverObject = hit.collider.gameObject;
        OnMouseHover?.Invoke(this, new OnMouseHoverEventArgs(mousePosition, hit.point, mouseHoverObject));
    }

    void HandleMouseButtonClick()
    {
        int buttonNumber = -1;
        if (Input.GetMouseButtonDown(0))
        {
            buttonNumber = 0;
        } else if (Input.GetMouseButtonDown(1))
        {
            buttonNumber = 1;
        }
        else if (Input.GetMouseButtonDown(2))
        {
            buttonNumber = 2;
        }
        if (buttonNumber < 0)
        {
            return;
        }
        if (this.mouseRaycastHit == null)
        {
            return;
        }
        Vector3 mousePosition = Input.mousePosition;
        RaycastHit hit = (RaycastHit)this.mouseRaycastHit;
        GameObject clickedObject = hit.collider.gameObject;
        OnMouseClick?.Invoke(this, new OnMouseClickEventArgs(buttonNumber, mousePosition, hit.point, clickedObject));
    }

    public class OnMouseHoverEventArgs : System.EventArgs
    {

        public Vector2 ScreenPosition { get; private set; }

        public Vector3 WorldPosition { get; private set; }

        public GameObject MouseHoveredObject { get; private set; }

        public OnMouseHoverEventArgs(Vector2 screenPosition, Vector3 worldPosition, GameObject mouseHoveredObject)
        {
            this.ScreenPosition = screenPosition;
            this.WorldPosition = worldPosition;
            this.MouseHoveredObject = mouseHoveredObject;
        }
    }

    public class OnMouseClickEventArgs : System.EventArgs
    {

        private int mouseButtonNumber;

        public Vector2 ScreenPosition { get; private set; }

        public Vector3 WorldPosition { get; private set; }

        public GameObject MouseClickedObject { get; private set; }

        public bool IsLeftMouseButtonClicked { get => this.mouseButtonNumber == 0; }
        public bool IsRightMouseButtonClicked { get => this.mouseButtonNumber == 1; }
        public bool IsMiddleMouseButtonClicked { get => this.mouseButtonNumber == 2; }

        public OnMouseClickEventArgs(int mouseButtonNumber, Vector2 screenPosition, Vector3 worldPosition, GameObject mouseClickedObject)
        {
            this.mouseButtonNumber = mouseButtonNumber;
            this.ScreenPosition = screenPosition;
            this.WorldPosition = worldPosition;
            this.MouseClickedObject = mouseClickedObject;
        }
    }

}
