using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Scorpio.Conventional;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection
{
    public class DependencyInjectionServiceCollectionExtensions_Tests
    {
        [Fact]
        public void AddConventionalRegistrar_Should_Add_Registrar_To_List()
        {
            // Arrange
            var services = new ServiceCollection();
            var registrar = new Mock<IConventionalRegistrar>().Object;

            // Act
            services.AddConventionalRegistrar(registrar);
            var list = services.GetSingletonInstance<ConventionalRegistrarList>();

            // Assert
            Assert.NotNull(list);
            Assert.Contains(registrar, list);
        }

        [Fact]
        public void AddConventionalRegistrar_Generic_Should_Create_And_Add_Instance()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddConventionalRegistrar<TestConventionalRegistrar>();
            var list = services.GetSingletonInstance<ConventionalRegistrarList>();

            // Assert
            Assert.NotNull(list);
            Assert.Single(list);
            Assert.IsType<TestConventionalRegistrar>(list[0]);
        }

        [Fact]
        public void RegisterAssemblyByConvention_Should_Call_All_Registrars()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockRegistrar1 = new Mock<IConventionalRegistrar>();
            var mockRegistrar2 = new Mock<IConventionalRegistrar>();
            var assembly = typeof(DependencyInjectionServiceCollectionExtensions_Tests).Assembly;

            services.AddConventionalRegistrar(mockRegistrar1.Object);
            services.AddConventionalRegistrar(mockRegistrar2.Object);

            // Act
            services.RegisterAssemblyByConvention(assembly);

            // Assert
            mockRegistrar1.Verify(r => r.Register(It.Is<ConventionalRegistrationContext>(c =>
                c.Assembly == assembly && c.Services == services)), Times.Once);
            mockRegistrar2.Verify(r => r.Register(It.Is<ConventionalRegistrationContext>(c =>
                c.Assembly == assembly && c.Services == services)), Times.Once);
        }

        // Test helper class
        private class TestConventionalRegistrar : IConventionalRegistrar
        {


            public void Register(IConventionalRegistrationContext context)
            {
                // Do nothing for test
            }
        }
    }
}