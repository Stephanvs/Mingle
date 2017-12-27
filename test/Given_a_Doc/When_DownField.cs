using System;
using FluentAssertions;
using LanguageExt;
using Machine.Specifications;

namespace Mingle.Tests
{
    [Subject(typeof(Doc))]
    public class When_DownField
    {
        Establish context = ()
            => Document = new Doc();

        Because of = ()
            => Subject = Document.DownField("key");

        It should_have_the_assigned_key = ()
            => Subject.Key.Should().Be("key");

        public static Doc Document;

        public static DownField Subject;
    }
}