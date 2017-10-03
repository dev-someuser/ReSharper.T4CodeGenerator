using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;

using T4CodeGenerator.Generators.Core.Contracts;

namespace T4CodeGenerator.Generators.Core.Generators
{
    [GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public abstract class GeneratorBase : IGenerator
    {
        private static readonly Dictionary<Type, string> ReflectionTypeToAliasIndex = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" }
        };

        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private readonly GeneratorBase _generator;

            public ToStringInstanceHelper(GeneratorBase generator)
            {
                _generator = generator;
            }

            private IFormatProvider _formatProviderField = System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public IFormatProvider FormatProvider
            {
                get => this._formatProviderField;
                set
                {
                    if (value != null)
                    {
                        _formatProviderField = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if (objectToConvert == null)
                {
                    throw new ArgumentNullException("objectToConvert");
                }

                return _generator.ConvertObjectToString(objectToConvert, _formatProviderField);
            }
        }

        private StringBuilder _generationEnvironment;

        public IGeneratorDataProvider DataProvider { get; set; }

        public abstract string Name { get; }

        public virtual string Description => null;

        public virtual bool Visible => true;

        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        /// <remarks>
        /// Always returns string.Empty because we do not need to manually manage identation.
        /// </remarks>
        public string CurrentIndent => string.Empty;

        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual IDictionary<string, object> Session { get; set; }

        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper { get; }

        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected StringBuilder GenerationEnvironment
        {
            get => _generationEnvironment ?? (_generationEnvironment = new StringBuilder());
            set => _generationEnvironment = value;
        }

        protected GeneratorBase()
        {
            this.ToStringHelper = new ToStringInstanceHelper(this);
        }

        public abstract bool IsAvailable();

        //public virtual void Initialize() { }

        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText() => this.GenerationEnvironment.ToString();

        public void LineBreak()
        {
            this.GenerationEnvironment.Append(Environment.NewLine);
        }

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }

            // Remove all the newlines, the code will be reformatted anyway.
            textToAppend = textToAppend.Replace(Environment.NewLine, string.Empty);

            this.GenerationEnvironment.Append(textToAppend);
        }

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            Write(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            throw new GeneratorErrorException(message);
        }

        public void Error(bool condition, string message)
        {
            if (condition)
            {
                throw new GeneratorErrorException(message);
            }
        }

        protected virtual string ConvertObjectToString(object obj, IFormatProvider formatProvider)
        {
            if (obj is IType type)
            {
                ProcessNamespacesOfType(type);
                return type.GetPresentableName(CSharpLanguage.Instance);
            }

            if (obj is Type reflectionType)
            {
                ProcessNamespacesOfType(reflectionType);
                return GetPresentableName(reflectionType);
            }

            Type t = obj.GetType();
            MethodInfo method = t.GetMethod("ToString", new[] { typeof(IFormatProvider) });
            if (method == null)
            {
                return obj.ToString();
            }

            return (string)method.Invoke(obj, new object[] { formatProvider });
        }

        /// <summary>
        /// Returns the attribute name without 'Attribute' prefix, adds missing namespaces.
        /// </summary>
        protected string Attribute<TAttribute>()
            where TAttribute : Attribute
        {
            var attributeType = typeof(TAttribute);
            ProcessNamespacesOfType(attributeType);

            return attributeType.Name.Replace("Attribute", string.Empty);
        }

        protected virtual void ProcessImportedNamespace(string @namespace) { }

        private void ProcessNamespacesOfType(IType type)
        {
            if (!type.IsSimplePredefined())
            {
                ITypeElement typeElement = type.GetTypeElement();
                string @namespace = typeElement?.GetContainingNamespace().QualifiedName;
                ProcessImportedNamespace(@namespace);
            }

            if (type is IArrayType arrayType)
            {
                ProcessNamespacesOfType(arrayType.ElementType);
            }
            else
            {
                foreach (IType genericUnderlyingType in GetGenericUnderlyingTypes(type))
                {
                    ProcessNamespacesOfType(genericUnderlyingType);
                }
            }
        }

        private void ProcessNamespacesOfType(Type reflectionType)
        {
            if (!ReflectionTypeToAliasIndex.ContainsKey(reflectionType))
            {
                ProcessImportedNamespace(reflectionType.Namespace);
            }

            if (reflectionType.ContainsGenericParameters)
            {
                Type[] genericArguments = reflectionType.GetGenericArguments();
                foreach (Type genericArgument in genericArguments)
                {
                    ProcessNamespacesOfType(genericArgument);
                }
            }
        }

        private static string GetPresentableName(Type reflectionType)
        {
            if (ReflectionTypeToAliasIndex.TryGetValue(reflectionType, out string alias))
            {
                return alias;
            }

            return reflectionType.Name;
        }

        private static IEnumerable<IType> GetGenericUnderlyingTypes(IType type)
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
    }
}