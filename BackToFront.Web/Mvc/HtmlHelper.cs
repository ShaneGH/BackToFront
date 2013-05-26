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

namespace BackToFront.Web.Mvc
{
    [DataContract]
    public class RuleCollection
    {
        [DataMember]
        public string Entity { get; set; }

        [DataMember]
        public List<RuleMeta> Rules { get; set; }
    }

    public static class HtmlHelper
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(RuleCollection));

        public static void RulesForModel<TModel>(this HtmlHelper<TModel> helper, Repository repository, bool includeScriptTags = true)
        {
            WriteRulesToStream<TModel>(repository, helper.ViewContext.HttpContext.Response.OutputStream, includeScriptTags);
        }

        public static MvcHtmlString RenderRulesForModel<TModel>(this HtmlHelper<TModel> helper, Repository repository, bool includeScriptTags = true)
        {
            using (var stream = new MemoryStream())
            {
                WriteRulesToStream<TModel>(repository, stream, includeScriptTags);
                stream.Position = 0;
                using (var r = new StreamReader(stream))
                {
                    return new MvcHtmlString(r.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// Write the rules in Json form to the given stream
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="stream"></param>
        private static void WriteRulesToStream<TEntity>(Repository repository, Stream stream, bool includeScriptTags)
        {
            WriteRulesToStream<TEntity>(new[] { repository }, stream, includeScriptTags);
        }

        private static void WriteRulesToStream<TEntity>(IEnumerable<Repository> repositories, Stream stream, bool includeScriptTags)
        {
            var type = typeof(TEntity);
            var rules = new RuleCollection
            {
                Entity = type.FullName,
                Rules = new List<RuleMeta>()
            };

            while (type != null)
            {
                rules.Rules.AddRange(repositories.Select(r => r.Rules(type).Select(a => a.Meta)).Aggregate());
                type = type.BaseType;
            }

            using (var writer = new StreamWriter(stream))
            {
                if (includeScriptTags)
                    writer.WriteLine("<script type=\"text/javascript\">");
                writer.WriteLine("if(!__BTF || !__BTF.RegisterRule || __BTF.RegisterRule.constructor !== Function) throw 'BackToFront has not been initialised';");
                writer.Write("__BTF.RegisterRule(");
                Serializer.WriteObject(stream, rules);
                writer.WriteLine(");");

                if (includeScriptTags)
                    writer.WriteLine("</script>");

                writer.Flush();
            }
        }
    }
}
