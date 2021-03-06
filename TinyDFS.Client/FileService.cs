﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IFileService")]
public interface IFileService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetChunkByGUID", ReplyAction="http://tempuri.org/IFileService/GetChunkByGUIDResponse")]
    System.IO.Stream GetChunkByGUID(string guid);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetChunkByGUID", ReplyAction="http://tempuri.org/IFileService/GetChunkByGUIDResponse")]
    System.Threading.Tasks.Task<System.IO.Stream> GetChunkByGUIDAsync(string guid);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/UploadChunk", ReplyAction="http://tempuri.org/IFileService/UploadChunkResponse")]
    bool UploadChunk(System.IO.Stream stream);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/UploadChunk", ReplyAction="http://tempuri.org/IFileService/UploadChunkResponse")]
    System.Threading.Tasks.Task<bool> UploadChunkAsync(System.IO.Stream stream);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/DeleteChunk", ReplyAction="http://tempuri.org/IFileService/DeleteChunkResponse")]
    bool DeleteChunk(string guid);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/DeleteChunk", ReplyAction="http://tempuri.org/IFileService/DeleteChunkResponse")]
    System.Threading.Tasks.Task<bool> DeleteChunkAsync(string guid);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IFileServiceChannel : IFileService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class FileServiceClient : System.ServiceModel.ClientBase<IFileService>, IFileService
{
    
    public FileServiceClient()
    {
    }
    
    public FileServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public FileServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public FileServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public FileServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public System.IO.Stream GetChunkByGUID(string guid)
    {
        return base.Channel.GetChunkByGUID(guid);
    }
    
    public System.Threading.Tasks.Task<System.IO.Stream> GetChunkByGUIDAsync(string guid)
    {
        return base.Channel.GetChunkByGUIDAsync(guid);
    }
    
    public bool UploadChunk(System.IO.Stream stream)
    {
        return base.Channel.UploadChunk(stream);
    }
    
    public System.Threading.Tasks.Task<bool> UploadChunkAsync(System.IO.Stream stream)
    {
        return base.Channel.UploadChunkAsync(stream);
    }
    
    public bool DeleteChunk(string guid)
    {
        return base.Channel.DeleteChunk(guid);
    }
    
    public System.Threading.Tasks.Task<bool> DeleteChunkAsync(string guid)
    {
        return base.Channel.DeleteChunkAsync(guid);
    }
}
