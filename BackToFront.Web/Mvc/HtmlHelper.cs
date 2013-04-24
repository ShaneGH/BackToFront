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
        public IEnumerable<PathElementMeta> Rules { get; set; }
    }

    public static class HtmlHelper
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(RuleCollection));

        public static void RulesForModel<TModel>(this HtmlHelper<TModel> helper)
        {
            WriteRulesToStream<TModel>(helper.ViewContext.HttpContext.Response.OutputStream);
        }

        public static MvcHtmlString RenderRulesForModel<TModel>(this HtmlHelper<TModel> helper)
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

        private static void WriteRulesToStream<TEntity>(Stream stream)
        {
            var rules = new RuleCollection
            {
                Entity = typeof(TEntity).FullName,
                Rules = Rules<TEntity>.ParentClassRepositories.Concat(new[] { Rules<TEntity>.Repository.Registered }).Aggregate().Select(r => r.Meta)
            };

            Serializer.WriteObject(stream, rules);
        }
    }
}
