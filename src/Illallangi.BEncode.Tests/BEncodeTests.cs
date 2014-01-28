namespace Illallangi.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class BEncodeTests
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(@"i3e", typeof(int)).Returns(3);
                yield return new TestCaseData(@"i34e", typeof(int)).Returns(34);
                yield return new TestCaseData(@"i345e", typeof(int)).Returns(345);

                yield return new TestCaseData(@"4:spam", typeof(string)).Returns(@"spam");
                yield return new TestCaseData(@"4:spamspam", typeof(string)).Returns(@"spam");

                yield return new TestCaseData(@"le", typeof(List<dynamic>)).Returns(new List<dynamic>());

                yield return new TestCaseData(@"12:spamspamspam", typeof(string)).Returns(@"spamspamspam");

                yield return new TestCaseData(@"l4:spam4:eggse", typeof(List<dynamic>)).Returns(new List<dynamic> { "spam", "eggs" });

                yield return new TestCaseData(@"de", typeof(Dictionary<string, dynamic>)).Returns(new Dictionary<string, dynamic>());

                yield return new TestCaseData(@"d3:cow3:moo4:spam4:eggse", typeof(Dictionary<string, dynamic>))
                                        .Returns(
                                            new Dictionary<string, dynamic>
                                                {
                                                    { "cow", "moo" }, 
                                                    { "spam", "eggs" },
                                                });

                yield return new TestCaseData(@"d4:spaml1:a1:bee'", typeof(Dictionary<string, dynamic>))
                                        .Returns(
                                            new Dictionary<string, dynamic>
                                                {
                                                    { "spam", new List<dynamic> { "a", "b" } },
                                                });

                yield return new TestCaseData(@"d9:publisher3:bob17:publisher-webpage15:www.example.com18:publisher.location4:homee", typeof(Dictionary<string, dynamic>))
                                        .Returns(
                                            new Dictionary<string, dynamic>
                                                {
                                                    { "publisher", "bob" },
                                                    { "publisher-webpage", "www.example.com" },
                                                    { "publisher.location", "home" },
                                                });
            }
        }

        [Test, TestCaseSource("TestCases")]
        public dynamic CanRead(string text, Type t)
        {
            var result = BEncode.ReadText(text);
            Assert.IsInstanceOfType(t, result);
            return result;
        }

        [Test, ExpectedException(typeof(FormatException))]
        public void ThrowsFormatException()
        {
            BEncode.ReadText("q=");
        }
    }
}
