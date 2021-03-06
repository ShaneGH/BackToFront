﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront;
using BackToFront.Logic;
using BackToFront.Validate;
using NUnit.Framework;

using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Meta;
using System.IO;
using BackToFront.Framework;

namespace BackToFront.Tests
{
    //[TestFixture]
    public class Testpad
    {
        public class ViolationClass : IViolation
        {
            private readonly string _UserMessage;
            public ViolationClass(string userMessage)
            {
                _UserMessage = userMessage;
            }

            public string UserMessage
            {
                get { return _UserMessage; }
            }

            public object ViolatedEntity
            {
                get;
                set;
            }

            public IEnumerable<MemberChainItem> Violated
            {
                get;
                set;
            }
        }

        public static Domain Repository;

        public static void SetupTestpad()
        {
            Repository.AddRule<Something>(trunk => trunk
                .RequireThat(b => b.Value1 == 0).WithModelViolation(() => new ViolationClass("Invalid")));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value1 != 0).RequirementFailed.WithModelViolation(() => new ViolationClass("Invalid")));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value1 == 2 && b.Value2 == 6)
                    .Then(branch1 =>
                    {
                        branch1.RequireThat(c => c.Value3 == c.Value4).WithModelViolation(() => new ViolationClass("Invalid"));
                        branch1.If(c => c.Value4 == 1).RequireThat(c => c.Value5 == 8 || c.Value5 == 8).WithModelViolation(() => new ViolationClass("Invalid"));
                        branch1.If(c => c.Value4 == 0).RequireThat(c => c.Value5 == 8).WithModelViolation(() => new ViolationClass("Invalid"));
                    }));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value4 == 0).RequireThat(b => b.Value5 == 8).WithModelViolation(() => new ViolationClass("Invalid")));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value4 == 1 && (b.Value5 == 3 && b.Value5 == 7) && b.Value1 == 7));

            var violation = new Something().Validate(Repository)
                .WithMockedParameter(a => a.Value3, 4)
                .WithMockedParameter(a => a.Value5, 9).FirstViolation;
        }

        public static void SetupTestpadWithRepository()
        {
            Repository.AddRule<Something, IRepository>((trunk, repo) => trunk
                .RequireThat(b => repo.Val.GetValues().Contains(b.Value1)).WithModelViolation(() => new ViolationClass("Invalid")));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value1 != 0).RequirementFailed.WithModelViolation(() => new ViolationClass("Invalid")));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value1 == 2 && b.Value2 == 6)
                    .Then(branch1 =>
                    {
                        branch1.RequireThat(c => c.Value3 == c.Value4).WithModelViolation(() => new ViolationClass("Invalid"));
                        branch1.If(c => c.Value4 == 1).RequireThat(c => c.Value5 == 8 || c.Value5 == 8).WithModelViolation(() => new ViolationClass("Invalid"));
                        branch1.If(c => c.Value4 == 0).RequireThat(c => c.Value5 == 8).WithModelViolation(() => new ViolationClass("Invalid"));
                    }));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value4 == 0).RequireThat(b => b.Value5 == 8).WithModelViolation(() => new ViolationClass("Invalid")));

            Repository.AddRule<Something>(trunk => trunk
                .If(b => b.Value4 == 1 && (b.Value5 == 3 && b.Value5 == 7) && b.Value1 == 7));

            var violation = new Something().Validate(Repository)
                .WithMockedParameter(a => a.Value3, 4)
                .WithMockedParameter(a => a.Value5, 9).FirstViolation;
        }
    }

    public interface IRepository
    {
        IEnumerable<int> GetValues();
    }

    public class Something
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public int Value4 { get; set; }
        public int Value5 { get; set; }
    }
}