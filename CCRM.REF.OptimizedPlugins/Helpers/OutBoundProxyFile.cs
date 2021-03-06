﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cmms.ServiceManagement.Services
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ServiceRequest", Namespace="http://schemas.datacontract.org/2004/07/Cmms.ServiceManagement.Services")]
    public partial class ServiceRequest : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string BuildingIdField;
        
        private string BuildingTimeZoneField;
        
        private string ContactAliasField;
        
        private string ContactBuildingNameField;
        
        private string ContactEmailField;
        
        private string ContactFirstNameField;
        
        private string ContactLastNameField;
        
        private string ContactPhoneNumberField;
        
        private string ContactPreferredLanguageField;
        
        private string ContactRoomNumberField;
        
        private string ContactRoomTypeField;
        
        private string ContactZoneField;
        
        private string CostCenterCodeField;
        
        private System.DateTime CreatedDateTimeField;
        
        private string CreatedDateTimeByBuildingTimeZoneField;
        
        private System.DateTime DueDateField;
        
        private string DueDateByBuildingTimeZoneField;
        
        private Cmms.ServiceManagement.Services.DynamicProblemTypeNotes[] DynamicProblemTypeNotesCollectionField;
        
        private string IoCodeField;
        
        private bool IsProviderUserField;
        
        private Cmms.ServiceManagement.Services.Notes[] NotesCollectionField;
        
        private string PriorityField;
        
        private string PriorityOverrideReasonField;
        
        private string ProblemBuildingAddressLine1Field;
        
        private string ProblemBuildingAddressLine2Field;
        
        private string ProblemBuildingCityField;
        
        private string ProblemBuildingCountryField;
        
        private string ProblemBuildingNameField;
        
        private string ProblemBuildingStateField;
        
        private string ProblemBuildingZipCodeField;
        
        private string ProblemClassIdField;
        
        private string ProblemClassNameField;
        
        private string ProblemCodeField;
        
        private string ProblemFloorNameField;
        
        private System.DateTime ProblemOccurredDateTimeField;
        
        private string ProblemOccurredDateTimeByBuildingTimeZoneField;
        
        private string ProblemRoomNumberField;
        
        private string ProblemRoomTypeNameField;
        
        private string ProblemTypeDescriptionField;
        
        private string ProblemTypeIdField;
        
        private string ProblemTypeNameField;
        
        private string ProblemZoneField;
        
        private string ProviderDueDateByBuildingTimeZoneField;
        
        private string ProviderNameField;
        
        private string ProviderWorkCompletionDateByBuildingTimeZoneField;
        
        private string ReferenceServiceRequestNumberField;
        
        private System.DateTime RequestedDueDateField;
        
        private string RequestorAliasField;
        
        private string RequestorBuildingNameField;
        
        private string RequestorEmailField;
        
        private string RequestorFirstNameField;
        
        private string RequestorLastNameField;
        
        private string RequestorPhoneNumberField;
        
        private string RequestorPreferredLanguageField;
        
        private string RequestorRoomField;
        
        private string RequestorRoomTypeField;
        
        private string RequestorZoneField;
        
        private string ServiceRequestDeviceTypeField;
        
        private string ServiceRequestNumberField;
        
        private string ServiceRequestOriginationField;
        
        private string StatusCodeField;
        
        private System.DateTime SubmittedDateTimeField;
        
        private string SubmittedDateTimeByBuildingTimeZoneField;
        
        private string TitleField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BuildingId
        {
            get
            {
                return this.BuildingIdField;
            }
            set
            {
                this.BuildingIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BuildingTimeZone
        {
            get
            {
                return this.BuildingTimeZoneField;
            }
            set
            {
                this.BuildingTimeZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactAlias
        {
            get
            {
                return this.ContactAliasField;
            }
            set
            {
                this.ContactAliasField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactBuildingName
        {
            get
            {
                return this.ContactBuildingNameField;
            }
            set
            {
                this.ContactBuildingNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactEmail
        {
            get
            {
                return this.ContactEmailField;
            }
            set
            {
                this.ContactEmailField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactFirstName
        {
            get
            {
                return this.ContactFirstNameField;
            }
            set
            {
                this.ContactFirstNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactLastName
        {
            get
            {
                return this.ContactLastNameField;
            }
            set
            {
                this.ContactLastNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactPhoneNumber
        {
            get
            {
                return this.ContactPhoneNumberField;
            }
            set
            {
                this.ContactPhoneNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactPreferredLanguage
        {
            get
            {
                return this.ContactPreferredLanguageField;
            }
            set
            {
                this.ContactPreferredLanguageField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactRoomNumber
        {
            get
            {
                return this.ContactRoomNumberField;
            }
            set
            {
                this.ContactRoomNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactRoomType
        {
            get
            {
                return this.ContactRoomTypeField;
            }
            set
            {
                this.ContactRoomTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactZone
        {
            get
            {
                return this.ContactZoneField;
            }
            set
            {
                this.ContactZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CostCenterCode
        {
            get
            {
                return this.CostCenterCodeField;
            }
            set
            {
                this.CostCenterCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CreatedDateTime
        {
            get
            {
                return this.CreatedDateTimeField;
            }
            set
            {
                this.CreatedDateTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CreatedDateTimeByBuildingTimeZone
        {
            get
            {
                return this.CreatedDateTimeByBuildingTimeZoneField;
            }
            set
            {
                this.CreatedDateTimeByBuildingTimeZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DueDate
        {
            get
            {
                return this.DueDateField;
            }
            set
            {
                this.DueDateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DueDateByBuildingTimeZone
        {
            get
            {
                return this.DueDateByBuildingTimeZoneField;
            }
            set
            {
                this.DueDateByBuildingTimeZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Cmms.ServiceManagement.Services.DynamicProblemTypeNotes[] DynamicProblemTypeNotesCollection
        {
            get
            {
                return this.DynamicProblemTypeNotesCollectionField;
            }
            set
            {
                this.DynamicProblemTypeNotesCollectionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string IoCode
        {
            get
            {
                return this.IoCodeField;
            }
            set
            {
                this.IoCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsProviderUser
        {
            get
            {
                return this.IsProviderUserField;
            }
            set
            {
                this.IsProviderUserField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Cmms.ServiceManagement.Services.Notes[] NotesCollection
        {
            get
            {
                return this.NotesCollectionField;
            }
            set
            {
                this.NotesCollectionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Priority
        {
            get
            {
                return this.PriorityField;
            }
            set
            {
                this.PriorityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PriorityOverrideReason
        {
            get
            {
                return this.PriorityOverrideReasonField;
            }
            set
            {
                this.PriorityOverrideReasonField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemBuildingAddressLine1
        {
            get
            {
                return this.ProblemBuildingAddressLine1Field;
            }
            set
            {
                this.ProblemBuildingAddressLine1Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemBuildingAddressLine2
        {
            get
            {
                return this.ProblemBuildingAddressLine2Field;
            }
            set
            {
                this.ProblemBuildingAddressLine2Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemBuildingCity
        {
            get
            {
                return this.ProblemBuildingCityField;
            }
            set
            {
                this.ProblemBuildingCityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemBuildingCountry
        {
            get
            {
                return this.ProblemBuildingCountryField;
            }
            set
            {
                this.ProblemBuildingCountryField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemBuildingName
        {
            get
            {
                return this.ProblemBuildingNameField;
            }
            set
            {
                this.ProblemBuildingNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemBuildingState
        {
            get
            {
                return this.ProblemBuildingStateField;
            }
            set
            {
                this.ProblemBuildingStateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemBuildingZipCode
        {
            get
            {
                return this.ProblemBuildingZipCodeField;
            }
            set
            {
                this.ProblemBuildingZipCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemClassId
        {
            get
            {
                return this.ProblemClassIdField;
            }
            set
            {
                this.ProblemClassIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemClassName
        {
            get
            {
                return this.ProblemClassNameField;
            }
            set
            {
                this.ProblemClassNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemCode
        {
            get
            {
                return this.ProblemCodeField;
            }
            set
            {
                this.ProblemCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemFloorName
        {
            get
            {
                return this.ProblemFloorNameField;
            }
            set
            {
                this.ProblemFloorNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ProblemOccurredDateTime
        {
            get
            {
                return this.ProblemOccurredDateTimeField;
            }
            set
            {
                this.ProblemOccurredDateTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemOccurredDateTimeByBuildingTimeZone
        {
            get
            {
                return this.ProblemOccurredDateTimeByBuildingTimeZoneField;
            }
            set
            {
                this.ProblemOccurredDateTimeByBuildingTimeZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemRoomNumber
        {
            get
            {
                return this.ProblemRoomNumberField;
            }
            set
            {
                this.ProblemRoomNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemRoomTypeName
        {
            get
            {
                return this.ProblemRoomTypeNameField;
            }
            set
            {
                this.ProblemRoomTypeNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemTypeDescription
        {
            get
            {
                return this.ProblemTypeDescriptionField;
            }
            set
            {
                this.ProblemTypeDescriptionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemTypeId
        {
            get
            {
                return this.ProblemTypeIdField;
            }
            set
            {
                this.ProblemTypeIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemTypeName
        {
            get
            {
                return this.ProblemTypeNameField;
            }
            set
            {
                this.ProblemTypeNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProblemZone
        {
            get
            {
                return this.ProblemZoneField;
            }
            set
            {
                this.ProblemZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProviderDueDateByBuildingTimeZone
        {
            get
            {
                return this.ProviderDueDateByBuildingTimeZoneField;
            }
            set
            {
                this.ProviderDueDateByBuildingTimeZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProviderName
        {
            get
            {
                return this.ProviderNameField;
            }
            set
            {
                this.ProviderNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProviderWorkCompletionDateByBuildingTimeZone
        {
            get
            {
                return this.ProviderWorkCompletionDateByBuildingTimeZoneField;
            }
            set
            {
                this.ProviderWorkCompletionDateByBuildingTimeZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReferenceServiceRequestNumber
        {
            get
            {
                return this.ReferenceServiceRequestNumberField;
            }
            set
            {
                this.ReferenceServiceRequestNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime RequestedDueDate
        {
            get
            {
                return this.RequestedDueDateField;
            }
            set
            {
                this.RequestedDueDateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorAlias
        {
            get
            {
                return this.RequestorAliasField;
            }
            set
            {
                this.RequestorAliasField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorBuildingName
        {
            get
            {
                return this.RequestorBuildingNameField;
            }
            set
            {
                this.RequestorBuildingNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorEmail
        {
            get
            {
                return this.RequestorEmailField;
            }
            set
            {
                this.RequestorEmailField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorFirstName
        {
            get
            {
                return this.RequestorFirstNameField;
            }
            set
            {
                this.RequestorFirstNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorLastName
        {
            get
            {
                return this.RequestorLastNameField;
            }
            set
            {
                this.RequestorLastNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorPhoneNumber
        {
            get
            {
                return this.RequestorPhoneNumberField;
            }
            set
            {
                this.RequestorPhoneNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorPreferredLanguage
        {
            get
            {
                return this.RequestorPreferredLanguageField;
            }
            set
            {
                this.RequestorPreferredLanguageField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorRoom
        {
            get
            {
                return this.RequestorRoomField;
            }
            set
            {
                this.RequestorRoomField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorRoomType
        {
            get
            {
                return this.RequestorRoomTypeField;
            }
            set
            {
                this.RequestorRoomTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RequestorZone
        {
            get
            {
                return this.RequestorZoneField;
            }
            set
            {
                this.RequestorZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceRequestDeviceType
        {
            get
            {
                return this.ServiceRequestDeviceTypeField;
            }
            set
            {
                this.ServiceRequestDeviceTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceRequestNumber
        {
            get
            {
                return this.ServiceRequestNumberField;
            }
            set
            {
                this.ServiceRequestNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceRequestOrigination
        {
            get
            {
                return this.ServiceRequestOriginationField;
            }
            set
            {
                this.ServiceRequestOriginationField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StatusCode
        {
            get
            {
                return this.StatusCodeField;
            }
            set
            {
                this.StatusCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime SubmittedDateTime
        {
            get
            {
                return this.SubmittedDateTimeField;
            }
            set
            {
                this.SubmittedDateTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SubmittedDateTimeByBuildingTimeZone
        {
            get
            {
                return this.SubmittedDateTimeByBuildingTimeZoneField;
            }
            set
            {
                this.SubmittedDateTimeByBuildingTimeZoneField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title
        {
            get
            {
                return this.TitleField;
            }
            set
            {
                this.TitleField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DynamicProblemTypeNotes", Namespace="http://schemas.datacontract.org/2004/07/Cmms.ServiceManagement.Services")]
    public partial class DynamicProblemTypeNotes : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string AnswerField;
        
        private string DynamicsProblemTypeIdField;
        
        private string DynamicsProblemTypeNameField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Answer
        {
            get
            {
                return this.AnswerField;
            }
            set
            {
                this.AnswerField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DynamicsProblemTypeId
        {
            get
            {
                return this.DynamicsProblemTypeIdField;
            }
            set
            {
                this.DynamicsProblemTypeIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DynamicsProblemTypeName
        {
            get
            {
                return this.DynamicsProblemTypeNameField;
            }
            set
            {
                this.DynamicsProblemTypeNameField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Notes", Namespace="http://schemas.datacontract.org/2004/07/Cmms.ServiceManagement.Services")]
    public partial class Notes : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string AttachmentUrlField;
        
        private string DescriptionField;
        
        private string NoteIdField;
        
        private System.DateTime NotesCreatedOnField;
        
        private string SubjectField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AttachmentUrl
        {
            get
            {
                return this.AttachmentUrlField;
            }
            set
            {
                this.AttachmentUrlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                this.DescriptionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NoteId
        {
            get
            {
                return this.NoteIdField;
            }
            set
            {
                this.NoteIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime NotesCreatedOn
        {
            get
            {
                return this.NotesCreatedOnField;
            }
            set
            {
                this.NotesCreatedOnField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Subject
        {
            get
            {
                return this.SubjectField;
            }
            set
            {
                this.SubjectField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="ICmmsServiceRequestManager")]
public interface ICmmsServiceRequestManager
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICmmsServiceRequestManager/UpdateServiceRequestDetails", ReplyAction="http://tempuri.org/ICmmsServiceRequestManager/UpdateServiceRequestDetailsResponse" +
        "")]
    bool UpdateServiceRequestDetails(string userName, string password, Cmms.ServiceManagement.Services.ServiceRequest serviceRequest);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICmmsServiceRequestManager/UpdateServiceRequestDetails", ReplyAction="http://tempuri.org/ICmmsServiceRequestManager/UpdateServiceRequestDetailsResponse" +
        "")]
    System.Threading.Tasks.Task<bool> UpdateServiceRequestDetailsAsync(string userName, string password, Cmms.ServiceManagement.Services.ServiceRequest serviceRequest);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICmmsServiceRequestManager/LogException", ReplyAction="http://tempuri.org/ICmmsServiceRequestManager/LogExceptionResponse")]
    bool LogException(string userName, string password, System.Exception serviceRequest);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICmmsServiceRequestManager/LogException", ReplyAction="http://tempuri.org/ICmmsServiceRequestManager/LogExceptionResponse")]
    System.Threading.Tasks.Task<bool> LogExceptionAsync(string userName, string password, System.Exception serviceRequest);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICmmsServiceRequestManager/GenerateToken", ReplyAction="http://tempuri.org/ICmmsServiceRequestManager/GenerateTokenResponse")]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Cmms.ServiceManagement.Services.ServiceRequest))]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Cmms.ServiceManagement.Services.DynamicProblemTypeNotes[]))]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Cmms.ServiceManagement.Services.DynamicProblemTypeNotes))]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Cmms.ServiceManagement.Services.Notes[]))]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Cmms.ServiceManagement.Services.Notes))]
    [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Exception))]
    string GenerateToken(object service);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICmmsServiceRequestManager/GenerateToken", ReplyAction="http://tempuri.org/ICmmsServiceRequestManager/GenerateTokenResponse")]
    System.Threading.Tasks.Task<string> GenerateTokenAsync(object service);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface ICmmsServiceRequestManagerChannel : ICmmsServiceRequestManager, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class CmmsServiceRequestManagerClient : System.ServiceModel.ClientBase<ICmmsServiceRequestManager>, ICmmsServiceRequestManager
{
    
    public CmmsServiceRequestManagerClient()
    {
    }
    
    public CmmsServiceRequestManagerClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public CmmsServiceRequestManagerClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public CmmsServiceRequestManagerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public CmmsServiceRequestManagerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public bool UpdateServiceRequestDetails(string userName, string password, Cmms.ServiceManagement.Services.ServiceRequest serviceRequest)
    {
        return base.Channel.UpdateServiceRequestDetails(userName, password, serviceRequest);
    }
    
    public System.Threading.Tasks.Task<bool> UpdateServiceRequestDetailsAsync(string userName, string password, Cmms.ServiceManagement.Services.ServiceRequest serviceRequest)
    {
        return base.Channel.UpdateServiceRequestDetailsAsync(userName, password, serviceRequest);
    }
    
    public bool LogException(string userName, string password, System.Exception serviceRequest)
    {
        return base.Channel.LogException(userName, password, serviceRequest);
    }
    
    public System.Threading.Tasks.Task<bool> LogExceptionAsync(string userName, string password, System.Exception serviceRequest)
    {
        return base.Channel.LogExceptionAsync(userName, password, serviceRequest);
    }
    
    public string GenerateToken(object service)
    {
        return base.Channel.GenerateToken(service);
    }
    
    public System.Threading.Tasks.Task<string> GenerateTokenAsync(object service)
    {
        return base.Channel.GenerateTokenAsync(service);
    }
}
