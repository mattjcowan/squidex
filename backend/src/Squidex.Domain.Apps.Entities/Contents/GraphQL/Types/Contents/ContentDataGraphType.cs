﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using GraphQL.Types;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas;

namespace Squidex.Domain.Apps.Entities.Contents.GraphQL.Types.Contents
{
    public sealed class ContentDataGraphType : ObjectGraphType<ContentData>
    {
        public ContentDataGraphType(ISchemaEntity schema, string schemaName, string schemaType, IGraphModel model)
        {
            Name = $"{schemaType}DataDto";

            foreach (var (field, fieldName, typeName) in schema.SchemaDef.Fields.SafeFields())
            {
                var (resolvedType, valueResolver, args) = model.GetGraphType(schema, field, typeName);

                if (valueResolver != null)
                {
                    var displayName = field.DisplayName();

                    var fieldGraphType = new ObjectGraphType
                    {
                        Name = $"{schemaType}Data{typeName}Dto"
                    };

                    var partitioning = model.ResolvePartition(field.Partitioning);

                    foreach (var partitionKey in partitioning.AllKeys)
                    {
                        fieldGraphType.AddField(new FieldType
                        {
                            Name = partitionKey.EscapePartition(),
                            Arguments = args,
                            ResolvedType = resolvedType,
                            Resolver = ContentResolvers.Partition(valueResolver, partitionKey),
                            Description = field.RawProperties.Hints
                        });
                    }

                    fieldGraphType.Description = $"The structure of the {displayName} field of the {schemaName} content type.";

                    AddField(new FieldType
                    {
                        Name = fieldName,
                        ResolvedType = fieldGraphType,
                        Resolver = ContentResolvers.Field(field),
                        Description = $"The {displayName} field."
                    });
                }
            }

            Description = $"The structure of the {schemaName} data type.";
        }
    }
}
