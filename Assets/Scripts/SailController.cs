
using UnityEngine;
using UnityEngine.InputSystem;

public class SailController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform sail;

    [Header("Input")]
    [SerializeField] private float openingSpeed = 50f;
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Sail opening")]
    [SerializeField, Range(0f, 100f)]
    private float opening = 100f;

    [Header("Sail rotation")]
    [SerializeField, Range(-45f, 45f)]
    private float sailRotation;

    private const float MinScaleY = 0.01f;
    private const float MaxScaleY = 1f;
    private const float MaxRotation = 45f;

    public float Opening => opening;
    public float SailRotation => sailRotation;

    private void Awake()
    {
        ApplyOpening();
        ApplyRotation();
    }

    
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        float openingInput = 0f;
        float rotationInput = 0f;

        if (Keyboard.current.wKey.isPressed)
            openingInput += 1f;

        if (Keyboard.current.sKey.isPressed)
            openingInput -= 1f;

        if (Keyboard.current.dKey.isPressed)
            rotationInput += 1f;

        if (Keyboard.current.aKey.isPressed)
            rotationInput -= 1f;

        SetOpening(opening + openingInput * openingSpeed * Time.deltaTime);

        SetSailRotation(
            sailRotation + rotationInput * rotationSpeed * Time.deltaTime
        );

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SetOpening(100f);
            SetSailRotation(0f);
        }
    }

    public void SetOpening(float value)
    {
        opening = Mathf.Clamp(value, 0f, 100f);
        ApplyOpening();
    }

    public void SetSailRotation(float value)
    {
        sailRotation = Mathf.Clamp(value, -MaxRotation, MaxRotation);

        ApplyRotation();
    }

    private void ApplyOpening()
    {
        if (pivot == null)
            return;

        Vector3 scale = pivot.localScale;

        scale.y = Mathf.Lerp(
            MinScaleY,
            MaxScaleY,
            opening / 100f
        );

        pivot.localScale = scale;
    }

    private void ApplyRotation()
    {
        if (sail == null)
            return;

        Vector3 angles = sail.localEulerAngles;

        angles.y = sailRotation;

        sail.localEulerAngles = angles;
    }
}
