using EFinanceira.Core.Factory;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteCriptografado;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteEventos;
using EFinanceira.Messages.Builders.Lotes.EnvioLoteEventosAssincrono;
using EFinanceira.Messages.Builders.Lotes.RetornoLoteEventos;

namespace EFinanceira.Messages.Extensions;

/// <summary>
/// Extensões para facilitar a criação de builders de lotes
/// </summary>
public static class LotesBuilderExtensions
{
    /// <summary>
    /// Cria um builder para EnvioLoteEventos
    /// </summary>
    /// <param name="factory">Factory do e-Financeira</param>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    /// <returns>Builder para EnvioLoteEventos</returns>
    public static EnvioLoteEventosBuilder CriarEnvioLoteEventos(this EFinanceiraMessageFactory factory, string version = "v1_2_0")
    {
        return new EnvioLoteEventosBuilder(version);
    }

    /// <summary>
    /// Cria um builder para EnvioLoteCriptografado
    /// </summary>
    /// <param name="factory">Factory do e-Financeira</param>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    /// <returns>Builder para EnvioLoteCriptografado</returns>
    public static EnvioLoteCriptografadoBuilder CriarEnvioLoteCriptografado(this EFinanceiraMessageFactory factory, string version = "v1_2_0")
    {
        return new EnvioLoteCriptografadoBuilder(version);
    }

    /// <summary>
    /// Cria um builder para EnvioLoteEventosAssincrono
    /// </summary>
    /// <param name="factory">Factory do e-Financeira</param>
    /// <param name="version">Versão do schema (padrão: v1_0_0)</param>
    /// <returns>Builder para EnvioLoteEventosAssincrono</returns>
    public static EnvioLoteEventosAssincronoBuilder CriarEnvioLoteEventosAssincrono(this EFinanceiraMessageFactory factory, string version = "v1_0_0")
    {
        return new EnvioLoteEventosAssincronoBuilder(version);
    }

    /// <summary>
    /// Cria um builder para RetornoLoteEventos
    /// </summary>
    /// <param name="factory">Factory do e-Financeira</param>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    /// <returns>Builder para RetornoLoteEventos</returns>
    public static RetornoLoteEventosBuilder CriarRetornoLoteEventos(this EFinanceiraMessageFactory factory, string version = "v1_2_0")
    {
        return new RetornoLoteEventosBuilder(version);
    }

    /// <summary>
    /// Cria um builder para RetornoLoteEventos a partir de XML
    /// </summary>
    /// <param name="factory">Factory do e-Financeira</param>
    /// <param name="xmlContent">Conteúdo XML do retorno</param>
    /// <param name="version">Versão do schema (padrão: v1_2_0)</param>
    /// <returns>Builder preenchido com os dados do XML</returns>
    public static RetornoLoteEventosBuilder ParseRetornoLoteEventos(this EFinanceiraMessageFactory factory, string xmlContent, string version = "v1_2_0")
    {
        return RetornoLoteEventosBuilder.FromXml(xmlContent, version);
    }
}