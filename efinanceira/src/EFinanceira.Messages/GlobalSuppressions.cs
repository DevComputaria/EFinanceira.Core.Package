// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Temporarily suppress StyleCop warnings for EvtMovimentacaoFinanceiraAnual builder
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Builder pattern with multiple related types in single file", Scope = "namespaceanddescendants", Target = "~N:EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceiraAnual")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Builder pattern with multiple related types in single file", Scope = "namespaceanddescendants", Target = "~N:EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceiraAnual")]
[assembly: SuppressMessage("Design", "CA1510:Use ArgumentNullException.ThrowIfNull instead of explicitly throwing a new exception instance", Justification = "Legacy pattern consistency", Scope = "namespaceanddescendants", Target = "~N:EFinanceira.Messages.Builders.Eventos.EvtMovimentacaoFinanceiraAnual")]

// Suppress StyleCop warnings for EvtPatrocinado builder pattern
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Builder pattern with multiple related types in single file", Scope = "namespaceanddescendants", Target = "~N:EFinanceira.Messages.Builders.Eventos.EvtPatrocinado")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Builder pattern with multiple related types in single file", Scope = "namespaceanddescendants", Target = "~N:EFinanceira.Messages.Builders.Eventos.EvtPatrocinado")]

// Suppress StyleCop warnings for EvtPrevidenciaPrivada builder pattern
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Builder pattern with multiple related types in single file", Scope = "namespaceanddescendants", Target = "~N:EFinanceira.Messages.Builders.Eventos.EvtPrevidenciaPrivada")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Builder pattern with multiple related types in single file", Scope = "namespaceanddescendants", Target = "~N:EFinanceira.Messages.Builders.Eventos.EvtPrevidenciaPrivada")]
