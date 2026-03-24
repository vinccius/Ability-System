# Ability System for Unity

---

## Folder Structure

```
Ability System/
├── Core/
|   ├── AbilityBehaviour.cs       # Abstract base for custom skill logic
│   ├── AbilityDefinition.cs      # ScriptableObject – static data per skill
│   └── AbilityInstance.cs        # Runtime state machine per skill
├── Enumerators/
|   ├── AbilityTriggerType.cs
|   ├── InputEventType.cs
|   └── AbilityState.cs
├── Input/
│   └── AbilityInputHandler.cs    # Abstracts New/Legacy Input System
├── Structs/
|   └── AbilityInputHandler.cs
├── UI/
│   ├── AbilityUI.cs              # Spawns buttons automatically
│   └── AbilityUIButton.cs        # Controls one button's visual state
├── AbilityController.cs          # Main MonoBehaviour – attach to Player
└── AbilitySystemDefinition.asmdef
```

---

## Quick Setup (5 steps)

### 1. Input System
Open `AbilityInputHandler.cs` and `AbilityController.cs`.
- **New Input System**: uncomment `#define USE_NEW_INPUT_SYSTEM` at the top of both files.
- **Legacy**: leave as-is. Fill `legacyKeyCode` in each SkillDefinition.

### 2. Create a AbilityDefinition
`Right-click in Project > Create > Ability System > Ability Definition`

Fill in:
| Field | Description |
|---|---|
| `ID` | Unique string ID (e.g. "shield") |
| `DisplayName` | Shown on UI button (It is not being used yet.) |
| `Description` | Displays a description of the skill in the UI. (It is not being used yet.) |
| `InputActionName` | Action name (New Input) or any string matching your key map |
| `LegacyInputKeyCode` | Fallback key (Legacy Input) |
| `TriggerType` | OnPress / OnHold / OnRelease / Toggle |
| `CastTime` | Seconds before skill activates (0 = instant) |
| `Duration` | How long the skill stays active (0 = instant) |
| `Cooldown` | Seconds before skill can be used again |
| `AnimationOverride` | Overwrites the animation clip in the player's AnimatorController. |
| `EffectPrefab` | VFX/mesh to pre-instantiate on startup |
| `BehaviourOverride` | Your custom AbilityBehaviour ScriptableObject |

### 3. Create a AbilityBehaviour
Create a class that inherits from the AbilityBehavior.cs class and override the methods you need:

```csharp
[CreateAssetMenu(fileName = "Fireball Behaviour", menuName = "Ability System/Behaviours/Fireball")]
public class FireballAbilityBehaviour : AbilityBehaviour
{
  public override void OnSetup(SkillInstance skill)    { } // called once at start
  public override void OnActivate(SkillInstance skill) { } // skill fires
  public override void OnDeactivate(SkillInstance skill){ } // skill ends
  public override void OnCancel(SkillInstance skill)   { } // interrupted
}
```

After creating all the skill behavior, create the scriptable object:
`Right-click > Create > Ability System > Behaviours > [name of ability]`

You can access the pre-instantiated effect via `ability.EffectInstance`.

### 4. Configure AbilityController
Attach `AbilityController` to your Player GameObject.
- Assign `AbilityDefinition` assets to the `Skill Definitions` list.
- Assign the `Animator` component reference.
- Assign an `Effect Spawn Point` transform (optional, defaults to player root).
- If using New Input System: assign the `PlayerInput` component.

### 5. Setup the UI
- Create a Canvas with a horizontal/vertical Layout Group panel.
- Attach `AbilityUI` to any persistent GameObject (e.g. UIManager).
- Assign the `Button Container` (the layout panel).
- Create a `AbilityUIButton` prefab with this hierarchy:
  ```
  AbilityButton [Image background] [SkillButtonUI component]
  ├── Icon [Image]
  ├── CooldownOverlay [Image, ImageType=Filled, FillMethod=Radial360]
  ├── KeyLabel [TextMeshProUGUI]
  └── NameLabel [TextMeshProUGUI]
  ```
- Assign the prefab to `AbilityUI._abilityButtonPrefab`.

When you add a new AbilityDefinition to AbilityController at runtime or in the Inspector, a button appears automatically.

---

## Animator Integration

The system overwrites the player's animation directly in the Animator. Each skill has its own animation, which replaces the AnimationClip in the AnimatorController.

<img width="937" height="566" alt="Captura de tela 2026-03-23 204904" src="https://github.com/user-attachments/assets/a279a959-a8f8-488c-976b-a4f571d88e25" />

Create an AnimatorOverrideController:
`Right-click > Create > Animation > Animator Override Controller`

Replace the Skill Animation clip with the Animation Clip chosen for your ability.

<img width="475" height="154" alt="Captura de tela 2026-03-23 213519" src="https://github.com/user-attachments/assets/74ca3127-0bd1-4684-b03b-56cc0c595c7b" />

Enter the Animator Override Controller in the `Animation Override` field in your `AbilityDefinition`.
```
