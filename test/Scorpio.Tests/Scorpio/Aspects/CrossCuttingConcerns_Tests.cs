using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Scorpio.Aspects;
using Scorpio.DynamicProxy;
using Shouldly;

namespace Scorpio.Aspects
{
    public class CrossCuttingConcerns_Tests
    {
        // Mock implementation for testing
        private class TestClass : IAvoidDuplicateCrossCuttingConcerns
        {
            public List<string> AppliedCrossCuttingConcerns { get; } = new List<string>();
        }

        private class NotConcernsTest
        {
            public List<string> AppliedCrossCuttingConcerns { get; } = new List<string>();
        }

        [Fact]
        public void AddApplied_ShouldAddConcerns()
        {
            // Arrange
            var testObj = new TestClass();
            var concerns = new[] { "Logging", "Caching" };

            // Act
            CrossCuttingConcerns.AddApplied(testObj, concerns);

            // Assert
            Assert.Equal(concerns.Length, testObj.AppliedCrossCuttingConcerns.Count);
            Assert.All(concerns, concern => Assert.Contains(concern, testObj.AppliedCrossCuttingConcerns));
        }

        [Fact]
        public void RemoveApplied_ShouldRemoveConcerns()
        {
            // Arrange
            var testObj = new TestClass();
            testObj.AppliedCrossCuttingConcerns.AddRange(new[] { "Logging", "Caching", "Authorization" });

            // Act
            CrossCuttingConcerns.RemoveApplied(testObj, "Logging", "Authorization");

            // Assert
            Assert.Single(testObj.AppliedCrossCuttingConcerns);
            Assert.Contains("Caching", testObj.AppliedCrossCuttingConcerns);
            Assert.DoesNotContain("Logging", testObj.AppliedCrossCuttingConcerns);
        }

        [Fact]
        public void RemoveApplied_ShouldThrowException_WhenObjIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CrossCuttingConcerns.RemoveApplied(null, "concern1"));
        }

        [Fact]
        public void RemoveApplied_ShouldThrowException_WhenConcernsIsEmpty()
        {
            // Arrange
            TestClass obj = new TestClass();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CrossCuttingConcerns.RemoveApplied(obj, Array.Empty<string>()));
        }

        [Fact]
        public void RemoveApplied_ShouldThrowException_WhenConcernIsNull()
        {
            // Arrange
            var obj = new TestClass();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CrossCuttingConcerns.RemoveApplied(obj, null));
        }

        [Fact]
        public void RemoveApplied_ShouldNotThrow_WhenNotConcernsApplied()
        {
            // Arrange
            var testObj = new NotConcernsTest();
            var concerns = new[] { "Logging", "Caching" };
            testObj.AppliedCrossCuttingConcerns.AddRange(concerns);
            // Act
            Shouldly.Should.NotThrow(() => CrossCuttingConcerns.RemoveApplied(testObj, "NonExistentConcern"));
            // Assert
            concerns.ShouldBe(testObj.AppliedCrossCuttingConcerns);
        }

        [Fact]
        public void RemoveApplied_ShouldNotThrow_WhenNoConcernsApplied()
        {
            // Arrange
            var testObj = new TestClass();
            var concerns = new[] { "Logging", "Caching" };
            testObj.AppliedCrossCuttingConcerns.AddRange(concerns);

            // Act
            CrossCuttingConcerns.RemoveApplied(testObj, "NonExistentConcern");
           concerns.ShouldBe(testObj.AppliedCrossCuttingConcerns);
        }

        [Fact]
        public void GetApplieds_ShouldReturnEmptyArray_WhenNoConcernsApplied()
        {
            // Arrange
            var testObj = new TestClass();

            // Act
            var result = CrossCuttingConcerns.GetApplieds(testObj);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void IsApplied_ShouldReturnTrue_WhenConcernIsApplied()
        {
            // Arrange
            var testObj = new TestClass();
            testObj.AppliedCrossCuttingConcerns.Add("Logging");

            // Act
            var result = CrossCuttingConcerns.IsApplied(testObj, "Logging");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsApplied_ShouldReturnFalse_WhenConcernIsNotApplied()
        {
            // Arrange
            var testObj = new TestClass();

            // Act
            var result = CrossCuttingConcerns.IsApplied(testObj, "Logging");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Applying_ShouldAddConcernsAndRemoveThemWhenDisposed()
        {
            // Arrange
            var testObj = new TestClass();
            var concerns = new[] { "Logging", "Caching" };

            // Act
            using (CrossCuttingConcerns.Applying(testObj, concerns))
            {
                // Assert - Concerns should be applied
                Assert.All(concerns, concern => Assert.Contains(concern, testObj.AppliedCrossCuttingConcerns));
            }

            // Assert - Concerns should be removed after disposal
            Assert.Empty(testObj.AppliedCrossCuttingConcerns);
        }

        [Fact]
        public void GetApplieds_ShouldReturnAllAppliedConcerns()
        {
            // Arrange
            var testObj = new TestClass();
            var concerns = new[] { "Logging", "Caching", "Authorization" };
            testObj.AppliedCrossCuttingConcerns.AddRange(concerns);

            // Act
            var result = CrossCuttingConcerns.GetApplieds(testObj);

            // Assert
            Assert.Equal(concerns.Length, result.Length);
            Assert.All(concerns, concern => Assert.Contains(concern, result));
        }

        [Fact]
        public void AddApplied_ShouldThrowException_WhenObjIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => CrossCuttingConcerns.AddApplied(null, "Logging"));
        }

        [Fact]
        public void AddApplied_ShouldThrowException_WhenConcernsIsEmpty()
        {
            var testObj = new TestClass();
            Assert.Throws<ArgumentNullException>(() => CrossCuttingConcerns.AddApplied(testObj, Array.Empty<string>()));
        }

        [Fact]
        public void IsApplied_ShouldThrowException_WhenConcernIsNull()
        {
            var testObj = new TestClass();
            Assert.Throws<ArgumentNullException>(() => CrossCuttingConcerns.IsApplied(testObj, null));
        }


    }
}