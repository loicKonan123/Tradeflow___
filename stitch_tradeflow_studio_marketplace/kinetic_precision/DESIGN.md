---
name: Kinetic Precision
colors:
  surface: '#031427'
  surface-dim: '#031427'
  surface-bright: '#2a3a4f'
  surface-container-lowest: '#000f21'
  surface-container-low: '#0b1c30'
  surface-container: '#102034'
  surface-container-high: '#1b2b3f'
  surface-container-highest: '#26364a'
  on-surface: '#d3e4fe'
  on-surface-variant: '#c6c6cd'
  inverse-surface: '#d3e4fe'
  inverse-on-surface: '#213145'
  outline: '#909097'
  outline-variant: '#45464d'
  surface-tint: '#bec6e0'
  primary: '#bec6e0'
  on-primary: '#283044'
  primary-container: '#0f172a'
  on-primary-container: '#798098'
  inverse-primary: '#565e74'
  secondary: '#adc6ff'
  on-secondary: '#002e6a'
  secondary-container: '#0566d9'
  on-secondary-container: '#e6ecff'
  tertiary: '#4ae176'
  on-tertiary: '#003915'
  tertiary-container: '#001d07'
  on-tertiary-container: '#009542'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#dae2fd'
  primary-fixed-dim: '#bec6e0'
  on-primary-fixed: '#131b2e'
  on-primary-fixed-variant: '#3f465c'
  secondary-fixed: '#d8e2ff'
  secondary-fixed-dim: '#adc6ff'
  on-secondary-fixed: '#001a42'
  on-secondary-fixed-variant: '#004395'
  tertiary-fixed: '#6bff8f'
  tertiary-fixed-dim: '#4ae176'
  on-tertiary-fixed: '#002109'
  on-tertiary-fixed-variant: '#005321'
  background: '#031427'
  on-background: '#d3e4fe'
  surface-variant: '#26364a'
typography:
  h1:
    fontFamily: Space Grotesk
    fontSize: 48px
    fontWeight: '700'
    lineHeight: '1.1'
  h2:
    fontFamily: Space Grotesk
    fontSize: 32px
    fontWeight: '600'
    lineHeight: '1.2'
  data-lg:
    fontFamily: Space Grotesk
    fontSize: 24px
    fontWeight: '500'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  body-main:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.6'
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: '1.5'
  label-mono:
    fontFamily: Space Grotesk
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1'
    letterSpacing: 0.05em
spacing:
  base: 4px
  xs: 8px
  sm: 16px
  md: 24px
  lg: 40px
  xl: 64px
  gutter: 16px
  margin: 24px
---

## Brand & Style

The design system is engineered for high-stakes financial environments where speed and clarity are paramount. The aesthetic combines the sophistication of **Minimalism** with the depth of **Glassmorphism**, creating an interface that feels both institutional and cutting-edge. 

The personality is defined by "Elite Precision"—a digital workspace for professionals who require a calm, dark environment to process complex data. Visual cues are taken from aerospace instrumentation: sharp edges, thin lines, and luminescent accents that guide the eye without causing fatigue. Every element is designed to evoke a sense of absolute control and technological superiority.

## Colors

The palette of the design system is anchored in a deep "Midnight Slate" to maximize contrast for data visualization. 

- **Primary & Background:** The core UI uses deep navy and obsidian tones to reduce eye strain during long trading sessions.
- **Electric Blue (Tech):** Reserved for interactive states, primary actions, and "system-active" indicators.
- **Success Green:** A high-vibrancy neon utilized strictly for profitable metrics and positive growth indicators.
- **Danger Red:** A piercing red used for risk alerts and losses, ensuring immediate cognitive recognition.
- **Accents:** Subtle slate-700/800 tones are used for borders to maintain a "sharp" structural feel without the harshness of pure white.

## Typography

The design system employs a dual-font strategy to balance legibility with a technical "quant" aesthetic.

- **Inter** handles all primary UI text, navigation, and long-form content. Its neutral, systematic nature ensures the interface feels professional and unobtrusive.
- **Space Grotesk** is used for headlines, numerals, and technical data points. Its geometric, slightly futuristic construction provides the "tech" edge required for a trading platform, standing in for traditional monospaced fonts to offer better readability at scale.
- **Weight Usage:** Use Medium (500) for data values to ensure they "pop" against the dark background. Use Light (300/400) for labels to create a clear hierarchy.

## Layout & Spacing

The design system utilizes a **Fixed Grid** model for dashboard environments to ensure data density remains consistent across professional-grade monitors. 

- **Grid:** A 12-column layout with tight 16px gutters allows for modular "widgets" that can be rearranged without breaking the visual rhythm.
- **Rhythm:** An 8px linear scale governs all padding and margins, ensuring mathematical precision in alignment.
- **Density:** The system favors "High Density" configurations. Information is packed tightly—relying on color and typography rather than excessive white space—to give traders a comprehensive view of the market at a glance.

## Elevation & Depth

Depth in the design system is achieved through **Glassmorphism** and tonal layering rather than traditional drop shadows.

- **Surface Layers:** The base layer is the darkest. Elevated elements (cards, modals) use a slightly lighter slate hex with a 1px "inner glow" border (white at 10% opacity) to simulate a glass edge.
- **Backdrop Blur:** Modals and dropdowns must employ a `blur(12px)` effect on the background to maintain context while ensuring legibility of the foreground.
- **Gradients:** Use subtle linear gradients (top-to-bottom) on primary surfaces to simulate a light source from above, reinforcing the "tactile" feel of a high-end console.

## Shapes

To reinforce the feeling of "Precision," the design system utilizes **Sharp** edges. 

- **Corners:** 0px radius is the standard for all primary containers, cards, and input fields. This creates a rigorous, architectural look that maximizes screen real estate.
- **Interactive Elements:** Buttons may use a minimal 2px radius (Soft) only if necessary for accessibility or to distinguish them from static containers, but the preference is for right angles to maintain the "Elite" aesthetic.
- **Dividers:** Use 1px solid lines with 10-15% opacity to separate data rows, ensuring the grid is felt but not distracting.

## Components

### Buttons
Primary buttons use the Electric Blue background with white text. Secondary buttons are "Ghost" style—transparent backgrounds with a 1px Electric Blue border. All buttons should have a `hover` state that increases the brightness of the accent color.

### Data Visualization
Charts must use the neon Success Green and Danger Red for trend lines. Grid lines within charts should be kept to a minimum (Slate-800). Use Area charts with subtle gradient fills (10% opacity) for a premium look.

### Input Fields
Inputs are dark-filled containers (#020617) with sharp borders. The active/focus state is indicated by an Electric Blue 1px border and a subtle outer glow. Labels always sit above the field in "label-mono" typography.

### Cards & Modules
Widgets are the building blocks of the interface. Each card should have a "Header" section with a 1px bottom border. For critical data modules, use a "glass-fill" (semi-transparent surface) to separate them from the background.

### Status Chips
Chips are used for order types (Limit, Market, Stop). They should be small, capitalized, and use Space Grotesk for a "terminal" feel.