using UnityEngine.XR.Interaction.Toolkit;

public class Socket_with_tag_check : XRSocketInteractor
{
    //Script description: Makes the Lever go upp automatically with out trying to get enything else 
    public string targetTag = string.Empty;

    [System.Obsolete]
    public override bool CanHover(XRBaseInteractable interactable)
    {
        return base.CanHover(interactable) && MatchingUsingTag(interactable);
    }

    [System.Obsolete]
    public override bool CanSelect(XRBaseInteractable interactable)
    {
        return base.CanSelect(interactable) && MatchingUsingTag(interactable);
    }

    private bool MatchingUsingTag(XRBaseInteractable interactable)
    {
        return interactable.CompareTag(targetTag);
    }

}
