# Sprint 3.5: Gift Reservation (Mark as Taken)

**Goal:** Allow wishlist viewers to mark gifts as "taken" (reserved) so other people know someone is already buying that gift.

**Duration:** ~3-4 days
**Status:** Planned

---

## Feature Overview

When a viewer wants to buy a gift for the wishlist owner:
1. They click "I'll Buy This" button on the gift card
2. A confirmation popup appears: "Are you sure you want to reserve this gift?"
3. If they click "Yes", the gift is marked as taken/reserved
4. The gift shows a "Taken" badge and becomes visually muted
5. Other viewers can see it's taken but NOT who reserved it
6. The wishlist OWNER cannot see reservation status (to keep it a surprise)

---

## Phase 1 — Database Changes

### 1.1 Gifts Table Already Has Reservation Fields

From Sprint 2, the Gifts table should already have:
```sql
[GiftIsReserved] BIT NOT NULL DEFAULT 0,
[GiftReservedByUserID] INT NULL,
[GiftReservedAt] DATETIME NULL
```

If not, run:
```sql
ALTER TABLE [dbo].[Gifts]
ADD [GiftIsReserved] BIT NOT NULL DEFAULT 0,
    [GiftReservedByUserID] INT NULL,
    [GiftReservedAt] DATETIME NULL;
```

### 1.2 Update GiftsIUD Stored Procedure

Ensure `GiftsIUD` handles reservation updates:
```sql
-- In UPDATE section of GiftsIUD:
UPDATE Gifts SET
    GiftIsReserved = ISNULL(JSON_VALUE(@Gift, '$.GiftIsReserved'), GiftIsReserved),
    GiftReservedByUserID = JSON_VALUE(@Gift, '$.GiftReservedByUserID'),
    GiftReservedAt = CASE
        WHEN JSON_VALUE(@Gift, '$.GiftIsReserved') = '1' AND GiftIsReserved = 0
        THEN GETDATE()
        ELSE GiftReservedAt
    END
WHERE GiftID = @GiftID;
```

### 1.3 Update GiftsListByGiftListID Function

Modify to conditionally hide reservation info from owner:
```sql
CREATE OR ALTER FUNCTION [dbo].[GiftsListByGiftListID]
(
    @GiftListID INT,
    @ViewerUserID INT = NULL  -- Pass viewer's UserID
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        g.GiftID,
        g.GiftListID,
        g.GiftTitle,
        g.GiftDescription,
        g.GiftUrl,
        g.GiftImageUrl,
        g.GiftPrice,
        g.GiftCurrency,
        g.GiftPriority,
        g.GiftIsActive,
        g.GiftDateCreated,
        -- Hide reservation from owner (gl.GiftListUserID)
        CASE
            WHEN gl.GiftListUserID = @ViewerUserID THEN 0  -- Owner sees all as unreserved
            ELSE g.GiftIsReserved
        END AS GiftIsReserved,
        CASE
            WHEN gl.GiftListUserID = @ViewerUserID THEN NULL
            WHEN g.GiftReservedByUserID = @ViewerUserID THEN g.GiftReservedByUserID
            ELSE NULL  -- Hide who reserved from other viewers
        END AS GiftReservedByUserID,
        g.GiftReservedAt
    FROM Gifts g
    INNER JOIN GiftLists gl ON g.GiftListID = gl.GiftListID
    WHERE g.GiftListID = @GiftListID
      AND g.GiftIsActive = 1
);
```

---

## Phase 2 — Backend: Repository & Model

### 2.1 GiftsRepository Methods

Add to `GiftsRepository.cs`:

