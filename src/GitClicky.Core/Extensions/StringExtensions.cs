using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitClicky.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> value, string seperator)
        {
            return string.Join(seperator, value);
        }

        public static string FormatWith(this string format, params object[] args)
        {
            args = args ?? new object[0];

            var distinctNumberedTemplateMatches =
                (from object match in new Regex(@"\{\d{1,2}\}").Matches(format) select match.ToString())
                .Distinct().Count();
            if (distinctNumberedTemplateMatches != args.Length)
            {
                var argsDic = GetDictionaryFromAnonObject(args[0]);

                if (argsDic.Count < 1)
                {
                    throw new InvalidOperationException("Please supply enough args for the numbered templates or use an anonymous object to identify the templates by name.");
                }

                return argsDic.Aggregate(format, (current, o) => current.Replace("{{0}}".FormatWith(o.Key), o.Value.ToString()));
            }

            var validationInput = format;
            for (var i = 0; i < args.Length; i++)
            {
                format = format.Replace("{" + i + "}", args[i] == null ? string.Empty : args[i].ToString());
            }
            if (validationInput == format)
            {
                throw new InvalidOperationException(
                    "You can not mix template types. Use numbered templates or named ones with an anonymous object.");
            }

            return format;
        }

        private static IDictionary<string, object> GetDictionaryFromAnonObject(object args)
        {
            if (args == null)
            {
                return new Dictionary<string, object>();
            }

            return TypeDescriptor.GetProperties(args).Cast<PropertyDescriptor>()
                .ToDictionary(
                    property => property.Name, 
                    property => property.GetValue(args));
        }
    }
}
