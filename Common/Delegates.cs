using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WebConnections.Core.Entity;

namespace WebConnections.Core.Common
{
    /// <summary>
    /// Antes do response de uma chamada
    /// </summary>
    /// <param name="sender">Objeto de origem</param>
    /// <param name="args">Informações sobre response</param>
    public delegate void BeforeResponseEventHandler(object sender, EventResponse args);

    /// <summary>
    /// Depois do response de uma chamada
    /// </summary>
    /// <param name="sender">Objeto de origem</param>
    /// <param name="args">Informações sobre response</param>
    public delegate void AfterResponseEventHandler(object sender, EventResponse args);

    /// <summary>
    /// Exceção disparada
    /// </summary>
    /// <param name="sender">Objeto de referência</param>
    /// <param name="e">Pilha da exceção</param>
    public delegate void ExceptionEventHandler(object sender, Exception e);

    /// <summary>
    /// Retorno de uma conexão bem sucedida
    /// </summary>
    /// <param name="login">Dados do login</param>
    /// <param name="context">Informações de contexto</param>
    public delegate void ReturnConnection(BaseConnection login, string context);

    /// <summary>
    /// Acompanhar o progresso de download de uma requisição
    /// </summary>
    /// <param name="sender">sender padrão do evento DownloadProgressChanged do webclient</param>
    /// <param name="e">Propriedades a requisição</param>
    public delegate void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);

    /// <summary>
    /// Retorno de uma chamada download do webclient
    /// </summary>
    /// <param name="sender">sender padrão do evento DownalodCompleted do webclient</param>
    /// <param name="args">args padrão do evento DownalodCompleted do webclient</param>
    public delegate void DownloadStringCompletedEvent(object sender, EventDownloadStringCompleted args);

    /// <summary>
    /// Retorno de uma entidade
    /// </summary>
    /// <param name="entity">Entidade instanciada</param>
    /// <param name="json">jSON retornado</param>
    public delegate void GetEntityHandler(BaseEntity entity, string json);

    /// <summary>
    /// Retorno de uma entidade
    /// </summary>
    /// <param name="baseAPI">API que foi chamada</param>
    /// <param name="entity">Entidade instanciada</param>
    /// <param name="json">jSON retornado</param>
    public delegate void GetEntityAPIHandler(BaseAPI baseAPI, BaseEntity entity, string json);
}
