namespace _4drafts.Data
{
    public class DataConstants
    {
        public const int IdMaxLength = 40;

        public const int UsernameMaxLength = 20;
        public const int UsernameMinLength = 3;
        public const int PasswordMinLength = 6;
        public const string UserEmailRegularExpression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        public const int ThreadTitleMinLength = 3;
        public const int ThreadTitleMaxLength = 80;

        public const int CommentMaxLength = 1000;
    }
}
