using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using T4CodeGenerator.Generators.Core.Helpers;

using DatabaseExampleCore;

namespace DatabaseExampleGenerators
{
    public static class Helpers
    {
        public static string RemoveFirstWord(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            int index;
            for (index = 1; index < value.Length; index++)
            {
                if (value[index].IsUpperFast())
                {
                    break;
                }
            }

            return value.Remove(0, index);
        }

        public static string Pluralize(this string word)
        {
            return GetPluralizationService().Pluralize(word);
        }

        public static string Singularize(this string word)
        {
            return GetPluralizationService().Singularize(word);
        }

        public static bool TryGetReferenceTableName(IPropertyDeclaration property, out string tableName)
        {
            IType referenceTableType =
                property.GetAttributeArgumentTypeValue<DatabaseExampleCore.TableReferenceAttribute>(0);
            IClassDeclaration referenceTableClassDeclaration =
                referenceTableType?.GetSingleDeclaration<IClassDeclaration>();

            if (referenceTableClassDeclaration != null)
            {
                tableName = GetTableName(referenceTableClassDeclaration);
                return tableName != null;
            }

            tableName = null;
            return false;
        }

        public static string GetTableName(IClassLikeDeclaration @class)
        {
            return @class.GetAttributeArgumentConstantValue<TableAttribute>(0) as string;
        }

        private static PluralizationService GetPluralizationService()
        {
            return PluralizationService.CreateService(new CultureInfo("en-US"));
        }
    }
}