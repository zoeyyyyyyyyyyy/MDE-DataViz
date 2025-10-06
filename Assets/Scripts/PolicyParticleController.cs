using UnityEngine;
using System.Collections.Generic;

public class PolicyParticleController : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem airParticles;
    
    [Header("Base Settings")]
    public int baseParticleCount = 1000;
    
    [System.Serializable]
    public class PolicyEffect
    {
        public string policyName;
        public float particleReductionPercent; // 0.0 to 1.0 (0% to 100%)
        public bool isActive = false;
    }
    
    [Header("Policy Effects")]
    public List<PolicyEffect> policies = new List<PolicyEffect>();
    
    private int currentParticleCount;
    private Dictionary<string, PolicyEffect> policyDictionary = new Dictionary<string, PolicyEffect>();
    
    void Start()
    {
        InitializePolicies();
        
        // Set initial particle count
        if (airParticles != null)
        {
            var emission = airParticles.emission;
            emission.enabled = true;
            emission.rateOverTime = 0; // No continuous emission
            
            // Set particles to live forever and prevent fading
            var main = airParticles.main;
            main.startLifetime = Mathf.Infinity; // Particles never die naturally
            main.startColor = Color.white; // Ensure particles are visible
            
            // Disable any fade-out effects
            var colorOverLifetime = airParticles.colorOverLifetime;
            colorOverLifetime.enabled = false; // Disable color fading
            
            var sizeOverLifetime = airParticles.sizeOverLifetime;
            sizeOverLifetime.enabled = false; // Disable size fading
            
            var velocityOverLifetime = airParticles.velocityOverLifetime;
            velocityOverLifetime.enabled = false; // Disable velocity changes
            
            // Emit all initial particles at once
            airParticles.Emit(baseParticleCount);
            currentParticleCount = baseParticleCount;
            
            // Disable emission after initial burst
            emission.enabled = false;
        }
        
        UpdateParticleCount();
    }
    
    void InitializePolicies()
    {
        // Initialize default policies
        policies.Add(new PolicyEffect { policyName = "Clean Air Act", particleReductionPercent = 0.5f });
        policies.Add(new PolicyEffect { policyName = "Industrial Regulations", particleReductionPercent = 0.3f });
        policies.Add(new PolicyEffect { policyName = "Vehicle Emissions Standards", particleReductionPercent = 0.4f });
        policies.Add(new PolicyEffect { policyName = "Renewable Energy Policy", particleReductionPercent = 0.6f });
        
        // Create dictionary for quick lookup
        foreach (var policy in policies)
        {
            policyDictionary[policy.policyName.ToLower()] = policy;
        }
    }
    
    public void TogglePolicy(string policyName)
    {
        string key = policyName.ToLower();
        if (policyDictionary.ContainsKey(key))
        {
            policyDictionary[key].isActive = !policyDictionary[key].isActive;
            UpdateParticleCount();
            Debug.Log($"Policy '{policyName}' {(policyDictionary[key].isActive ? "activated" : "deactivated")}");
        }
        else
        {
            Debug.LogWarning($"Policy '{policyName}' not found!");
        }
    }
    
    public void ActivatePolicy(string policyName)
    {
        string key = policyName.ToLower();
        if (policyDictionary.ContainsKey(key))
        {
            policyDictionary[key].isActive = true;
            UpdateParticleCount();
            Debug.Log($"Policy '{policyName}' activated");
        }
        else
        {
            Debug.LogWarning($"Policy '{policyName}' not found!");
        }
    }
    
    public void DeactivatePolicy(string policyName)
    {
        string key = policyName.ToLower();
        if (policyDictionary.ContainsKey(key))
        {
            policyDictionary[key].isActive = false;
            UpdateParticleCount();
            Debug.Log($"Policy '{policyName}' deactivated");
        }
        else
        {
            Debug.LogWarning($"Policy '{policyName}' not found!");
        }
    }
    
    public void AddCustomPolicy(string policyName, float reductionPercent)
    {
        PolicyEffect newPolicy = new PolicyEffect 
        { 
            policyName = policyName, 
            particleReductionPercent = Mathf.Clamp01(reductionPercent),
            isActive = false 
        };
        
        policies.Add(newPolicy);
        policyDictionary[policyName.ToLower()] = newPolicy;
        Debug.Log($"Added custom policy '{policyName}' with {reductionPercent * 100}% reduction");
    }
    
  void UpdateParticleCount()
{
    if (airParticles == null) return;
    
    float totalReduction = 0f;
    int activePolicies = 0;
    
    // Calculate total reduction from all active policies
    foreach (var policy in policies)
    {
        if (policy.isActive)
        {
            totalReduction += policy.particleReductionPercent;
            activePolicies++;
        }
    }
    
    // Cap total reduction at 90% to maintain some visual effect
    totalReduction = Mathf.Clamp(totalReduction, 0f, 0.9f);
    
    // Calculate new particle count (minimum 10% of original)
    currentParticleCount = Mathf.RoundToInt(baseParticleCount * (1f - totalReduction));
    
    Debug.Log($"Target particle count: {currentParticleCount} (Base: {baseParticleCount}, Reduction: {totalReduction * 100}%)");
    
    // Clear all existing particles
    airParticles.Clear();
    
    // Emit the new number of particles
    airParticles.Emit(currentParticleCount);
    
    Debug.Log($"Emitted {currentParticleCount} particles");
}
    
    // Public getters for UI or other systems
    public int GetCurrentParticleCount() => currentParticleCount;
    public int GetBaseParticleCount() => baseParticleCount;
    public float GetCurrentReductionPercent() => (baseParticleCount - currentParticleCount) / (float)baseParticleCount * 100f;
    public List<string> GetActivePolicies()
    {
        List<string> active = new List<string>();
        foreach (var policy in policies)
        {
            if (policy.isActive)
                active.Add(policy.policyName);
        }
        return active;
    }
    
    // Method to reset all policies
    public void ResetAllPolicies()
    {
        foreach (var policy in policies)
        {
            policy.isActive = false;
        }
        
        // Clear all existing particles
        airParticles.Clear();
        
        // Emit all original particles again
        airParticles.Emit(baseParticleCount);
        currentParticleCount = baseParticleCount;
        
        Debug.Log("All policies reset - particles restored to original count");
    }
    
    // Add debug method to test if UI is working
    public void TestPolicyToggle()
    {
        Debug.Log("TestPolicyToggle called!");
        TogglePolicy("Clean Air Act");
    }

    // Example usage methods (can be called from buttons, triggers, etc.)
    void Update()
    {
        if (airParticles != null)
        {
            Debug.Log($"Current particles on screen: {airParticles.particleCount}");
        }
    }
}

