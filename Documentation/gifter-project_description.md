This is the final, consolidated version of your project description. It merges your **Gifter** core idea with the specific **63BITS** architectural requirements and technical standards derived from your onboarding documents.

Save this as **`gifter-description.md`**.

---

# Project Gifter: Official Specification & 63BITS Implementation Guide

## 1. Project Overview

**Gifter** is a social wishlist platform. Users curate lists of desired items and share them with their network. Friends can "reserve" gifts to avoid duplicate purchases. The project must be built using the **63BITS Boilerplate** and adhere strictly to company development standards.

---

## 2. Functional Requirements

### 2.1 User Accounts & Social Loop

* **Authentication**: Signup/Login (Email and Social).
* **Profiles**: Management of user name and avatar/photo.
* **Following System**: Users must "Follow" a person to see their lists and reserve gifts.
* **Activity Wall**: A global or friend-filtered feed displaying newly added gifts.

### 2.2 Wishlist Management

* **Types**:
* **Public/Open**: Visible to all followers.
* **Secret/Private**: Hidden; requires a shareable unique link or direct invitation.


* **Fields**: Title, Description, Occasion Type (Wedding, Birthday, etc.), Privacy Toggle, Optional End Date.
* **Actions**: Create, Edit, Delete, and generate shareable links.

### 2.3 Item (Gift) Management

* **Quick Add (URL Scraping)**: System must fetch `og:title`, `og:image`, and `og:description` using Open Graph tags when a link is pasted.
* **Manual Add**: Traditional form with image upload, title, description, price, and quantity.
* **Integrated Search**: Built-in search for major retailers (Amazon, AliExpress) to find items quickly.
* **Reservation**: A gift can be marked as "Reserved" by a follower. This status must be handled with strict concurrency checks.

### 2.4 Notifications

* **Delivery**: Transactional Email (SMTP).
* **Triggers**:
1. New Follower alert.
2. Gift Reservation confirmation (to both parties).



---

## 3. Technical Architecture (63BITS Standards)

### 3.1 Project Structure

* **SixtyThreeBits.Core**:
* `Abstractions/`: All `IRepository` and `IService` interfaces.
* `Factories/`: `RepositoryFactory` (the only way to instantiate repositories) and `DbContextFactory`.
* `Infrastructure/`: Libraries and the `SixtyThreeBitsDataObjectBase`.


* **SixtyThreeBits.Web**:
* `Website/`: Public-facing controllers and models.
* `Admin/`: Administration module with DevExtreme/Permissions.



### 3.2 The Model-View-ViewModel (MVC) Rule

* **ViewModels**: Found in `SixtyThreeBits.Web.Models`. Must contain **Properties only**. No logic. Use `public record` with `get; init;`.
* **Models**: Must inherit from `ModelBase`. Must contain **Methods only** (e.g., `GetViewModel`, `SaveGift`). Responsible for calling the `RepositoryFactory`.
* **Controllers**: Strictly for routing. Inherit from `WebsiteControllerBase<T>` or `AdminControllerBase`.

### 3.3 Data Access & Error Handling

* **Repositories**: Must inherit from `SixtyThreeBitsDataObjectBase`.
* **The Wrapper**: Every external call (SQL/API) must use the `TryToReturn` pattern:
```csharp
return TryToReturn(() => { /* logic */ }, $"nameof(Method) (param={param})");

```


* **Naming**:
* Tables: PascalCase and Plural (e.g., `GiftLists`, `Gifts`).
* Columns: `TableName` + `Property` (e.g., `GiftTitle`, `GiftID`).



---

## 4. Database Schema Preview

Following the **63BITS Database Development Rules**:

| Table | Primary Key | Required Columns |
| --- | --- | --- |
| **GiftLists** | `GiftListID` | `GiftListTitle`, `GiftListDescription`, `GiftListIsSecret` (bit) |
| **Gifts** | `GiftID` | `GiftGiftListID` (FK), `GiftTitle`, `GiftPrice`, `GiftStatus` |
| **Followers** | `FollowerID` | `FollowerFollowingUserID`, `FollowerFollowedUserID` |

* **Modification**: Use `GiftsIUD` stored procedure for Insert/Update/Delete.
* **Retrieval**: Use Table-valued or Scalar functions.

---

## 5. Non-Functional Requirements

* **Performance**: All lists and items must load in **< 2 seconds**.
* **Responsiveness**: Full support for Mobile and Desktop (Bootstrap-based).
* **Security**: Protect user data; ensure "Secret" lists are not indexed by search engines.
* **Code Quality**: Opening braces on new lines (Allman style), implicit `private` modifiers, and strict `#region` organization.