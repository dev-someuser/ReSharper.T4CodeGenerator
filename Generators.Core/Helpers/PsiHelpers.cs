using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util.Extension;

namespace T4CodeGenerator.Generators.Core.Helpers
{
    public static class PsiHelpers
    {
        private const string AttributePostfix = "Attribute";

        public static bool IsPublic([NotNull] this ICSharpModifiersOwnerDeclaration declaration)
        {
            return declaration.HasModifier(CSharpTokenType.PUBLIC_KEYWORD);
        }

        public static bool IsPrivate([NotNull] this ICSharpModifiersOwnerDeclaration declaration)
        {
            return declaration.HasModifier(CSharpTokenType.PRIVATE_KEYWORD);
        }

        public static bool IsProtected([NotNull] this ICSharpModifiersOwnerDeclaration declaration)
        {
            return declaration.HasModifier(CSharpTokenType.PROTECTED_KEYWORD);
        }

        [NotNull]
        public static IReference[] FindReferences([NotNull] this ITreeNode treeNode, [NotNull]IDeclaredElement declaredElement)
        {
            IPsiServices psiServices = treeNode.GetPsiServices();

            return psiServices.Finder.FindReferences(
                declaredElement,
                psiServices.SearchDomainFactory.CreateSearchDomain(treeNode),
                NullProgressIndicator.Create()
            );
        }

        public static bool Is<TReflectionType>([NotNull]this IType type)
        {
            return type.Is(typeof(TReflectionType));
        }

        public static bool IsArrayOf<TElementType>([NotNull] this IType type, int rank = 1)
        {
            return type.IsArrayOf(typeof(TElementType), rank);
        }

        public static bool IsArrayOf([NotNull] this IType type, [NotNull]Type reflectionElementType, int rank = 1)
        {
            if (!(type is IArrayType arrayType))
            {
                return false;
            }

            if (!arrayType.ElementType.Is(reflectionElementType))
            {
                return false;
            }

            if (arrayType.Rank != rank)
            {
                return false;
            }

            return true;
        }

        public static bool IsGenericArray([NotNull] this IType type, int rank = 1)
        {
            if (!(type is IArrayType arrayType))
            {
                return false;
            }

            if (arrayType.Rank != rank)
            {
                return false;
            }

            return true;
        }

        public static bool Is([NotNull]this IType type, [NotNull]Type reflecationType)
        {
            if (reflecationType.IsArray && !type.IsArrayOf(reflecationType.GetElementType(), reflecationType.GetArrayRank()))
            {
                return false;
            }

            if (!(type is IDeclaredType declaredType))
            {
                return false;
            }

            Type[] genericArguments = reflecationType.GetGenericArguments();
            if (genericArguments.Length == 0 || reflecationType.ContainsGenericParameters)
            {
                var clrTypeName = new ClrTypeName(reflecationType.FullName);
                return declaredType.GetClrName().Equals(clrTypeName);
            }

            using (IEnumerator<IType> underlyingTypesEnumerator = GetGenericUnderlyingTypes(type).GetEnumerator())
            {
                foreach (Type genericArgumentType in genericArguments)
                {
                    if (!underlyingTypesEnumerator.MoveNext())
                    {
                        return false;
                    }

                    if (!underlyingTypesEnumerator.Current.Is(genericArgumentType))
                    {
                        return false;
                    }
                }

                if (underlyingTypesEnumerator.MoveNext())
                {
                    return false;
                }
            }
            
            return true;
        }

        [NotNull]
        public static ISymbolScope GetSymbolScope(
            [NotNull] this IGeneratorDataProvider dataProvider,
            bool withReferences = true,
            bool caseSensitive = true)
        {
            return dataProvider.PsiServices.Symbols.GetSymbolScope(dataProvider.PsiModule, withReferences, caseSensitive);
        }

        [NotNull]
        public static string GetCSharpTypeName([NotNull] this IType type)
        {
            return type.GetPresentableName(CSharpLanguage.Instance);
        }

        [CanBeNull]
        public static IAttribute GetAttribute([NotNull]this IAttributesOwnerDeclaration attributesOwner, [NotNull]string name)
        {
            return attributesOwner.Attributes.FirstOrDefault(x => x.Name.QualifiedName == name);
        }

        [CanBeNull]
        public static IAttribute GetAttribute<TAttribute>([NotNull]this IAttributesOwnerDeclaration attributesOwner)
            where TAttribute: Attribute
        {
            Type reflectionType = typeof(TAttribute);
            return attributesOwner.Attributes.FirstOrDefault(x => AttributeMatchedReflectionType(x, reflectionType));
        }

        public static bool HasAttribute([NotNull]this IAttributesOwnerDeclaration attributesOwner, [NotNull]string name)
        {
            return attributesOwner.GetAttribute(name) != null;
        }

        public static bool HasAttribute<TAttribute>([NotNull]this IAttributesOwnerDeclaration attributesOwner)
            where TAttribute : Attribute
        {
            return attributesOwner.GetAttribute<TAttribute>() != null;
        }