```csharp
#region Reservation Methods

public async Task<bool> ReserveGiftAsync(int giftId, int userId)
{
    return await TryToReturnAsync(async () =>
    {
        var gift = await GiftsGetSingleByID(giftId);
        if (gift == null || gift.GiftIsReserved)
            return false;

        var giftJson = JsonConvert.SerializeObject(new GiftIudDTO
        {
            GiftIsReserved = true,
            GiftReservedByUserID = userId
        });

        await DbContext.StoredProcedureAsync("GiftsIUD",
            (int)Enums.DatabaseActions.UPDATE, giftId, giftJson);
        return true;
    },
    $"{nameof(ReserveGiftAsync)} (giftId={giftId}, userId={userId})");
}

public async Task<bool> UnreserveGiftAsync(int giftId, int userId)
{
    return await TryToReturnAsync(async () =>
    {
        var gift = await GiftsGetSingleByID(giftId);
        if (gift == null || !gift.GiftIsReserved || gift.GiftReservedByUserID != userId)
            return false;

        var giftJson = JsonConvert.SerializeObject(new GiftIudDTO
        {
            GiftIsReserved = false,
            GiftReservedByUserID = null
        });

        await DbContext.StoredProcedureAsync("GiftsIUD",
            (int)Enums.DatabaseActions.UPDATE, giftId, giftJson);
        return true;
    },
    $"{nameof(UnreserveGiftAsync)} (giftId={giftId}, userId={userId})");
}

#endregion
```

### 2.2 GiftIudDTO Updates

Add to `GiftIudDTO.cs`:
```csharp
public bool? GiftIsReserved { get; init; }
public int? GiftReservedByUserID { get; init; }
```

### 2.3 WishlistDetailModel Updates

Add to `WishlistDetailModel.cs`:
```csharp
public async Task<ReservationResult> ReserveGiftAsync(int giftId, int userId)
{
    // Verify user is not the owner
    if (Wishlist?.GiftListUserID == userId)
        return new ReservationResult { Success = false, Message = "Owner cannot reserve their own gifts" };

    var success = await GiftsRepository.ReserveGiftAsync(giftId, userId);
    return new ReservationResult
    {
        Success = success,
        Message = success ? "Gift reserved successfully" : "Gift is already reserved"
    };
}

public async Task<ReservationResult> UnreserveGiftAsync(int giftId, int userId)
{
    var success = await GiftsRepository.UnreserveGiftAsync(giftId, userId);
    return new ReservationResult
    {
        Success = success,
        Message = success ? "Reservation cancelled" : "Cannot cancel this reservation"
    };
}

public class ReservationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
```

---

## Phase 3 — Backend: Controller Endpoints

### 3.1 WishlistController Reserve/Unreserve Actions

Already defined in Sprint 3, implement:

```csharp
[HttpPost("{id:int}/reserve/{giftId:int}")]
[Route(ControllerActionRouteNames.WebWishlistControllerReserve)]
public async Task<IActionResult> Reserve(int id, int giftId)
{
    var user = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
    if (user == null)
        return Json(new { success = false, message = "Please sign in to reserve gifts" });

    var result = await Model.ReserveGiftAsync(giftId, user.UserID.Value);
    return Json(result);
}

[HttpPost("{id:int}/unreserve/{giftId:int}")]
[Route(ControllerActionRouteNames.WebWishlistControllerUnreserve)]
public async Task<IActionResult> Unreserve(int id, int giftId)
{
    var user = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
    if (user == null)
        return Json(new { success = false, message = "Please sign in" });

    var result = await Model.UnreserveGiftAsync(giftId, user.UserID.Value);
    return Json(result);
}
```

---

## Phase 4 — Frontend: Confirmation Modal

### 4.1 Confirmation Modal HTML (DetailView.cshtml)

Add inside the viewer section (already in Sprint 3 template):

```html
<!-- Reserve Confirmation Modal -->
<div class="modal fade" id="reserveConfirmModal" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-sm">
        <div class="modal-content glass">
            <div class="modal-body p-4 text-center">
                <div class="fs-1 mb-3">🎁</div>
                <h5 class="mb-3">Reserve this gift?</h5>
                <p class="text-muted mb-4">
                    Are you sure you want to buy
                    <strong id="reserveGiftTitle"></strong>
                    for <strong>@Model.OwnerFirstname</strong>?
                </p>
                <p class="small text-muted mb-4">
                    <ion-icon name="eye-off-outline"></ion-icon>
                    @Model.OwnerFirstname won't see that you reserved it.
                </p>
                <input type="hidden" id="reserveGiftId" />
                <div class="d-grid gap-2">
                    <button type="button" class="btn btn-success btn-squish" id="confirmReserveBtn">
                        <ion-icon name="checkmark-outline" class="me-1"></ion-icon>
                        Yes, I'll buy it
                    </button>
                    <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                        Cancel
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>
```

