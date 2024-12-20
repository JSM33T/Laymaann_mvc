﻿namespace Laymaann.Entities.Shared
{
    public class LaymaannConfig
    {
        public string ConnectionString { get; set; }
        public CryptographySettings Cryptography { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public SmtpSettings SmtpSettings { get; set; }
        public TelegramBotSettings TelegramBot { get; set; }
        public GoogleKeys GoogleKeys { get; set; }
    }

    public class CryptographySettings
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }

    public class JwtSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string IssuerSigningKey { get; set; }
    }

    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TelegramBotSettings
    {
        public string BotToken { get; set; }
        public string LogChatId { get; set; }
        public bool LogToTele { get; set; }
    }

    public class GoogleKeys
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }



}
