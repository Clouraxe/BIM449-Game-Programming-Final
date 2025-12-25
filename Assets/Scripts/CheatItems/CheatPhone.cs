using UnityEngine;

public class CheatPhone : CheatMethod
{
    private Color[] originalColors; // Stores the original colors of the phone parts
    [SerializeField] private Transform phoneModel;

    [SerializeField] private Transform phoneHoldPoint;
    private Vector3 initialPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
        originalColors = new Color[phoneModel.childCount];
        initialPosition = transform.position;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void OnLook()
    {
        if (isPlayerCheating) return;
        for (int i = 0; i < phoneModel.childCount; i++)
        {
            Transform child = phoneModel.GetChild(i);
            if (child.TryGetComponent(out Renderer rend))
            {
                originalColors[i] = rend.material.color;
                Color highlightColor = Color.yellow;
                rend.material.color = highlightColor;
            }
        }
    }

    public override void OnUnlook()
    {
        for (int i = 0; i < phoneModel.childCount; i++)
        {
            Transform child = phoneModel.GetChild(i);
            if (child.TryGetComponent(out Renderer rend))
            {
                Color originalColor = originalColors[i];
                rend.material.color = originalColor;
            }
        }
    }

    public override void OnClick()
    {
        transform.position = phoneHoldPoint.position;
        transform.eulerAngles = phoneHoldPoint.eulerAngles;
        StartCheating();
    }
    
    public override void OnUnclick()
    {
        StopCheating();
        transform.position = initialPosition;
        transform.eulerAngles = Vector3.zero;
    }
}
