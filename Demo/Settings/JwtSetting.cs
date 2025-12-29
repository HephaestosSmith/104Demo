using System;

namespace Demo.Settings
{
    public class JwtSetting
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Secret { get; set; }

        public int Expires { get; set; }
    }
}