using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Meta;
using System.Runtime.Serialization;
using WebExpressions.Meta;
using BackToFront.Utilities;

namespace BackToFront.Web.Mvc
{
    public static class HtmlHelper
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(RuleCollectionMeta), new DataContractJsonSerializerSettings
        {
            KnownTypes = ExpressionMeta.MetaTypes.Union(new[] { typeof(MemberChainItem[]) }),
            EmitTypeInformation = EmitTypeInformation.Always
        });

        public static void RulesForModel<TModel>(this HtmlHelper<TModel> helper, Repository repository, bool includeScriptTags = true)
        {
            WriteRulesToStream<TModel>(repository, helper.ViewContext.HttpContext.Response.OutputStream, includeScriptTags).Dispose();
        }

        public static MvcHtmlString RenderRulesForModel<TModel>(this HtmlHelper<TModel> helper, Repository repository, bool includeScriptTags = true)
        {
            using (var stream = new MemoryStream())
            {
                using (WriteRulesToStream<TModel>(repository, stream, includeScriptTags))
                {
                    stream.Position = 0;
                    using (var r = new StreamReader(stream))
                    {
                        return new MvcHtmlString(r.ReadToEnd());
                    }
                }
            }
        }

        /// <summary>
        /// Write the rules in Json form to the given stream
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="stream"></param>
        private static IDisposable WriteRulesToStream<TEntity>(Repository repository, Stream stream, bool includeScriptTags)
        {
            return WriteRulesToStream<TEntity>(new[] { repository }, stream, includeScriptTags);
        }

        private static IDisposable WriteRulesToStream<TEntity>(IEnumerable<Repository> repositories, Stream stream, bool includeScriptTags)
        {
            var type = typeof(TEntity);
            var rules = new RuleCollectionMeta
            {
                Entity = type.FullName,
                Rules = new List<RuleMeta>()
            };

            while (type != null)
            {
                rules.Rules.AddRange(repositories.Select(r => r.Rules(type).Select(a => a.Meta.Meta)).Aggregate());
                type = type.BaseType;
            }

            var writer = new StreamWriter(stream);
            if (includeScriptTags)
                writer.WriteLine("<script type=\"text/javascript\">");

            writer.WriteLine("if(!__BTF || !__BTF.Validation || !__BTF.Validation.JQueryValidator || !__BTF.Validation.JQueryValidator.RegisterRule || __BTF.Validation.JQueryValidator.RegisterRule.constructor !== Function) throw 'BackToFront has not been initialised';");
            writer.Write("__BTF.Validation.JQueryValidator.RegisterRule(");
            writer.Flush();
            Serializer.WriteObject(stream, rules);
            writer.WriteLine(");");

            if (includeScriptTags)
                writer.WriteLine("</script>");

            writer.Flush();
            return writer;
        }
    }
}
