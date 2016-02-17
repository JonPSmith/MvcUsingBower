#region licence
// ======================================================================================
// Mvc5WithBowerAndGrunt - An example of how to change a MVC5 project to Bower and Grunt
// Filename: CdnInfo.cs
// Date Created: 2016/02/08
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// ======================================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace B4BCore.Internal
{
    /// <summary>
    /// This contains the object information and methods for handling a cdn
    /// </summary>
    internal class CdnInfo 
    {
        private readonly string _bundleName;
        private readonly JObject _jObject;

        private const string CdnHtmlIncludeFileUrlReplaceString = "{fileUrl}";
        internal const string CdnObjectDevelopmentPropertyName = "development";

        internal const string CdnObjectProductionPropertyName = "production";

        /// <summary>
        /// The name of the file to use in debug mode.
        /// </summary>
        public string Development => _jObject[CdnObjectDevelopmentPropertyName] != null &&
                                     _jObject[CdnObjectDevelopmentPropertyName].Type == JTokenType.String
            ? _jObject[CdnObjectDevelopmentPropertyName].Value<string>()
            : null;

        /// <summary>
        /// The backup file to use in production mode if the CdnSuccessTest fails
        /// </summary>
        public string Production => _jObject[CdnObjectProductionPropertyName] != null &&
                                     _jObject[CdnObjectProductionPropertyName].Type == JTokenType.String
            ? _jObject[CdnObjectProductionPropertyName].Value<string>()
            : null;

        public CdnInfo(string bundleName, JObject jObject)
        {
            _bundleName = bundleName;
            _jObject = jObject;
        }

        /// <summary>
        /// This creates the CDN html include string using the properties in the json bundle object
        /// </summary>
        /// <param name="cdnHtmlFormatString"></param>
        /// <param name="httpFileUrl">the fully qualified path to the file</param>
        /// <param name="getChecksumOfProductionFile"></param>
        /// <returns></returns>
        public string BuildCdnIncludeString(string cdnHtmlFormatString, string httpFileUrl, Func<string> getChecksumOfProductionFile)
        {
            if (Production == null)
                throw new NullReferenceException($"The bundle {_bundleName} did not have a property called '{CdnObjectProductionPropertyName}'");

            cdnHtmlFormatString = OtherProperties
                .Aggregate(cdnHtmlFormatString, (current, jProperty) => current.Replace("{" + jProperty.Name + "}", jProperty.Value.ToString()));

            cdnHtmlFormatString = cdnHtmlFormatString.Replace(CdnHtmlIncludeFileUrlReplaceString, httpFileUrl);
            if (cdnHtmlFormatString.Contains(FileTypeConfigInfo.CachebusterParam))
            {
                cdnHtmlFormatString = cdnHtmlFormatString.Replace(FileTypeConfigInfo.CachebusterParam, getChecksumOfProductionFile());
            }
            return cdnHtmlFormatString;
        }

        private static readonly Regex MatchReplace = new Regex("{\\w*}", RegexOptions.Compiled);

        /// <summary>
        /// This checks all the properties provided in the CDN object against the replace strings needed by the cdnHtmlFormatString
        /// </summary>
        /// <param name="cdnHtmlFormatString"></param>
        /// <returns></returns>
        public IEnumerable<string> FindMissingPropertiesNeededByHtmlInclude(string cdnHtmlFormatString)
        {
            if (Development == null)
                yield return CdnObjectDevelopmentPropertyName;
            if (Production == null)
                yield return CdnObjectProductionPropertyName;

            var extractedReplaces = MatchReplace.Matches(cdnHtmlFormatString).Cast<Match>().Select(m => m.Value)
                .Where(x => x != CdnHtmlIncludeFileUrlReplaceString && x != FileTypeConfigInfo.CachebusterParam)
                .ToList();

            var propsWithCurlyBrackets = OtherProperties.Select(x => "{" + x.Name + "}").ToList();

            foreach (var missingReplace in extractedReplaces.Where(x => !propsWithCurlyBrackets.Contains(x)))
            {
                yield return missingReplace.Substring(1, missingReplace.Length - 2);
            }
        }

        public override string ToString()
        {
            var nameValues = string.Join(", ", OtherProperties.Select(x => $"{x.Name}: {x.Value.ToString()}"));
            return $"Development: {Development}, Production: {Production}, {nameValues}";
        }

        //------------------------------------------------
        //private helpers

        private IEnumerable<JProperty> OtherProperties => _jObject.Properties()
            .Where(x => x.Name != CdnObjectDevelopmentPropertyName && x.Name != CdnObjectProductionPropertyName);
    }
}