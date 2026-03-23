# SkillSystem for Unity

A modular, decoupled, input-agnostic skill system.

---

## Folder Structure

```
SkillSystem/
├── Core/
│   ├── SkillDefinition.cs      # ScriptableObject – static data per skill
│   ├── SkillBehaviour.cs       # Abstract base for custom skill logic
│   └── SkillInstance.cs        # Runtime state machine per skill
├── Input/
│   └── SkillInputHandler.cs    # Abstracts New/Legacy Input System
├── UI/
│   ├── SkillHUDManager.cs      # Spawns buttons automatically
│   └── SkillButtonUI.cs        # Controls one button's visual state
├── Behaviours/
│   ├── ShieldBehaviour.cs      # Example: toggle shield VFX + damage reduction
│   └── FireballBehaviour.cs    # Example: projectile pool
├── SkillController.cs          # Main MonoBehaviour – attach to Player
└── SkillSystem.asmdef
```

---

## Quick Setup (5 steps)

### 1. Input System
Open `SkillInputHandler.cs` and `SkillController.cs`.
- **New Input System**: uncomment `#define USE_NEW_INPUT_SYSTEM` at the top of both files.
- **Legacy**: leave as-is. Fill `legacyKeyCode` in each SkillDefinition.

### 2. Create a SkillDefinition
`Right-click in Project > Create > SkillSystem > SkillDefinition`

Fill in:
| Field | Description |
|---|---|
| `skillId` | Unique string ID (e.g. "shield") |
| `displayName` | Shown on UI button |
| `inputActionName` | Action name (New Input) or any string matching your key map |
| `legacyKeyCode` | Fallback key (Legacy Input) |
| `triggerType` | OnPress / OnHold / OnRelease / Toggle |
| `castTime` | Seconds before skill activates (0 = instant) |
| `duration` | How long the skill stays active (0 = instant) |
| `cooldown` | Seconds before skill can be used again |
| `animPrepare/Active/Cooldown/Cancel` | Animator parameter names |
| `effectPrefab` | VFX/mesh to pre-instantiate on startup |
| `behaviourOverride` | Your custom SkillBehaviour ScriptableObject |

### 3. Create a SkillBehaviour (optional)
`Right-click > Create > SkillSystem > Behaviours > Shield (or Fireball)`

Override the methods you need:
```csharp
public override void OnSetup(SkillInstance skill)    { } // called once at start
public override void OnActivate(SkillInstance skill) { } // skill fires
public override void OnDeactivate(SkillInstance skill){ } // skill ends
public override void OnCancel(SkillInstance skill)   { } // interrupted
```

Access the pre-instantiated effect via `skill.EffectInstance`.

### 4. Configure SkillController
Attach `SkillController` to your Player GameObject.
- Assign `SkillDefinition` assets to the `Skill Definitions` list.
- Assign the `Animator` component reference.
- Assign an `Effect Spawn Point` transform (optional, defaults to player root).
- If using New Input System: assign the `PlayerInput` component.

### 5. Setup the UI
- Create a Canvas with a horizontal/vertical Layout Group panel.
- Attach `SkillHUDManager` to any persistent GameObject (e.g. UIManager).
- Assign the `Button Container` (the layout panel).
- Create a `SkillButton` prefab with this hierarchy:
  ```
  SkillButton [Image bg] [SkillButtonUI component]
  ├── Icon [Image]
  ├── CooldownOverlay [Image, ImageType=Filled, FillMethod=Radial360]
  ├── KeyLabel [TextMeshProUGUI]
  └── NameLabel [TextMeshProUGUI]
  ```
- Assign the prefab to `SkillHUDManager.skillButtonPrefab`.

**That's it.** When you add a new SkillDefinition to SkillController at runtime or in the Inspector, a button appears automatically.

---

## Adding a New Skill at Runtime (e.g. from loot/unlock)

```csharp
[SerializeField] SkillDefinition newSkillDef;
SkillController controller = player.GetComponent<SkillController>();
controller.RegisterSkill(newSkillDef);
// SkillHUDManager hears SkillController.OnSkillRegistered and spawns the button automatically
```

---

## Animator Integration

The system maps animator parameters by name from SkillDefinition fields:
- `animPrepare` → played during cast time
- `animActive`  → played while skill is active
- `animCooldown`→ played when cooldown starts
- `animCancel`  → played on cancel

Both **Trigger** and **Bool** parameter types are supported automatically.

---

## Decoupling Architecture

| Concern | Mechanism |
|---|---|
| UI ↔ Skill logic | `static events` on SkillController (no direct reference) |
| Skill ↔ Health/Damage | `IDamageable` interface (no direct dependency) |
| Input ↔ Skills | `SkillInputHandler` event layer |
| Skill logic | `SkillBehaviour` ScriptableObjects (swappable data) |

---

## Extending

**New skill type example – Dash:**
1. Create `DashBehaviour : SkillBehaviour` ScriptableObject
2. In `OnActivate`, move the character via `CharacterController` or `Rigidbody`
3. In `OnSetup`, cache the `CharacterController` component

```csharp
[CreateAssetMenu(menuName = "SkillSystem/Behaviours/Dash")]
public class DashBehaviour : SkillBehaviour
{
    public float dashDistance = 5f;
    private CharacterController _cc;

    public override void OnSetup(SkillInstance skill)
        => _cc = skill.OwnerObject.GetComponent<CharacterController>();

    public override void OnActivate(SkillInstance skill)
        => _cc?.Move(skill.OwnerObject.transform.forward * dashDistance);
}
```
