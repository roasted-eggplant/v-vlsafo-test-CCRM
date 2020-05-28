// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  NotesFromSRToWorkOrder
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Common
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;

    public static class AttachmentHelper
    {
        public static Dictionary<string, string> RetrieveNotesAttachmentEntities(IOrganizationService service)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            QueryExpression notesattachmentquery = new QueryExpression("msdyn_notesattachmententitysetting");
            notesattachmentquery.ColumnSet = new ColumnSet("msdyn_entityname");
            EntityCollection notesattachmentcollection = service.RetrieveMultiple(notesattachmentquery);

            if (notesattachmentcollection.Entities != null && notesattachmentcollection.Entities.Count > 0)
            {
                foreach (Entity notesattachmentEntity in notesattachmentcollection.Entities)
                {
                    string entityname = notesattachmentEntity.Attributes["msdyn_entityname"].ToString();
                    string objecttypecode = RetrieveObjectTypeCode(service, entityname);
                    list.Add(objecttypecode, entityname);
                }
            }

            return list;
        }

        public static string RetrieveObjectTypeCode(IOrganizationService service, string entityname)
        {
            Entity entity = new Entity(entityname);
            RetrieveEntityRequest entityRequest = new RetrieveEntityRequest();
            entityRequest.LogicalName = entity.LogicalName;
            entityRequest.EntityFilters = EntityFilters.All;
            RetrieveEntityResponse responseent = (RetrieveEntityResponse)service.Execute(entityRequest);
            EntityMetadata ent = (EntityMetadata)responseent.EntityMetadata;
            string objectTypeCode = ent.ObjectTypeCode.ToString();
            return objectTypeCode;
        }
    }
}