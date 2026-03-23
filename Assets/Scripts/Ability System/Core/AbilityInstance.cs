using System;
using UnityEngine;

/// <summary>
/// Runtime wrapper for a SkillDefinition.
/// Tracks timers, state, and owns the pre-instantiated effect GameObject.
/// </summary>
public class AbilityInstance
{
    public AbilityDefinition Definition { get; }
    public SkillState State { get; private set; } = SkillState.Idle;
    public float CooldownRemaining { get; private set; }
    public float CastTimeRemaining { get; private set; }
    public float DurationRemaining { get; private set; }
    public GameObject OwnerObject { get; }
    public Animator OwnerAnimator { get; }

    /// <summary>
    /// Pre-instantiated effect GameObject (e.g. shield mesh/VFX). Null if not applicable.
    /// </summary>
    public GameObject EffectInstance { get; internal set; }

    public event Action<AbilityInstance, SkillState> OnStateChanged;
    public event Action<AbilityInstance> OnCooldownTick;

    private AbilityBehaviour abilityBehaviour;

    public AbilityInstance(AbilityDefinition definition, GameObject owner, Animator animator)
    {
        Definition = definition;
        OwnerObject = owner;
        OwnerAnimator = animator;
        abilityBehaviour = definition.BehaviourOverride;
    }

    public bool CanActivate() => State == SkillState.Idle && CooldownRemaining <= 0f;

    public void TryActivate()
    {
        if (!CanActivate()) return;

        if (Definition.CastTime > 0f)
        {
            CastTimeRemaining = Definition.CastTime;
            ChangeState(SkillState.Preparing);
            abilityBehaviour?.OnPrepare(this);
        }
        else
        {
            Activate();
        }
    }

    public void Cancel()
    {
        if (State == SkillState.Idle || State == SkillState.Cooldown) return;
        ChangeState(SkillState.Cancelled);
        abilityBehaviour?.OnCancel(this);
        StartCooldown();
    }

    public void Tick(float deltaTime)
    {
        switch (State)
        {
            case SkillState.Preparing:
                CastTimeRemaining -= deltaTime;
                if (CastTimeRemaining <= 0f)
                    Activate();
                break;

            case SkillState.Active:
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

            case SkillState.Cooldown:
                CooldownRemaining -= deltaTime;
                OnCooldownTick?.Invoke(this);
                if (CooldownRemaining <= 0f)
                {
                    CooldownRemaining = 0f;
                    ChangeState(SkillState.Idle);
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
        if (State == SkillState.Active)
            Deactivate();
    }

    internal void RunSetup() => abilityBehaviour?.OnSetup(this);
    internal void RunTeardown() => abilityBehaviour?.OnTeardown(this);

    private void Activate()
    {
        DurationRemaining = Definition.Duration;
        ChangeState(SkillState.Active);
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
            ChangeState(SkillState.Cooldown);
            abilityBehaviour?.OnCooldownStart(this);
        }
        else
        {
            ChangeState(SkillState.Idle);
        }
    }

    private void ChangeState(SkillState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(this, newState);
    }
}
