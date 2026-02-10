Design Instructions: Gifter Platform
Document Type: UI/UX Specification & Brand Guidelines

Last Updated: February 6, 2026

Project Context: Modern Social Wishlist Application (The "63BITS" Standard)

🎨 1. Brand Identity & Visual Language
Core Theme: "Midnight & Neon"
Primary Aesthetic: Dark mode by default, utilizing deep grays and blacks for depth.

Accent Color: "Electric Blue" (#3B82F6 or similar) used for high-importance Actions (CTA buttons, Active states).

Secondary Accents: * Success: Emerald Green for "Purchased" or "Signed in" indicators.

Alert: Gold/Yellow stars for "Favorite" items.

Typography: Sans-serif (Inter or System UI) for maximum readability on mobile.

Component Styles
Cards: Rounded corners (approx. 16px-24px), subtle borders, and slight background elevation.

Modals: Centered, dark semi-transparent overlays, and prominent close (X) buttons.

Buttons: Full-width on mobile, rounded (pill or 8px), with subtle glow effects on primary actions.

📱 2. Core Page Blueprint
2.1 User Dashboard (The "Home Base")
Header: Quick profile access and global settings icon.

CTA: High-visibility "Create New Wishlist" button.

Grid: 2-column (Mobile) or 3/4-column (Desktop) card layout.

States: Cards must show item count and a "Preview coming soon" placeholder for empty lists.

2.2 Wishlist Details (Owner vs. Viewer)
Owner View:

Include "Add Product" button at the top.

Display total list value (e.g., "$339 Total").

Show retailers (Amazon, Etsy, eBay) as small, colored tags.

Viewer View:

Constraint: Remove "Add Product" and "Edit" capabilities.

Feature: Replace with "I'll Buy This" checkbox logic.

State: Reserved items must show a "Purchased" or "Reserved" green badge.

🛠️ 3. Frontend Implementation Rules
Folder Structure
Source: All raw HTML/Tailwind data lives in /Lovable.

Target: Production views are mapped to /Views/[Controller]/[Action].cshtml.

Interaction Logic
Modals: All login/registration and creation flows must be modal-based to keep the user in context.

Scraping Feedback: When adding a URL, show a "Scraping Image..." placeholder with a loading state.

Real-time States: Use Toast notifications (Success/Error) for every backend action (e.g., "Signed in successfully").

📋 4. Pending Design Queue
The following screens must be generated in Lovable following the established Midnight theme:

[ ] Public Profile: Follower-only view of lists + "Follow" button.

[ ] Search Results: List of users with "Follow/Unfollow" toggle.

[ ] Reserved Items Page: List of gifts the current user has reserved for others.

[ ] User Settings: Profile edit forms and avatar upload zones.