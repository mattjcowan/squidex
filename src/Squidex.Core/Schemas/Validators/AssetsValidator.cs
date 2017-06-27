﻿// ==========================================================================
//  AssetsValidator.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Squidex.Core.Schemas.Validators
{
    public sealed class AssetsValidator : IValidator
    {
        private readonly bool isRequired;

        public AssetsValidator(bool isRequired)
        {
            this.isRequired = isRequired;
        }

        public async Task ValidateAsync(object value, ValidationContext context, Action<string> addError)
        {
            var assets = value as AssetsValue;

            if (assets == null || assets.AssetIds.Count == 0)
            {
                if (isRequired && !context.IsOptional)
                {
                    addError("<FIELD> is required");
                }

                return;
            }

            var assetTasks = assets.AssetIds.Select(x => CheckAssetAsync(context, x)).ToArray();

            await Task.WhenAll(assetTasks);

            foreach (var notFoundId in assetTasks.Where(x => !x.Result.IsFound).Select(x => x.Result.AssetId))
            {
                addError($"<FIELD> contains invalid asset '{notFoundId}'");
            }
        }

        private static async Task<(Guid AssetId, bool IsFound)> CheckAssetAsync(ValidationContext context, Guid id)
        {
            var isFound = await context.IsValidAssetIdAsync(id);

            return (id, isFound);
        }
    }
}
