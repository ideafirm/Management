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

using Microsoft.Extensions.Configuration;
using Steeltoe.Management.Endpoint.Test;
using System;
using System.Collections.Generic;
using Xunit;

namespace Steeltoe.Management.Endpoint.Info.Test
{
    public class InfoOptionsTest : BaseTest
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            InfoOptions opts = new InfoOptions();
            Assert.True(opts.Enabled);
            Assert.False(opts.Sensitive);
            Assert.Equal("info", opts.Id);
        }

        [Fact]
        public void Constructor_ThrowsIfConfigNull()
        {
            IConfiguration config = null;
            Assert.Throws<ArgumentNullException>(() => new InfoOptions(config));
        }

        [Fact]
        public void Constructor_BindsConfigurationCorrectly()
        {
            var appsettings = new Dictionary<string, string>()
            {
                ["management:endpoints:enabled"] = "false",
                ["management:endpoints:sensitive"] = "false",
                ["management:endpoints:path"] = "/management",
                ["management:endpoints:info:enabled"] = "false",
                ["management:endpoints:info:sensitive"] = "false",
                ["management:endpoints:info:id"] = "infomanagement"
            };
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(appsettings);
            var config = configurationBuilder.Build();

            InfoOptions opts = new InfoOptions(config);
            Assert.False(opts.Enabled);
            Assert.False(opts.Sensitive);
            Assert.Equal("infomanagement", opts.Id);

            Assert.NotNull(opts.Global);
            Assert.False(opts.Global.Enabled);
            Assert.False(opts.Global.Sensitive);
            Assert.Equal("/management", opts.Global.Path);
        }
    }
}
