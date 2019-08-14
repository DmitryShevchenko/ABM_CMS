namespace ABM_CMS.Helpers
{
    public class AppSettings
    {
        //Props for JWT token Signature
        public string Site { get; set; }
        public string Audience { get; set; }
        public string ExpireTime { get; set; }
        public string Secret { get; set; }
    }
}