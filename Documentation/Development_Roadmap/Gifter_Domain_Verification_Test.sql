-- =============================================
-- GIFTER DOMAIN VERIFICATION TEST
-- =============================================
-- Run this in SSMS (not SmarterASP — it is a multi-statement batch).
-- Assumes at least two Users rows exist (UserID 1 and 2).
-- All test data is cleaned up at the end.
-- =============================================

-- 1. GiftListsIUD — INSERT
DECLARE @TestGiftListID INT;
EXEC [dbo].[GiftListsIUD]
    @Action       = 0,
    @GiftListID   = @TestGiftListID OUTPUT,
    @GiftListJson = '{"GiftListUserID":1,"GiftListTitle":"Test Wishlist","GiftListDescription":"Verification test","GiftListIsSecret":"false","GiftListIsPublished":"true"}';
PRINT 'GiftListsIUD INSERT — GiftListID = ' + CAST(@TestGiftListID AS VARCHAR);

-- 2. GiftListsGetSingleByID
SELECT 'GiftListsGetSingleByID' AS [Test], [dbo].[GiftListsGetSingleByID](@TestGiftListID) AS [Result];

-- 3. GiftListsListByUserID
SELECT 'GiftListsListByUserID' AS [Test], * FROM [dbo].[GiftListsListByUserID](1);

-- 4. GiftsIUD — INSERT
DECLARE @TestGiftID INT;
EXEC [dbo].[GiftsIUD]
    @Action   = 0,
    @GiftID   = @TestGiftID OUTPUT,
    @GiftJson = '{"GiftGiftListID":' + CAST(@TestGiftListID AS VARCHAR) + ',"GiftTitle":"Test Gift","GiftPrice":"99.99","GiftCurrency":"GEL"}';
PRINT 'GiftsIUD INSERT — GiftID = ' + CAST(@TestGiftID AS VARCHAR);

-- 5. GiftsGetSingleByID
SELECT 'GiftsGetSingleByID' AS [Test], [dbo].[GiftsGetSingleByID](@TestGiftID) AS [Result];

-- 6. GiftsListByGiftListID
SELECT 'GiftsListByGiftListID' AS [Test], * FROM [dbo].[GiftsListByGiftListID](@TestGiftListID);

-- 7. GiftListsIUD — UPDATE
EXEC [dbo].[GiftListsIUD]
    @Action       = 1,
    @GiftListID   = @TestGiftListID OUTPUT,
    @GiftListJson = '{"GiftListTitle":"Updated Test Wishlist"}';
SELECT 'GiftListsIUD UPDATE' AS [Test], [dbo].[GiftListsGetSingleByID](@TestGiftListID) AS [Result];

-- 8. GiftsIUD — UPDATE (reserve the gift)
EXEC [dbo].[GiftsIUD]
    @Action   = 1,
    @GiftID   = @TestGiftID OUTPUT,
    @GiftJson = '{"GiftIsReserved":"true","GiftReservedByUserID":1}';
SELECT 'GiftsIUD UPDATE (reserve)' AS [Test], [dbo].[GiftsGetSingleByID](@TestGiftID) AS [Result];

-- 9. FollowersFollow
EXEC [dbo].[FollowersFollow] @FollowingUserID = 1, @FollowedUserID = 2;
SELECT 'FollowersFollow' AS [Test], * FROM [dbo].[Followers] WHERE FollowerFollowingUserID = 1 AND FollowerFollowedUserID = 2;

-- 10. FollowersIsFollowing — should return 1
SELECT 'FollowersIsFollowing (expect 1)' AS [Test], [dbo].[FollowersIsFollowing](1, 2) AS [Result];

-- 11. FollowersList (who user 1 is following)
SELECT 'FollowersList' AS [Test], * FROM [dbo].[FollowersList](1);

-- 12. FollowersMyFollowers (who is following user 2)
SELECT 'FollowersMyFollowers' AS [Test], * FROM [dbo].[FollowersMyFollowers](2);

-- 13. FollowersUnfollow
EXEC [dbo].[FollowersUnfollow] @FollowingUserID = 1, @FollowedUserID = 2;
SELECT 'FollowersIsFollowing after unfollow (expect 0)' AS [Test], [dbo].[FollowersIsFollowing](1, 2) AS [Result];

-- 14. OrdersInsert — one row per OrderType
EXEC [dbo].[OrdersInsert] @OrderUserID = 1, @OrderType = 1, @OrderGiftListID = @TestGiftListID;
EXEC [dbo].[OrdersInsert] @OrderUserID = 1, @OrderType = 2, @OrderGiftListID = @TestGiftListID, @OrderGiftID = @TestGiftID;
EXEC [dbo].[OrdersInsert] @OrderUserID = 1, @OrderType = 3, @OrderTargetUserID = 2;
EXEC [dbo].[OrdersInsert] @OrderUserID = 1, @OrderType = 4, @OrderGiftListID = @TestGiftListID, @OrderGiftID = @TestGiftID, @OrderTargetUserID = 2;
SELECT 'OrdersInsert (expect 4 rows)' AS [Test], * FROM [dbo].[Orders] WHERE OrderUserID = 1 AND OrderGiftListID = @TestGiftListID OR (OrderUserID = 1 AND OrderType = 3 AND OrderTargetUserID = 2);

-- 15. OrdersList
SELECT 'OrdersList' AS [Test], * FROM [dbo].[OrdersList]();

-- 16. GiftsIUD — DELETE
EXEC [dbo].[GiftsIUD]
    @Action   = 2,
    @GiftID   = @TestGiftID OUTPUT,
    @GiftJson = '{}';
SELECT 'GiftsIUD DELETE (expect 0 rows)' AS [Test], COUNT(*) AS [GiftCount] FROM [dbo].[Gifts] WHERE GiftID = @TestGiftID;

-- 17. GiftListsIUD — DELETE (cascades Orders)
EXEC [dbo].[GiftListsIUD]
    @Action       = 2,
    @GiftListID   = @TestGiftListID OUTPUT,
    @GiftListJson = '{}';
SELECT 'GiftListsIUD DELETE (expect 0 rows)' AS [Test], COUNT(*) AS [GiftListCount] FROM [dbo].[GiftLists] WHERE GiftListID = @TestGiftListID;

-- 18. Final cleanup — remove the follow-only Order (OrderType 3) which has no GiftListID
DELETE FROM [dbo].[Orders] WHERE OrderUserID = 1 AND OrderType = 3 AND OrderTargetUserID = 2 AND OrderGiftListID IS NULL;

PRINT '============================================='
PRINT 'All Gifter domain checks complete.'
PRINT 'Review the result sets above for correctness.'
PRINT '============================================='
