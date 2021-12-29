using System.Text.Json;
using WelcomeTo.Shared.Extensions;
using Xunit;

namespace WelcomeTo.Test.Shared.Extensions
{
    public class JsonExtensionsTest
    {

        private static readonly TestSerializerClass _testObject = new() { IntValue = 1, StringValue = "Value", BooleanValue = true, camelCaseStringValue = "value", camelCaseBooleanValue = false };
        private static readonly JsonElement _objectJsonElement = JsonDocument.Parse(JsonSerializer.Serialize(_testObject)).RootElement;

        private static readonly TestWithObjectProperty _testWithObjectProperty = new() { ObjectProperty = _testObject, camelCaseObjectProperty = _testObject };
        private static readonly JsonElement _testWithObjectPropertyJsonElement = JsonDocument.Parse(JsonSerializer.Serialize(_testWithObjectProperty)).RootElement;

        [Fact]
        public void GetStringProperty_ValidJsonElementObjectPascalCase_ExtractsStringPropertySuccessfully()
        {
            Assert.Equal("Value", _objectJsonElement.GetStringProperty("StringValue"));
        }

        [Fact]
        public void GetStringProperty_ValidJsonElementObjectCamelCase_ExtractsStringPropertySuccessfully()
        {
            Assert.Equal("value", _objectJsonElement.GetStringProperty("camelCaseStringValue"));
            Assert.Equal("value", _objectJsonElement.GetStringProperty("CamelCaseStringValue"));
        }

        [Fact]
        public void GetObjectProperty_ValidJsonElementObjectPascalCase_ExtractsObjectPropertySuccessfully()
        {
            var result = _testWithObjectPropertyJsonElement.GetObjectProperty<TestSerializerClass>("ObjectProperty");
            Assert.IsType<TestSerializerClass>(result);
            Assert.True(result.ValueEquals(_testObject));
        }

        [Fact]
        public void GetObjectProperty_ValidJsonElementObjectCamelCase_ExtractsObjectPropertySuccessfully()
        {
            var result1 = _testWithObjectPropertyJsonElement.GetObjectProperty<TestSerializerClass>("camelCaseObjectProperty");
            var result2 = _testWithObjectPropertyJsonElement.GetObjectProperty<TestSerializerClass>("CamelCaseObjectProperty");

            Assert.IsType<TestSerializerClass>(result1);
            Assert.True(result1.ValueEquals(_testObject));
            Assert.IsType<TestSerializerClass>(result2);
            Assert.True(result2.ValueEquals(_testObject));
        }

        [Fact]
        public void GetBooleanProperty_ValidJsonElementObjectPascalCase_ExtractsBooleanPropertySuccessfully()
        {
            Assert.True(_objectJsonElement.GetBooleanProperty("BooleanValue"));
        }

        [Fact]
        public void GetBooleanProperty_ValidJsonElementObjectCamelCase_ExtractsBooleanPropertySuccessfully()
        {
            Assert.False(_objectJsonElement.GetBooleanProperty("camelCaseBooleanValue"));
            Assert.False(_objectJsonElement.GetBooleanProperty("CamelCaseBooleanValue"));
        }

        private class TestSerializerClass
        {
            public int IntValue { get; set; }

            public string StringValue { get; set; }

            public bool BooleanValue { get; set; }

            public bool camelCaseBooleanValue { get; set; }

            public string camelCaseStringValue { get; set; }

            public bool ValueEquals(TestSerializerClass other)
            {
                return IntValue == other.IntValue && StringValue == other.StringValue && camelCaseStringValue == other.camelCaseStringValue;
            }
        }

        private class TestWithObjectProperty
        {
            public object ObjectProperty { get; set; }

            public object camelCaseObjectProperty { get; set; }
        }
    }
}