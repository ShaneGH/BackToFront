using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront;

using BackToFront.UnitTests;

namespace BackToFront.Messing
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        public static void Testtt()
        {
            Rules.Add<Something, Test.ViolationClass>(trunk => trunk
                .If(b => b.Value1).IsEqualTo(0).ModelViolationIs(new Test.ViolationClass("Invalid"))
                .If(b => b.Value1).IsEqualTo(2).And(b => b.Value2).IsEqualTo(6)
                    .Then(branch1 => 
                    {
                        branch1.RequireThat(c => c.Value3).IsEqualTo(c => c.Value4).OrModelViolationIs(new Test.ViolationClass("Invalid"));

                        branch1.If(c => c.Value4).IsEqualTo(1).RequireThat(c => c.Value5).IsEqualTo(c => c.Value2).OrModelViolationIs(new Test.ViolationClass("Invalid"))
                            .If(c => c.Value4).IsEqualTo(0).RequireThat(c => c.Value5).IsEqualTo(8).OrModelViolationIs(new Test.ViolationClass("Invalid"));
                    })
                .If(b => b.Value4).IsEqualTo(0).RequireThat(b => b.Value5).IsEqualTo(8).OrModelViolationIs(new Test.ViolationClass("Invalid")));
        }
    }
}