### 4.2 Gift Card with Reserve Button

```html
<div class="col-12 col-sm-6">
    <div class="bento-card p-3 @(gift.GiftIsReserved ? "gift-reserved" : "")">
        <div class="position-relative mb-3">
            <img src="@gift.GiftImageUrl" alt="@gift.GiftTitle"
                 class="w-100 rounded-3" style="aspect-ratio: 4/3; object-fit: cover;" />
            <span class="badge bg-primary position-absolute bottom-0 start-0 m-2">
                @gift.GiftPrice @gift.GiftCurrency
            </span>
            @if (gift.GiftIsReserved)
            {
                <span class="badge bg-success position-absolute top-0 end-0 m-2">
                    <ion-icon name="checkmark-circle-outline" class="me-1"></ion-icon>
                    Taken
                </span>
            }
        </div>
        <h4 class="h6 fw-semibold mb-2">@gift.GiftTitle</h4>

        @if (!isOwner)
        {
            @if (!gift.GiftIsReserved)
            {
                <button class="btn btn-outline-success btn-sm w-100 btn-reserve"
                        data-gift-id="@gift.GiftID"
                        data-gift-title="@gift.GiftTitle"
                        data-bs-toggle="modal"
                        data-bs-target="#reserveConfirmModal">
                    <ion-icon name="gift-outline" class="me-1"></ion-icon>
                    I'll Buy This
                </button>
            }
            else if (gift.GiftReservedByUserID == Model.CurrentUserID)
            {
                <button class="btn btn-outline-danger btn-sm w-100 btn-unreserve"
                        data-gift-id="@gift.GiftID">
                    <ion-icon name="close-outline" class="me-1"></ion-icon>
                    Cancel My Reservation
                </button>
            }
            else
            {
                <button class="btn btn-secondary btn-sm w-100" disabled>
                    <ion-icon name="checkmark-circle-outline" class="me-1"></ion-icon>
                    Already Taken
                </button>
            }
        }
    </div>
</div>
```

### 4.3 CSS for Reserved Gifts (gifter.css)

```css
/* Reserved gift styling */
.gift-reserved {
    opacity: 0.6;
    position: relative;
}

.gift-reserved::after {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(135deg, transparent 40%, rgba(0, 170, 85, 0.1) 100%);
    pointer-events: none;
    border-radius: var(--radius-md);
}

.gift-reserved img {
    filter: grayscale(30%);
}
```

---

## Phase 5 — Frontend: JavaScript (app.js)

### 5.1 Reserve Confirmation Modal Handler

```javascript
// Initialize Reserve Confirmation Modal
function initReserveConfirmModal() {
    const modal = document.getElementById('reserveConfirmModal');
    if (!modal) return;

    // When modal opens, populate with gift data from button
    modal.addEventListener('show.bs.modal', function(event) {
        const button = event.relatedTarget;
        const giftId = button.getAttribute('data-gift-id');
        const giftTitle = button.getAttribute('data-gift-title');

        document.getElementById('reserveGiftId').value = giftId;
        document.getElementById('reserveGiftTitle').textContent = giftTitle;
    });

    // Confirm button click
    document.getElementById('confirmReserveBtn')?.addEventListener('click', async function() {
        const giftId = document.getElementById('reserveGiftId').value;
        const btn = this;

        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Reserving...';

        try {
            const result = await reserveGift(giftId);
            if (result.success) {
                showToast('Gift reserved! ' + ownerName + ' won\'t see who reserved it.', 'success');
                bootstrap.Modal.getInstance(modal).hide();
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message || 'Failed to reserve gift', 'error');
            }
        } catch (error) {
            console.error('Reserve error:', error);
            showToast('An error occurred. Please try again.', 'error');
        } finally {
            btn.disabled = false;
            btn.innerHTML = '<ion-icon name="checkmark-outline" class="me-1"></ion-icon> Yes, I\'ll buy it';
        }
    });
}

// Reserve API call
async function reserveGift(giftId) {
    const response = await fetch(`/${languageCode}/wishlist/${wishlistId}/reserve/${giftId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    });
    return response.json();
}

