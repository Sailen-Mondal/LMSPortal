using System.Linq;
using LMSPortal.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMSPortal.Tests
{
    [TestClass]
    public class SecurePasswordGeneratorTests
    {
        [TestMethod]
        public void Generate_DefaultLength_Returns16Characters()
        {
            string password = SecurePasswordGenerator.Generate(16);

            Assert.IsNotNull(password);
            Assert.AreEqual(16, password.Length, "Password should be exactly 16 characters.");
        }

        [TestMethod]
        public void Generate_MeetsComplexityRequirements()
        {
            string password = SecurePasswordGenerator.Generate(16);

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => "!@#$%^&*()-_=+".Contains(c));

            Assert.IsTrue(hasUpper, "Password must contain at least one uppercase letter.");
            Assert.IsTrue(hasLower, "Password must contain at least one lowercase letter.");
            Assert.IsTrue(hasDigit, "Password must contain at least one digit.");
            Assert.IsTrue(hasSpecial, "Password must contain at least one special character.");
        }

        [TestMethod]
        public void Generate_SubsequentCalls_ProduceUniqueEntropy()
        {
            string pwd1 = SecurePasswordGenerator.Generate(16);
            string pwd2 = SecurePasswordGenerator.Generate(16);

            Assert.AreNotEqual(pwd1, pwd2, "Two generated passwords must be cryptographically distinct.");
        }
    }
}
