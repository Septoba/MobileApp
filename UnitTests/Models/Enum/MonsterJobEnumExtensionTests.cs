﻿using NUnit.Framework;

using Game.Models;

namespace UnitTests.Models
{
    [TestFixture]
    public class MonsterJobEnumExtensionTest
    {
        [Test]
        public void MonsterJobEnumExtensionsTests_Unknown_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = MonsterJobEnum.Unknown.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("Monster", result);
        }

        [Test]
        public void MonsterJobEnumExtensionsTests_Bodyguard_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = MonsterJobEnum.Bodyguard.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("Bodyguard", result);
        }

        [Test]
        public void MonsterJobEnumExtensionsTests_Mercenary_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = MonsterJobEnum.Mercenary.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("Mercenary", result);
        }

        [Test]
        public void MonsterJobEnumExtensionsTests_Guard_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = MonsterJobEnum.Guard.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("Security Guard", result);
        }


        [Test]
        public void MonsterJobEnumExtensionsTests_Henchman_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = MonsterJobEnum.Henchman.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("Security Henchman", result);
        }
    }
}
