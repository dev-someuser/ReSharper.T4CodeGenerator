﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ExampleGenerators
{
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Feature.Services.StructuralSearch;
    using JetBrains.ReSharper.Feature.Services.LinqTools;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Resolve;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.ReSharper.Psi.Tree;
    using System.Collections.Generic;
    using T4CodeGenerator.Generators.Core.Generators.Common;
    using T4CodeGenerator.Generators.Core.Helpers;
    using T4CodeGenerator.Generators.Core.StructuralSearch;
    using T4CodeGenerator.Generators.Core.StructuralSearch.Attributes;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class ReplaceLinqFirstExpressionBase : ByPatternInPlaceGeneratorBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\n");
            this.Write("\r\n");
            this.Write("\r\n");
            this.Write("\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 18 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"

    [ExpressionPlaceholder]
	public IExpression Enumerable { get; set; }

    [ArgumentPlaceholder]
    public ILambdaParameterDeclaration Argument { get; set; }

    [ExpressionPlaceholder]
    public IExpression Condition { get; set; }

    public  override bool Visible => false;

    public override string Name => "Enumerable.First to foreach";

    protected override ByPatternInPlaceGenerationMode GenerationMode =>
        ByPatternInPlaceGenerationMode.ReplaceMatchedText;

    protected override string Pattern => throw new NotImplementedException();

    protected virtual ITreeNode Target => throw new NotImplementedException();

	protected virtual IType TargetType => throw new NotImplementedException();

	private readonly List<LinqExpression> _linqExpressions = new List<LinqExpression>();

	private readonly HashSet<string> _lambdaArgumentUsedNames = new HashSet<string>();

    protected void Render()
    {
        IExpression enumerable = ParseEnumerableRecursive(this.Enumerable);

        string labdaArgumentName = GetLambdaArgumentName(-1 /* expressionIndex */);

		
        
        #line default
        #line hidden
        
        #line 51 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write("\r\n\t\t\t");

        
        #line default
        #line hidden
        
        #line 53 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(this.Target.GetText()));

        
        #line default
        #line hidden
        
        #line 53 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(" = default(");

        
        #line default
        #line hidden
        
        #line 53 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(this.TargetType.GetCSharpTypeName()));

        
        #line default
        #line hidden
        
        #line 53 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(");\r\n\r\n\t\t\tbool found = false;\r\n\r\n\t\t\tforeach(var ");

        
        #line default
        #line hidden
        
        #line 57 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(labdaArgumentName));

        
        #line default
        #line hidden
        
        #line 57 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(" in ");

        
        #line default
        #line hidden
        
        #line 57 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(enumerable.GetText()));

        
        #line default
        #line hidden
        
        #line 57 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(")\r\n\t\t\t{\r\n\t\t\t\t");

        
        #line default
        #line hidden
        
        #line 59 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
WriteExpression(0 /* expressionIndex */, labdaArgumentName);
        
        #line default
        #line hidden
        
        #line 59 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write("\t\t\t}\r\n\r\n\t\t\tif (!found)\r\n\t\t\t{\r\n\t\t\t\t// Element is not found.");

        
        #line default
        #line hidden
        
        #line 64 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
LineBreak();
        
        #line default
        #line hidden
        
        #line 64 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write("\t\t\t}\r\n\r\n\t\t");

        
        #line default
        #line hidden
        
        #line 67 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"

    }

    private IExpression ParseEnumerableRecursive(IExpression enumerableExpression)
    {
        IExpression result = enumerableExpression;

        StructuralSearchEngine sse = this.DataProvider.Solution.GetComponent<StructuralSearchEngine>();

        IExpression enu;
        ILambdaParameterDeclaration x;
        IExpression exp;

        if (enumerableExpression.Matched("$enu$.Select($x$ => $exp$)", sse, out enu, out x, out exp))
        {
            result = ParseEnumerableRecursive(enu);
            _linqExpressions.Add(new SelectLinqExpression(x, exp));
        }

        if (enumerableExpression.Matched("$enu$.Where($x$ => $arg$)", sse, out enu, out x, out exp))
        {
            result = ParseEnumerableRecursive(enu);
            _linqExpressions.Add(new WhereLinqExpression(x, exp));
        }

        return result;
    }

    private string GetLambdaArgumentName(int expressionIndex)
    {
        IIdentifier lamdbaArgumentIdentifier;

        if (expressionIndex < _linqExpressions.Count - 1)
        {
            lamdbaArgumentIdentifier = _linqExpressions[expressionIndex + 1].LamdbaArgument.NameIdentifier;
        }
        else
        {
            lamdbaArgumentIdentifier = this.Argument.NameIdentifier;
        }

        int index = 0;
        string name = lamdbaArgumentIdentifier.Name;
        while (!_lambdaArgumentUsedNames.Add(name))
        {
            index++;
            name = lamdbaArgumentIdentifier.Name + index;
        }

        return name;
    }

	private void WriteExpression(int expressionIndex, string lamdbaArgumentName)
    {
        if (expressionIndex == _linqExpressions.Count)
        {
            WriteFirstExpression(lamdbaArgumentName);
			return;
        }

        LinqExpression expression = _linqExpressions[expressionIndex];

        IExpression lambdaExpression = 
			RenameArgument(expression.LamdbaArgument.DeclaredElement, expression.LambdaExpression, lamdbaArgumentName);

        switch (expression)
        {
			case SelectLinqExpression _:
			    lamdbaArgumentName = GetLambdaArgumentName(expressionIndex);
				
        
        #line default
        #line hidden
        
        #line 136 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write("var ");

        
        #line default
        #line hidden
        
        #line 136 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(lamdbaArgumentName));

        
        #line default
        #line hidden
        
        #line 136 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(" = ");

        
        #line default
        #line hidden
        
        #line 136 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(lambdaExpression.GetText()));

        
        #line default
        #line hidden
        
        #line 136 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(";");

        
        #line default
        #line hidden
        
        #line 136 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"

			    WriteExpression(expressionIndex + 1, lamdbaArgumentName);
			    break;

			case WhereLinqExpression _:
				
        
        #line default
        #line hidden
        
        #line 141 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write("\t\t\t\t\tif (");

        
        #line default
        #line hidden
        
        #line 142 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(lambdaExpression.GetText()));

        
        #line default
        #line hidden
        
        #line 142 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(")\r\n\t\t\t\t\t{\r\n\t\t\t\t\t\t");

        
        #line default
        #line hidden
        
        #line 144 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
WriteExpression(expressionIndex + 1, lamdbaArgumentName);
        
        #line default
        #line hidden
        
        #line 144 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write("\t\t\t\t\t}\r\n\t\t\t\t");

        
        #line default
        #line hidden
        
        #line 146 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"

				break;
        }
    }

	private void WriteFirstExpression(string lamdbaArgumentName)
    {
        IExpression condition = RenameArgument(this.Argument.DeclaredElement, this.Condition, lamdbaArgumentName);

		
        
        #line default
        #line hidden
        
        #line 155 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write("\t\t\tif (");

        
        #line default
        #line hidden
        
        #line 156 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(condition.GetText()));

        
        #line default
        #line hidden
        
        #line 156 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(")\r\n\t\t\t{\r\n\t\t\t\t");

        
        #line default
        #line hidden
        
        #line 158 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(this.Target.GetText()));

        
        #line default
        #line hidden
        
        #line 158 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(" = ");

        
        #line default
        #line hidden
        
        #line 158 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(lamdbaArgumentName));

        
        #line default
        #line hidden
        
        #line 158 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"
this.Write(";\r\n\t\t\t\tfound = true;\r\n\t\t\t}\r\n\t\t");

        
        #line default
        #line hidden
        
        #line 161 "C:\T4CodeGeneratorExamples\ExampleGenerators\ReplaceLinqFirstExpressionBase.tt"

    }

    private IExpression RenameArgument(
        IDeclaredElement declaredElement,
        IExpression expression,
        string newName)
    {
        IExpression newExpression = 
			this.DataProvider.ElementFactory.CreateExpression(expression.GetText());
        newExpression.SetResolveContextForSandBox(expression);

        foreach (IReference reference in newExpression.FindReferences(declaredElement))
        {
            IReferenceExpression newReferenceExpression = 
				this.DataProvider.ElementFactory.CreateReferenceExpression(newName);
            ((IExpression) reference.GetTreeNode()).ReplaceBy(newReferenceExpression);
        }

        return newExpression;
    }
	
    private abstract  class LinqExpression
    {
		public ILambdaParameterDeclaration LamdbaArgument { get; }

        public IExpression LambdaExpression { get; }

        protected LinqExpression(ILambdaParameterDeclaration lamdbaArgument, IExpression lambdaExpression)
        {
            this.LamdbaArgument = lamdbaArgument;
			this.LambdaExpression = lambdaExpression;
        }
    }

    private class WhereLinqExpression : LinqExpression
    {
        public WhereLinqExpression(ILambdaParameterDeclaration lamdbaArgument, IExpression lambdaExpression) : base(lamdbaArgument, lambdaExpression)
        {
        }
    }

    private class SelectLinqExpression : LinqExpression
    {
        public SelectLinqExpression(ILambdaParameterDeclaration lamdbaArgument, IExpression lambdaExpression) : base(lamdbaArgument, lambdaExpression)
        {
        }
    }

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
}
