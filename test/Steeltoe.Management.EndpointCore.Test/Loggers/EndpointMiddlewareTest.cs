﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Steeltoe.Extensions.Logging;
using Steeltoe.Management.Endpoint.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Steeltoe.Management.Endpoint.Loggers.Test
{
    public class EndpointMiddlewareTest : BaseTest
    {
        private static Dictionary<string, string> appsettings = new Dictionary<string, string>()
        {
            ["Logging:IncludeScopes"] = "false",
            ["Logging:LogLevel:Default"] = "Warning",
            ["Logging:LogLevel:Pivotal"] = "Information",
            ["Logging:LogLevel:Steeltoe"] = "Information",
            ["management:endpoints:enabled"] = "true",
            ["management:endpoints:sensitive"] = "false",
            ["management:endpoints:path"] = "/cloudfoundryapplication",
            ["management:endpoints:loggers:enabled"] = "true",
            ["management:endpoints:loggers:sensitive"] = "false",
        };

        [Fact]
        public void IsLoggersRequest_ReturnsExpected()
        {
            var opts = new LoggersOptions();

            var ep = new LoggersEndpoint(opts, null);
            var middle = new LoggersEndpointMiddleware(null, ep);

            var context = CreateRequest("GET", "/loggers");
            Assert.True(middle.IsLoggerRequest(context));

            var context2 = CreateRequest("PUT", "/loggers");
            Assert.False(middle.IsLoggerRequest(context2));

            var context3 = CreateRequest("GET", "/badpath");
            Assert.False(middle.IsLoggerRequest(context3));

            var context4 = CreateRequest("POST", "/loggers");
            Assert.True(middle.IsLoggerRequest(context4));

            var context5 = CreateRequest("POST", "/badpath");
            Assert.False(middle.IsLoggerRequest(context5));

            var context6 = CreateRequest("POST", "/loggers/Foo.Bar.Class");
            Assert.True(middle.IsLoggerRequest(context6));

            var context7 = CreateRequest("POST", "/badpath/Foo.Bar.Class");
            Assert.False(middle.IsLoggerRequest(context7));
        }

        [Fact]
        public async void HandleLoggersRequestAsync_ReturnsExpected()
        {
            var opts = new LoggersOptions();

            var ep = new TestLoggersEndpoint(opts);
            var middle = new LoggersEndpointMiddleware(null, ep);
            var context = CreateRequest("GET", "/loggers");
            await middle.HandleLoggersRequestAsync(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader rdr = new StreamReader(context.Response.Body);
            string json = await rdr.ReadToEndAsync();
            Assert.Equal("{}", json);
        }

        [Fact]
        public async void LoggersActuator_ReturnsExpectedData()
        {
            var builder = new WebHostBuilder()
               .UseStartup<Startup>()
               .ConfigureAppConfiguration((builderContext, config) => config.AddInMemoryCollection(appsettings))
               .ConfigureLogging((context, loggingBuilder) => loggingBuilder.AddDynamicConsole(context.Configuration));

            using (var server = new TestServer(builder))
            {
                var client = server.CreateClient();
                var result = await client.GetAsync("http://localhost/cloudfoundryapplication/loggers");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                var json = await result.Content.ReadAsStringAsync();
                Assert.NotNull(json);

                var loggers = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                Assert.NotNull(loggers);
                Assert.True(loggers.ContainsKey("levels"));
                Assert.True(loggers.ContainsKey("loggers"));

                // at least one logger should be returned
                Assert.True(loggers["loggers"].ToString().Length > 2);

                // parse the response into a dynamic object, verify that Default was returned and configured at Warning
                dynamic parsedObject = JsonConvert.DeserializeObject(json);
                Assert.Equal("WARN", parsedObject.loggers.Default.configuredLevel.ToString());
            }
        }

        [Fact]
        public async void LoggersActuator_AcceptsPost()
        {
             var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) => config.AddInMemoryCollection(appsettings))
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    loggingBuilder.AddDynamicConsole();
                });

            using (var server = new TestServer(builder))
            {
                var client = server.CreateClient();
                HttpContent content = new StringContent("{\"configuredLevel\":\"ERROR\"}");
                var changeResult = await client.PostAsync("http://localhost/cloudfoundryapplication/loggers/Default", content);
                Assert.Equal(HttpStatusCode.OK, changeResult.StatusCode);

                var validationResult = await client.GetAsync("http://localhost/cloudfoundryapplication/loggers");
                var json = await validationResult.Content.ReadAsStringAsync();
                dynamic parsedObject = JsonConvert.DeserializeObject(json);
                Assert.Equal("ERROR", parsedObject.loggers.Default.effectiveLevel.ToString());
            }
        }

        [Fact]
        public async void LoggersActuator_UpdateNameSpace_UpdatesChildren()
        {
            var builder = new WebHostBuilder()
               .UseStartup<Startup>()
               .ConfigureAppConfiguration((builderContext, config) => config.AddInMemoryCollection(appsettings))
               .ConfigureLogging((context, loggingBuilder) =>
               {
                   loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                   loggingBuilder.AddDynamicConsole();
               });

            using (var server = new TestServer(builder))
            {
                var client = server.CreateClient();
                HttpContent content = new StringContent("{\"configuredLevel\":\"TRACE\"}");
                var changeResult = await client.PostAsync("http://localhost/cloudfoundryapplication/loggers/Steeltoe", content);
                Assert.Equal(HttpStatusCode.OK, changeResult.StatusCode);

                var validationResult = await client.GetAsync("http://localhost/cloudfoundryapplication/loggers");
                var json = await validationResult.Content.ReadAsStringAsync();
                dynamic parsedObject = JsonConvert.DeserializeObject(json);
                Assert.Equal("TRACE", parsedObject.loggers.Steeltoe.effectiveLevel.ToString());
                Assert.Equal("TRACE", parsedObject.loggers["Steeltoe.Management"].effectiveLevel.ToString());
                Assert.Equal("TRACE", parsedObject.loggers["Steeltoe.Management.Endpoint"].effectiveLevel.ToString());
                Assert.Equal("TRACE", parsedObject.loggers["Steeltoe.Management.Endpoint.Loggers"].effectiveLevel.ToString());
                Assert.Equal("TRACE", parsedObject.loggers["Steeltoe.Management.Endpoint.Loggers.LoggersEndpointMiddleware"].effectiveLevel.ToString());
            }
        }

        private HttpContext CreateRequest(string method, string path)
        {
            HttpContext context = new DefaultHttpContext
            {
                TraceIdentifier = Guid.NewGuid().ToString()
            };
            context.Response.Body = new MemoryStream();
            context.Request.Method = method;
            context.Request.Path = new PathString(path);
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("localhost");
            return context;
        }
    }
}
