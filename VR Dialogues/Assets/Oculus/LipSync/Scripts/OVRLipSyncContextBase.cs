using UnityEngine;

//-------------------------------------------------------------------------------------
// ***** OVRLipSyncContextBase
//
/// <summary>
/// OVRLipSyncContextBase interfaces into the Oculus phoneme recognizer.
/// This component should be added into the scene once for each Audio Source.
/// </summary>
public class OVRLipSyncContextBase : MonoBehaviour
{
    // Public members
    public AudioSource audioSource = null;  // We now rely on the user to assign this in the inspector.

    [Tooltip("Which lip sync provider to use for viseme computation.")]
    public OVRLipSync.ContextProviders provider = OVRLipSync.ContextProviders.Enhanced;

    [Tooltip("Enable DSP offload on supported Android devices.")]
    public bool enableAcceleration = true;

    // Private members
    private OVRLipSync.Frame frame = new OVRLipSync.Frame();
    private uint context = 0;  // 0 means no context

    private int _smoothing;
    public int Smoothing
    {
        set
        {
            OVRLipSync.Result result = OVRLipSync.SendSignal(context, OVRLipSync.Signals.VisemeSmoothing, value, 0);

            if (result != OVRLipSync.Result.Success)
            {
                if (result == OVRLipSync.Result.InvalidParam)
                {
                    Debug.LogError("OVRLipSyncContextBase.SetSmoothing: A viseme smoothing parameter is invalid, it should be between 1 and 100!");
                }
                else
                {
                    Debug.LogError("OVRLipSyncContextBase.SetSmoothing: An unexpected error occurred.");
                }
            }

            _smoothing = value;
        }
        get
        {
            return _smoothing;
        }
    }

    public uint Context
    {
        get
        {
            return context;
        }
    }

    protected OVRLipSync.Frame Frame
    {
        get
        {
            return frame;
        }
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        // Ensure the audio source has been manually assigned, otherwise throw an error.
        if (!audioSource)
        {
            Debug.LogError("OVRLipSyncContextBase: No AudioSource assigned! Please assign a valid AudioSource.");
            return;
        }

        lock (this)
        {
            if (context == 0)
            {
                if (OVRLipSync.CreateContext(ref context, provider, 0, enableAcceleration) != OVRLipSync.Result.Success)
                {
                    Debug.LogError("OVRLipSyncContextBase.Start ERROR: Could not create Phoneme context.");
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    void OnDestroy()
    {
        // Destroy the phoneme context
        lock (this)
        {
            if (context != 0)
            {
                if (OVRLipSync.DestroyContext(context) != OVRLipSync.Result.Success)
                {
                    Debug.LogError("OVRLipSyncContextBase.OnDestroy ERROR: Could not delete Phoneme context.");
                }
            }
        }
    }

    // Public Functions

    public OVRLipSync.Frame GetCurrentPhonemeFrame()
    {
        return frame;
    }

    public void SetVisemeBlend(int viseme, int amount)
    {
        OVRLipSync.Result result = OVRLipSync.SendSignal(context, OVRLipSync.Signals.VisemeAmount, viseme, amount);

        if (result != OVRLipSync.Result.Success)
        {
            Debug.LogError("OVRLipSyncContextBase.SetVisemeBlend: An error occurred.");
        }
    }

    public void SetLaughterBlend(int amount)
    {
        OVRLipSync.Result result = OVRLipSync.SendSignal(context, OVRLipSync.Signals.LaughterAmount, amount, 0);

        if (result != OVRLipSync.Result.Success)
        {
            Debug.LogError("OVRLipSyncContextBase.SetLaughterBlend: An error occurred.");
        }
    }

    public OVRLipSync.Result ResetContext()
    {
        frame.Reset();
        return OVRLipSync.ResetContext(context);
    }
}
