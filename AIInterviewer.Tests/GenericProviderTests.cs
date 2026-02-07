using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using AIInterviewer.ServiceInterface.Providers;
using AIInterviewer.ServiceModel.Types.Ai;
using AIInterviewer.ServiceModel.Tables.Configuration;

namespace AIInterviewer.Tests;

[TestFixture]
[Category("Integration")]
[Explicit("Requires configured user secrets")]
public class GenericProviderTests
{
    private IConfiguration _configuration;

    [OneTimeSetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<GenericProviderTests>();

        _configuration = builder.Build();
    }

    [Test]
    public async Task Gemini_GenerateSchema_IntegrationTest()
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            Assert.Ignore("Gemini:ApiKey secret not configured. Use `dotnet user-secrets set \"Gemini:ApiKey\" \"<API_KEY>\"` to run.");
        }

        var config = new AiServiceConfig
        {
             ApiKey = apiKey,
             ModelId = _configuration["Gemini:ModelId"],
             Name = "TestGemini",
             ProviderType = "Gemini"
        };
        
        var provider = new GeminiAiProvider(config, NullLogger<GeminiAiProvider>.Instance);
        
        var schema = new AiSchemaDefinition
        {
             Description = "A simple person object",
             Properties = new Dictionary<string, AiSchemaDefinition>
             {
                 { "name", new AiSchemaDefinition { Type = "string", Description = "Name of the person" } },
                 { "age", new AiSchemaDefinition { Type = "integer", Description = "Age of the person" } }
             },
             Required = new List<string> { "name", "age" }
        };
        
        var prompt = "Generate a JSON for a person named Alice who is 25 years old.";
        
        var result = await provider.GenerateJsonAsync<PersonTest>(prompt, schema, "Person");
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Alice"));
        Assert.That(result.Age, Is.EqualTo(25));
    }

    [Test]
    public async Task OpenAI_GenerateSchema_IntegrationTest()
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            Assert.Ignore("OpenAI:ApiKey secret not configured. Use `dotnet user-secrets set \"OpenAI:ApiKey\" \"<API_KEY>\"` to run.");
        }

         var config = new AiServiceConfig
        {
             ApiKey = apiKey,
             ModelId = _configuration["OpenAI:ModelId"] ?? "gpt-4o",
             Name = "TestOpenAI",
             ProviderType = "OpenAI"
        };

        var provider = new OpenAiProvider(config, NullLogger<OpenAiProvider>.Instance);
        
        var schema = new AiSchemaDefinition
        {
             Description = "A simple person object",
             Properties = new Dictionary<string, AiSchemaDefinition>
             {
                 { "name", new AiSchemaDefinition { Type = "string", Description = "Name of the person" } },
                 { "age", new AiSchemaDefinition { Type = "integer", Description = "Age of the person" } }
             },
             Required = new List<string> { "name", "age" }
        };
        
        var prompt = "Generate a JSON for a person named Bob who is 40 years old.";
        
        var result = await provider.GenerateJsonAsync<PersonTest>(prompt, schema, "Person");
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Bob"));
        Assert.That(result.Age, Is.EqualTo(40));
    }

    public class PersonTest
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
