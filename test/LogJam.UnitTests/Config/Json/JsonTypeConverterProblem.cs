// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTypeConverterProblem.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Diagnostics.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Xunit;
using Xunit.Abstractions;


public class A
{

    public string Id { get; set; }
    public A Child { get; set; }

}


public class B : A
{

}


public class C : A
{

}


/// <summary>
/// Shows the problem I'm having serializing classes with Json.
/// </summary>
public sealed class JsonTypeConverterProblem
{

    private readonly ITestOutputHelper _testOutputHelper;

    public JsonTypeConverterProblem(ITestOutputHelper testOutputHelper)
    {
        Contract.Requires<ArgumentNullException>(testOutputHelper != null);

        _testOutputHelper = testOutputHelper;
    }

    [Fact(Skip = "Bug still exists")]
    public void ShowSerializationBug()
    {
        A a = new B()
              {
                  Id = "foo",
                  Child = new C()
                          {
                              Id = "bar"
                          }
              };

        JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
        jsonSettings.ContractResolver = new TypeHintContractResolver();
        string json = JsonConvert.SerializeObject(a, Formatting.Indented, jsonSettings);
        _testOutputHelper.WriteLine(json);

        Assert.Contains(@"""Target"": ""B""", json);
        Assert.Contains(@"""Is"": ""C""", json);
    }

    [Fact]
    public void DeserializationWorks()
    {
        string json =
            @"{
  ""Target"": ""B"",
  ""Id"": ""foo"",
  ""Child"": { 
		""Is"": ""C"",
		""Id"": ""bar"",
	}
}";

        JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
        jsonSettings.ContractResolver = new TypeHintContractResolver();
        A a = JsonConvert.DeserializeObject<A>(json, jsonSettings);

        Assert.IsType<B>(a);
        Assert.IsType<C>(a.Child);
    }

}


public class TypeHintContractResolver : DefaultContractResolver
{

    public override JsonContract ResolveContract(Type type)
    {
        JsonContract contract = base.ResolveContract(type);
        if ((contract is JsonObjectContract)
            && ((type == typeof(A)) || (type == typeof(B)))) // In the real implementation, this is checking against a registry of types
        {
            contract.Converter = new TypeHintJsonConverter(type);
        }
        return contract;
    }

}


public class TypeHintJsonConverter : JsonConverter
{

    private readonly Type _declaredType;

    public TypeHintJsonConverter(Type declaredType)
    {
        _declaredType = declaredType;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == _declaredType;
    }

    // The real implementation of the next 2 methods uses reflection on concrete types to determine the declaredType hint.
    // TypeFromTypeHint and TypeHintPropertyForType are the inverse of each other.

    private Type TypeFromTypeHint(JObject jo)
    {
        if (new JValue("B").Equals(jo["Target"]))
        {
            return typeof(B);
        }
        else if (new JValue("A").Equals(jo["Hint"]))
        {
            return typeof(A);
        }
        else if (new JValue("C").Equals(jo["Is"]))
        {
            return typeof(C);
        }
        else
        {
            throw new ArgumentException("Type not recognized from JSON");
        }
    }

    private JProperty TypeHintPropertyForType(Type type)
    {
        if (type == typeof(A))
        {
            return new JProperty("Hint", "A");
        }
        else if (type == typeof(B))
        {
            return new JProperty("Target", "B");
        }
        else if (type == typeof(C))
        {
            return new JProperty("Is", "C");
        }
        else
        {
            return null;
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (! CanConvert(objectType))
        {
            throw new InvalidOperationException("Can't convert declaredType " + objectType + "; expected " + _declaredType);
        }

        // Load JObject from stream.  Turns out we're also called for null arrays of our objects,
        // so handle a null by returning one.
        var jToken = JToken.Load(reader);
        if (jToken.Type == JTokenType.Null)
        {
            return null;
        }
        if (jToken.Type != JTokenType.Object)
        {
            throw new InvalidOperationException("Json: expected " + _declaredType + "; got " + jToken.Type);
        }
        JObject jObject = (JObject) jToken;

        // Select the declaredType based on TypeHint
        Type deserializingType = TypeFromTypeHint(jObject);

        var target = Activator.CreateInstance(deserializingType);
        serializer.Populate(jObject.CreateReader(), target);
        return target;
    }

    public override bool CanWrite { get { return true; } }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JProperty typeHintProperty = TypeHintPropertyForType(value.GetType());

        //BUG: JsonSerializationException : Self referencing loop detected with type 'B'. Path ''.
        // Same error occurs whether I use the serializer parameter or a separate serializer.
        JObject jo = JObject.FromObject(value, serializer);
        if (typeHintProperty != null)
        {
            jo.AddFirst(typeHintProperty);
        }
        writer.WriteToken(jo.CreateReader());
    }

}
