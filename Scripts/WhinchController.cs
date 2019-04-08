using UnityEngine;

public class WhinchController : MonoBehaviour
{
    #region MEMBERS

    [SerializeField] private float radius = 0.9f;
    [SerializeField] private AudioSource cachedAudioSource;

    #endregion

    #region PROPERTIES

    private float Radius { get { return radius; } }
    private AudioSource CachedAudioSource { get { return cachedAudioSource; } }

    #endregion 

    #region FUNCTIONS

    public void SetSoundState(bool state)
    {
        if (state == true)
            CachedAudioSource.Play();
        else
            CachedAudioSource.Stop();
    }

    public void ApplyLiftPositionDelta(float delta)
    {
        transform.Rotate((delta / (radius * Mathf.PI)) * 360.0f, 0.0f, 0.0f);
    }

    #endregion
}