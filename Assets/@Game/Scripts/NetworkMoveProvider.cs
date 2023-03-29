using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkMoveProvider : ActionBasedContinuousMoveProvider
{
    [SerializeField]
    public bool enableInputActions;  

    protected override Vector2 ReadInput()
    {
        if (enableInputActions == false)
        {
            return Vector2.zero;
        }

        return base.ReadInput();
    }
} 