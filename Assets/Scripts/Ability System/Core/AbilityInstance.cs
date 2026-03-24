using System;
using UnityEngine;

/// <summary>
/// Runtime wrapper for a SkillDefinition.
/// Tracks timers, state, and owns the pre-instantiated effect GameObject.
/// </summary>
public class AbilityInstance
{
    public AbilityDefinition Definition { get; }
    public AbilityState State { get; private set; } = AbilityState.Idle;
    public float CooldownRemaining { get; private set; }
    public float CastTimeRemaining { get; private set; }
    public float DurationRemaining { get; private set; }
    public GameObject OwnerObject { get; }
    public Animator OwnerAnimator { get; }

    /// <summary>
    /// Pre-instantiated effect GameObject (e.g. shield mesh/VFX). Null if not applicable.
    /// </summary>
    public GameObject EffectInstance { get; internal set; }

    public event Action<AbilityInstance, AbilityState> OnStateChanged;
    public event Action<AbilityInstance> OnCooldownTick;

    private AbilityBehaviour abilityBehaviour;

    public AbilityInstance(AbilityDefinition definition, GameObject owner, Animator animator)
    {
        Definition = definition;
        OwnerObject = owner;
        OwnerAnimator = animator;
        abilityBehaviour = definition.BehaviourOverride;
    }

    public bool CanActivate() => State == AbilityState.Idle && CooldownRemaining <= 0f;

    public void TryActivate()
    {
        if (!CanActivate()) return;

        if (Definition.CastTime > 0f)
        {
            CastTimeRemaining = Definition.CastTime;
            ChangeState(AbilityState.Preparing);
            abilityBehaviour?.OnPrepare(this);
        }
        else
        {
            Activate();
        }
    }

    public void Cancel()
    {
        if (State == AbilityState.Idle || State == AbilityState.Cooldown) return;
        ChangeState(AbilityState.Cancelled);
        abilityBehaviour?.OnCancel(this);
        StartCooldown();
    }

    public void Tick(float deltaTime)
    {
        switch (State)
        {
            case AbilityState.Preparing:
                CastTimeRemaining -= deltaTime;
                if (CastTimeRemaining <= 0f)
                    Activate();
                break;

            case AbilityState.Active:
                if (Definition.Duration > 0f)
                {
                    DurationRemaining -= deltaTime;
                    abilityBehaviour?.OnUpdateActive(this);
                    if (DurationRemaining <= 0f)
                        Deactivate();
                }
                else
                {
                    abilityBehaviour?.OnUpdateActive(this);
                }
                break;

            case AbilityState.Cooldown:
                CooldownRemaining -= deltaTime;
                OnCooldownTick?.Invoke(this);
                if (CooldownRemaining <= 0f)
                {
                    CooldownRemaining = 0f;
                    ChangeState(AbilityState.Idle);
                    abilityBehaviour?.OnCooldownEnd(this);
                }
                break;
        }
    }

    /// <summary>
    /// For Toggle / OnHold skills: manually deactivate while still active.
    /// </summary>
    public void ForceDeactivate()
    {
        if (State == AbilityState.Active)
            Deactivate();
    }

    internal void RunSetup() => abilityBehaviour?.OnSetup(this);
    internal void RunTeardown() => abilityBehaviour?.OnTeardown(this);

    private void Activate()
    {
        DurationRemaining = Definition.Duration;
        ChangeState(AbilityState.Active);
        abilityBehaviour?.OnActivate(this);

        OwnerAnimator.runtimeAnimatorController = Definition.AnimationOverride;
        OwnerAnimator.Play("Ability", 0, 0);

        if (Definition.Duration <= 0f)
            Deactivate();
    }

    private void Deactivate()
    {
        abilityBehaviour?.OnDeactivate(this);
        StartCooldown();
    }

    private void StartCooldown()
    {
        if (Definition.Cooldown > 0f)
        {
            CooldownRemaining = Definition.Cooldown;
            ChangeState(AbilityState.Cooldown);
            abilityBehaviour?.OnCooldownStart(this);
        }
        else
        {
            ChangeState(AbilityState.Idle);
        }
    }

    private void ChangeState(AbilityState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(this, newState);
    }
}
