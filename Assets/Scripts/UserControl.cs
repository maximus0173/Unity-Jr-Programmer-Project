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
    [SerializeField] private Vector2 cameraCageX = new Vector2(-25f, 9f);
    [SerializeField] private Vector2 cameraCageZ = new Vector2(-12f, 0f);

    private RaycastHit? mouseRaycastHit = null;

    public event System.EventHandler<OnMouseHoverEventArgs> OnMouseHover;
    public event System.EventHandler<OnMouseClickEventArgs> OnMouseClick;

    private int selectionLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        this.selectionLayerMask = LayerMask.GetMask("Selection");
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
        Vector3 newPosition = gameCamera.transform.position + new Vector3(move.y, 0, -move.x) * PanSpeed * Time.deltaTime;
        if (newPosition.x < this.cameraCageX.x)
        {
            newPosition.x = this.cameraCageX.x;
        } else if (newPosition.x > this.cameraCageX.y)
        {
            newPosition.x = this.cameraCageX.y;
        }
        if (newPosition.z < this.cameraCageZ.x)
        {
            newPosition.z = this.cameraCageZ.x;
        }
        else if (newPosition.z > this.cameraCageZ.y)
        {
            newPosition.z = this.cameraCageZ.y;
        }
        gameCamera.transform.position = newPosition;
    }

    void HandleMouseRaycastHit()
    {
        Vector3 mousePosition = Input.mousePosition;
        var ray = gameCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, this.selectionLayerMask))
        {
            this.mouseRaycastHit = hit;
            return;
        }
        if (Physics.Raycast(ray, out hit))
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
