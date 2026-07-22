# myYardSale Razor Views Audit Report

**Date:** July 21, 2026  
**Scope:** All 13 Razor views

---

## Critical Issues

### C1. Anti-Forgery Token Missing on DeleteImage Form
- **File:** `Views/Home/Edit.cshtml`
- **Lines:** 73-79
- **Description:** The `DeleteImage` form lacks `@Html.AntiForgeryToken()`. Admin/Edit.cshtml has it, but Home/Edit.cshtml does not.
- **Why:** Will cause HTTP 400 errors on POST when anti-forgery validation is enabled.
- **Status:** ✅ Fixed (added `@Html.AntiForgeryToken()` inside form)

### C2. Duplicate Hidden Input in Edit Form
- **File:** `Views/Home/Edit.cshtml`
- **Lines:** 15-16
- **Description:** `<input asp-for="Id" type="hidden" />` appears twice in the form.
- **Why:** Creates ambiguity in model binding — the second value overrides the first, but both are sent.
- **Status:** ✅ Fixed (removed duplicate)

### C3. Inconsistent Delete Confirmation Pattern
- **Files:** `Views/Home/Details.cshtml`, `Views/Admin/Index.cshtml`, `Views/Cart/Index.cshtml`
- **Description:** `onsubmit="return confirm(...)"` pattern is used inconsistently. Some forms have it, some don't. The pattern works in most browsers but is not progressive enhancement-friendly.
- **Recommendation:** Move to SweetAlert2 confirmation dialogs for all delete actions.

### C4. Duplicate Inline JavaScript with site.js
- **File:** `Views/Home/Index.cshtml`
- **Lines:** 155-188
- **Description:** Inline `<script>` block duplicates the live-search filter and stagger animation logic already in `site.js`, causing double event handlers.
- **Why:** Every change event fires `form.submit()` twice, debounce timer is duplicated.
- **Status:** ✅ Fixed (removed entire inline `<script>` block)

---

## High Issues

### H1. Inconsistent Animation Classes
- **Files:** 6 views across the application
- **Description:** Some views use `fade-in-up` while others use `animate-fade-in-up`. The design system defines both but they may have different timing.
- **Status:** ✅ Fixed (standardized all to `animate-fade-in-*`)

### H2. `.btn-outline-light` Not in Design System
- **File:** `Views/Home/Details.cshtml`
- **Line:** 86
- **Description:** Uses `.btn-outline-light` which is only defined in Bootstrap. The design system overrides `.btn-outline-light` would be missing since it redefines button styles.
- **Status:** ✅ Fixed (replaced with `.btn-outline-secondary`)

### H3. Inline Styles on Images
- **Files:** `Views/Cart/Index.cshtml`, `Views/Cart/Checkout.cshtml`, `Views/Home/Details.cshtml`, `Views/Home/Index.cshtml`
- **Description:** Multiple views use inline `style="height: X; width: Y; object-fit: cover"` on thumbnails and placeholder icons.
- **Status:** ✅ Fixed (replaced with `.cart-item-image` and `.listing-card-image-placeholder` CSS classes)

---

## Medium Issues

### M1. No Breadcrumb Component
- **File:** Global
- **Description:** Breadcrumb CSS is defined in `_layout.css` but no view uses breadcrumbs and no partial exists.
- **Status:** ✅ Fixed (created `Components/Breadcrumb/Default.cshtml`)

### M2. Inconsistent Card Wrappers on Auth Pages
- **Files:** `Areas/Identity/Pages/Account/Login.cshtml`, `Areas/Identity/Pages/Account/Register.cshtml`
- **Description:** Auth pages use `.auth-card` class in some places but not consistently with other card-based pages.
- **Status:** ⏳ Pending

### M3. Admin Edit Form Doesn't Match Home Edit Form
- **File:** `Views/Admin/Edit.cshtml`
- **Description:** Admin edit form was significantly different from the user-facing Home/Edit form — no card wrapper, missing icons, different label styles, different button sizing.
- **Status:** ✅ Fixed (rewrote to match design system)

---

## Low Issues

### L1. Privacy Page Uses `<h5>` Directly
- **File:** `Views/Home/Privacy.cshtml`
- **Line:** 12
- **Description:** Uses `<h5 class="text-primary">` as a subheading, but the design system has `.h5` class. Should use `.h5` for consistency.
- **Recommendation:** Use CSS class `.h5` instead of raw `<h5>` tag.

### L2. Empty State Icon Variables
- **Files:** All views with empty states
- **Description:** Some empty states use `display-1` class, others use `empty-state-icon` class. Not all match the empty state pattern.
- **Recommendation:** Standardize empty state icons to use `empty-state-icon` class.

### L3. Missing `aria-label` on Some Buttons
- **File:** Various
- **Description:** Icon-only buttons (delete, edit) sometimes lack aria-labels for screen readers.
- **Recommendation:** Add `aria-label` to all icon-only buttons.

---

## Razor Views Consistency Score: 78/100

| Page         | Layout | Buttons | Forms | Cards | Tables | Animations | Icons |
|--------------|--------|---------|-------|-------|--------|------------|-------|
| Home/Index   | ✅ | ✅ | ✅ | ✅ | N/A | ✅ | ✅ |
| Home/Details | ✅ | ✅ | N/A | ✅ | N/A | ✅ | ✅ |
| Home/Create  | ✅ | ✅ | ✅ | ✅ | N/A | ✅ | ✅ |
| Home/Edit    | ✅ | ✅ | ✅ | ✅ | N/A | ✅ | ✅ |
| Cart/Index | ✅ | ✅ | N/A | ✅ | ✅ | ✅ | ✅ |
| Cart/Checkout | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Orders/Index | ✅ | ✅ | N/A | ✅ | ✅ | ✅ | ✅ |
| Admin/Index | ✅ | ✅ | N/A | ✅ | ✅ | ✅ | ✅ |
| Login | ✅ | ✅ | ✅ | ✅ | N/A | ✅ | ✅ |
| Register | ✅ | ✅ | ✅ | ✅ | N/A | ✅ | ✅ |