# myYardSale JavaScript & Accessibility Audit Report

**Date:** July 21, 2026  
**Scope:** All JavaScript files, inline scripts, and accessibility patterns

---

## JAVASCRIPT AUDIT

### 1. Module Structure

**Score:** 82/100 — Good modular structure with 7 named modules.

### 2. Duplicate Functionality

#### Finding 1: Duplicate Live-Search Filter Logic
- **Files:** `wwwroot/js/site.js` (lines 314-338) AND `Views/Home/Index.cshtml`
- **Severity:** High
- **Description:** The exact same live-search filter logic exists in both site.js and an inline `<script>` block in Index.cshtml. The inline script runs *after* site.js, attaching duplicate event handlers.
- **Impact:** Double submissions on every change event, duplicate debounce timers.
- **Status:** ✅ Fixed (removed inline `<script>` block)

#### Finding 2: Stagger Animation Logic Duplicated
- **Files:** `wwwroot/js/site.js` AND `Views/Home/MyListings.cshtml`
- **Severity:** Medium
- **Description:** Stagger card animation logic exists in both site.js (Animations module) and an inline script in MyListings.cshtml.
- **Status:** ✅ Fixed (removed inline script from MyListings.cshtml)

### 3. Event Handling Patterns

#### Finding 3: Form Submit Handler Not Using Event Delegation
- **File:** `wwwroot/js/site.js` (Forms module)
- **Severity:** Low
- **Description:** `document.querySelectorAll('form').forEach(...)` attaches handlers individually. Forms added dynamically (e.g., via AJAX) won't get the handler.
- **Recommendation:** Use event delegation: `document.addEventListener('submit', function(e) { ... })`.

#### Finding 4: SweetAlert2 Toast Duplication
- **File:** `wwwroot/js/site.js` (Alerts module)
- **Severity:** Medium
- **Description:** The Alerts module converts `.alert` elements into SweetAlert2 toasts, but if multiple alerts exist, they all get converted. The `.alert` elements remain visible on the page while toasts are shown, causing visual duplication.
- **Recommendation:** Hide the `.alert` elements after converting: `alert.style.display = 'none'`.

### 4. Progressive Enhancement

#### Finding 5: No JavaScript Fail-Safe
- **File:** `wwwroot/js/site.js` (all modules)
- **Severity:** Low
- **Description:** If JavaScript fails to load (network error, CDN outage, script blocking), filter forms still submit via normal GET/POST. This is acceptable for most features but some UI enhancements (toasts, animations) won't degrade gracefully.
- **Recommendation:** Ensure all core functionality works without JS; use `<noscript>` tags for critical warnings.

### 5. Error Handling

#### Finding 6: No Error Handling in Fetch/Async Calls
- **File:** `wwwroot/js/site.js`
- **Severity:** Medium
- **Description:** The site.js uses no fetch/async API calls currently, so error handling is minimal. However, the `Images.initLightbox()` method assumes `bootstrap.Modal` exists without a check for Bootstrap availability.

### 6. Performance

#### Finding 7: SweetAlert2 Loaded Globally
- **File:** `_Layout.cshtml`
- **Severity:** Medium
- **Description:** SweetAlert2 CDN script (22KB) loaded on every page but only used for alert-toast conversion, which only fires when `.alert` elements exist.
- **Recommendation:** Lazy-load SweetAlert2 only when needed, or inline the toast functionality.

#### Finding 8: No Debounce on Scroll Handlers
- **File:** `wwwroot/js/site.js` (Navigation module)
- **Severity:** Low
- **Description:** Scroll handlers for sticky header and back-to-top button fire on every scroll event without debouncing.
- **Recommendation:** Add `{ passive: true }` option (already present) and consider `requestAnimationFrame` for scroll handlers.

---

## ACCESSIBILITY AUDIT

### Color Contrast

#### Finding A1: `.text-muted` on Light Backgrounds
- **Files:** All views
- **Severity:** High
- **Description:** `.text-muted` uses `var(--color-text-secondary)` (#475569) which has a contrast ratio of approximately 4.5:1 on white backgrounds — meets WCAG AA for normal text but is very close to the minimum threshold.
- **Recommendation:** Ensure text-muted is never used for small text (<14px) where WCAG AA requires 4.5:1.

### Heading Hierarchy

#### Finding A2: Skipped Heading Levels
- **Files:** `Views/Home/Privacy.cshtml`
- **Severity:** Medium
- **Description:** Privacy page uses `<h5>` after `<h1>` without `<h2>`, `<h3>`, `<h4>` in between. Screen readers navigate by heading level and expect sequential hierarchy.
- **Recommendation:** Use appropriate heading levels: `<h2>` for sections instead of `<h5>`.

### Form Labels

#### Finding A3: All Forms Have Labels
- **Severity:** ✅ Pass
- **All forms use proper `<label asp-for="...">` elements with correct `for` attributes.**

### Alt Text

#### Finding A4: All Images Have Alt Text
- **Severity:** ✅ Pass
- **All `<img>` tags include `alt` attributes.**

### Keyboard Navigation

#### Finding A5: Focus Management on Modals
- **File:** `Views/Home/Details.cshtml`
- **Severity:** Medium
- **Description:** The lightbox modal does not trap focus. When the modal is open, pressing Tab can move focus to elements behind the modal. Bootstrap's modal should handle this, but the lightbox uses custom implementation.
- **Recommendation:** Ensure Bootstrap's modal focus management is properly initialized.

### ARIA Attributes

#### Finding A6: Missing `aria-label` on Icon-Only Buttons
- **Files:** Multiple views
- **Severity:** Medium
- **Description:** Several icon-only buttons (delete, edit, close) lack `aria-label` attributes. Screen readers will read the icon as "button" without context.
- **Recommendation:** Add `aria-label` to all icon-only buttons. Example: `<button aria-label="Remove item">`

#### Finding A7: Navigation Has ARIA Labels
- **Severity:** ✅ Pass
- **The main navigation has `aria-label="Main navigation"` and the skip link works correctly.**

### Focus Indicators

#### Finding A8: Custom Focus Styles Present
- **Severity:** ✅ Pass
- **Custom focus styles are defined with `outline: 2px solid var(--color-primary)` and `outline-offset: 2px`.**

---

## Summary

| Category | Score |
|----------|-------|
| JavaScript Quality | 72/100 |
| Accessibility | 74/100 |
| Performance | 70/100 |
| Progressive Enhancement | 65/100 |

### Fixed Issues
1. ✅ Removed duplicate inline JS from Index.cshtml
2. ✅ Removed duplicate inline JS from MyListings.cshtml

### Issues to Address
1. ⏳ Lazy-load SweetAlert2
2. ⏳ Add event delegation for form submit handlers
3. ⏳ Hide `.alert` elements after converting to SweetAlert2 toasts
4. ⏳ Add `aria-label` to icon-only buttons
5. ⏳ Improve heading hierarchy on Privacy page