using NUnit.Framework;
using AIInterviewer.ServiceInterface.Utilities;

namespace AIInterviewer.Tests;

public class AiSchemaGeneratorTests
{
    private class SimpleObj
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Test]
    public void Can_Generate_Schema_For_Simple_Object()
    {
        var schema = AiSchemaGenerator.Generate(typeof(SimpleObj));
        Assert.That(schema.Type, Is.EqualTo("object"));
        Assert.That(schema.Properties, Contains.Key("Name"));
        Assert.That(schema.Properties["Name"].Type, Is.EqualTo("string"));
        Assert.That(schema.Properties, Contains.Key("Age"));
        Assert.That(schema.Properties["Age"].Type, Is.EqualTo("integer"));
        Assert.That(schema.Required, Contains.Item("Name"));
        Assert.That(schema.Required, Contains.Item("Age"));
    }
}
