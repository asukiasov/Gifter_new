var followersModule = {
    follow: function (userId, callback) {
        $.ajax({
            url: '/followers/follow',
            type: 'POST',
            data: { followedUserID: userId },
            dataType: 'json',
            success: function (response) {
                if (callback) callback(response);
            }
        });
    },
    unfollow: function (userId, callback) {
        $.ajax({
            url: '/followers/unfollow',
            type: 'POST',
            data: { followedUserID: userId },
            dataType: 'json',
            success: function (response) {
                if (callback) callback(response);
            }
        });
    },
    isFollowing: function (userId, callback) {
        $.ajax({
            url: '/followers/is-following/' + userId,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (callback) callback(response);
            }
        });
    },
    getFollowing: function (callback) {
        $.ajax({
            url: '/followers/following',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (callback) callback(response);
            }
        });
    },
    getMyFollowers: function (callback) {
        $.ajax({
            url: '/followers/my-followers',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (callback) callback(response);
            }
        });
    }
};
