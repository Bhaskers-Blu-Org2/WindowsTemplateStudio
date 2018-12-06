﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Templates.Core.Diagnostics;
using Microsoft.Templates.Core.Gen;
using Microsoft.Templates.Core.PostActions.Catalog.Merge;
using Microsoft.Templates.Fakes;
using Xunit;

namespace Microsoft.Templates.Core.Test.PostActions.Catalog
{
    [Trait("ExecutionSet", "Minimum")]
    public class MergeResourceDictionaryPostactionTest
    {
        [Fact]
        public async Task MergeResourceDictionary_ExecuteAsync()
        {
            var source = Path.GetFullPath(@".\TestData\Merge\Style.xaml");
            var postaction = Path.GetFullPath(@".\TestData\Merge\Style_postaction.xaml");
            var expected = File.ReadAllText(@".\TestData\Merge\Style_expected.xaml").Replace("\r\n", string.Empty).Replace("\n", string.Empty);

            var config = new MergeConfiguration(postaction, true);

            var mergeResourceDictionaryPostAction = new MergeResourceDictionaryPostAction("Test", config);
            await mergeResourceDictionaryPostAction.ExecuteAsync();

            var result = File.ReadAllText(source).Replace("\r\n", string.Empty).Replace("\n", string.Empty);

            Assert.Equal(result, expected);
        }

        [Fact]
        public async Task MergeResourceDictionaryPostaction_FailingAsync()
        {
            var source = Path.GetFullPath(@".\TestData\Merge\Style_fail.xaml");
            var postaction = Path.GetFullPath(@".\TestData\Merge\Style_fail_postaction.xaml");
            var expected = File.ReadAllText(@".\TestData\Merge\Style_expected.xaml");

            GenContext.Current = new FakeContextProvider
            {
                ProjectName = "TestResourceDictionaryPostAction",
                DestinationPath = Directory.GetCurrentDirectory(),
            };

            var config = new MergeConfiguration(postaction, true);

            var mergeResourceDictionaryPostAction = new MergeResourceDictionaryPostAction("TestTemplate", config);

            Exception ex = await Assert.ThrowsAsync<Exception>(async () => await mergeResourceDictionaryPostAction.ExecuteAsync());
            Assert.NotNull(ex.InnerException);
            Assert.Equal(typeof(System.IO.InvalidDataException), ex.InnerException.GetType());
            Assert.Equal(string.Format(Resources.StringRes.PostActionException, "Microsoft.Templates.Core.PostActions.Catalog.Merge.MergeResourceDictionaryPostAction", "TestTemplate"), ex.Message);
        }
    }
}
