namespace _4drafts.Data
{
    public class DataConstants
    {
        public const int IdMaxLength = 40;

        public const int UsernameMaxLength = 20;
        public const int UsernameMinLength = 3;
        public const int PasswordMinLength = 6;
        public const string UserEmailRegularExpression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public const string ImageUrlRegex = @"(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png)";

        public const int ThreadTitleMinLength = 3;
        public const int ThreadTitleMaxLength = 80;
        public const int ThreadDescriptionMinLength = 10;
        public const int ThreadDescriptionMaxLength = 500;

        public const int CommentMaxLength = 1000;

        //Global
        public const string UnauthorizedAction = "Whoops! Looks like you're not authorized to do this...";

        //User
        public const string InvalidGender = "Invalid gender selection";
        public const string InvalidCredentials = "Whoops! Invalid credentials.";
        public const string UsernameTaken = "Username is already taken.";
        public const string EmailTaken = "Email address is already taken.";

        //Category
        public const string InexistentCategory = "Invalid category selection.";

        //Threads
        public const string InexistentThread = "Whoops! Looks like no such thread exists...";
        public const string SuccessfullyUpdatedThread = "The thread has been successfully updated.";

        //Comment
        public const string InexistentComment = "Whoops! Looks like no such comment exists...";
        public const string SuccessfullyUpdatedComment = "The comment has been successfully updated.";
        public const string SuccessfullyDeletedComment = "The comment has been successfully deleted.";
        public const string EmptyComment = "Comments cannot be empty...";
        public const string MaxLengthComment = "Comments cannot be longer than 500 characters...";

        //Draft
        public const string SuccessfullyDeletedDraft = "The draft has been successfully deleted.";
        public const string DraftSaved = "Whoops! You can only have 10 drafts at a time...";
        public const string SuccessfullyUpdatedDraft = "The draft has been successfully updated.";
        public const string MissingDraftTitle = "Whoops! Drafts require a title...";
        public const string ReachedDraftLimit = "Whoops! You can only have 10 drafts at a time...";
        public const string InexistentDraft = "Whoops! Looks like no such draft exists...";
    }
}
