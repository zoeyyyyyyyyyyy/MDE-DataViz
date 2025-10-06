using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PolicyUIController : MonoBehaviour
{
    [Header("Policy Controller")]
    public PolicyParticleController policyController;
    
    [Header("UI Buttons")]
    public Button cleanAirButton;
    public Button industrialRegButton;
    public Button vehicleEmissionsButton;
    public Button renewableEnergyButton;
    public Button resetButton;
    
    [Header("Status Display")]
    public TextMeshProUGUI statusText;
    
    void Start()
    {
        // Find the policy controller if not assigned
        if (policyController == null)
            policyController = FindObjectOfType<PolicyParticleController>();
        
        // Set up button listeners
        if (cleanAirButton != null)
            cleanAirButton.onClick.AddListener(() => TogglePolicy("Clean Air Act"));
        
        if (industrialRegButton != null)
            industrialRegButton.onClick.AddListener(() => TogglePolicy("Industrial Regulations"));
        
        if (vehicleEmissionsButton != null)
            vehicleEmissionsButton.onClick.AddListener(() => TogglePolicy("Vehicle Emissions Standards"));
        
        if (renewableEnergyButton != null)
            renewableEnergyButton.onClick.AddListener(() => TogglePolicy("Renewable Energy Policy"));
        
        if (resetButton != null)
            resetButton.onClick.AddListener(() => ResetAllPolicies());
        
        // Update UI initially
        UpdateUI();
    }
    
    void TogglePolicy(string policyName)
    {
        if (policyController != null)
        {
            policyController.TogglePolicy(policyName);
            UpdateUI();
        }
    }
    
    void ResetAllPolicies()
    {
        if (policyController != null)
        {
            policyController.ResetAllPolicies();
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        if (policyController == null || statusText == null) return;
        
        int currentCount = policyController.GetCurrentParticleCount();
        int baseCount = policyController.GetBaseParticleCount();
        float reduction = policyController.GetCurrentReductionPercent();
        var activePolicies = policyController.GetActivePolicies();
        
        string status = $"Particles: {currentCount}/{baseCount} ({reduction:F1}% reduction)\n";
        status += $"Active Policies: {activePolicies.Count}\n";
        
        if (activePolicies.Count > 0)
        {
            status += "Active: " + string.Join(", ", activePolicies);
        }
        else
        {
            status += "No active policies";
        }
        
        statusText.text = status;
        
        // Update button colors to show active state
        UpdateButtonColors();
    }
    
    void UpdateButtonColors()
    {
        var activePolicies = policyController.GetActivePolicies();
        
        // Update button colors based on active state
        UpdateButtonColor(cleanAirButton, "Clean Air Act", activePolicies);
        UpdateButtonColor(industrialRegButton, "Industrial Regulations", activePolicies);
        UpdateButtonColor(vehicleEmissionsButton, "Vehicle Emissions Standards", activePolicies);
        UpdateButtonColor(renewableEnergyButton, "Renewable Energy Policy", activePolicies);
    }
    
    void UpdateButtonColor(Button button, string policyName, System.Collections.Generic.List<string> activePolicies)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        if (activePolicies.Contains(policyName))
        {
            colors.normalColor = Color.green;
            colors.highlightedColor = Color.green * 0.8f;
        }
        else
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
        }
        button.colors = colors;
    }
    
    void Update()
    {
        // Update UI every frame to show real-time changes

    }
}
