using BackToFront.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Tests.Base
{
    public class RulesRepositoryTestBase : TestBase
    {
        public Repository Repository;

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();
            Repository = new Repository();
        }

        public void AddRule<TEntity>(Action<IRule<TEntity>> rule)
        {
            Repository.AddRule<TEntity>(rule);
        }

        public void AddRule<TEntity, TDependency>(Action<IRule<TEntity>, DependencyWrapper<TDependency>> rule)
            where TDependency : class

        {
            Repository.AddRule<TEntity, TDependency>(rule);
        }
    }
}
