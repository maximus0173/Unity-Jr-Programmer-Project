using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PalletTime : MonoBehaviour
{

    [SerializeField]
    private Image image;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Color color1;

    [SerializeField]
    private Color color2;

    [SerializeField]
    private Color color3;

    private int priority;
    private Pallet pallet;
    private int timeInSeconds;

    public void FollowPallet(int priority, Pallet pallet, int timeInSeconds)
    {
        if (priority < 1 || priority > 3)
        {
            throw new System.Exception("Wrong priority");
        }
        this.priority = priority;
        this.pallet = pallet;
        this.timeInSeconds = timeInSeconds;
        switch (this.priority)
        {
            case 1:
                transform.localScale = 1.7f * transform.localScale;
                break;
            case 2:
                transform.localScale = 1.0f * transform.localScale;
                break;
            case 3:
                transform.localScale = 0.7f * transform.localScale;
                break;
        }
        UpdateColor();
    }

    private void Update()
    {
        UpdatePosition();
        UpdateTime();
    }

    private void UpdateColor()
    {
        Color color = this.color1;
        switch (this.priority)
        {
            case 1: color = this.color1; break;
            case 2: color = this.color2; break;
            case 3: color = this.color3; break;
        }
        this.image.color = color;
        this.timeText.color = color;
    }

    private void UpdatePosition()
    {
        if (this.pallet == null)
        {
            return;
        }
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(this.pallet.Position);
        transform.position = screenPosition;
    }

    private void UpdateTime()
    {
        if (this.pallet == null)
        {
            return;
        }
        int elapsedTime = TruckShippingManager.Instance.ElapsedTime;
        int timeDiff = this.timeInSeconds - elapsedTime;

        System.TimeSpan time = System.TimeSpan.FromSeconds(Mathf.Abs(timeDiff));
        string timeStr = time.ToString(@"mm\:ss");
        if (timeDiff < 0)
        {
            timeStr = '-' + timeStr;
        }

        this.timeText.text = timeStr;
    }

}
