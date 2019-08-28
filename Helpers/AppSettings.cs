namespace ABM_CMS.Helpers
{
    public class AppSettings
    {
        //Props for JWT token Signature
        public string Site { get; set; }
        public string Audience { get; set; }
        public string ExpireTime { get; set; }
        public string Secret { get; set; }
        
        //Token Refresh Props
        public string RefreshToken { get; set; }
        public string GrantType { get; set; }
        public string ClientId { get; set; }
    }
}
