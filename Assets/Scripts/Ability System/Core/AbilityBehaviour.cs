using UnityEngine;

/// <summary>
/// Base class for custom Ability logic.
/// Subclass this to define what a Ability actually does at each state.
/// Assign the subclass as a ScriptableObject to SkillDefinition.behaviourOverride.
/// </summary>
public abstract class AbilityBehaviour : ScriptableObject
{
    /// <summary>
    /// Called once when the System initializes. Set up pooled objects, caches, etc.
    /// </summary>
    public virtual void OnSetup(AbilityInstance ability) { }

    /// <summary>
    /// Called when the Ability begins its cast/prepare phase.
    /// </summary>
    public virtual void OnPrepare(AbilityInstance ability) { }

    /// <summary>
    /// Called when the Ability becomes fully active (after cast time).
    /// </summary>
    public virtual void OnActivate(AbilityInstance ability) { }

    /// <summary>
    /// Called every frame while the skill is active.
    /// </summary>
    public virtual void OnUpdateActive(AbilityInstance ability) { }

    /// <summary>
    /// Called when the Ability's duration ends or it is manually deactivated.
    /// </summary>
    public virtual void OnDeactivate(AbilityInstance ability) { }
        
    /// <summary>
    /// Called when the Ability enters cooldown.
    /// </summary>
    public virtual void OnCooldownStart(AbilityInstance ability) { }

    /// <summary>
    /// Called when cooldown finishes.
    /// </summary>
    public virtual void OnCooldownEnd(AbilityInstance ability) { }

    /// <summary>
    /// Called if the skill is cancelled mid-execution.
    /// </summary>
    public virtual void OnCancel(AbilityInstance ability) { }

    /// <summary>
    /// Called when the system shuts down or Ability is unregistered.
    /// </summary>
    public virtual void OnTeardown(AbilityInstance ability) { }
}
