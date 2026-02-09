public interface IInteractableObject
{
    void OnHoverEnter();
    void OnHoverExit();
}

// Untuk yang butuh di-HOLD (Bom/Trap)
public interface IHoldInteractable : IInteractableObject
{
    void OnHoldStart();
    void OnHoldCancel();
    void OnHoldSuccess();
}

// Untuk yang cukup di-TAP (Chest/Pintu)
public interface ITapInteractable : IInteractableObject
{
    void OnTap();
}