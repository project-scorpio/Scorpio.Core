using System;
using System.Collections;
using System.Collections.Generic;

using Shouldly;

using Xunit;

namespace System.Reflection
{
    public class TypeHelper_Tests
    {
        #region IsNonNullablePrimitiveType

        [Theory]
        [InlineData(typeof(byte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(int))]
        [InlineData(typeof(long))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(float))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(Guid))]
        public void IsNonNullablePrimitiveType_ShouldReturnTrue(Type type)
        {
            TypeHelper.IsNonNullablePrimitiveType(type).ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(object))]
        [InlineData(typeof(int?))]
        [InlineData(typeof(DateTime?))]
        public void IsNonNullablePrimitiveType_ShouldReturnFalse(Type type)
        {
            TypeHelper.IsNonNullablePrimitiveType(type).ShouldBeFalse();
        }

        #endregion

        #region IsFunc

        [Fact]
        public void IsFunc_Null_ShouldReturnFalse()
        {
            TypeHelper.IsFunc(null).ShouldBeFalse();
        }

        [Fact]
        public void IsFunc_NonFunc_ShouldReturnFalse()
        {
            TypeHelper.IsFunc("string").ShouldBeFalse();
            TypeHelper.IsFunc(42).ShouldBeFalse();
        }

        [Fact]
        public void IsFunc_Func_ShouldReturnTrue()
        {
            Func<int> func = () => 1;
            TypeHelper.IsFunc(func).ShouldBeTrue();
        }

        [Fact]
        public void IsFunc_FuncWithParam_ShouldReturnFalse()
        {
            // Func<int, int> is not Func<> (no-arg), so should return false
            Func<int, int> func = x => x;
            TypeHelper.IsFunc(func).ShouldBeFalse();
        }

        [Fact]
        public void IsFunc_Generic_ShouldReturnTrue()
        {
            Func<string> func = () => "hello";
            TypeHelper.IsFunc<string>(func).ShouldBeTrue();
        }

        [Fact]
        public void IsFunc_Generic_WrongReturnType_ShouldReturnFalse()
        {
            Func<int> func = () => 1;
            TypeHelper.IsFunc<string>(func).ShouldBeFalse();
        }

        [Fact]
        public void IsFunc_Generic_Null_ShouldReturnFalse()
        {
            TypeHelper.IsFunc<string>(null).ShouldBeFalse();
        }

        #endregion

        #region IsPrimitiveExtended

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(string))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(Guid))]
        public void IsPrimitiveExtended_ShouldReturnTrue(Type type)
        {
            TypeHelper.IsPrimitiveExtended(type).ShouldBeTrue();
        }

        [Fact]
        public void IsPrimitiveExtended_Nullable_ShouldReturnTrue()
        {
            TypeHelper.IsPrimitiveExtended(typeof(int?)).ShouldBeTrue();
            TypeHelper.IsPrimitiveExtended(typeof(DateTime?)).ShouldBeTrue();
            TypeHelper.IsPrimitiveExtended(typeof(Guid?)).ShouldBeTrue();
        }

        [Fact]
        public void IsPrimitiveExtended_Nullable_ExcludeNullables_ShouldReturnFalse()
        {
            TypeHelper.IsPrimitiveExtended(typeof(int?), includeNullables: false).ShouldBeFalse();
        }

        [Fact]
        public void IsPrimitiveExtended_Enum_ExcludeEnums_ShouldReturnFalse()
        {
            TypeHelper.IsPrimitiveExtended(typeof(DayOfWeek), includeEnums: false).ShouldBeFalse();
        }

        [Fact]
        public void IsPrimitiveExtended_Enum_IncludeEnums_ShouldReturnTrue()
        {
            TypeHelper.IsPrimitiveExtended(typeof(DayOfWeek), includeEnums: true).ShouldBeTrue();
        }

        [Fact]
        public void IsPrimitiveExtended_Object_ShouldReturnFalse()
        {
            TypeHelper.IsPrimitiveExtended(typeof(object)).ShouldBeFalse();
        }

        #endregion

        #region IsNullable

        [Fact]
        public void IsNullable_NullableTypes_ShouldReturnTrue()
        {
            TypeHelper.IsNullable(typeof(int?)).ShouldBeTrue();
            TypeHelper.IsNullable(typeof(DateTime?)).ShouldBeTrue();
            TypeHelper.IsNullable(typeof(bool?)).ShouldBeTrue();
        }

        [Fact]
        public void IsNullable_NonNullableTypes_ShouldReturnFalse()
        {
            TypeHelper.IsNullable(typeof(int)).ShouldBeFalse();
            TypeHelper.IsNullable(typeof(string)).ShouldBeFalse();
            TypeHelper.IsNullable(typeof(object)).ShouldBeFalse();
        }

        #endregion

        #region GetFirstGenericArgumentIfNullable

        [Fact]
        public void GetFirstGenericArgumentIfNullable_Nullable_ShouldReturnUnderlyingType()
        {
            typeof(int?).GetFirstGenericArgumentIfNullable().ShouldBe(typeof(int));
            typeof(DateTime?).GetFirstGenericArgumentIfNullable().ShouldBe(typeof(DateTime));
        }

        [Fact]
        public void GetFirstGenericArgumentIfNullable_NonNullable_ShouldReturnSameType()
        {
            typeof(int).GetFirstGenericArgumentIfNullable().ShouldBe(typeof(int));
            typeof(string).GetFirstGenericArgumentIfNullable().ShouldBe(typeof(string));
        }

        #endregion

        #region IsEnumerable

        [Fact]
        public void IsEnumerable_GenericList_ShouldReturnTrue()
        {
            TypeHelper.IsEnumerable(typeof(List<string>), out var itemType).ShouldBeTrue();
            itemType.ShouldBe(typeof(string));
        }

        [Fact]
        public void IsEnumerable_Array_ShouldReturnTrue()
        {
            TypeHelper.IsEnumerable(typeof(int[]), out var itemType).ShouldBeTrue();
            itemType.ShouldBe(typeof(int));
        }

        [Fact]
        public void IsEnumerable_NonGenericIEnumerable_ShouldReturnObjectItemType()
        {
            TypeHelper.IsEnumerable(typeof(ArrayList), out var itemType).ShouldBeTrue();
            itemType.ShouldBe(typeof(object));
        }

        [Fact]
        public void IsEnumerable_String_IncludePrimitives_ShouldReturnTrue()
        {
            TypeHelper.IsEnumerable(typeof(string), out _, includePrimitives: true).ShouldBeTrue();
        }

        [Fact]
        public void IsEnumerable_String_ExcludePrimitives_ShouldReturnFalse()
        {
            TypeHelper.IsEnumerable(typeof(string), out var itemType, includePrimitives: false).ShouldBeFalse();
            itemType.ShouldBeNull();
        }

        [Fact]
        public void IsEnumerable_NonEnumerable_ShouldReturnFalse()
        {
            TypeHelper.IsEnumerable(typeof(int), out var itemType).ShouldBeFalse();
            itemType.ShouldBeNull();
        }

        #endregion

        #region IsDictionary

        [Fact]
        public void IsDictionary_GenericDictionary_ShouldReturnTrue()
        {
            TypeHelper.IsDictionary(typeof(Dictionary<string, int>), out var keyType, out var valueType).ShouldBeTrue();
            keyType.ShouldBe(typeof(string));
            valueType.ShouldBe(typeof(int));
        }

        [Fact]
        public void IsDictionary_NonGenericIDictionary_ShouldReturnObjectTypes()
        {
            TypeHelper.IsDictionary(typeof(Hashtable), out var keyType, out var valueType).ShouldBeTrue();
            keyType.ShouldBe(typeof(object));
            valueType.ShouldBe(typeof(object));
        }

        [Fact]
        public void IsDictionary_NonDictionary_ShouldReturnFalse()
        {
            TypeHelper.IsDictionary(typeof(List<string>), out var keyType, out var valueType).ShouldBeFalse();
            keyType.ShouldBeNull();
            valueType.ShouldBeNull();
        }

        #endregion

        #region GetDefaultValue

        [Fact]
        public void GetDefaultValue_Generic_ShouldReturnDefault()
        {
            TypeHelper.GetDefaultValue<int>().ShouldBe(0);
            TypeHelper.GetDefaultValue<bool>().ShouldBe(false);
            TypeHelper.GetDefaultValue<string>().ShouldBeNull();
        }

        [Fact]
        public void GetDefaultValue_ValueType_ShouldReturnDefaultInstance()
        {
            TypeHelper.GetDefaultValue(typeof(int)).ShouldBe(0);
            TypeHelper.GetDefaultValue(typeof(Guid)).ShouldBe(Guid.Empty);
            TypeHelper.GetDefaultValue(typeof(bool)).ShouldBe(false);
        }

        [Fact]
        public void GetDefaultValue_ReferenceType_ShouldReturnNull()
        {
            TypeHelper.GetDefaultValue(typeof(string)).ShouldBeNull();
            TypeHelper.GetDefaultValue(typeof(object)).ShouldBeNull();
        }

        #endregion

        #region GetFullNameHandlingNullableAndGenerics

        [Fact]
        public void GetFullNameHandlingNullableAndGenerics_Null_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => TypeHelper.GetFullNameHandlingNullableAndGenerics(null));
        }

        [Fact]
        public void GetFullNameHandlingNullableAndGenerics_NullableType_ShouldHaveQuestionMark()
        {
            var result = TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(int?));
            result.ShouldBe("System.Int32?");
        }

        [Fact]
        public void GetFullNameHandlingNullableAndGenerics_GenericType_ShouldIncludeTypeArgs()
        {
            var result = TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(List<string>));
            result.ShouldBe("System.Collections.Generic.List<System.String>");
        }

        [Fact]
        public void GetFullNameHandlingNullableAndGenerics_PlainType_ShouldReturnFullName()
        {
            var result = TypeHelper.GetFullNameHandlingNullableAndGenerics(typeof(string));
            result.ShouldBe("System.String");
        }

        #endregion

        #region GetSimplifiedName

        [Fact]
        public void GetSimplifiedName_Null_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => TypeHelper.GetSimplifiedName(null));
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(char))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(TimeSpan))]
        [InlineData(typeof(Guid))]
        public void GetSimplifiedName_StringLikeTypes_ShouldReturnString(Type type)
        {
            TypeHelper.GetSimplifiedName(type).ShouldBe("string");
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(bool))]
        public void GetSimplifiedName_NumberTypes_ShouldReturnNumber(Type type)
        {
            TypeHelper.GetSimplifiedName(type).ShouldBe("number");
        }

        [Fact]
        public void GetSimplifiedName_ObjectType_ShouldReturnObject()
        {
            TypeHelper.GetSimplifiedName(typeof(object)).ShouldBe("object");
        }

        [Fact]
        public void GetSimplifiedName_NullableType_ShouldHaveQuestionMark()
        {
            TypeHelper.GetSimplifiedName(typeof(int?)).ShouldBe("number?");
            TypeHelper.GetSimplifiedName(typeof(DateTime?)).ShouldBe("string?");
        }

        [Fact]
        public void GetSimplifiedName_GenericType_ShouldIncludeTypeArgs()
        {
            var result = TypeHelper.GetSimplifiedName(typeof(List<string>));
            result.ShouldBe("System.Collections.Generic.List<string>");
        }

        [Fact]
        public void GetSimplifiedName_UnknownType_ShouldReturnFullName()
        {
            TypeHelper.GetSimplifiedName(typeof(TypeHelper_Tests)).ShouldBe(typeof(TypeHelper_Tests).FullName);
        }

        #endregion

        #region ConvertFromString

        [Fact]
        public void ConvertFromString_NullValue_ShouldReturnNull()
        {
            TypeHelper.ConvertFromString(typeof(string), null).ShouldBeNull();
        }

        [Fact]
        public void ConvertFromString_IntValue_ShouldConvert()
        {
            TypeHelper.ConvertFromString(typeof(int), "42").ShouldBe(42);
        }

        [Fact]
        public void ConvertFromString_Generic_IntValue_ShouldConvert()
        {
            TypeHelper.ConvertFromString<int>("100").ShouldBe(100);
        }

        [Fact]
        public void ConvertFromString_FloatValue_ShouldConvert()
        {
            var result = TypeHelper.ConvertFromString(typeof(double), "3.14");
            ((double)result).ShouldBe(3.14, 0.001);
        }

        [Fact]
        public void ConvertFromString_FloatValue_WithComma_ShouldConvert()
        {
            var result = TypeHelper.ConvertFromString(typeof(double), "3,14");
            ((double)result).ShouldBe(3.14, 0.001);
        }

        [Fact]
        public void ConvertFromString_BoolValue_ShouldConvert()
        {
            TypeHelper.ConvertFromString(typeof(bool), "true").ShouldBe(true);
            TypeHelper.ConvertFromString(typeof(bool), "false").ShouldBe(false);
        }

        #endregion

        #region IsFloatingType

        [Theory]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        public void IsFloatingType_FloatingTypes_ShouldReturnTrue(Type type)
        {
            TypeHelper.IsFloatingType(type).ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(bool))]
        public void IsFloatingType_NonFloatingTypes_ShouldReturnFalse(Type type)
        {
            TypeHelper.IsFloatingType(type).ShouldBeFalse();
        }

        [Fact]
        public void IsFloatingType_NullableFloat_IncludeNullable_ShouldReturnTrue()
        {
            TypeHelper.IsFloatingType(typeof(float?), includeNullable: true).ShouldBeTrue();
            TypeHelper.IsFloatingType(typeof(decimal?), includeNullable: true).ShouldBeTrue();
        }

        [Fact]
        public void IsFloatingType_NullableFloat_ExcludeNullable_ShouldReturnFalse()
        {
            TypeHelper.IsFloatingType(typeof(float?), includeNullable: false).ShouldBeFalse();
        }

        #endregion

        #region ConvertFrom

        [Fact]
        public void ConvertFrom_Generic_ShouldConvert()
        {
            TypeHelper.ConvertFrom<int>("42").ShouldBe(42);
        }

        [Fact]
        public void ConvertFrom_ShouldConvert()
        {
            TypeHelper.ConvertFrom(typeof(int), "100").ShouldBe(100);
        }

        #endregion

        #region StripNullable

        [Fact]
        public void StripNullable_NullableType_ShouldReturnUnderlyingType()
        {
            TypeHelper.StripNullable(typeof(int?)).ShouldBe(typeof(int));
            TypeHelper.StripNullable(typeof(DateTime?)).ShouldBe(typeof(DateTime));
        }

        [Fact]
        public void StripNullable_NonNullableType_ShouldReturnSameType()
        {
            TypeHelper.StripNullable(typeof(int)).ShouldBe(typeof(int));
            TypeHelper.StripNullable(typeof(string)).ShouldBe(typeof(string));
        }

        #endregion

        #region IsDefaultValue

        [Fact]
        public void IsDefaultValue_Null_ShouldReturnTrue()
        {
            TypeHelper.IsDefaultValue(null).ShouldBeTrue();
        }

        [Fact]
        public void IsDefaultValue_DefaultValueTypes_ShouldReturnTrue()
        {
            TypeHelper.IsDefaultValue(0).ShouldBeTrue();
            TypeHelper.IsDefaultValue(false).ShouldBeTrue();
            TypeHelper.IsDefaultValue(Guid.Empty).ShouldBeTrue();
        }

        [Fact]
        public void IsDefaultValue_NonDefaultValueTypes_ShouldReturnFalse()
        {
            TypeHelper.IsDefaultValue(1).ShouldBeFalse();
            TypeHelper.IsDefaultValue(true).ShouldBeFalse();
            TypeHelper.IsDefaultValue(Guid.NewGuid()).ShouldBeFalse();
        }

        [Fact]
        public void IsDefaultValue_NonNullString_ShouldReturnFalse()
        {
            TypeHelper.IsDefaultValue("hello").ShouldBeFalse();
        }

        #endregion
    }
}