        [CanBeNull]
        public static object GetAttributeArgumentConstantValue<TAttribute>(
            [NotNull]this IAttributesOwnerDeclaration attributesOwner, 
            int argumentPosition)
            where TAttribute : Attribute
        {
            return attributesOwner.GetAttribute<TAttribute>()?.GetArgumentConstantValue(argumentPosition);
        }

        [CanBeNull]
        public static object GetAttributeArgumentConstantValue<TAttribute, TPropertyType>(
            [NotNull]this IAttributesOwnerDeclaration attributesOwner,
            [NotNull]Expression<Func<TAttribute, TPropertyType>> propertyExpression)
            where TAttribute : Attribute
        {
            string propertyName = GetPropertyNameFromExpression(propertyExpression);

            if (propertyName != null)
            {
                return attributesOwner.GetAttribute<TAttribute>()?.GetArgumentConstantValue(propertyName);
            }

            return null;
        }

        [CanBeNull]
        public static IType GetAttributeArgumentTypeValue<TAttribute>(
            [NotNull]this IAttributesOwnerDeclaration attributesOwner,
            int argumentPosition)
            where TAttribute : Attribute
        {
            return attributesOwner.GetAttribute<TAttribute>()?.GetArgumentTypeValue(argumentPosition);
        }

        [CanBeNull]
        public static IType GetAttributeArgumentTypeValue<TAttribute, TPropertyType>(
            [NotNull]this IAttributesOwnerDeclaration attributesOwner,
            [NotNull]Expression<Func<TAttribute, TPropertyType>> propertyExpression)
            where TAttribute : Attribute
        {
            string propertyName = GetPropertyNameFromExpression(propertyExpression);

            if (propertyName != null)
            {
                return attributesOwner.GetAttribute<TAttribute>()?.GetArgumentTypeValue(propertyName);
            }

            return null;
        }

        [CanBeNull]
        public static object GetArgumentConstantValue([NotNull]this IAttribute attribute, int argumentPosition)
        {
            if (argumentPosition < attribute.Arguments.Count)
            {
                ICSharpArgument argument = attribute.Arguments[argumentPosition];
                return argument.Value?.ConstantValue.Value;
            }

            return null;
        }

        [CanBeNull]
        public static object GetArgumentConstantValue([NotNull]this IAttribute attribute, [NotNull]string argumentName)
        {
            ICSharpArgument argument = attribute.Arguments.FirstOrDefault(x => x.ArgumentName == argumentName);
            return argument?.Value?.ConstantValue.Value;
        }

        [CanBeNull]
        public static IType GetArgumentTypeValue([NotNull]this IAttribute attribute, int argumentPosition)
        {
            if (argumentPosition < attribute.Arguments.Count)
            {
                ICSharpArgument argument = attribute.Arguments[argumentPosition];
                return (argument.Value as ITypeofExpression)?.ArgumentType;
            }

            return null;
        }

        [CanBeNull]
        public static IType GetArgumentTypeValue([NotNull]this IAttribute attribute, [NotNull]string argumentName)
        {
            ICSharpArgument argument = attribute.Arguments.FirstOrDefault(x => x.ArgumentName == argumentName);
            return (argument?.Value as ITypeofExpression)?.ArgumentType;
        }

        [CanBeNull]
        public static TDeclaration GetSingleDeclaration<TDeclaration>([NotNull]this IType type)
            where TDeclaration : class, IDeclaration
        {
            return type.GetTypeElement()?.GetSingleDeclaration<TDeclaration>();
        }

        [NotNull]
        private static IEnumerable<IType> GetGenericUnderlyingTypes([NotNull]IType type)
        {
            if (!(type is IDeclaredType declaredType))
            {
                yield break;
            }

            ITypeElement typeElement = type.GetTypeElement();
            if (typeElement == null)
            {
                yield break;
            }

            ISubstitution substitution = declaredType.GetSubstitution();
            foreach (ITypeParameter typeParameter in typeElement.GetAllTypeParameters())
            {
                yield return substitution[typeParameter];
            }
        }

        private static bool AttributeMatchedReflectionType([NotNull]IAttribute x, [NotNull]Type reflectionType)
        {
            if (x.Name.QualifiedName != reflectionType.Name.RemoveEnd(AttributePostfix))
            {
                return false;
            }

            ResolveResultWithInfo resolveResultWithInfo = x.Name.Reference.Resolve();
            if (!resolveResultWithInfo.IsValid())
            {
                return false;
            }

            INamespace attributeNamespace = 
                (resolveResultWithInfo.DeclaredElement as IClass)?.GetContainingNamespace();

            if (attributeNamespace == null)
            {
                return false;
            }

            return attributeNamespace.QualifiedName == reflectionType.Namespace;
        }

        [CanBeNull]
        private static string GetPropertyNameFromExpression<TObject, TPropertyType>(
            [NotNull]Expression<Func<TObject, TPropertyType>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            return null;
        }
    }
}