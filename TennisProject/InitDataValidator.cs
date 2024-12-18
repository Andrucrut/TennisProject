﻿using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Options;
using Models.Models;
using Newtonsoft.Json.Linq;

namespace TennisProject
{
    public class InitDataValidator
    {
        private readonly string _secretToken;
        private readonly string token;

        public InitDataValidator(IOptions<BotConfiguration> options)
        {
            var botConfiguration = options.Value;
            _secretToken = botConfiguration.SecretToken;
            token = botConfiguration.BotToken;
        }
        public async Task<ValidatorResponse> Validate(string initData)
        {
            if (initData != null)
            {
                var data = HttpUtility.ParseQueryString(initData);

                // Put data in a alphabetically sorted dict.
                var dataDict = new SortedDictionary<string, string>(
                    data.AllKeys.ToDictionary(x => x!, x => data[x]!),
                    StringComparer.Ordinal);

                // Constant key to genrate secret key.
                var constantKey = "WebAppData";

                // https://core.telegram.org/bots/webapps#validating-data-received-via-the-web-app:
                // Data-check-string is a chain of all received fields,
                // sorted alphabetically.
                // in the format key=<value>.
                // with a line feed character ('\n', 0x0A) used as separator.
                // e.g., 'auth_date=<auth_date>\nquery_id=<query_id>\nuser=<user>'
                var dataCheckString = string.Join(
                    '\n', dataDict.Where(x => x.Key != "hash") // Hash should be removed.
                        .Select(x => $"{x.Key}={x.Value}")); // like auth_date=<auth_date> ..

                // secrecKey is the HMAC-SHA-256 signature of the bot's token
                // with the constant string WebAppData used as a key.
                var secretKey = HMACSHA256.HashData(
                    Encoding.UTF8.GetBytes(constantKey), // WebAppData
                    Encoding.UTF8.GetBytes(token)); // Bot's token

                var generatedHash = HMACSHA256.HashData(
                    secretKey,
                    Encoding.UTF8.GetBytes(dataCheckString)); // data_check_string

                // Convert received hash from telegram to a byte array.
                var actualHash = Convert.FromHexString(dataDict["hash"]); // .NET 5.0 

                // Compare our hash with the one from telegram.
                if (actualHash.SequenceEqual(generatedHash))
                {
                    JObject json = JObject.Parse(dataDict["user"]);
                    return new ValidatorResponse { IsValidated = true, TelegramId = (long?)json["id"], TelegramUsername = (string?)json["username"] };
                }
            }
            return new ValidatorResponse { IsValidated = false, Message = "InitData NotValidated" };
        }
    }
}
