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
        public PathElementMeta[] Rules { get; set; }
    }

    public static class HtmlHelper
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(RuleCollection));

        public static void RulesForModel<TModel>(this HtmlHelper<TModel> helper)
        {
            WriteRulesToStream<TModel>(helper.ViewContext.HttpContext.Response.OutputStream);
        }

        public static MvcHtmlString RenderRulesForModel<TModel>(this HtmlHelper<TModel> helper, bool includeScriptTags = true)
        {
            using (var stream = new MemoryStream())
            {
                WriteRulesToStream<TModel>(stream);
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
        private static void WriteRulesToStream<TEntity>(Stream stream)
        {
            Rules<TEntity>.ParentClassRepositories.Aggregate();
            var rules = new RuleCollection
            {
                Entity = typeof(TEntity).FullName,
                Rules = Rules<TEntity>.Repository.Registered.Concat(Rules<TEntity>.ParentClassRepositories.Aggregate()).Select(r => r.Meta).ToArray()
            };

            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("<script type=\"text/javascript\">");
                writer.WriteLine("if(!__BTF || !__BTF.RegisterRule || __BTF.RegisterRule.constructor !== Function) throw 'BackToFront has not been initialised';");
                writer.Write("__BTF.RegisterRule(");
                Serializer.WriteObject(stream, rules);
                writer.WriteLine(");");
                writer.WriteLine("</script>");

                writer.Flush();
            }
        }
    }
}