// Unreserve API call
async function unreserveGift(giftId) {
    const response = await fetch(`/${languageCode}/wishlist/${wishlistId}/unreserve/${giftId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    });
    return response.json();
}

// Initialize unreserve buttons
function initUnreserveButtons() {
    document.querySelectorAll('.btn-unreserve').forEach(btn => {
        btn.addEventListener('click', async function() {
            const giftId = this.getAttribute('data-gift-id');

            this.disabled = true;
            this.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

            try {
                const result = await unreserveGift(giftId);
                if (result.success) {
                    showToast('Reservation cancelled', 'success');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    showToast(result.message || 'Failed to cancel', 'error');
                }
            } catch (error) {
                showToast('An error occurred', 'error');
            } finally {
                this.disabled = false;
                this.innerHTML = '<ion-icon name="close-outline" class="me-1"></ion-icon> Cancel My Reservation';
            }
        });
    });
}

// Call in initWishlistDetail()
function initWishlistDetail() {
    // ... existing code ...

    if (!isOwner) {
        initReserveConfirmModal();
        initUnreserveButtons();
    }
}
```

---

## Verification Checklist

### Database
- [ ] `GiftIsReserved`, `GiftReservedByUserID`, `GiftReservedAt` columns exist
- [ ] `GiftsIUD` handles reservation updates
- [ ] `GiftsListByGiftListID` hides reservation from owner

### Backend
- [ ] `GiftsRepository.ReserveGiftAsync()` works
- [ ] `GiftsRepository.UnreserveGiftAsync()` works
- [ ] Reserve endpoint validates user is not owner
- [ ] Unreserve endpoint validates user is the reserver

### Frontend - Viewer Experience
- [ ] "I'll Buy This" button appears on unreserved gifts
- [ ] Clicking button opens confirmation modal
- [ ] Modal shows gift title and owner name
- [ ] "Yes, I'll buy it" calls API and reserves gift
- [ ] Reserved gifts show "Taken" badge with muted style
- [ ] Reserver sees "Cancel My Reservation" button
- [ ] Other viewers see disabled "Already Taken" button

### Frontend - Owner Experience
- [ ] Owner does NOT see "I'll Buy This" buttons
- [ ] Owner does NOT see "Taken" badges
- [ ] Owner sees all gifts as normal (surprise preserved)

### Edge Cases
- [ ] Cannot reserve already-reserved gift (API returns error)
- [ ] Cannot unreserve someone else's reservation
- [ ] Owner cannot reserve their own gifts
- [ ] Guest users prompted to sign in

---

## User Flow Diagram

```
Viewer opens wishlist
        │
        ▼
    ┌─────────────────┐
    │  See gift card  │
    │  with "I'll Buy │
    │  This" button   │
    └────────┬────────┘
             │ Click
             ▼
    ┌─────────────────┐
    │ Confirmation    │
    │ Modal appears   │
    │ "Are you sure?" │
    └────────┬────────┘
             │
     ┌───────┴───────┐
     │               │
     ▼               ▼
 ┌───────┐      ┌─────────┐
 │Cancel │      │Yes, buy │
 └───────┘      └────┬────┘
                     │
                     ▼
              ┌─────────────┐
              │ API call    │
              │ Reserve gift│
              └──────┬──────┘
                     │
                     ▼
              ┌─────────────┐
              │ Gift shows  │
              │ "Taken"     │
              │ badge       │
              └─────────────┘
```

---

## Dependencies

### From Sprint 3
- [x] WishlistController with Reserve/Unreserve route names
- [x] DetailView.cshtml with gift cards
- [x] app.js with wishlist functions

### New for Sprint 3.5
- [ ] Confirmation modal UI
- [ ] Reserve/Unreserve API implementations
- [ ] Gift card states (unreserved/reserved/my-reservation)
- [ ] CSS for reserved gift styling
