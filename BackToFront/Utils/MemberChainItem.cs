﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils
{
    public class MemberChainItem
    {
        public readonly MemberInfo Member;
        public MemberChainItem _NextItem;
        public MemberChainItem NextItem
        {
            get
            {
                return _NextItem;
            }
            set
            {

                if (value != null && value.Member.DeclaringType != Member.MemberType())
                    throw new InvalidOperationException("##");

                _NextItem = value;
            }
        }

        public MemberChainItem(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            Member = member;
        }

        public void SetNext(MemberInfo member)
        {
            NextItem = new MemberChainItem(member);
        }
    }
}
