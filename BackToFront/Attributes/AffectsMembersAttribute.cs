using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class AffectsMembersAttribute : Attribute
    {
        private static readonly object Threadlock = new object();
        private static readonly Regex MemberQualifier = new Regex(@"^([a-zA-Z_][a-zA-Z0-9_]*\.)*[a-zA-Z_][a-zA-Z0-9_]*$");
        IDictionary<Type, IEnumerable<MemberInfo>> _cache = new Dictionary<Type, IEnumerable<MemberInfo>>();

        private readonly IEnumerable<string> _MemberNames;

        /// <summary>
        /// The names of the properties which this Method/Property affects. properties of properties can be delimited with a "."
        /// </summary>
        /// <param name="members"></param>
        public AffectsMembersAttribute(params string[] members)
        {
            _MemberNames = (members ?? Enumerable.Empty<string>()).ToArray();
            _MemberNames.Each(a =>
            {
                if (!MemberQualifier.IsMatch(a))
                    throw new InvalidOperationException("##");
            });
        }

        /// <summary>
        /// Should only really be called for the type which owns the method/property this attribute applies to.
        /// </summary>
        /// <param name="forType"></param>
        /// <returns></returns>
        internal IEnumerable<MemberInfo> GetMembers(Type forType)
        {
            lock (Threadlock)
            {
                if (!_cache.ContainsKey(forType))
                {
                    IList<MemberInfo> members = new List<MemberInfo>();
                    foreach (var memberNames in _MemberNames.Select(m => new Queue<string>(m.Split('.'))))
                    {
                        var current = forType;
                        while (memberNames.Count() != 0)
                        {
                            var m = current.GetMember(memberNames.Dequeue());
                            if (m.Count() == 1)
                            {
                                var member = m.First();
                                if (member is PropertyInfo)
                                {
                                    current = (member as PropertyInfo).PropertyType;
                                    members.Add(member);
                                    continue;
                                }
                                else if (member is FieldInfo)
                                {
                                    current = (member as FieldInfo).FieldType;
                                    members.Add(member);
                                    continue;
                                }
                            }

                            //invallid member
                            throw new InvalidOperationException("##");
                        }
                    }

                    _cache[forType] = members.Distinct();
                }
            }

            return _cache[forType];
        }
    }
}
