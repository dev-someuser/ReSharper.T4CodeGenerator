﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace DatabaseExampleGenerators
{
    using T4CodeGenerator.Generators.Core.Generators.Common;
    using T4CodeGenerator.Generators.Core.Helpers;
    using DatabaseExampleCore;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\T4CodeGeneratorExamples\DatabaseExampleGenerators\TableAttributeGenerator.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class TableAttributeGenerator : ByClassInPlaceGeneratorBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\n");
            this.Write("\r\n");
            this.Write("\r\n[");
            
            #line 8 "C:\T4CodeGeneratorExamples\DatabaseExampleGenerators\TableAttributeGenerator.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Attribute<TableAttribute>()));
            
            #line default
            #line hidden
            this.Write("(\"");
            
            #line 8 "C:\T4CodeGeneratorExamples\DatabaseExampleGenerators\TableAttributeGenerator.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.Class.DeclaredName.Pluralize()));
            
            #line default
            #line hidden
            this.Write("\")]\r\n");
            
            #line 9 "C:\T4CodeGeneratorExamples\DatabaseExampleGenerators\TableAttributeGenerator.tt"
LineBreak();
            
            #line default
            #line hidden
            this.Write("\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 11 "C:\T4CodeGeneratorExamples\DatabaseExampleGenerators\TableAttributeGenerator.tt"

	public override string Name => "Table attribute";

    public override bool IsAvailableCore() => !this.Class.HasAttribute<TableAttribute>();

    public override int GetPositionToInsert() => this.Class.GetTreeStartOffset().Offset;  

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
}