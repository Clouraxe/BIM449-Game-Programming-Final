using Unity.VisualScripting;
using UnityEngine;

public class CheatStudentPaper : CheatMethod
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void OnLook()
    {
        StartCheating();
    }
    
    public override void OnUnlook()
    {
        StopCheating();
    }

    public override void OnClick()
    {
        // No action on click for student paper
    }
    
    public override void OnUnclick()
    {
        // No action on unclick for student paper
    }
    
}
