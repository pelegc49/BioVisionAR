using UnityEngine;

public class AppModeManager : MonoBehaviour
{
    [Header("UI Panels & Containers")]
    public GameObject filterMenuPanel; 
    public GameObject eyeModelContainer; // The eye model
    public GameObject ishiharaGameContainer; // The game

    [Header("AR Controller")]
    public ARContentPlacer arPlacer; // Reference to the placement script

    // Button 1
    public void ToggleFilterMenu()
    {
        bool isActive = filterMenuPanel.activeSelf;
        filterMenuPanel.SetActive(!isActive);
    }

    // Button 2: Eye Model
    public void ActivateEyeModel()
    {
        // 1. Turn off other models
        filterMenuPanel.SetActive(false);
        ishiharaGameContainer.SetActive(false); 
        
        // 2. Temporarily turn off the eye (the Placer will activate it when placed)
        eyeModelContainer.SetActive(false);

        // 3. Start placement process for the eye
        if (arPlacer != null)
        {
            arPlacer.enabled = true;
            // Send the eye model for placement!
            arPlacer.StartPlacementProcess(eyeModelContainer); 
        }
    }

    // Button 3: Ishihara Game
    public void ActivateIshihara()
    {
        filterMenuPanel.SetActive(false);
        eyeModelContainer.SetActive(false);
        ishiharaGameContainer.SetActive(false);

        // Start placement process for the game
        if (arPlacer != null)
        {
            arPlacer.enabled = true;
            // Send the game for placement!
            arPlacer.StartPlacementProcess(ishiharaGameContainer);
        }
    }
}