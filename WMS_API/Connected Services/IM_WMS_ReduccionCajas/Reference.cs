﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IM_WMS_ReduccionCajas
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CallContext", Namespace="http://schemas.microsoft.com/dynamics/2010/01/datacontracts")]
    public partial class CallContext : object
    {
        
        private string CompanyField;
        
        private string LanguageField;
        
        private string LogonAsUserField;
        
        private string MessageIdField;
        
        private string PartitionKeyField;
        
        private System.Collections.Generic.Dictionary<string, string> PropertyBagField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Company
        {
            get
            {
                return this.CompanyField;
            }
            set
            {
                this.CompanyField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Language
        {
            get
            {
                return this.LanguageField;
            }
            set
            {
                this.LanguageField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogonAsUser
        {
            get
            {
                return this.LogonAsUserField;
            }
            set
            {
                this.LogonAsUserField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MessageId
        {
            get
            {
                return this.MessageIdField;
            }
            set
            {
                this.MessageIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PartitionKey
        {
            get
            {
                return this.PartitionKeyField;
            }
            set
            {
                this.PartitionKeyField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, string> PropertyBag
        {
            get
            {
                return this.PropertyBagField;
            }
            set
            {
                this.PropertyBagField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AifFault", Namespace="http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
    public partial class AifFault : object
    {
        
        private string CustomDetailXmlField;
        
        private IM_WMS_ReduccionCajas.FaultMessageList[] FaultMessageListArrayField;
        
        private IM_WMS_ReduccionCajas.InfologMessage[] InfologMessageListField;
        
        private string StackTraceField;
        
        private int XppExceptionTypeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomDetailXml
        {
            get
            {
                return this.CustomDetailXmlField;
            }
            set
            {
                this.CustomDetailXmlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IM_WMS_ReduccionCajas.FaultMessageList[] FaultMessageListArray
        {
            get
            {
                return this.FaultMessageListArrayField;
            }
            set
            {
                this.FaultMessageListArrayField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IM_WMS_ReduccionCajas.InfologMessage[] InfologMessageList
        {
            get
            {
                return this.InfologMessageListField;
            }
            set
            {
                this.InfologMessageListField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StackTrace
        {
            get
            {
                return this.StackTraceField;
            }
            set
            {
                this.StackTraceField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int XppExceptionType
        {
            get
            {
                return this.XppExceptionTypeField;
            }
            set
            {
                this.XppExceptionTypeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FaultMessageList", Namespace="http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
    public partial class FaultMessageList : object
    {
        
        private string DocumentField;
        
        private string DocumentOperationField;
        
        private IM_WMS_ReduccionCajas.FaultMessage[] FaultMessageArrayField;
        
        private string FieldField;
        
        private string ServiceField;
        
        private string ServiceOperationField;
        
        private string ServiceOperationParameterField;
        
        private string XPathField;
        
        private string XmlLineField;
        
        private string XmlPositionField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Document
        {
            get
            {
                return this.DocumentField;
            }
            set
            {
                this.DocumentField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DocumentOperation
        {
            get
            {
                return this.DocumentOperationField;
            }
            set
            {
                this.DocumentOperationField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IM_WMS_ReduccionCajas.FaultMessage[] FaultMessageArray
        {
            get
            {
                return this.FaultMessageArrayField;
            }
            set
            {
                this.FaultMessageArrayField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Field
        {
            get
            {
                return this.FieldField;
            }
            set
            {
                this.FieldField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Service
        {
            get
            {
                return this.ServiceField;
            }
            set
            {
                this.ServiceField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceOperation
        {
            get
            {
                return this.ServiceOperationField;
            }
            set
            {
                this.ServiceOperationField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceOperationParameter
        {
            get
            {
                return this.ServiceOperationParameterField;
            }
            set
            {
                this.ServiceOperationParameterField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string XPath
        {
            get
            {
                return this.XPathField;
            }
            set
            {
                this.XPathField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string XmlLine
        {
            get
            {
                return this.XmlLineField;
            }
            set
            {
                this.XmlLineField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string XmlPosition
        {
            get
            {
                return this.XmlPositionField;
            }
            set
            {
                this.XmlPositionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="InfologMessage", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.Dynamics.AX.Framework.Services")]
    public partial class InfologMessage : object
    {
        
        private IM_WMS_ReduccionCajas.InfologMessageType InfologMessageTypeField;
        
        private string MessageField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IM_WMS_ReduccionCajas.InfologMessageType InfologMessageType
        {
            get
            {
                return this.InfologMessageTypeField;
            }
            set
            {
                this.InfologMessageTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FaultMessage", Namespace="http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
    public partial class FaultMessage : object
    {
        
        private string CodeField;
        
        private string MessageField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                this.CodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="InfologMessageType", Namespace="http://schemas.datacontract.org/2004/07/Microsoft.Dynamics.AX.Framework.Services")]
    public enum InfologMessageType : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Info = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Warning = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Error = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://tempuri.org", ConfigurationName="IM_WMS_ReduccionCajas.IM_WMS_Reduccion_Cajas")]
    public interface IM_WMS_Reduccion_Cajas
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IM_WMS_Reduccion_Cajas/init", ReplyAction="http://tempuri.org/IM_WMS_Reduccion_Cajas/initResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(IM_WMS_ReduccionCajas.AifFault), Action="http://tempuri.org/IM_WMS_Reduccion_Cajas/initAifFaultFault", Name="AifFault", Namespace="http://schemas.microsoft.com/dynamics/2008/01/documents/Fault")]
        System.Threading.Tasks.Task<IM_WMS_ReduccionCajas.IM_WMS_Reduccion_CajasInitResponse> initAsync(IM_WMS_ReduccionCajas.IM_WMS_Reduccion_CajasInitRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="IM_WMS_Reduccion_CajasInitRequest", WrapperNamespace="http://tempuri.org", IsWrapped=true)]
    public partial class IM_WMS_Reduccion_CajasInitRequest
    {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://schemas.microsoft.com/dynamics/2010/01/datacontracts")]
        public IM_WMS_ReduccionCajas.CallContext CallContext;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org", Order=0)]
        public string _dataValidationXML;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org", Order=1)]
        public string _lineXML;
        
        public IM_WMS_Reduccion_CajasInitRequest()
        {
        }
        
        public IM_WMS_Reduccion_CajasInitRequest(IM_WMS_ReduccionCajas.CallContext CallContext, string _dataValidationXML, string _lineXML)
        {
            this.CallContext = CallContext;
            this._dataValidationXML = _dataValidationXML;
            this._lineXML = _lineXML;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="IM_WMS_Reduccion_CajasInitResponse", WrapperNamespace="http://tempuri.org", IsWrapped=true)]
    public partial class IM_WMS_Reduccion_CajasInitResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org", Order=0)]
        public string response;
        
        public IM_WMS_Reduccion_CajasInitResponse()
        {
        }
        
        public IM_WMS_Reduccion_CajasInitResponse(string response)
        {
            this.response = response;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public interface IM_WMS_Reduccion_CajasChannel : IM_WMS_ReduccionCajas.IM_WMS_Reduccion_Cajas, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public partial class M_WMS_Reduccion_CajasClient : System.ServiceModel.ClientBase<IM_WMS_ReduccionCajas.IM_WMS_Reduccion_Cajas>, IM_WMS_ReduccionCajas.IM_WMS_Reduccion_Cajas
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public M_WMS_Reduccion_CajasClient() : 
                base(M_WMS_Reduccion_CajasClient.GetDefaultBinding(), M_WMS_Reduccion_CajasClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.NetTcpBinding_IM_WMS_Reduccion_Cajas.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public M_WMS_Reduccion_CajasClient(EndpointConfiguration endpointConfiguration) : 
                base(M_WMS_Reduccion_CajasClient.GetBindingForEndpoint(endpointConfiguration), M_WMS_Reduccion_CajasClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public M_WMS_Reduccion_CajasClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(M_WMS_Reduccion_CajasClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public M_WMS_Reduccion_CajasClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(M_WMS_Reduccion_CajasClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public M_WMS_Reduccion_CajasClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<IM_WMS_ReduccionCajas.IM_WMS_Reduccion_CajasInitResponse> IM_WMS_ReduccionCajas.IM_WMS_Reduccion_Cajas.initAsync(IM_WMS_ReduccionCajas.IM_WMS_Reduccion_CajasInitRequest request)
        {
            return base.Channel.initAsync(request);
        }
        
        public System.Threading.Tasks.Task<IM_WMS_ReduccionCajas.IM_WMS_Reduccion_CajasInitResponse> initAsync(IM_WMS_ReduccionCajas.CallContext CallContext, string _dataValidationXML, string _lineXML)
        {
            IM_WMS_ReduccionCajas.IM_WMS_Reduccion_CajasInitRequest inValue = new IM_WMS_ReduccionCajas.IM_WMS_Reduccion_CajasInitRequest();
            inValue.CallContext = CallContext;
            inValue._dataValidationXML = _dataValidationXML;
            inValue._lineXML = _lineXML;
            return ((IM_WMS_ReduccionCajas.IM_WMS_Reduccion_Cajas)(this)).initAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.NetTcpBinding_IM_WMS_Reduccion_Cajas))
            {
                System.ServiceModel.NetTcpBinding result = new System.ServiceModel.NetTcpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.NetTcpBinding_IM_WMS_Reduccion_Cajas))
            {
                return new System.ServiceModel.EndpointAddress("net.tcp://gim-dev-aos:8201/DynamicsAx/Services/IM_WMS_Reduccion_CajasGP");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return M_WMS_Reduccion_CajasClient.GetBindingForEndpoint(EndpointConfiguration.NetTcpBinding_IM_WMS_Reduccion_Cajas);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return M_WMS_Reduccion_CajasClient.GetEndpointAddress(EndpointConfiguration.NetTcpBinding_IM_WMS_Reduccion_Cajas);
        }
        
        public enum EndpointConfiguration
        {
            
            NetTcpBinding_IM_WMS_Reduccion_Cajas,
        }
    }
}
