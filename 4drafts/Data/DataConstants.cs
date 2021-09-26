namespace _4drafts.Data
{
    public class DataConstants
    {
        public class Global
        {
            public const string UnauthorizedAction = "Whoops! Looks like you're not authorized to do this...";
            public const string GeneralError = "Whoops! Looks like something went wrong...";
        }

        public class Users
        {
            public const string EmailRegularExpression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            public const string ImageUrlRegex = @"(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png)";
            public const string InvalidGender = "Invalid gender selection";
            public const string InvalidCredentials = "Whoops! Invalid credentials.";
            public const string UsernameTaken = "Username is already taken.";
            public const string EmailTaken = "Email address is already taken.";
            public const string UsernameLengthMsg = "The username must be between 3 and 20 characters long.";
            public const string PasswordLengthMsg = "The password must be between 6 and 20 characters long.";
            public const string PasswordConfirmMsg = "The password and confirmation password do not match.";
            public const string InvalidImageUrl = "Invalid image URL";
            public const int UsernameMaxLength = 20;
            public const int UsernameMinLength = 3;
            public const int AboutMeMaxLength = 500;
            public const int GenderMaxLength = 6;
            public const int AgeMin = 1;
            public const int AgeMax = 100;
            public const int PasswordMinLength = 6;
            public const int PasswordMaxLength = 20;
            public const string ProfileUpdated = "Profile has been updated.";
        }

        public class Genres
        {
            public const string Inexistent = "Invalid genre selection.";
            public const string TooMany = "You can choose a maximum of 3 genres.";
        }

        public class Threads
        {
            public const int TitleMinLength = 3;
            public const int TitleMaxLength = 80;
            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 500;
            public const int ContentMinLength = 100;
            public const string Inexistent = "Whoops! Looks like no such thread exists...";
            public const string Updated = "The thread has been updated.";
            public const string ContentMinLengthMsg = "The content field must have a minimum length of 100.";
            public const string PromptContent = "Prompts must be between 100 and 300 characters long.";
            public const string TitleLengthMsg = "The title field must be between 3 and 80 characters long.";
            public const string Disliked = "The thread has been removed from favourites.";
            public const string Liked = "The thread has been added from favourites.";
            public const string InexistentType = "Invalid thread type!";
            public const string TitleRequired = "The title field is required!";
        }

        public class Comments
        {
            public const int MaxLength = 1000;
            public const string Inexistent = "Whoops! Looks like no such comment exists...";
            public const string Updated = "The comment has been updated.";
            public const string Deleted = "The comment has been deleted.";
            public const string Empty = "Comments cannot be empty...";
            public const string ReachedMax = "Comments cannot be longer than 500 characters...";
        }

        public class Drafts
        {
            public const string Deleted = "The draft has been deleted.";
            public const string Saved = "The draft has been saved.";
            public const string Updated = "The draft has been updated.";
            public const string MissingTitle = "Whoops! Drafts require a title...";
            public const string ReachedLimit = "Whoops! You can only have 10 drafts at a time...";
            public const string Inexistent = "Whoops! Looks like no such draft exists...";
        }

    }
}
