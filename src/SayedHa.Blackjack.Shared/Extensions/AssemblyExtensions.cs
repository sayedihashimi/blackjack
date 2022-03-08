using SayedHa.Blackjack.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Extensions {
    public static class AssemblyExtensions {
        public static Stream GetEmbeddedResourceStream(this Assembly assembly, string relativeResourcePath) {
            if (string.IsNullOrEmpty(relativeResourcePath))
                throw new ArgumentNullException("relativeResourcePath");

            var resourcePath = String.Format("{0}.{1}",
                Regex.Replace(assembly.ManifestModule.Name, @"\.(exe|dll)$",
                      string.Empty, RegexOptions.IgnoreCase), relativeResourcePath);

            var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
                throw new ArgumentException(String.Format("The specified embedded resource \"{0}\" is not found.", relativeResourcePath));
            return stream;
        }
    }
}
