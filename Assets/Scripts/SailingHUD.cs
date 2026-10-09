
using UnityEngine;
using UnityEngine.UIElements;

public class SailingHUD : MonoBehaviour
{
    [SerializeField] private SailController sailController;

    private Label openingLabel;
    private Label rotationLabel;

    private void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        openingLabel = root.Q<Label>("openingLabel");
        rotationLabel = root.Q<Label>("rotationLabel");
    }

    private void Update()
    {
        if (sailController == null)
            return;

        openingLabel.text =
            $"Otwarcie żagla: {sailController.Opening:0}%";

        rotationLabel.text =
            $"Obrót żagla: {sailController.SailRotation:0}°";
    }
}