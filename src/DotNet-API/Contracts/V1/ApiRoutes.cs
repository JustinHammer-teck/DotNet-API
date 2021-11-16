namespace DotNet_API.Contracts.V1
{
    public static class ApiRoutes
    {
        private const string Root = "api";
        private const string Versioning = "v1";
        private const string Base = Root + "/" + Versioning;

        public static class Posts
        {
            public const string GetAll = Base + "/post";

            public const string Get = Base + "/post/{postId}";

            public const string Post = Base + "/post";

            public const string Delete = Base + "/post/{postId}";

            public const string Put = Base + "/post/{postId}";
        }
        
        public static class Indentity
        {
            public const string Login = Base + "/identity/login";

            public const string Register = Base + "/identity/register";
        }
    }
}