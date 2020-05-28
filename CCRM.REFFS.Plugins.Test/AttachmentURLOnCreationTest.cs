//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="AttachmentURLOnCreationTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  AttachmentURLOnCreationTest Plug in Test
//// </summary>
//// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Test
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Fakes;

    [TestClass]
    public class AttachmentURLOnCreationTest
    {
        //private string bloburl = "https://ref.blob.core.windows.net/refnotesattachments/ee321259-6992-e811-812d-3863bb3ce590_IMG_3892.jpg";

        public static void PluginVariables(StubIServiceProvider serviceProvider, StubIPluginExecutionContext pluginContext, StubIOrganizationService organizationService, int stageNumber, string messageName, EntityImageCollection postImage)
        {
            var serviceFactory = new StubIOrganizationServiceFactory();
            var tracingService = new StubITracingService();

            if (serviceProvider != null)
            {
                serviceProvider.GetServiceType = type =>
                {
                    if (type == typeof(IPluginExecutionContext))
                    {
                        return pluginContext;
                    }
                    else if (type == typeof(ITracingService))
                    {
                        return tracingService;
                    }
                    else if (type == typeof(IOrganizationServiceFactory))
                    {
                        return serviceFactory;
                    }

                    return null;
                };
            }

            pluginContext.DepthGet = () => 1;
            pluginContext.UserIdGet = () => new Guid();
            pluginContext.MessageNameGet = () => messageName;
            pluginContext.StageGet = () => stageNumber;
            pluginContext.InitiatingUserIdGet = () => Guid.Parse("F83DA6A6-748E-E412-9412-00155D614A70");
            pluginContext.CorrelationIdGet = () => new Guid();
            pluginContext.PrimaryEntityIdGet = () =>
            {
                return Guid.NewGuid();
            };

            pluginContext.PostEntityImagesGet = () => { return postImage; };
            pluginContext.PreEntityImagesGet = () => { return postImage; };
            serviceFactory.CreateOrganizationServiceNullableOfGuid = t1 => organizationService;

            tracingService.TraceStringObjectArray = Trace;
        }

        public static void Trace(string message, params object[] value)
        {
        }

        [TestMethod]
        public void AttachmentURLOnCreationTestMethodforWorkOrder()
        {
            using (ShimsContext.Create())
            {
                AttachmentURLOnCreation attachmentURLOnCreate = new AttachmentURLOnCreation();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                pluginContext.PrimaryEntityNameGet = () => "smp_attachmentsurl";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity attachmentUrl = new Entity("smp_attachmentsurl");
                attachmentUrl.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                attachmentUrl["smp_isdelete"] = false;
                attachmentUrl["smp_source"] = new OptionSetValue(Constants.SourceAX);
                attachmentUrl["smp_name"] = "testfile";
                attachmentUrl["smp_objectid"] = "884A078B-0467-E711-80F5-3863BB3C0660";
                attachmentUrl["smp_objecttypecode"] = "10266";
                attachmentUrl["smp_mimetype"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                ////attachmentUrl["smp_bloburl"] = this.bloburl;
                paramCollection.Add("Target", attachmentUrl);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", null);
                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((Microsoft.Xrm.Sdk.Query.FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                        {
                            entityName = "ava_keyvaluepair";
                        }
                    }
                    else if (query.GetType().Name.Equals("QueryExpression"))
                    {
                        entityName = ((QueryExpression)query).EntityName;
                    }
                    else
                    {
                        entityName = ((QueryByAttribute)query).EntityName;
                    }

                    if (entityName == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        collection.Entities.Add(note);
                    }

                    return collection;
                };
                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "smp_attachmentsurl")
                    {
                        Entity attachmentUrlentity = new Entity("smp_attachmentsurl");
                        attachmentUrlentity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        attachmentUrlentity.Attributes["smp_objectid"] = "54D94FC2-52AD-E511-8158-1458D04DB4D1";
                        attachmentUrlentity.Attributes["smp_name"] = "Test";
                        attachmentUrlentity.Attributes["smp_source"] = new OptionSetValue(180620001);
                        attachmentUrlentity.Attributes["smp_notesid"] = "884A078B-0467-E711-80F5-3863BB3C0660";
                        return attachmentUrlentity;
                    }

                    if (entity == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        return note;
                    }

                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity
                        {
                            LogicalName = "msdyn_workorder",
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0680")
                        };
                        return workorder;
                    }

                    return null;
                };

                attachmentURLOnCreate.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void AttachmentURLOnCreationTestMethodforPR()
        {
            using (ShimsContext.Create())
            {
                AttachmentURLOnCreation attachmentURLOnCreate = new AttachmentURLOnCreation();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                pluginContext.PrimaryEntityNameGet = () => "smp_attachmentsurl";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity attachmentUrl = new Entity("smp_attachmentsurl");
                attachmentUrl.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                attachmentUrl["smp_isdelete"] = false;
                attachmentUrl["smp_source"] = new OptionSetValue(Constants.SourceAX);
                attachmentUrl["smp_name"] = "testfile";
                attachmentUrl["smp_objectid"] = "884A078B-0467-E711-80F5-3863BB3C0660";
                attachmentUrl["smp_objecttypecode"] = "10237";
                attachmentUrl["smp_mimetype"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                ////attachmentUrl["smp_bloburl"] = this.bloburl;
                paramCollection.Add("Target", attachmentUrl);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", null);
                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((Microsoft.Xrm.Sdk.Query.FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                        {
                            entityName = "ava_keyvaluepair";
                        }
                    }
                    else if (query.GetType().Name.Equals("QueryExpression"))
                    {
                        entityName = ((QueryExpression)query).EntityName;
                    }
                    else
                    {
                        entityName = ((QueryByAttribute)query).EntityName;
                    }

                    if (entityName == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        collection.Entities.Add(note);
                    }

                    return collection;
                };
                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "smp_attachmentsurl")
                    {
                        Entity attachmentUrlentity = new Entity("smp_attachmentsurl");
                        attachmentUrlentity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        attachmentUrlentity.Attributes["smp_objectid"] = "54D94FC2-52AD-E511-8158-1458D04DB4D1";
                        attachmentUrlentity.Attributes["smp_name"] = "Test";
                        attachmentUrlentity.Attributes["smp_source"] = new OptionSetValue(180620001);
                        attachmentUrlentity.Attributes["smp_notesid"] = "884A078B-0467-E711-80F5-3863BB3C0660";
                        return attachmentUrlentity;
                    }

                    if (entity == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        return note;
                    }

                    if (entity == "msdyn_purchaseorder")
                    {
                        Entity pr = new Entity
                        {
                            LogicalName = "msdyn_purchaseorder",
                            Id = new Guid("884A078B-0467-E711-80F5-3863B43C0680")
                        };
                        return pr;
                    }

                    return null;
                };

                attachmentURLOnCreate.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void AttachmentURLOnCreationTestMethodforPRProduct()
        {
            using (ShimsContext.Create())
            {
                AttachmentURLOnCreation attachmentURLOnCreate = new AttachmentURLOnCreation();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                pluginContext.PrimaryEntityNameGet = () => "smp_attachmentsurl";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity attachmentUrl = new Entity("smp_attachmentsurl");
                attachmentUrl.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                attachmentUrl["smp_isdelete"] = false;
                attachmentUrl["smp_source"] = new OptionSetValue(Constants.SourceAX);
                attachmentUrl["smp_name"] = "testfile";
                attachmentUrl["smp_objectid"] = "884A078B-0467-E711-80F5-3863BB3C0660";
                attachmentUrl["smp_objecttypecode"] = "10239";
                attachmentUrl["smp_mimetype"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                ////attachmentUrl["smp_bloburl"] = this.bloburl;
                paramCollection.Add("Target", attachmentUrl);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", null);
                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((Microsoft.Xrm.Sdk.Query.FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                        {
                            entityName = "ava_keyvaluepair";
                        }
                    }
                    else if (query.GetType().Name.Equals("QueryExpression"))
                    {
                        entityName = ((QueryExpression)query).EntityName;
                    }
                    else
                    {
                        entityName = ((QueryByAttribute)query).EntityName;
                    }

                    if (entityName == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        collection.Entities.Add(note);
                    }

                    return collection;
                };
                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "smp_attachmentsurl")
                    {
                        Entity attachmentUrlentity = new Entity("smp_attachmentsurl");
                        attachmentUrlentity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        attachmentUrlentity.Attributes["smp_objectid"] = "54D94FC2-52AD-E511-8158-1458D04DB4D1";
                        attachmentUrlentity.Attributes["smp_name"] = "Test";
                        attachmentUrlentity.Attributes["smp_source"] = new OptionSetValue(180620001);
                        attachmentUrlentity.Attributes["smp_notesid"] = "884A078B-0467-E711-80F5-3863BB3C0660";
                        return attachmentUrlentity;
                    }

                    if (entity == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        return note;
                    }

                    if (entity == "msdyn_purchaseorderproduct")
                    {
                        Entity prproduct = new Entity
                        {
                            LogicalName = "msdyn_purchaseorderproduct",
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0980")
                        };
                        return prproduct;
                    }

                    return null;
                };

                attachmentURLOnCreate.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void AttachmentURLOnCreationTestMethodforProduct()
        {
            using (ShimsContext.Create())
            {
                AttachmentURLOnCreation attachmentURLOnCreate = new AttachmentURLOnCreation();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                pluginContext.PrimaryEntityNameGet = () => "smp_attachmentsurl";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity attachmentUrl = new Entity("smp_attachmentsurl");
                attachmentUrl.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                attachmentUrl["smp_isdelete"] = false;
                attachmentUrl["smp_source"] = new OptionSetValue(Constants.SourceAX);
                attachmentUrl["smp_name"] = "testfile";
                attachmentUrl["smp_objectid"] = string.Empty;
                attachmentUrl["smp_objecttypecode"] = Constants.ProductObjectCode;
                attachmentUrl["smp_mimetype"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                ////attachmentUrl["smp_bloburl"] = this.bloburl;
                attachmentUrl["smp_keyvalue"] = "1234";
                paramCollection.Add("Target", attachmentUrl);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", null);
                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((Microsoft.Xrm.Sdk.Query.FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                        {
                            entityName = "ava_keyvaluepair";
                        }
                    }
                    else if (query.GetType().Name.Equals("QueryExpression"))
                    {
                        entityName = ((QueryExpression)query).EntityName;
                    }
                    else
                    {
                        entityName = ((QueryByAttribute)query).EntityName;
                    }

                    if (entityName == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        collection.Entities.Add(note);
                    }

                    if (entityName == "product")
                    {
                        Entity product = new Entity
                        {
                            LogicalName = "product",
                            Id = new Guid("884A078B-0467-E711-80F5-3868BB3C0980")
                        };
                        product.Attributes["productnumber"] = "1234";
                        collection.Entities.Add(product);
                    }

                    return collection;
                };
                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "smp_attachmentsurl")
                    {
                        Entity attachmentUrlentity = new Entity("smp_attachmentsurl");
                        attachmentUrlentity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        attachmentUrlentity.Attributes["smp_objectid"] = "54D94FC2-52AD-E511-8158-1458D04DB4D1";
                        attachmentUrlentity.Attributes["smp_name"] = "Test";
                        attachmentUrlentity.Attributes["smp_source"] = new OptionSetValue(180620001);
                        attachmentUrlentity.Attributes["smp_notesid"] = "884A078B-0467-E711-80F5-3863BB3C0660";
                        return attachmentUrlentity;
                    }

                    if (entity == "annotation")
                    {
                        Entity note = new Entity("annotation");
                        note.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        note.Attributes["smp_name"] = "Test";
                        return note;
                    }

                    if (entity == "product")
                    {
                        Entity product = new Entity
                        {
                            LogicalName = "product",
                            Id = new Guid("884A078B-0467-E711-80F5-3868BB3C0980")
                        };
                        product.Attributes["productnumber"] = "1234";
                        return product;
                    }

                    return null;
                };

                attachmentURLOnCreate.Execute(serviceProvider);
            }
        }
    }
}
