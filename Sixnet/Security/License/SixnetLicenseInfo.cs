// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.VisualBasic;

using Sixnet.Environments;
using Sixnet.Exceptions;
using Sixnet.Localization;
using Sixnet.Security.Cryptography;

using static Sixnet.Development.Data.Dapper.SqlMapper;

namespace Sixnet.Security.License
{
    /// <summary>
    /// License info
    /// </summary>
    public class SixnetLicenseInfo
    {
        /// <summary>
        /// Gets or sets the license security key
        /// </summary>
        public static string LicenseSecurityKey { get; set; } = "MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCostvuH6BPJnRhOVn3MjXsYZGgV3NVxEhrvLnV1K9LpfZXuggxmbNKJSq4kPqxPQ+GoZ6hCnM00eQEm6d0pszTaroUhQtdOjr29aJDypdgeXsi4rOLua+RSvgJbeFYSSUPH6B3ocXBYGrIQlhfyfzS4Xmv49+j3fOd3OMsZgUCteQqou8Ha3uUmiPqD8L8kIwZaVquNDPrHEuMSgXqvl3Pu9T76b0CJoXdrbxfln2KYukvGldfXxKNKVFV4730hIWApw8H89evDBcYzcgTAAWKmjyQsKvHVu+fGRvCgLvgZZKN0RXch63i0hpRkd1wKLf4/cEUz8ja3PuoXWOVgP5PAgMBAAECggEAHtJ/CGTS32NBRDo8S8PqwkOZXdhchEEWkkirE3yGgqXR+nlM4fMZJOThh1NF2ngzi/+fggDsx6vJo2XlFJUdO0t2sCUWzHPFLjX27bfoLIJFkzwkXdURVNSDuQVQdNb4ZGj84p62CPzaxbgJeYboIId9ujtCMyhsSF2BTC1foPzawFI52du96wpVjG55s2yA2GGoxEL+rnI7KlP49aKPnVkJu5odGXrEUgjQw4AwqZXoJFcXg4uHw96pNFk/jxwOwSp+RqIhQ+VC2dCovpzkaRCUFlMVZmiz67Kbfw4KjGdIyW6ncMLvCJej3JFC/O7jchPeESXeQWBEvo5uQd3+4QKBgQDRZtQm5RJbhEQpYmzq4tWrjinFfFTWUGmmAB+IJo24vwtYD88hGIrKN30s726cjUQAtFUuD1drYZThreCCSFcJzhH/vTTYpq4b/oCRUCxgNyYBzf2hyuAVt9ZCnT29x9IeBTRvHkOfVB4vAn6FahRk5Zt3NUWEVQSUd1IKYltthwKBgQDOPUQcEaNRQ1eiNymeYKnPdsxzPuBfJ9OqSCeDa0LKcFP5+sYPEnJCwGB591cf2YhmsySwVTlllxxCcCjHGmBplZCU+ewcBeA1s+dOdxuWfUtTtnH3GPCUdCRK5H3AmT4KFyQCN8ehs82xfm3KDwTLdx0D8KUtEGrEASD3nd5a+QKBgQDP2u9JsGZ5eyAhqDaPLRyFTvc9tX2MwoMsKMEj54kT0mcTQZYLtw3FTjEtknlMYpkWzeojb00KJPGg1nSdPetPq0KIhSpYx5LZ7NV36Ioz82oBrpRNrlCinjnLI8PuhtOVwvraNcNP+zJ+3U3zZmnaAWRBfxqDEdUa/hnsftdcLwKBgCgh8ewIXqCEmzv8wgOIyeKOOpC+jojVxjGfotjG+ZNNMno1m275ZvSoXN2/DNwsx4c8mwoZO3cSZbRkAPtlnZdOPHlQ/OojFpM8s+kn8l1helQK77hmyQIKa7mLJxFggJsUD7TCx/0mcQN2F8U3EPbK8gF/RZU0WaJUmea1eLx5AoGBAIAJMgojeDk5iL7OkChnwZ04TYr/gGlT6Ijnw7EbDupnoGE99MFXk8ILYtquTvX5XtYZYAYk06WygFvy5EfiK18mOBKyijmHNSUtMt0R9vXgBSJCSHyBk7nzBVG8ID6ZX8axNApRtgQkVMWwz1XBErHL0pW4rnn0/Yvt7tst/oNh";

