using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void Interact();

    void HelperInteract();

    TriggeredAreaData AIInteract(AIController aiController);
   
    void AIExit(AIController aiController);

    void EndInteraction();
}