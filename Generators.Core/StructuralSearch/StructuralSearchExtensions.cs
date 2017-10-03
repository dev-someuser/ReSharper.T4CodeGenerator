using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using JetBrains.Application.UI.Extensions;
using JetBrains.ReSharper.Feature.Services.StructuralSearch;
using JetBrains.ReSharper.Feature.Services.StructuralSearch.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

using T4CodeGenerator.Generators.Core.StructuralSearch.Attributes;

namespace T4CodeGenerator.Generators.Core.StructuralSearch
{
    public static class StructuralSearchExtensions
    {
        private static readonly Regex PlaceholderNameRegex = new Regex("\\$(?<name>\\w+)\\$", RegexOptions.Compiled);

        public static bool Matched<TPlaceholderValue>(
            this ITreeNode treeNode,
            string pattern,
            StructuralSearchEngine structuralSearchEngine,
            out TPlaceholderValue placeholderValue)
            where TPlaceholderValue : class
        {
            placeholderValue = null;

            bool matched = Matched(
                treeNode,
                pattern,
                structuralSearchEngine,
                1 /* placeholdersCount */,
                out IStructuralMatchResult matchResult,
                out List<string> orderedPlaceholderNames
            );

            if (matched)
            {
                placeholderValue = GetPlaceholderMatchedValue<TPlaceholderValue>(matchResult, orderedPlaceholderNames[0]);
                if (placeholderValue == null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool Matched<TPlaceholder1Value, TPlaceholder2Value>(
            this ITreeNode treeNode, 
            string pattern,
            StructuralSearchEngine structuralSearchEngine,
            out TPlaceholder1Value placeholder1Value,
            out TPlaceholder2Value placeholder2Value)
            where TPlaceholder1Value : class
            where TPlaceholder2Value : class
        {
            placeholder1Value = null;
            placeholder2Value = null;

            bool matched = Matched(
                treeNode,
                pattern,
                structuralSearchEngine,
                2 /* placeholdersCount */,
                out IStructuralMatchResult matchResult,
                out List<string> orderedPlaceholderNames
            );

            if (matched)
            {
                placeholder1Value = GetPlaceholderMatchedValue<TPlaceholder1Value>(matchResult, orderedPlaceholderNames[0]);
                if (placeholder1Value == null)
                {
                    return false;
                }

                placeholder2Value = GetPlaceholderMatchedValue<TPlaceholder2Value>(matchResult, orderedPlaceholderNames[1]);
                if (placeholder2Value == null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool Matched<TPlaceholder1Value, TPlaceholder2Value, TPlaceholder3Value>(
            this ITreeNode treeNode,
            string pattern,
            StructuralSearchEngine structuralSearchEngine,
            out TPlaceholder1Value placeholder1Value,
            out TPlaceholder2Value placeholder2Value,
            out TPlaceholder3Value placeholder3Value)
            where TPlaceholder1Value : class
            where TPlaceholder2Value : class
            where TPlaceholder3Value : class
        {
            placeholder1Value = null;
            placeholder2Value = null;
            placeholder3Value = null;

            bool matched = Matched(
                treeNode,
                pattern,
                structuralSearchEngine,
                3 /* placeholdersCount */,
                out IStructuralMatchResult matchResult,
                out List<string> orderedPlaceholderNames
            );

            if (matched)
            {
                placeholder1Value = GetPlaceholderMatchedValue<TPlaceholder1Value>(matchResult, orderedPlaceholderNames[0]);
                if (placeholder1Value == null)
                {
                    return false;
                }

                placeholder2Value = GetPlaceholderMatchedValue<TPlaceholder2Value>(matchResult, orderedPlaceholderNames[1]);
                if (placeholder2Value == null)
                {
                    return false;
                }

                placeholder3Value = GetPlaceholderMatchedValue<TPlaceholder3Value>(matchResult, orderedPlaceholderNames[2]);
                if (placeholder3Value == null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool Matched<TPlaceholder1Value, TPlaceholder2Value, TPlaceholder3Value, TPlaceholder4Value>(
            this ITreeNode treeNode,
            string pattern,
            StructuralSearchEngine structuralSearchEngine,
            out TPlaceholder1Value placeholder1Value,
            out TPlaceholder2Value placeholder2Value,
            out TPlaceholder3Value placeholder3Value,
            out TPlaceholder4Value placeholder4Value)
            where TPlaceholder1Value : class
            where TPlaceholder2Value : class
            where TPlaceholder3Value : class
            where TPlaceholder4Value : class
        {
            placeholder1Value = null;
            placeholder2Value = null;
            placeholder3Value = null;
            placeholder4Value = null;

            bool matched = Matched(
                treeNode,
                pattern,
                structuralSearchEngine,
                4 /* placeholdersCount */,
                out IStructuralMatchResult matchResult,
                out List<string> orderedPlaceholderNames
            );

            if (matched)
            {
                placeholder1Value = GetPlaceholderMatchedValue<TPlaceholder1Value>(matchResult, orderedPlaceholderNames[0]);
                if (placeholder1Value == null)
                {
                    return false;
                }

                placeholder2Value = GetPlaceholderMatchedValue<TPlaceholder2Value>(matchResult, orderedPlaceholderNames[1]);
                if (placeholder2Value == null)
                {
                    return false;
                }

                placeholder3Value = GetPlaceholderMatchedValue<TPlaceholder3Value>(matchResult, orderedPlaceholderNames[2]);
                if (placeholder3Value == null)
                {
                    return false;
                }

                placeholder4Value = GetPlaceholderMatchedValue<TPlaceholder4Value>(matchResult, orderedPlaceholderNames[3]);
                if (placeholder4Value == null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool Matched<TPlaceholder1Value, TPlaceholder2Value, TPlaceholder3Value, TPlaceholder4Value, TPlaceholder5Value>(
            this ITreeNode treeNode,
            string pattern,
            StructuralSearchEngine structuralSearchEngine,
            out TPlaceholder1Value placeholder1Value,
            out TPlaceholder2Value placeholder2Value,
            out TPlaceholder3Value placeholder3Value,
            out TPlaceholder4Value placeholder4Value,
            out TPlaceholder5Value placeholder5Value)
            where TPlaceholder1Value : class
            where TPlaceholder2Value : class
            where TPlaceholder3Value : class
            where TPlaceholder4Value : class
            where TPlaceholder5Value : class 
        {
            placeholder1Value = null;
            placeholder2Value = null;
            placeholder3Value = null;
            placeholder4Value = null;
            placeholder5Value = null;

            bool matched = Matched(
                treeNode,
                pattern,
                structuralSearchEngine,
                5 /* placeholdersCount */,
                out IStructuralMatchResult matchResult,
                out List<string> orderedPlaceholderNames
            );

            if (matched)
            {
                placeholder1Value = GetPlaceholderMatchedValue<TPlaceholder1Value>(matchResult, orderedPlaceholderNames[0]);
                if (placeholder1Value == null)
                {
                    return false;
                }

                placeholder2Value = GetPlaceholderMatchedValue<TPlaceholder2Value>(matchResult, orderedPlaceholderNames[1]);
                if (placeholder2Value == null)
                {
                    return false;
                }

                placeholder3Value = GetPlaceholderMatchedValue<TPlaceholder3Value>(matchResult, orderedPlaceholderNames[2]);
                if (placeholder3Value == null)
                {
                    return false;
                }

                placeholder4Value = GetPlaceholderMatchedValue<TPlaceholder4Value>(matchResult, orderedPlaceholderNames[3]);
                if (placeholder4Value == null)
                {
                    return false;
                }

                placeholder5Value = GetPlaceholderMatchedValue<TPlaceholder5Value>(matchResult, orderedPlaceholderNames[4]);
                if (placeholder5Value == null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static IStructuralMatchResult IsTreeNodeOrParentsMatched(
            this IStructuralMatcher matcher,
            ITreeNode treeNode,
            object placeholdersBindingObject)
        {
            PropertyInfo[] placeholderProperties = GetPlaceholderProperties(placeholdersBindingObject.GetType());

            ITreeNode initialTreeNode = treeNode;
            
            while (treeNode != null)
            {
                IStructuralMatchResult matchResult = TreeNodeMatched(
                    matcher,
                    treeNode,
                    initialTreeNode,
                    placeholdersBindingObject,
                    placeholderProperties
                );

                if (matchResult.Matched)
                {
                    return matchResult;
                }

                treeNode = treeNode.Parent;
            }

            return StructuralMatchResult.NOT_MATCHED;
        }

        public static IStructuralMatcher CreateMatcherWithPlaceholdersBindingObject(
            this StructuralSearchEngine structuralSearchEngine,
            string pattern,
            PsiLanguageType languageType,
            Type placeholdersBindingObjectType,
            IPlaceholder[] additionalPlaceholders = null)
        {
            IStructuralSearcherFactory factory = structuralSearchEngine.GetFactory(languageType);

            List<IPlaceholder> placeholders = 
                GetPlaceholdersFromBindingObjectType(placeholdersBindingObjectType);

            if (additionalPlaceholders != null)
            {
                placeholders.AddRange(placeholders);
            }

            return factory.CreatePattern(pattern, placeholders.ToArray()).CreateMatcher();
        }

        private static bool Matched(
            ITreeNode treeNode,
            string pattern,
            StructuralSearchEngine structuralSearchEngine,
            int placeholdersCount,
            out IStructuralMatchResult matchResult,
            out List<string> orderedPlaceholderNames)
        {
            matchResult = null;
            orderedPlaceholderNames = null;

            IStructuralSearchPattern structuralSearchPattern =
                structuralSearchEngine.GetFactory(CSharpLanguage.Instance).CreatePattern(pattern);
            if (!structuralSearchPattern.GuessPlaceholders() || structuralSearchPattern.Placeholders.Count != placeholdersCount)
            {
                return false;
            }

            orderedPlaceholderNames = GetPatternOrderedPlaceholderNames(pattern);
            if (orderedPlaceholderNames.Count != placeholdersCount)
            {
                return false;
            }

            IStructuralMatcher matcher = structuralSearchPattern.CreateMatcher();
            matchResult = matcher.Match(treeNode);

            return matchResult.Matched;
        }

        private static List<string> GetPatternOrderedPlaceholderNames(string pattern)
        {
            MatchCollection matches = PlaceholderNameRegex.Matches(pattern);
            var result = new List<string>(matches.Count);
            
            foreach (Match match in matches)
            {
                string placeholderName = match.Groups["name"].Value;

                if (!result.Contains(placeholderName))
                {
                    result.Add(placeholderName);
                }
            }

            return result;
        }

        private static T GetPlaceholderMatchedValue<T>(IStructuralMatchResult matchResult, string placeholderName)
            where T : class
        {
            object value = matchResult.GetMatch(placeholderName);

            if (value is T result)
            {
                return result;
            }

            return null;
        }

        private static IStructuralMatchResult TreeNodeMatched(
            IStructuralMatcher matcher,
            ITreeNode treeNode,
            ITreeNode initialTreeNode,
            object placeholdersBindingObject,
            PropertyInfo[] placeholderProperties)
        {
            IStructuralMatchResult matchResult = matcher.Match(treeNode);
            if (matchResult.Matched && matchResult.MatchedElement.Contains(initialTreeNode))
            {
                var propertyToMatchedObjectPairs = new List<(PropertyInfo property, object matchedObject)>();

                foreach (PropertyInfo property in placeholderProperties)
                {
                    object matchedObject = matchResult.GetMatch(property.Name);
                    if (property.PropertyType.IsInstanceOfType(matchedObject))
                    {
                        propertyToMatchedObjectPairs.Add((property, matchedObject));
                    }
                    else
                    {
                        return StructuralMatchResult.NOT_MATCHED;
                    }
                }

                foreach ((PropertyInfo property, object matchedObject) in propertyToMatchedObjectPairs)
                {
                    property.SetValue(placeholdersBindingObject, matchedObject);
                }

                return matchResult;
            }

            return StructuralMatchResult.NOT_MATCHED;
        }

        private static PropertyInfo[] GetPlaceholderProperties(Type placeholdersBindingObjectType)
        {
            return placeholdersBindingObjectType.GetProperties()
                .Where(x => x.HasAttribute(typeof(PlaceholderAttributeBase)))
                .ToArray();
        }

        private static List<IPlaceholder> GetPlaceholdersFromBindingObjectType(Type placeholdersBindingObjectType)
        {
            var placeholders = new List<IPlaceholder>();

            foreach (PropertyInfo property in placeholdersBindingObjectType.GetProperties())
            {
                PlaceholderAttributeBase placeholderAttribute = 
                    property.GetCustomAttribute<PlaceholderAttributeBase>();
                if (placeholderAttribute != null)
                {
                    placeholders.Add(placeholderAttribute.CreatePlaceholder(property.Name));
                }
            }
            
            return placeholders;
        }
    }
}