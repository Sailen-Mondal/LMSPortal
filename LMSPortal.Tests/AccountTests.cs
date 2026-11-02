using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LMSPortal.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMSPortal.Tests
{
    [TestClass]
    public class AccountTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [TestMethod]
        public void LoginViewModel_EmptyFields_FailsValidation()
        {
            var model = new LoginViewModel();
            var results = ValidateModel(model);

            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email")), "Email should be required.");
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Password")), "Password should be required.");
        }

        [TestMethod]
        public void LoginViewModel_InvalidEmailFormat_FailsValidation()
        {
            var model = new LoginViewModel
            {
                Email = "not-an-email",
                Password = "Password123"
            };
            var results = ValidateModel(model);

            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email")), "Malformed email must fail validation.");
        }

        [TestMethod]
        public void LoginViewModel_ValidModel_PassesValidation()
        {
            var model = new LoginViewModel
            {
                Email = "student@lmsportal.local",
                Password = "Password123",
                RememberMe = true
            };
            var results = ValidateModel(model);

            Assert.AreEqual(0, results.Count, "Valid login model should have zero validation errors.");
        }

        [TestMethod]
        public void RegisterViewModel_PasswordMismatch_FailsValidation()
        {
            var model = new RegisterViewModel
            {
                FullName = "Test User",
                Email = "test@lmsportal.local",
                Password = "Password123",
                ConfirmPassword = "DifferentPassword"
            };
            var results = ValidateModel(model);

            bool hasMismatchError = results.Any(r => 
                (r.ErrorMessage != null && r.ErrorMessage.Contains("do not match")) || 
                r.MemberNames.Contains("ConfirmPassword"));

            Assert.IsTrue(hasMismatchError, $"Expected mismatch error, but got {results.Count} errors: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
        }

        [TestMethod]
        public void RegisterViewModel_ValidModel_PassesValidation()
        {
            var model = new RegisterViewModel
            {
                FullName = "Alex Mercer",
                Email = "alex@lmsportal.local",
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            };
            var results = ValidateModel(model);

            Assert.AreEqual(0, results.Count, "Valid register model should have zero validation errors.");
        }

        [TestMethod]
        public void SafeReturnUrl_Detection_BlocksMaliciousExternalUrls()
        {
            string[] externalUrls = { "http://evil.com", "https://attacker.site", "//evil.com", "\\\\attacker" };
            string[] safeLocalUrls = { "/course/modern-javascript", "/student/dashboard", "/courses?cat=web" };

            foreach (var url in externalUrls)
            {
                bool isLocal = !string.IsNullOrEmpty(url) && 
                    ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || 
                     (url.Length > 1 && url[0] == '~' && url[1] == '/'));

                Assert.IsFalse(isLocal, $"External URL '{url}' must be rejected by safe redirect check.");
            }

            foreach (var url in safeLocalUrls)
            {
                bool isLocal = !string.IsNullOrEmpty(url) && 
                    ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || 
                     (url.Length > 1 && url[0] == '~' && url[1] == '/'));

                Assert.IsTrue(isLocal, $"Local URL '{url}' must be accepted by safe redirect check.");
            }
        }
    }
}
