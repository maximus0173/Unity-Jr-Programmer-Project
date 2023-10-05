using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectSpeedSound : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minSpeed = 0f;
    [SerializeField] private float thresholdSpeed = 2f;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 1.3f;
    [SerializeField] private float maxVolume = 0.8f;
    [SerializeField] private float pitchChangeSpeed = 2f;
    [SerializeField] private bool localSpeed = false;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float pitchOffset = 0f;

    private void Start()
    {
        this.lastPosition = this.localSpeed ? transform.localPosition : transform.position;
        InvokeRepeating("pitchOffsetChange", 0f, 2f);
    }

    private void Update()
    {
        Vector3 pos = this.localSpeed ? transform.localPosition : transform.position;
        Quaternion rot = this.localSpeed ? transform.localRotation : transform.rotation;
        float moveDiff = Vector3.Distance(pos, this.lastPosition);
        float rotationDiff = Quaternion.Angle(rot, this.lastRotation) * 0.01f;
        float speed = Mathf.Max(moveDiff / Time.deltaTime, rotationDiff / Time.deltaTime);
        if (speed < this.minSpeed)
        {
            speed = 0f;
        }
        float newPitch = Mathf.Lerp(this.minPitch, this.maxPitch, Mathf.Clamp01(Mathf.InverseLerp(0, this.thresholdSpeed, speed)));
        float newTargetPitch = newPitch + this.pitchOffset;
        this.audioSource.pitch = Mathf.MoveTowards(this.audioSource.pitch, newTargetPitch, this.pitchChangeSpeed * Time.deltaTime);
        bool playSound = newPitch > this.minPitch;
        this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, playSound ? this.maxVolume : 0f, 2f * this.pitchChangeSpeed * Time.deltaTime);
        if (!this.audioSource.isPlaying && this.audioSource.volume > 0f)
        {
            this.audioSource.Play();
        }
        else if (this.audioSource.isPlaying && this.audioSource.volume < 0.1f)
        {
            this.audioSource.Stop();
        }

        this.lastPosition = pos;
        this.lastRotation = rot;
    }

    private void pitchOffsetChange()
    {
        this.pitchOffset = Random.Range(-0.005f, 0.005f);
    }

}
