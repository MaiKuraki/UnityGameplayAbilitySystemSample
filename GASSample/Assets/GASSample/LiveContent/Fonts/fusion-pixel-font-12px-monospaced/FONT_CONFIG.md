# Unity TextMeshPro Font Asset Configuration Checklist

This document records the generation parameters and usage guidelines for the core font assets used in this project to ensure consistency and maintainability.

---

## 1. Primary Font: Fusion Pixel (Simplified Chinese + Latin)

- **Font Name**: `fusion-pixel-12px-monospaced-zh_hans`
- **Source File**: `fusion-pixel-12px-monospaced-zh_hans.ttf`
- **Final Asset Name**: `fusion-pixel-12px-monospaced-zh_hans_RASTER_HINTED.asset`

### Core Generation Parameters (`Font Asset Creator`)

| Parameter             | Recommended Value                 | Notes / Reason                                                       |
| :-------------------- | :-------------------------------- | :------------------------------------------------------------------- |
| `Sampling Point Size` | `12`                              | **MUST** match the font's native design size to ensure 1:1 pixel accuracy. |
| `Padding`             | **Calculate as needed** (see below) | Reserves space for effects and prevents rendering artifacts. Use `1` for no effects. |
| `Packing Method`      | `Optimum`                         | Maximizes space efficiency to reduce memory footprint and final build size.  |
| `Atlas Resolution`    | `4096x4096`                       | Adjust based on character set size, aiming to fit all glyphs on a single page. |
| `Character Set`       | `Characters From File`            | Use a `.txt` file to manage the character set for easy maintenance.   |
| `Render Mode`         | `RASTER_HINTED`                   | The **only correct choice** for pixel fonts; preserves sharp edges and disables anti-aliasing. |

### Padding Calculation Rules (For Effect Support)

**Formula**: `Padding ≥ Outline Thickness + |Max Shadow Offset| + Shadow Softness`

- **Scenario A: No Effects / Simple Hard Shadow**
  - **Recommended Value**: `1` or `2`
- **Scenario B: 2px Outline + (4,-4) Shadow Offset / 3px Softness**
  - **Calculation**: `2 + 4 + 3 = 9`
  - **Recommended Value**: `10` (to have a slight margin of safety)

### Fallback Chain

The fallback list for this font asset, used to support other languages and symbols.

1.  `fusion-pixel-12px-monospaced-zh_hant_RASTER_HINTED` (Traditional Chinese)
2.  `fusion-pixel-12px-monospaced-ja_RASTER_HINTED.asset` (Japanese)
3.  `fusion-pixel-12px-monospaced-latin_RASTER_HINTED.asset` (Latin)
4.  `fusion-pixel-12px-monospaced-ko_RASTER_HINTED` (Korean)

---

## 2. Usage Guidelines in Scene (for the `TextMeshProUGUI` Component)

| Parameter             | Recommended Value                 | Notes / Reason                                                       |
| :-------------------- | :-------------------------------- | :------------------------------------------------------------------- |
| **`Font Asset`** | `fusion-pixel-12px-monospaced-zh_hans_RASTER_HINTED`       | Always use the primary font asset we generated.                      |
| **`Font Size`** | `12` (or `24`, `36`, `48`...)     | **MUST** be an integer multiple of `12` to ensure clean, blocky pixel scaling. |
| **`Character Spacing`** | `0` (default), adjust as needed | Used to fine-tune visual spacing, especially to improve readability after adding an outline. |