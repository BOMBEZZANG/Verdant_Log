# Unity Setup Guide - TextMeshPro Version

## Updated UI Setup with TextMeshPro

All UI scripts have been updated to use **TextMeshPro** instead of legacy Text components for better text rendering and performance.

## Step-by-Step Setup

### 1. Initial Setup
1. Create new **2D Core** Unity project
2. Copy Assets folder from implementation
3. **Window → TextMeshPro → Import TMP Essential Resources**
4. Wait for compilation to complete

### 2. Create ScriptableObject Assets
**Menu: Verdant Log → Create Test Assets → Create All Test Assets**

### 3. Scene Setup

#### A. Create Main Scene
1. **File → New Scene → 2D**
2. Save as `MainGame.unity`

#### B. Core GameObjects
1. Create Empty GameObject → "GameManager"
   - Add `GameManager` component
   - Check "Auto Initialize Systems"

2. **GameObject → UI → Canvas**
   - **Render Mode**: Screen Space - Overlay
   - **UI Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1920x1080

### 4. UI Prefab Creation

#### A. Item Slot Prefab
```
ItemSlot (Panel)
├── Icon (Image)
└── NameText (TextMeshPro - UI)
```

**Setup:**
1. **GameObject → UI → Panel** (under Canvas)
2. Rename to "ItemSlot"
3. Size: 80x80
4. Add child **Image** → "Icon"
5. Add child **GameObject → UI → Text - TextMeshPro** → "NameText"
6. **Drag to Assets/Prefabs/UI/** → Delete from scene

#### B. Notification Prefab
```
Notification (Panel)
├── Background (Image)
└── Message (TextMeshPro - UI)
```

**Setup:**
1. **GameObject → UI → Panel** (under Canvas)
2. Rename to "Notification"
3. Size: 300x60
4. Add **TextMeshPro - UI** component
5. Add **CanvasGroup** component
6. **Drag to Assets/Prefabs/UI/** → Delete from scene

#### C. Encyclopedia Entry Prefab
```
EncyclopediaEntry (Button)
└── PlantName (TextMeshPro - UI)
```

**Setup:**
1. **GameObject → UI → Button - TextMeshPro** (under Canvas)
2. Rename to "EncyclopediaEntry"
3. The button automatically has TextMeshPro text
4. **Drag to Assets/Prefabs/UI/** → Delete from scene

### 5. Main UI Panel Structure

#### A. HUD Panel (Always Visible)
```
Canvas
└── HUD Panel (Panel)
    ├── Top Bar (Empty GameObject)
    │   ├── Level Text (TextMeshPro - UI)
    │   ├── EXP Text (TextMeshPro - UI)
    │   ├── EXP Bar (Slider)
    │   ├── Time Text (TextMeshPro - UI)
    │   └── Day/Night Text (TextMeshPro - UI)
    └── Notification Container (Empty GameObject)
```

**Setup Top Bar Layout:**
1. Add **Horizontal Layout Group** to Top Bar
2. Settings:
   - **Padding**: 20 all sides
   - **Spacing**: 15
   - **Child Alignment**: Middle Left
   - **Child Force Expand**: Width ✓

#### B. Inventory Panel (Initially Disabled)
```
Canvas
└── Inventory Panel (Panel)
    ├── Background (Image)
    ├── Header (Empty GameObject)
    │   ├── Title (TextMeshPro - UI) "Inventory"
    │   ├── Slots Text (TextMeshPro - UI) "Slots: 0/30"
    │   └── Close Button (Button - TextMeshPro)
    └── Items Container (Empty GameObject + Grid Layout Group)
```

**Grid Layout Settings:**
- **Cell Size**: 80x80
- **Spacing**: 5x5
- **Constraint**: Fixed Column Count (6)
- **Padding**: 10 all sides

#### C. Encyclopedia Panel (Initially Disabled)
```
Canvas
└── Encyclopedia Panel (Panel)
    ├── Background (Image)
    ├── Header (Empty GameObject)
    │   ├── Title (TextMeshPro - UI) "Encyclopedia"
    │   ├── Completion Text (TextMeshPro - UI) "0/0"
    │   └── Close Button (Button - TextMeshPro)
    ├── Content (Empty GameObject + Horizontal Layout Group)
    │   ├── Left Panel (Panel)
    │   │   └── Entries Container (Empty + Vertical Layout Group)
    │   └── Right Panel (Panel)
    │       ├── Plant Name (TextMeshPro - UI)
    │       ├── Plant Image (Image)
    │       └── Plant Description (TextMeshPro - UI)
```

### 6. Component Assignment

#### A. PlayerStatsUI Component
**Add to HUD Panel**
- **Level Text**: Top Bar → Level Text
- **Exp Text**: Top Bar → EXP Text  
- **Exp Bar**: Top Bar → EXP Bar
- **Level Up Effect**: Optional GameObject for effects

#### B. TimeUI Component
**Add to HUD Panel**
- **Time Text**: Top Bar → Time Text
- **Day Night Text**: Top Bar → Day/Night Text
- **Day Night Icon**: Optional Image component
- **Day/Night Icons**: Assign sprite assets

#### C. NotificationUI Component
**Add to HUD Panel**
- **Notification Prefab**: Assets/Prefabs/UI/Notification
- **Notification Container**: HUD Panel → Notification Container
- **Notification Duration**: 3
- **Fade Out Duration**: 0.5

#### D. InventoryUI Component
**Add to Inventory Panel**
- **Inventory Panel**: The panel itself
- **Items Container**: Items Container GameObject
- **Item Slot Prefab**: Assets/Prefabs/UI/ItemSlot
- **Slots Text**: Header → Slots Text

#### E. EncyclopediaUI Component
**Add to Encyclopedia Panel**
- **Encyclopedia Panel**: The panel itself
- **Entries Container**: Left Panel → Entries Container
- **Entry Prefab**: Assets/Prefabs/UI/EncyclopediaEntry
- **Completion Text**: Header → Completion Text
- **Selected Plant Name**: Right Panel → Plant Name
- **Selected Plant Description**: Right Panel → Plant Description
- **Selected Plant Image**: Right Panel → Plant Image

### 7. TextMeshPro Settings

#### Recommended TextMeshPro Settings:
**For UI Text:**
- **Font**: Use TMP default font or import custom fonts
- **Font Size**: 18-24 for body text, 28-36 for headers
- **Auto Size**: Enable for dynamic sizing
- **Overflow**: Ellipsis for long text
- **Alignment**: Center for buttons, Left for descriptions

**For Inventory Items:**
- **Font Size**: 14-16
- **Auto Size**: Enable
- **Best Fit**: Min 8, Max 18

**For Headers:**
- **Font Size**: 24-32
- **Font Style**: Bold
- **Color**: White or theme color

### 8. Testing Setup

#### A. Add Test Manager
1. Create Empty GameObject: "TestManager"
2. Add `TestSystemIntegration` component
3. Enable "Enable Test Keys"

#### B. Final Scene Hierarchy
```
Main Camera
GameManager
TestManager
EventSystem
Canvas
├── HUD Panel (Active)
│   ├── PlayerStatsUI (Component)
│   ├── TimeUI (Component)
│   ├── NotificationUI (Component)
│   └── UI Elements...
├── Inventory Panel (Inactive)
│   ├── InventoryUI (Component)
│   └── UI Elements...
└── Encyclopedia Panel (Inactive)
    ├── EncyclopediaUI (Component)
    └── UI Elements...
```

### 9. TextMeshPro Advantages

**Why TextMeshPro:**
- **Better Performance**: More efficient text rendering
- **Superior Quality**: Crisp text at any resolution
- **Rich Text Support**: HTML-like tags for styling
- **Emoji Support**: Built-in emoji and special characters
- **Gradients & Effects**: Built-in text effects
- **Mobile Optimized**: Better performance on mobile devices

**Rich Text Examples:**
```csharp
// In code, you can use rich text tags:
text.text = "<color=green>Level Up!</color> You are now <b>Level 5</b>";
text.text = "EXP: <color=yellow>150</color>/<color=white>200</color>";
text.text = "New plant discovered: <i>Moonflower</i>";
```

### 10. Testing

**Test Keys:**
- **1-3**: Add seeds/materials
- **4-6**: Test cultivation
- **7**: Add 100 EXP
- **8**: Check zones
- **I**: Toggle inventory
- **L**: Toggle encyclopedia
- **T**: Toggle day/night

### 11. Troubleshooting

**Common Issues:**

**TextMeshPro not found:**
- Import TMP Essential Resources
- Check using TMPro; statement in scripts

**Text not displaying:**
- Verify TextMeshPro - UI components (not 3D Text)
- Check Canvas settings
- Ensure proper parent-child hierarchy

**Layout issues:**
- Use Content Size Fitter for dynamic sizing
- Check Layout Group settings
- Verify anchor settings

**Performance:**
- Use TextMeshPro - UI for UI elements
- Avoid frequent text updates in Update()
- Cache TextMeshPro references

This setup provides a modern, high-performance UI system using TextMeshPro for your Verdant Log game!