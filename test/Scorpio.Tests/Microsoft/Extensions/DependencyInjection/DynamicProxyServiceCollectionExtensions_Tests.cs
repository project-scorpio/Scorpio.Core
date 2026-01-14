using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Scorpio.Conventional;
using Scorpio.DynamicProxy;
using Xunit;
using Moq;

namespace Microsoft.Extensions.DependencyInjection
{
    public class DynamicProxyServiceCollectionExtensions_Tests
    {
        [Fact]
        public void RegisterConventionalInterceptor_ShouldReturnSameServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();
            var types = new List<Type> { typeof(string) };
            Action<IConventionalConfiguration<ConventionalInterceptorAction>> configureAction = config => { };

            // Act
            var result = services.RegisterConventionalInterceptor(types, configureAction);

            // Assert
            Assert.Same(services, result);
        }

        [Fact]
        public void RegisterConventionalInterceptor_ShouldCallDoConventionalAction()
        {
            // Arrange
            var mockServices = new Mock<IServiceCollection>();
            var types = new List<Type> { typeof(string) };
            Action<IConventionalConfiguration<ConventionalInterceptorAction>> configureAction = config => { };


            // Act
            var result = mockServices.Object.RegisterConventionalInterceptor(types, configureAction);

            // Assert
            Assert.Same(mockServices.Object, result);
        }
    }
}