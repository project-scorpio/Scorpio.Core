using System;

using Microsoft.Extensions.Localization;

using NSubstitute;
using NSubstitute.Extensions;

using Shouldly;

using Xunit;

namespace Scorpio.Localization
{
    public class LocalizationContext_Tests
    {
        [Fact]
        public void Ctor()
        {
            Should.Throw<ArgumentNullException>(() => new LocalizationContext(null)).ParamName.ShouldBe("serviceProvider");
            var servieProvider = Substitute.For<IServiceProvider>();
            Should.Throw<InvalidOperationException>(() => new LocalizationContext(servieProvider));
            var factory = Substitute.For<IStringLocalizerFactory>();
            servieProvider.Configure().GetService(typeof(IStringLocalizerFactory)).Returns(factory);
            Should.NotThrow(() => new LocalizationContext(servieProvider)).Action(c =>
            {
                c.ServiceProvider.ShouldBe(servieProvider);
                c.LocalizerFactory.ShouldBe(factory);
            });
        }

        [Fact]
        public void Use_ShouldSetAndRestoreCurrent()
        {
            LocalizationContext.Current.ShouldBeNull();
            var sp = Substitute.For<IServiceProvider>();
            var factory = Substitute.For<IStringLocalizerFactory>();
            sp.Configure().GetService(typeof(IStringLocalizerFactory)).Returns(factory);
            var context = new LocalizationContext(sp);
            using (LocalizationContext.Use(context))
            {
                LocalizationContext.Current.ShouldBe(context);
            }
            LocalizationContext.Current.ShouldBeNull();
        }

        [Fact]
        public void Use_ShouldSupportNesting()
        {
            var sp = Substitute.For<IServiceProvider>();
            var factory = Substitute.For<IStringLocalizerFactory>();
            sp.Configure().GetService(typeof(IStringLocalizerFactory)).Returns(factory);
            var outer = new LocalizationContext(sp);
            var inner = new LocalizationContext(sp);
            using (LocalizationContext.Use(outer))
            {
                LocalizationContext.Current.ShouldBe(outer);
                using (LocalizationContext.Use(inner))
                {
                    LocalizationContext.Current.ShouldBe(inner);
                }
                LocalizationContext.Current.ShouldBe(outer);
            }
        }

        [Fact]
        public void Use_WithNull_ShouldSetCurrentToNull()
        {
            var sp = Substitute.For<IServiceProvider>();
            var factory = Substitute.For<IStringLocalizerFactory>();
            sp.Configure().GetService(typeof(IStringLocalizerFactory)).Returns(factory);
            var context = new LocalizationContext(sp);
            using (LocalizationContext.Use(context))
            {
                LocalizationContext.Current.ShouldBe(context);
                using (LocalizationContext.Use(null))
                {
                    LocalizationContext.Current.ShouldBeNull();
                }
                LocalizationContext.Current.ShouldBe(context);
            }
        }
    }
}