        /// <summary>
        /// License type
        /// </summary>
        public SixnetLicenseType Type { get; set; }

        /// <summary>
        /// License validation method
        /// </summary>
        public SixnetLicenseValidationMethod ValidationMethod { get; set; }

        /// <summary>
        /// Application name
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Machine codes
        /// </summary>
        public HashSet<string> MachineCodes { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// Notice date
        /// </summary>
        public DateTimeOffset? NoticeDate { get; set; }

        /// <summary>
        /// Validate
        /// </summary>
        /// <returns></returns>
        public SixnetLicenseValidationResult Validate()
        {
            return ExecuteValidation();
        }

        /// <summary>
        /// Execute validation
        /// </summary>
        /// <returns></returns>
        protected virtual SixnetLicenseValidationResult ExecuteValidation()
        {
            var nowDate = DateTime.Now.Date;
            if (StartDate.HasValue && nowDate < StartDate.Value.Date)
            {
                return SixnetLicenseValidationResult.Create(false, SixnetLicenseStatus.NotEnabled);
            }

            var isAvailable = !EndDate.HasValue || nowDate <= EndDate.Value.Date;
            var status = SixnetLicenseStatus.Normal;
            if (!isAvailable)
            {
                isAvailable = Type == SixnetLicenseType.Official && ValidationMethod == SixnetLicenseValidationMethod.Loose;
                status = SixnetLicenseStatus.Expired;
            }
            if (isAvailable)
            {
                var machineCode = SixnetEnvironment.GetMachineUniqueCode();
                if (!(MachineCodes?.Contains(machineCode) ?? false))
                {
                    isAvailable = false;
                    status = SixnetLicenseStatus.DeviceError;
                }

            }
            return SixnetLicenseValidationResult.Create(isAvailable, status);
        }

        /// <summary>
        /// Parse license code
        /// </summary>
        /// <param name="licenseCode"></param>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public static T Parse<T>(string licenseCode) where T : SixnetLicenseInfo
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(licenseCode), "License is null or empty");
            var licenseSigns = licenseCode.LSplit("$");
            SixnetDirectThrower.ThrowArgNullIf(licenseSigns.Length < 3, "License is invalid");

            var signKey = SixnetRSAHelper.Decrypt(licenseSigns[1], LicenseSecurityKey);
            var licenseDataString = SixnetAesHelper.Decrypt(string.Join("", licenseSigns.Skip(2)), signKey);
            return JsonSerializer.Deserialize<T>(licenseDataString);
        }
    }

    public enum SixnetLicenseType
    {
        Trial = 13650,
        Official = 13660
    }

    public enum SixnetLicenseValidationMethod
    {
        /// <summary>
        /// Strict
        /// </summary>
        Strict = 1370,
        /// <summary>
        /// Loose
        /// </summary>
        Loose = 1375,
    }

    public enum SixnetLicenseStatus
    {
        NotEnabled = 1,
        Expired = 2,
        DeviceError = 3,
        Normal = 200
    }

    public struct SixnetLicenseValidationResult
    {
        /// <summary>
        /// Is available
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// License status
        /// </summary>
        public SixnetLicenseStatus Status { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        public static SixnetLicenseValidationResult Create(bool isAvailable, SixnetLicenseStatus status, string message = "")
        {
            return new SixnetLicenseValidationResult()
            {
                IsAvailable = isAvailable,
                Status = status,
                Message = string.IsNullOrWhiteSpace(message) ? SixnetLocalizer.GetString(GetMessageByStatus(status)) : message
            };
        }

        static string GetMessageByStatus(SixnetLicenseStatus status)
        {
            return $"{status.GetEnumName()}_message";
        }
    }
}
